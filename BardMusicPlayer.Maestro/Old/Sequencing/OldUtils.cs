using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian.Structs;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Maestro.Old.Sequencing
{
    internal static class OldUtils
    {
        public static MemoryStream ConvertToSanfordSpec(this MidiFile midiFile)
        {
            try
            {
                Console.WriteLine("Scrubbing ");
                var loaderWatch = Stopwatch.StartNew();
                var newTrackChunks = new ConcurrentDictionary<int, TrackChunk>();
                var tempoMap = midiFile.GetTempoMap().Clone();
                long firstNote = midiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000;
                long firstNoteus = midiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds;
                var originalTrackChunks = new List<TrackChunk>();

                TrackChunk allTracks = new TrackChunk();
                allTracks.AddObjects(originalTrackChunks.GetNotes());

                foreach (var trackChunk in midiFile.GetTrackChunks())
                {
                    if (trackChunk.Events.Count > 0)
                    {
                        allTracks.AddObjects(trackChunk.GetNotes());
                        allTracks.AddObjects(trackChunk.GetTimedEvents());

                        //Cleanup track 0
                        using (var timedEventsManager = new TimedObjectsManager<TimedEvent>(allTracks.Events))
                        {
                            TimedObjectsCollection<TimedEvent> events = timedEventsManager.Objects;
                            List<TimedEvent> tlist = events.Where(static e => e.Event is LyricEvent).ToList();
                            foreach (TimedEvent tevent in tlist)
                                events.Remove(tevent);
                            tlist = events.Where(static e => e.Event is ProgramChangeEvent).ToList();
                            foreach (TimedEvent tevent in tlist)
                                events.Remove(tevent);
                        }
                    }
                    var thisTrack = new TrackChunk(new SequenceTrackNameEvent(trackChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text));
                    thisTrack.AddObjects(trackChunk.GetNotes());
                    thisTrack.AddObjects(trackChunk.GetTimedEvents());
                    originalTrackChunks.Add(thisTrack);
                }
                originalTrackChunks.Add(allTracks);

                Parallel.ForEach(originalTrackChunks.Where(static x => x.GetNotes().Any() || x.Events.OfType<LyricEvent>().Any()), (originalChunk, loopState, index) =>
                {
                    var watch = Stopwatch.StartNew();
                    var tempoMap = midiFile.GetTempoMap().Clone();
                    int noteVelocity = int.Parse(index.ToString()) + 1;

                    Dictionary<int, Dictionary<long, Note>> allNoteEvents = new Dictionary<int, Dictionary<long, Note>>();
                    for (int i = 0; i < 128; i++) allNoteEvents.Add(i, new Dictionary<long, Note>());

                    foreach (Note note in originalChunk.GetNotes())
                    {
                        long noteOnMS;
                        long noteOffMS;

                        try
                        {
                            noteOnMS = 5000000 + (note.GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds) - firstNoteus;
                            noteOffMS = 5000000 + (note.GetTimedNoteOffEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds) - firstNoteus;
                        }
                        catch (Exception) { continue; }
                        int noteNumber = note.NoteNumber;

                        Note newNote = new Note((SevenBitNumber)noteNumber,
                                                time: noteOnMS / 1000,
                                                length: (noteOffMS / 1000) - (noteOnMS / 1000)
                                                )
                        {
                            Channel = (FourBitNumber)0,
                            Velocity = (SevenBitNumber)noteVelocity,
                            OffVelocity = (SevenBitNumber)noteVelocity
                        };

                        if (allNoteEvents[noteNumber].ContainsKey(noteOnMS))
                        {
                            Note previousNote = allNoteEvents[noteNumber][noteOnMS];
                            if (previousNote.Length < note.Length) allNoteEvents[noteNumber][noteOnMS] = newNote;
                        }
                        else allNoteEvents[noteNumber].Add(noteOnMS, newNote);
                    }
                    watch.Stop();

                    Debug.WriteLine("step 1: " + noteVelocity + ": " + watch.ElapsedMilliseconds);
                    watch = Stopwatch.StartNew();

                    TrackChunk newChunk = new TrackChunk();
                    for (int i = 0; i < 128; i++)
                    {
                        long lastNoteTimeStamp = -1;
                        foreach (var noteEvent in allNoteEvents[i])
                        {
                            if (lastNoteTimeStamp >= 0 && allNoteEvents[i][lastNoteTimeStamp].Length + lastNoteTimeStamp >= noteEvent.Key)
                                allNoteEvents[i][lastNoteTimeStamp].Length -= allNoteEvents[i][lastNoteTimeStamp].Length + lastNoteTimeStamp + 1 - noteEvent.Key;

                            lastNoteTimeStamp = noteEvent.Key;
                        }
                    }
                    newChunk.AddObjects(allNoteEvents.SelectMany(static s => s.Value).Select(static s => s.Value).ToArray());
                    allNoteEvents = null;
                    watch.Stop();
                    Debug.WriteLine("step 2 [Fix Chords]: " + noteVelocity + ": " + watch.ElapsedMilliseconds);
                    watch = Stopwatch.StartNew();

                    Note[] notesToFix = newChunk.GetNotes().Reverse().ToArray();
                    for (int i = 1; i < notesToFix.Count(); i++)
                    {
                        int noteNum = notesToFix[i].NoteNumber;
                        long time = (notesToFix[i].GetTimedNoteOnEvent().Time);
                        long dur = notesToFix[i].Length;
                        int velocity = notesToFix[i].Velocity;

                        long lowestParent = notesToFix[0].GetTimedNoteOnEvent().Time;
                        for (int k = i - 1; k >= 0; k--)
                        {
                            long lastOn = notesToFix[k].GetTimedNoteOnEvent().Time;
                            if (lastOn < lowestParent) lowestParent = lastOn;
                        }
                        if (lowestParent <= time + 50)
                        {
                            time = lowestParent - 50;
                            if (time < 0) continue;
                            notesToFix[i].Time = time;
                            dur = 25;
                            notesToFix[i].Length = dur;
                        }
                    }

                    watch.Stop();
                    Debug.WriteLine("step 3: " + noteVelocity + ": " + watch.ElapsedMilliseconds);

                    #region calc shortest note
                    watch = Stopwatch.StartNew();

                    notesToFix = notesToFix.Reverse().ToArray();
                    List<Note> fixedNotes = new List<Note>();
                    for (int j = 0; j < notesToFix.Count(); j++)
                    {
                        var noteNum = notesToFix[j].NoteNumber;
                        var time = notesToFix[j].Time;
                        var dur = notesToFix[j].Length;
                        var channel = notesToFix[j].Channel;
                        var velocity = notesToFix[j].Velocity;

                        if (j + 1 < notesToFix.Count())
                        {
                            switch (notesToFix[j].Length)
                            {
                                // BACON MEOWCHESTRA
                                // Bandaid fix: If sustained note is 100ms or greater, ensure 60ms between the end of that note and the beginning of the next note.
                                // Otherwise, leave the behavior as it was before.
                                case >= 100 when notesToFix[j + 1].Time <= notesToFix[j].Time + notesToFix[j].Length + 60:
                                    dur = notesToFix[j + 1].Time - notesToFix[j].Time - 60;
                                    dur = dur < 60 ? 60 : dur;
                                    break;
                                case < 100 when notesToFix[j + 1].Time <= notesToFix[j].Time + notesToFix[j].Length + 25:
                                    dur = notesToFix[j + 1].Time - notesToFix[j].Time - 25;
                                    dur = dur < 25 ? 25 : dur;
                                    break;
                            }
                        }

                        fixedNotes.Add(new Note(noteNum, dur, time)
                        {
                            Channel = channel,
                            Velocity = velocity,
                            OffVelocity = velocity
                        });
                    }
                    notesToFix = null;

                    watch.Stop();
                    Debug.WriteLine("step 4: " + noteVelocity + ": " + watch.ElapsedMilliseconds);
                    #endregion

                    #region Tracknaming and octave shifting
                    watch = Stopwatch.StartNew();

                    int octaveShift = 0;
                    var trackName = originalChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text ?? "";
                    string o_trackName = trackName;

                    Regex rex = new Regex(@"^([A-Za-z _]+)([-+]\d)?");
                    if (rex.Match(trackName) is Match match)
                    {
                        if (!string.IsNullOrEmpty(match.Groups[1].Value))
                        {
                            trackName = Instrument.Parse(match.Groups[1].Value).Name;
                            if (!string.IsNullOrEmpty(match.Groups[2].Value))
                                if (int.TryParse(match.Groups[2].Value, out int os))
                                    octaveShift = os;

                            trackName = octaveShift switch
                            {
                                > 0 => trackName + "+" + octaveShift,
                                < 0 => trackName + octaveShift,
                                _ => trackName
                            };
                        }

                        //last try with the program number
                        if ((string.IsNullOrEmpty(match.Groups[1].Value)) || trackName.Equals("Unknown") || trackName.Equals("None"))
                        {
                            ProgramChangeEvent prog = originalChunk.Events.OfType<ProgramChangeEvent>().FirstOrDefault();
                            if (prog != null)
                                trackName = Instrument.ParseByProgramChange(prog.ProgramNumber).Name;
                        }

                    }
                    //If we have a lyrics tracks
                    if (o_trackName.StartsWith("Lyrics:"))
                        trackName = o_trackName;

                    newChunk = new TrackChunk(new SequenceTrackNameEvent(trackName));
                    #endregion Tracknaming and octave shifting

                    //Create Progchange Event if no IgnoreProgChange is set
                    if (!BmpPigeonhole.Instance.IgnoreProgChange || o_trackName.Contains("Program:ElectricGuitar"))
                    {
                        foreach (var timedEvent in originalChunk.GetTimedEvents())
                        {
                            if (timedEvent.Event is not ProgramChangeEvent programChangeEvent)
                                continue;
                            //Skip all except guitar | implement if we need this again
                            if ((programChangeEvent.ProgramNumber < 27) || (programChangeEvent.ProgramNumber > 31))
                                continue;

                            var channel = programChangeEvent.Channel;
                            using (var manager = new TimedObjectsManager(newChunk.Events, ObjectType.TimedEvent | ObjectType.Note))
                            {
                                TimedObjectsCollection<ITimedObject> timedEvents = manager.Objects;
                                if ((5000 + (timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote) < 0)
                                    timedEvents.Add(new TimedEvent(new ProgramChangeEvent(programChangeEvent.ProgramNumber), 5000));
                                else
                                    timedEvents.Add(new TimedEvent(new ProgramChangeEvent(programChangeEvent.ProgramNumber), 5000 + (timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote/* Absolute time too */));
                            }
                        }
                    }
                    //Create lyrics from midi
                    foreach (var timedEvent in originalChunk.GetTimedEvents())
                    {
                        if (timedEvent.Event is not LyricEvent lyricsEvent)
                            continue;

                        using (var manager = new TimedObjectsManager(newChunk.Events, ObjectType.TimedEvent | ObjectType.Note))
                        {
                            TimedObjectsCollection<ITimedObject> timedEvents = manager.Objects;
                            if ((5000 + (timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote) < 5000)
                                timedEvents.Add(new TimedEvent(new LyricEvent(lyricsEvent.Text), 5000));
                            else
                                timedEvents.Add(new TimedEvent(new LyricEvent(lyricsEvent.Text), 5000 + (timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote));
                        }
                    }

                    //Create Aftertouch Event
                    /*foreach (var timedEvent in originalChunk.GetTimedEvents())
                    {
                        var programChangeEvent = timedEvent.Event as ChannelAftertouchEvent;
                        if (programChangeEvent == null)
                            continue;
                        var channel = programChangeEvent.Channel;
                        using (var manager = new TimedObjectsManager(newChunk.Events, ObjectType.TimedEvent | ObjectType.Note))
                        {
                            TimedObjectsCollection<ITimedObject> timedEvents = manager.Objects;
                            timedEvents.Add(new TimedEvent(new ChannelAftertouchEvent(programChangeEvent.AftertouchValue), 5000 + (timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote));
                        }
                    }*/
                    newChunk.AddObjects(fixedNotes);

                    watch.Stop();
                    Debug.WriteLine("step 5: " + noteVelocity + ": " + watch.ElapsedMilliseconds);
                    watch = Stopwatch.StartNew();

                    newTrackChunks.TryAdd(noteVelocity, newChunk);

                    watch.Stop();
                    Debug.WriteLine("step 6: " + noteVelocity + ": " + watch.ElapsedMilliseconds);

                });

                var newMidiFile = new MidiFile();
                newTrackChunks.TryRemove(newTrackChunks.Count, out TrackChunk trackZero);
                newMidiFile.Chunks.Add(trackZero);
                newMidiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision(375);
                using (TempoMapManager tempoManager = newMidiFile.ManageTempoMap())
                    tempoManager.SetTempo(0, Tempo.FromBeatsPerMinute(160));

                newMidiFile.Chunks.AddRange(newTrackChunks.Values);



                tempoMap = newMidiFile.GetTempoMap();
                long delta = (newMidiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000);

                Parallel.ForEach(newMidiFile.GetTrackChunks(), chunk =>
                            {
                                int offset = Instrument.Parse(chunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text).SampleOffset; //get the offset
                                /*using (var notesManager = chunk.ManageNotes())
                                {
                                    foreach (Note note in notesManager.Notes)
                                    {
                                        long newStart = note.Time + offset - delta;
                                        note.Time = newStart;
                                    }
                                }*/
                                using (var manager = chunk.ManageTimedEvents())
                                {
                                    foreach (TimedEvent _event in manager.Objects)
                                    {
                                        var noteEvent = _event.Event as NoteEvent;
                                        var programChangeEvent = _event.Event as ProgramChangeEvent;
                                        var lyricsEvent = _event.Event as LyricEvent;

                                        //Note alignment
                                        if (noteEvent != null)
                                        {
                                            long newStart = _event.Time + offset - delta;
                                            _event.Time = newStart;
                                        }

                                        //Prog alignment
                                        if (programChangeEvent != null)
                                        {
                                            long newStart = _event.Time + offset - delta;
                                            if (newStart <= -1)
                                                manager.Objects.Remove(_event);
                                            else
                                                _event.Time = newStart;

                                            //if theres a new offset, use this one
                                            if ((programChangeEvent.ProgramNumber >= 27) && (programChangeEvent.ProgramNumber <= 31))
                                                offset = Instrument.ParseByProgramChange(programChangeEvent.ProgramNumber).SampleOffset;
                                        }

                                        //and lyrics
                                        if (lyricsEvent != null)
                                        {

                                        }

                                    }

                                    foreach (TimedEvent _event in manager.Objects)
                                    {
                                        var lyricsEvent = _event.Event as LyricEvent;
                                        if (lyricsEvent == null)
                                            continue;

                                        long newStart = _event.Time - delta;
                                        if (newStart <= -1)
                                            manager.Objects.Remove(_event);
                                        else
                                            _event.Time = newStart;
                                    }

                                    /*foreach (TimedEvent _event in manager.Objects)
                                    {
                                        var programChangeEvent = _event.Event as ChannelAftertouchEvent;
                                        if (programChangeEvent == null)
                                            continue;
                                        long newStart = _event.Time - delta;
                                        if (newStart <= -1)
                                            manager.Objects.Remove(_event);
                                        else
                                            _event.Time = newStart;
                                    }*/

                                }
                            });

                //Append the lyrics from the lrc
                /*var lrcTrack = new TrackChunk(new SequenceTrackNameEvent("Lyrics: "));
                using (var manager = new TimedObjectsManager(lrcTrack.Events, ObjectType.TimedEvent | ObjectType.Note))
                {
                    TimedObjectsCollection<ITimedObject> timedEvents = manager.Objects;
                    foreach (var line in LyricsContainer)
                    {
                        var timedEvent = new TimedEvent(new LyricEvent(line.Value)) as ITimedObject;
                        timedEvent.SetTime(new MetricTimeSpan(line.Key.Hour, line.Key.Minute, line.Key.Second, line.Key.Millisecond), tempoMap);
                        timedEvents.Add(timedEvent);
                    }
                }
                newMidiFile.Chunks.Add(lrcTrack);*/


                var stream = new MemoryStream();

                using (var manager = new TimedObjectsManager<TimedEvent>(newMidiFile.GetTrackChunks().First().Events))
                    manager.Objects.Add(new TimedEvent(new MarkerEvent(), (newMidiFile.GetDuration<MetricTimeSpan>().TotalMicroseconds / 1000)));

                newMidiFile.Write(stream, MidiFileFormat.MultiTrack, settings: new WritingSettings
                {
                    TextEncoding = Encoding.UTF8
                });

                stream.Flush();
                stream.Position = 0;

                loaderWatch.Stop();
                Console.WriteLine("Scrubbing MS: " + loaderWatch.ElapsedMilliseconds);
                return stream;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
