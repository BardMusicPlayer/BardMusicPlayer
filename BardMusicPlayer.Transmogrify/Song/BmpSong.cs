/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Transmogrify.Processor.Utilities;
using BardMusicPlayer.Transmogrify.Song.Config;
using BardMusicPlayer.Transmogrify.Song.Importers;
using BardMusicPlayer.Transmogrify.Song.Utilities;
using LiteDB;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Transmogrify.Song
{
    public sealed class BmpSong
    {
        /// <summary>
        /// 
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string DisplayedTitle { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public List<string> Tags { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        public TempoMap SourceTempoMap { get; set; } = TempoMap.Default;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<long, TrackContainer> TrackContainers { get; set; } = new();

        public TimeSpan Duration { get; set; } = new();


        /// <summary>
        /// opens a file and selects the processing by file ext.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<BmpSong> OpenFile(string path)
        {
            BmpSong song = null;
            if (Path.GetExtension(path).Equals(".mmsong"))
                song = CovertMidiToSong(MMSongImporter.OpenMMSongFile(path), path);
            else if (Path.GetExtension(path).Equals(".mml"))
                song = CovertMidiToSong(MMLSongImporter.OpenMMLSongFile(path), path);
            else
                song = OpenMidiFile(path);
            return Task.FromResult(song);
        }

        /// <summary>
        /// Open and process the mididata as byte[], tracks with note placed first
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<BmpSong> ImportMidiFromByte(byte[] data, string name)
        {
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(data, 0, data.Length);
            memoryStream.Position = 0;
            var midiFile = memoryStream.ReadAsMidiFile();
            memoryStream.Dispose();

            //some midifiles have a ChannelPrefixEvent with a channel greater than 0xF. remove 'em.
            foreach (var chunk in midiFile.GetTrackChunks())
            {
                using (TimedEventsManager timedEventsManager = chunk.ManageTimedEvents())
                {
                    TimedEventsCollection events = timedEventsManager.Events;
                    List<TimedEvent> prefixList = events.Where(e => e.Event is ChannelPrefixEvent).ToList();
                    foreach (TimedEvent tevent in prefixList)
                        if ((tevent.Event as ChannelPrefixEvent).Channel > 0xF)
                            events.Remove(tevent);
                }
            }

            return Task.FromResult(CovertMidiToSong(midiFile, name));
        }

        #region Import functions
        /// <summary>
        /// Open and process the midifile, tracks with note placed first
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static BmpSong OpenMidiFile(string path)
        {
            if (!File.Exists(path)) throw new BmpTransmogrifyException("File " + path + " does not exist!");

            using var fileStream = File.OpenRead(path);

            var midiFile = fileStream.ReadAsMidiFile();
            fileStream.Dispose();

            //some midifiles have a ChannelPrefixEvent with a channel greater than 0xF. remove 'em.
            foreach (var chunk in midiFile.GetTrackChunks())
            {
                using (TimedEventsManager timedEventsManager = chunk.ManageTimedEvents())
                {
                    TimedEventsCollection events = timedEventsManager.Events;
                    List<TimedEvent> prefixList = events.Where(e => e.Event is ChannelPrefixEvent).ToList();
                    foreach (TimedEvent tevent in prefixList)
                        if ((tevent.Event as ChannelPrefixEvent).Channel > 0xF)
                            events.Remove(tevent);
                }
            }

            return CovertMidiToSong(midiFile, path);
        }
        #endregion

        /// <summary>
        /// convert an imported file to a BmpSong
        /// </summary>
        /// <param name="midiFile"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static BmpSong CovertMidiToSong(MidiFile midiFile, string path)
        {
            TempoMap tempoMap = midiFile.GetTempoMap();
            TimeSpan midiFileDuration = midiFile.GetTimedEvents().LastOrDefault(e => e.Event is NoteOffEvent)?.TimeAs<MetricTimeSpan>(tempoMap) ?? new MetricTimeSpan();

            var timer = new Stopwatch();
            timer.Start();

            var song = new BmpSong
            {
                Title = Path.GetFileNameWithoutExtension(path),
                SourceTempoMap = midiFile.GetTempoMap().Clone(),
                TrackContainers = new Dictionary<long, TrackContainer>(),
                Duration = midiFileDuration
            };

            var trackChunkArray = midiFile.GetTrackChunks().ToArray();

            //Set note tracks at first
            List<int> skippedTracks = new List<int>();
            int index = 0;
            for (var i = 0; i < midiFile.GetTrackChunks().Count(); i++)
            {
                //ignore tracks without notes
                if (trackChunkArray[i].ManageNotes().Notes.Count() > 0)
                {
                    song.TrackContainers[index] = new TrackContainer { SourceTrackChunk = (TrackChunk)trackChunkArray[i].Clone() };
                    index++;
                }
                else
                    skippedTracks.Add(i);
            }
            //set the ignored tracks for data
            foreach (int i in skippedTracks)
            {
                song.TrackContainers[index] = new TrackContainer { SourceTrackChunk = (TrackChunk)trackChunkArray[i].Clone() };
                index++;
            }

            //check the tracks for data
            for (var i = 0; i < song.TrackContainers.Count(); i++)
            {
                song.TrackContainers[i].ConfigContainers = song.TrackContainers[i].SourceTrackChunk.ReadConfigs(i, song);
            }
            //process the tracks we've got
            Parallel.For(0, song.TrackContainers.Count, i =>
            {
                Parallel.For(0, song.TrackContainers[i].ConfigContainers.Count, async j =>
                {
                    switch (song.TrackContainers[i].ConfigContainers[j].ProcessorConfig)
                    {
                        case ClassicProcessorConfig classicConfig:
                            Console.WriteLine("Processing: Track:" + i + " ConfigContainer:" + j + " ConfigType:" +
                                              classicConfig.GetType() +
                                              " Instrument:" + classicConfig.Instrument + " OctaveRange:" +
                                              classicConfig.OctaveRange + " PlayerCount:" + classicConfig.PlayerCount +
                                              " IncludeTracks:" + string.Join(",", classicConfig.IncludedTracks));
                            song.TrackContainers[i].ConfigContainers[j].ProccesedTrackChunks =
                                await song.TrackContainers[i].ConfigContainers[j].RefreshTrackChunks(song);
                            break;
                        case LyricProcessorConfig lyricConfig:
                            Console.WriteLine("Processing: Track:" + i + " ConfigContainer:" + j + " ConfigType:" +
                                              lyricConfig.GetType() + " PlayerCount:" + lyricConfig.PlayerCount +
                                              " IncludeTracks:" + string.Join(",", lyricConfig.IncludedTracks));
                            song.TrackContainers[i].ConfigContainers[j].ProccesedTrackChunks =
                                await song.TrackContainers[i].ConfigContainers[j].RefreshTrackChunks(song);
                            break;
                        case VSTProcessorConfig vstConfig:
                            Console.WriteLine("Processing: Track:" + i + " ConfigContainer:" + j + " ConfigType:" +
                                              vstConfig.GetType() + " PlayerCount:" + vstConfig.PlayerCount +
                                              " IncludeTracks:" + string.Join(",", vstConfig.IncludedTracks));
                            song.TrackContainers[i].ConfigContainers[j].ProccesedTrackChunks =
                                await song.TrackContainers[i].ConfigContainers[j].RefreshTrackChunks(song);
                            break;
                        default:
                            Console.WriteLine("error unknown config.");
                            break;
                    }
                });
            });
            skippedTracks.Clear();

            timer.Stop();
            var timeTaken = timer.Elapsed;
            Console.WriteLine("Time taken: " + timeTaken.ToString(@"m\:ss\.fff"));

            return song;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<MidiFile> GetProcessedMidiFile()
        {
            var sourceMidiData = new MidiFile(TrackContainers.Values.SelectMany(track => track.ConfigContainers).SelectMany(track => track.Value.ProccesedTrackChunks));
            sourceMidiData.ReplaceTempoMap(Tools.GetMsTempoMap());
            var midiFile = new MidiFile();
            if (sourceMidiData.GetNotes().Count < 1) return Task.FromResult(midiFile);
            var delta = sourceMidiData.GetNotes().First().Time;
            foreach (var trackChunk in sourceMidiData.GetTrackChunks())
            {
                var trackName = trackChunk.Events.OfType<SequenceTrackNameEvent>().First().Text;
                if (trackName.StartsWith("tone:"))
                {
                    var newTrackChunk = new TrackChunk(new SequenceTrackNameEvent(trackName));
                    var newNotes = new List<Note>();
                    foreach (var note in trackChunk.GetNotes())
                    {
                        if (note.Time - delta < 0) continue; // TODO: log this error, though this shouldn't be possible.
                        note.Time -= delta;
                        newNotes.Add(note);
                    }
                    newTrackChunk.AddObjects(newNotes);
                    midiFile.Chunks.Add(newTrackChunk);
                }
                else if (trackName.StartsWith("lyric:"))
                {
                    var newTrackChunk = new TrackChunk(new SequenceTrackNameEvent(trackName));
                    var newLyrics = new List<TimedEvent>();
                    foreach (var midiEvent in trackChunk.GetTimedEvents().Where(e => e.Event.EventType == MidiEventType.Lyric))
                    {
                        if (midiEvent.Time - delta < 0) continue; // TODO: log that you cannot have lyrics come before the first note.
                        midiEvent.Time -= delta;
                        newLyrics.Add(midiEvent);
                    }
                    newTrackChunk.AddObjects(newLyrics);
                    midiFile.Chunks.Add(newTrackChunk);
                }
            }
            midiFile.ReplaceTempoMap(Tools.GetMsTempoMap());
            return Task.FromResult(midiFile);
        }

        /// <summary>
        /// Creates a midi from the song for the sequencer
        /// </summary>
        /// <returns>MemoryStream</returns>
        public MemoryStream GetSequencerMidi()
        {
            try
            {
                List<TrackChunk> c = new List<TrackChunk>();
                foreach (var tc in TrackContainers.Values)
                    c.Add(tc.SourceTrackChunk);

                var midiFile = new MidiFile(c);
                midiFile.ReplaceTempoMap(SourceTempoMap);

                Console.WriteLine("Scrubbing ");
                var loaderWatch = Stopwatch.StartNew();
                var newTrackChunks = new ConcurrentDictionary<int, TrackChunk>();
                var tempoMap = midiFile.GetTempoMap().Clone();
                long firstNote = midiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000;

                var originalTrackChunks = new List<TrackChunk>();

                TrackChunk allTracks = new TrackChunk();
                allTracks.AddObjects(originalTrackChunks.GetNotes());

                foreach (var trackChunk in midiFile.GetTrackChunks())
                {
                    if (trackChunk.Events.Count > 0)
                    {
                        if (trackChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text != null)
                            if ((trackChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text).Contains("lyric:"))
                                continue;
                        allTracks.AddObjects(trackChunk.GetNotes());
                        allTracks.AddObjects(trackChunk.GetTimedEvents());
                    }
                    var thisTrack = new TrackChunk(new SequenceTrackNameEvent(trackChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text));
                    thisTrack.AddObjects(trackChunk.GetNotes());
                    thisTrack.AddObjects(trackChunk.GetTimedEvents());
                    originalTrackChunks.Add(thisTrack);
                }
                originalTrackChunks.Add(allTracks);

                Parallel.ForEach(originalTrackChunks.Where(x => x.GetNotes().Any() || x.Events.OfType<LyricEvent>().Any() ), (originalChunk, loopState, index) =>
                {
                    var watch = Stopwatch.StartNew();
                    var tempoMap = midiFile.GetTempoMap().Clone();
                    int noteVelocity = int.Parse(index.ToString()) + 1;

                    Dictionary<int, Dictionary<long, Note>> allNoteEvents = new Dictionary<int, Dictionary<long, Note>>();
                    for (int i = 0; i < 127; i++) allNoteEvents.Add(i, new Dictionary<long, Note>());

                    foreach (Note note in originalChunk.GetNotes())
                    {
                        long noteOnMS;
                        long noteOffMS;

                        try
                        {
                            noteOnMS = 5000 + (note.GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote;
                            noteOffMS = 5000 + (note.GetTimedNoteOffEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote;
                        }
                        catch (Exception) { continue; }
                        int noteNumber = note.NoteNumber;

                        Note newNote = new Note((SevenBitNumber)noteNumber,
                                                time: noteOnMS,
                                                length: noteOffMS - noteOnMS
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
                    for (int i = 0; i < 127; i++)
                    {
                        long lastNoteTimeStamp = -1;
                        foreach (var noteEvent in allNoteEvents[i])
                        {
                            if (lastNoteTimeStamp >= 0 && allNoteEvents[i][lastNoteTimeStamp].Length + lastNoteTimeStamp >= noteEvent.Key)
                                allNoteEvents[i][lastNoteTimeStamp].Length = allNoteEvents[i][lastNoteTimeStamp].Length - (allNoteEvents[i][lastNoteTimeStamp].Length + lastNoteTimeStamp + 1 - noteEvent.Key);

                            lastNoteTimeStamp = noteEvent.Key;
                        }
                    }
                    newChunk.AddObjects(allNoteEvents.SelectMany(s => s.Value).Select(s => s.Value).ToArray());
                    allNoteEvents = null;
                    watch.Stop();
                    Debug.WriteLine("step 2: " + noteVelocity + ": " + watch.ElapsedMilliseconds);
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
                        if (lowestParent <= time + 30)
                        {
                            time = lowestParent - 30;
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
                            if (notesToFix[j + 1].Time <= notesToFix[j].Time + notesToFix[j].Length + 25)
                            {
                                dur = notesToFix[j + 1].Time - notesToFix[j].Time - 25;
                                dur = dur < 25 ? 1 : dur;
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
                    string trackName = originalChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text;
                    if (trackName == null) trackName = "";
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

                            if (octaveShift > 0)
                                trackName = trackName + "+" + octaveShift;
                            else if (octaveShift < 0)
                                trackName = trackName + octaveShift;
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
                            var programChangeEvent = timedEvent.Event as ProgramChangeEvent;
                            if (programChangeEvent == null)
                                continue;
                            //Skip all except guitar | implement if we need this again
                            if ((programChangeEvent.ProgramNumber < 27) || (programChangeEvent.ProgramNumber > 31))
                                continue;

                            var channel = programChangeEvent.Channel;
                            using (var manager = new TimedEventsManager(newChunk.Events))
                            {
                                TimedEventsCollection timedEvents = manager.Events;
                                timedEvents.Add(new TimedEvent(new ProgramChangeEvent(programChangeEvent.ProgramNumber), 5000 + (timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote/* Absolute time too */));
                            }
                        }
                    }
                    //Create lyrics
                    /*foreach (var timedEvent in originalChunk.GetTimedEvents())
                    {
                        var lyricsEvent = timedEvent.Event as LyricEvent;
                        if (lyricsEvent == null)
                            continue;

                        using (var manager = new TimedEventsManager(newChunk.Events))
                        {
                            TimedEventsCollection timedEvents = manager.Events;
                            timedEvents.Add(new TimedEvent(new LyricEvent(lyricsEvent.Text), 5000 + (timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote));
                        }
                    }*/

                    //Create Progchange Event
                    /*foreach (var timedEvent in originalChunk.GetTimedEvents())
                    {
                        var programChangeEvent = timedEvent.Event as ChannelAftertouchEvent;
                        if (programChangeEvent == null)
                            continue;

                        var channel = programChangeEvent.Channel;
                        using (var manager = new TimedEventsManager(newChunk.Events))
                        {
                            TimedEventsCollection timedEvents = manager.Events;
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
                using (TempoMapManager tempoManager = newMidiFile.ManageTempoMap()) tempoManager.SetTempo(0, Tempo.FromBeatsPerMinute(160));
                //old div
                //newMidiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision(600);
                //using (TempoMapManager tempoManager = newMidiFile.ManageTempoMap()) tempoManager.SetTempo(0, Tempo.FromBeatsPerMinute(100));
                newMidiFile.Chunks.AddRange(newTrackChunks.Values);

                tempoMap = newMidiFile.GetTempoMap();
                long delta = newMidiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000;

                Parallel.ForEach(newMidiFile.GetTrackChunks(), chunk =>
                {
                    using (var notesManager = chunk.ManageNotes())
                    {
                        foreach (Note note in notesManager.Notes)
                        {
                            long newStart = note.Time - delta;
                            note.Time = newStart;
                        }
                    }
                    using (var manager = chunk.ManageTimedEvents())
                    {
                        foreach (TimedEvent _event in manager.Events)
                        {
                            var programChangeEvent = _event.Event as ProgramChangeEvent;
                            if (programChangeEvent == null)
                                continue;

                            long newStart = _event.Time - delta;
                            if (newStart <= -1)
                                manager.Events.Remove(_event);
                            else
                                _event.Time = newStart;
                        }

                        foreach (TimedEvent _event in manager.Events)
                        {
                            var lyricsEvent = _event.Event as LyricEvent;
                            if (lyricsEvent == null)
                                continue;

                            long newStart = _event.Time - delta;
                            if (newStart <= -1)
                                manager.Events.Remove(_event);
                            else
                                _event.Time = newStart;
                        }

                        /*foreach (TimedEvent _event in manager.Events)
                        {
                            var programChangeEvent = _event.Event as ChannelAftertouchEvent;
                            if (programChangeEvent == null)
                                continue;

                            long newStart = _event.Time - delta;
                            if (newStart <= -1)
                                manager.Events.Remove(_event);
                            else
                                _event.Time = newStart;
                        }*/

                    }
                });

                var stream = new MemoryStream();

                using (var manager = new TimedEventsManager(newMidiFile.GetTrackChunks().First().Events))
                    manager.Events.Add(new TimedEvent(new MarkerEvent(), (newMidiFile.GetDuration<MetricTimeSpan>().TotalMicroseconds / 1000)));

                //newMidiFile.Write(stream, MidiFileFormat.MultiTrack, new WritingSettings { });

                newMidiFile.Write(stream, MidiFileFormat.MultiTrack, settings: new WritingSettings
                {
                    TextEncoding = System.Text.Encoding.UTF8
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
                throw ex;
            }
        }

        /// <summary>
        /// Exports the song to a midi file
        /// </summary>
        /// <returns></returns>
        public MemoryStream GetExportMidi()
        {

            List<TrackChunk> c = new List<TrackChunk>();
            foreach (var tc in TrackContainers.Values)
                c.Add(tc.SourceTrackChunk);

            var midiFile = new MidiFile(c);
            midiFile.ReplaceTempoMap(SourceTempoMap);

            var stream = new MemoryStream();

            using (var manager = new TimedEventsManager(midiFile.GetTrackChunks().First().Events))
                manager.Events.Add(new TimedEvent(new MarkerEvent(), (midiFile.GetDuration<MetricTimeSpan>().TotalMicroseconds / 1000)));

            midiFile.Write(stream, MidiFileFormat.MultiTrack, new WritingSettings { });
            stream.Flush();
            stream.Position = 0;

            return stream;
        }

        /// <summary>
        /// Exports the song to a midi file
        /// </summary>
        /// <returns></returns>
        public MemoryStream GetExportMidiB()
        {
            try
            {
                List<TrackChunk> c = new List<TrackChunk>();
                foreach (var tc in TrackContainers.Values)
                    c.Add(tc.SourceTrackChunk);

                var midiFile = new MidiFile(c);
                midiFile.ReplaceTempoMap(SourceTempoMap);


                Console.WriteLine("Exporting... ");
                var loaderWatch = Stopwatch.StartNew();
                var newTrackChunks = new ConcurrentDictionary<int, TrackChunk>();
                var tempoMap = midiFile.GetTempoMap().Clone();
                long firstNote = midiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000;

                var originalTrackChunks = new List<TrackChunk>();

                foreach (var trackChunk in midiFile.GetTrackChunks())
                {
                    var thisTrack = new TrackChunk(new SequenceTrackNameEvent(trackChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text));
                    thisTrack.AddObjects(trackChunk.GetNotes());
                    thisTrack.AddObjects(trackChunk.GetTimedEvents());
                    originalTrackChunks.Add(thisTrack);
                }

                Parallel.ForEach(originalTrackChunks.Where(x => x.GetNotes().Any()), (originalChunk, loopState, index) =>
                {
                    var watch = Stopwatch.StartNew();
                    var tempoMap = midiFile.GetTempoMap().Clone();
                    int noteVelocity = int.Parse(index.ToString()) + 1;

                    Dictionary<int, Dictionary<long, Note>> allNoteEvents = new Dictionary<int, Dictionary<long, Note>>();
                    for (int i = 0; i < 127; i++) allNoteEvents.Add(i, new Dictionary<long, Note>());

                    foreach (Note note in originalChunk.GetNotes())
                    {
                        long noteOnMS;
                        long noteOffMS;

                        try
                        {
                            noteOnMS = 5000 + (note.GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote;
                            noteOffMS = 5000 + (note.GetTimedNoteOffEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote;
                        }
                        catch (Exception) { continue; }
                        int noteNumber = note.NoteNumber;

                        Note newNote = new Note((SevenBitNumber)noteNumber,
                                                time: noteOnMS,
                                                length: noteOffMS - noteOnMS
                                                )
                        {
                            Channel = (FourBitNumber)index,
                            Velocity = (SevenBitNumber)126,
                            OffVelocity = (SevenBitNumber)126
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
                    for (int i = 0; i < 127; i++)
                    {
                        long lastNoteTimeStamp = -1;
                        foreach (var noteEvent in allNoteEvents[i])
                        {
                            if (lastNoteTimeStamp >= 0 && allNoteEvents[i][lastNoteTimeStamp].Length + lastNoteTimeStamp >= noteEvent.Key)
                                allNoteEvents[i][lastNoteTimeStamp].Length = allNoteEvents[i][lastNoteTimeStamp].Length - (allNoteEvents[i][lastNoteTimeStamp].Length + lastNoteTimeStamp + 1 - noteEvent.Key);

                            lastNoteTimeStamp = noteEvent.Key;
                        }
                    }
                    newChunk.AddObjects(allNoteEvents.SelectMany(s => s.Value).Select(s => s.Value).ToArray());
                    allNoteEvents = null;
                    watch.Stop();
                    Debug.WriteLine("step 2: " + noteVelocity + ": " + watch.ElapsedMilliseconds);
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
                            if (notesToFix[j + 1].Time <= notesToFix[j].Time + notesToFix[j].Length + 25)
                            {
                                dur = notesToFix[j + 1].Time - notesToFix[j].Time - 25;
                                dur = dur < 25 ? 1 : dur;
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
                    watch = Stopwatch.StartNew();

                    #region Tracknaming and octave shifting
                    int octaveShift = 0;
                    string trackName = originalChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text;

                    if (trackName == null) trackName = "";
                    trackName = trackName.ToLower().Trim().Replace(" ", String.Empty);
                    string o_trackName = trackName;
                    Regex rex = new Regex(@"^([A-Za-z]+)([-+]\d)?");
                    if (rex.Match(trackName) is Match match)
                    {
                        if (!string.IsNullOrEmpty(match.Groups[1].Value))
                        {
                            trackName = Instrument.Parse(match.Groups[1].Value).Name;
                            if (!string.IsNullOrEmpty(match.Groups[2].Value))
                                if (int.TryParse(match.Groups[2].Value, out int os))
                                    octaveShift = os;

                            if (octaveShift > 0)
                                trackName = trackName + "+" + octaveShift;
                            else if (octaveShift < 0)
                                trackName = trackName + octaveShift;
                        }

                        //last try with the program number
                        if ((string.IsNullOrEmpty(match.Groups[1].Value)) || trackName.Equals("Unknown") || trackName.Equals("None"))
                        {
                            ProgramChangeEvent prog = originalChunk.Events.OfType<ProgramChangeEvent>().FirstOrDefault();
                            if (prog != null)
                                trackName = Instrument.ParseByProgramChange(prog.ProgramNumber).Name;
                        }

                    }
                    newChunk = new TrackChunk(new SequenceTrackNameEvent(trackName));
                    #endregion Tracknaming and octave shifting

                    //Create Progchange Event
                    foreach (var timedEvent in originalChunk.GetTimedEvents())
                    {
                        var programChangeEvent = timedEvent.Event as ProgramChangeEvent;
                        if (programChangeEvent == null)
                            continue;
                        //Skip all except guitar | implement if we need this again
                        if ((programChangeEvent.ProgramNumber < 27) || (programChangeEvent.ProgramNumber > 31))
                            continue;

                        var channel = programChangeEvent.Channel;
                        using (var manager = new TimedEventsManager(newChunk.Events))
                        {
                            TimedEventsCollection timedEvents = manager.Events;
                            timedEvents.Add(new TimedEvent(new ProgramChangeEvent(programChangeEvent.ProgramNumber), 5000 + (timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote/* Absolute time too */));
                        }
                    }
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
                newMidiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision(600);
                using (TempoMapManager tempoManager = newMidiFile.ManageTempoMap()) tempoManager.SetTempo(0, Tempo.FromBeatsPerMinute(100));
                newMidiFile.Chunks.AddRange(newTrackChunks.Values);

                tempoMap = newMidiFile.GetTempoMap();
                long delta = newMidiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000;

                Parallel.ForEach(newMidiFile.GetTrackChunks(), chunk =>
                {
                    var te = chunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text;
                    var channel = chunk.Events.OfType<NoteOnEvent>().FirstOrDefault()?.Channel;
                    using (var manager = chunk.ManageTimedEvents())
                    {
                        var prog = new ProgramChangeEvent((SevenBitNumber)Instrument.Parse(te).MidiProgramChangeCode);
                        prog.Channel = (FourBitNumber)channel;
                        manager.Events.Add(new TimedEvent(prog, 5000));
                    }

                    using (var notesManager = chunk.ManageNotes())
                    {
                        foreach (Note note in notesManager.Notes)
                        {
                            long newStart = note.Time - delta;
                            note.Time = newStart;
                        }
                    }
                    using (var manager = chunk.ManageTimedEvents())
                    {
                        foreach (TimedEvent _event in manager.Events)
                        {
                            var programChangeEvent = _event.Event as ProgramChangeEvent;
                            if (programChangeEvent == null)
                                continue;

                            long newStart = _event.Time - delta;
                            if (newStart <= -1)
                                manager.Events.Remove(_event);
                            else
                                _event.Time = newStart;
                        }
                    }
                });

                var stream = new MemoryStream();

                using (var manager = new TimedEventsManager(newMidiFile.GetTrackChunks().First().Events))
                    manager.Events.Add(new TimedEvent(new MarkerEvent(), (newMidiFile.GetDuration<MetricTimeSpan>().TotalMicroseconds / 1000)));

                newMidiFile.Write(stream, MidiFileFormat.MultiTrack, new WritingSettings { });
                stream.Flush();
                stream.Position = 0;

                loaderWatch.Stop();
                Console.WriteLine("Export finished MS: " + loaderWatch.ElapsedMilliseconds);
                
                return stream;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
    }
}
