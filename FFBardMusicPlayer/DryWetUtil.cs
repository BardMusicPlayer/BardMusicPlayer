using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Sharlayan.Core.Enums.Performance;

namespace FFBardMusicPlayer
{
    class DryWetUtil
    {
        private static string[] InstrumentEnumNamesAsStringsSorted = Array.ConvertAll((Instrument[])Enum.GetValues(typeof(Instrument)), s => s.ToString()).OrderByDescending(s => s.Length).ToArray();

        private static string lastMD5 = "invalid";
        private static MidiFile lastFile = null;

        public static MemoryStream ScrubFile(string filePath)
        {
            MidiFile midiFile;
            IEnumerable<TrackChunk> originalTrackChunks;
            TempoMap tempoMap;

            MidiFile newMidiFile;
            ConcurrentDictionary<int, TrackChunk> newTrackChunks;

            try
            {
                string md5 = CalculateMD5(filePath);
                if (lastMD5.Equals(md5) && lastFile != null)
                {
                    var oldfile = new MemoryStream();
                    lastFile.Write(oldfile, MidiFileFormat.MultiTrack, new WritingSettings { CompressionPolicy = CompressionPolicy.NoCompression });
                    oldfile.Flush();
                    oldfile.Position = 0;
                    return oldfile;
                }

                if (Path.GetExtension(filePath).ToLower().Equals(".mmsong"))
                {
                    midiFile = Plugin_MMsong.Load(filePath).Clone();
                }
                else
                {
                    midiFile = MidiFile.Read(filePath, new ReadingSettings
                    {
                        ReaderSettings = new ReaderSettings
                        {
                            ReadFromMemory = true
                        },
                        InvalidChunkSizePolicy = InvalidChunkSizePolicy.Ignore,
                        InvalidMetaEventParameterValuePolicy = InvalidMetaEventParameterValuePolicy.SnapToLimits,
                        InvalidChannelEventParameterValuePolicy = InvalidChannelEventParameterValuePolicy.SnapToLimits,
                        InvalidSystemCommonEventParameterValuePolicy = InvalidSystemCommonEventParameterValuePolicy.SnapToLimits,
                        MissedEndOfTrackPolicy = MissedEndOfTrackPolicy.Ignore,
                        NotEnoughBytesPolicy = NotEnoughBytesPolicy.Ignore,
                        UnexpectedTrackChunksCountPolicy = UnexpectedTrackChunksCountPolicy.Ignore,
                        UnknownChannelEventPolicy = UnknownChannelEventPolicy.SkipStatusByteAndOneDataByte,
                        UnknownChunkIdPolicy = UnknownChunkIdPolicy.ReadAsUnknownChunk
                    });

                    #region Require

                    if (midiFile == null)
                    {
                        throw new ArgumentNullException();
                    }
                    else
                    {
                        try
                        {
                            if (midiFile.Chunks.Count < 1) throw new NotSupportedException();

                            MidiFileFormat fileFormat = midiFile.OriginalFormat;

                            if (fileFormat == MidiFileFormat.MultiSequence)
                            {
                                throw new NotSupportedException();
                            }
                        }
                        catch (Exception exception) when (exception is UnknownFileFormatException || exception is InvalidOperationException)
                        {
                            throw exception;
                        }
                    }
                    #endregion
                }

                Console.WriteLine("Scrubbing " + filePath);
                var loaderWatch = Stopwatch.StartNew();

                originalTrackChunks = midiFile.GetTrackChunks();

                tempoMap = midiFile.GetTempoMap();
                newTrackChunks = new ConcurrentDictionary<int, TrackChunk>();

                long firstNote = originalTrackChunks.GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000;

                TrackChunk allTracks = new TrackChunk();
                allTracks.AddNotes(originalTrackChunks.GetNotes());
                midiFile.Chunks.Add(allTracks);
                originalTrackChunks = midiFile.GetTrackChunks();

                Parallel.ForEach(originalTrackChunks.Where(x => x.GetNotes().Count() > 0), (originalChunk, loopState, index) =>
                {
                    var watch = Stopwatch.StartNew();

                    int noteVelocity = int.Parse(index.ToString()) + 1;

                    Dictionary<int, Dictionary<long, Note>> allNoteEvents = new Dictionary<int, Dictionary<long, Note>>();
                    for (int i = 0; i < 127; i++) allNoteEvents.Add(i, new Dictionary<long, Note>());

                    foreach (Note note in originalChunk.GetNotes())
                    {
                        long noteOnMS = 0;

                        long noteOffMS = 0;

                        try
                        {
                            noteOnMS = 5000 + (note.GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote;
                            noteOffMS = 5000 + (note.GetTimedNoteOffEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000) - firstNote;
                        }
                        catch (Exception) { continue; }
                        int noteNumber = note.NoteNumber;

                        Note newNote = new Note(noteNumber: (SevenBitNumber)noteNumber,
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
                            {
                                allNoteEvents[i][lastNoteTimeStamp].Length = allNoteEvents[i][lastNoteTimeStamp].Length - (allNoteEvents[i][lastNoteTimeStamp].Length + lastNoteTimeStamp + 1 - noteEvent.Key);
                            }

                            lastNoteTimeStamp = noteEvent.Key;
                        }
                    }
                    newChunk.AddNotes(allNoteEvents.SelectMany(s => s.Value).Select(s => s.Value).ToArray());
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

                    int octaveShift = 0;
                    string trackName = originalChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text;
                    if (trackName == null) trackName = "";
                    trackName = trackName.ToLower().Trim().Replace(" ", String.Empty);
                    Regex rex = new Regex(@"^([A-Za-z]+)([-+]\d)?");
                    if (rex.Match(trackName) is Match match)
                    {
                        if (!string.IsNullOrEmpty(match.Groups[1].Value))
                        {
                            trackName = match.Groups[1].Value;
                            if (!string.IsNullOrEmpty(match.Groups[2].Value)) if (int.TryParse(match.Groups[2].Value, out int os)) octaveShift = os;

                            (bool success, string parsedTrackName) = TrackNameToEnumInstrumentName(trackName);

                            if (success) trackName = parsedTrackName;
                            else
                            {
                                (success, parsedTrackName) = TrackNameToStringInstrumentName(trackName);

                                if (success) trackName = parsedTrackName;
                                else
                                {
                                    var originalInstrument = originalChunk.Events.OfType<ProgramChangeEvent>().FirstOrDefault()?.ProgramNumber;
                                    if (!(originalInstrument is null) && originalInstrument.Equals(typeof(SevenBitNumber))) (success, parsedTrackName) = ProgramToStringInstrumentName((SevenBitNumber)originalInstrument);
                                    if (success) trackName = parsedTrackName;
                                }
                            }

                            if (octaveShift > 0) trackName = trackName + "+" + octaveShift;
                            else if (octaveShift < 0) trackName = trackName + octaveShift;
                        }
                    }

                    newChunk = new TrackChunk(new SequenceTrackNameEvent(trackName));
                    newChunk.AddNotes(fixedNotes);
                    fixedNotes = null;

                    watch.Stop();
                    Debug.WriteLine("step 5: " + noteVelocity + ": " + watch.ElapsedMilliseconds);
                    watch = Stopwatch.StartNew();

                    newTrackChunks.TryAdd(noteVelocity, newChunk);

                    watch.Stop();
                    Debug.WriteLine("step 6: " + noteVelocity + ": " + watch.ElapsedMilliseconds);

                });

                newMidiFile = new MidiFile();
                newTrackChunks.TryRemove(newTrackChunks.Count, out TrackChunk trackZero);
                newMidiFile.Chunks.Add(trackZero);
                newMidiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision(600);
                using (TempoMapManager tempoManager = newMidiFile.ManageTempoMap()) tempoManager.SetTempo(0, Tempo.FromBeatsPerMinute(100));
                newMidiFile.Chunks.AddRange(newTrackChunks.Values);

                tempoMap = newMidiFile.GetTempoMap();
                long delta = newMidiFile.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000;
                foreach (TrackChunk chunk in newMidiFile.GetTrackChunks())
                {
                    using (var notesManager = chunk.ManageNotes())
                    {
                        foreach (Note note in notesManager.Notes)
                        {
                            long newStart = note.Time - delta;
                            note.Time = newStart;
                        }
                    }
                }

                var stream = new MemoryStream();
                
                using (var manager = new TimedEventsManager(newMidiFile.GetTrackChunks().First().Events))
                    manager.Events.Add(new TimedEvent(new MarkerEvent(), (newMidiFile.GetDuration<MetricTimeSpan>().TotalMicroseconds / 1000) + 100));

                newMidiFile.Write(stream, MidiFileFormat.MultiTrack, new WritingSettings { CompressionPolicy = CompressionPolicy.NoCompression });
                stream.Flush();
                stream.Position = 0;

                loaderWatch.Stop();
                Console.WriteLine("Scrubbing MS: " + loaderWatch.ElapsedMilliseconds);

                lastMD5 = md5;
                lastFile = newMidiFile;

                return stream;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            finally
            {
                newTrackChunks = null;
                tempoMap = null;
                originalTrackChunks = null;
                midiFile = null;
            }
        }

        private static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", String.Empty).ToLowerInvariant();
                }
            }
        }

        private static (bool, string) TrackNameToEnumInstrumentName(string trackName)
        {
            if (string.IsNullOrEmpty(trackName)) return (false, trackName);
            foreach (string ins in InstrumentEnumNamesAsStringsSorted) if (trackName.Contains(ins.ToString().ToLower())) return (true, ins.ToString());
            return (false, trackName);
        }

        private static (bool, string) TrackNameToStringInstrumentName(string trackName)
        {
            if (string.IsNullOrEmpty(trackName)) return (false, trackName);
            switch (trackName)
            {
                case "harp":
                case "orchestralharp":
                case "orchestralharps":
                case "harps": return (true, "Harp");
                case "piano":
                case "acousticgrandpiano":
                case "acousticgrandpianos":
                case "pianos": return (true, "Piano");
                case "lute":
                case "guitar":
                case "guitars":
                case "lutes": return (true, "Lute");
                case "fiddle":
                case "pizzicatostrings":
                case "pizzicatostring":
                case "fiddles": return (true, "Fiddle");
                case "flute":
                case "flutes": return (true, "Flute");
                case "oboe":
                case "oboes": return (true, "Oboe");
                case "clarinet":
                case "clarinets": return (true, "Clarinet");
                case "fife":
                case "piccolo":
                case "piccolos":
                case "fifes":
                case "ocarina":
                case "ocarinas": return (true, "Fife");
                case "panpipes":
                case "panflute":
                case "panflutes":
                case "panpipe": return (true, "Panpipes");
                case "timpani":
                case "timpanis": return (true, "Timpani");
                case "bongos":
                case "bongo": return (true, "Bongo");
                case "bass_drum":
                case "bass_drums":
                case "bassdrum":
                case "bassdrums": return (true, "BassDrum");
                case "snaredrum":
                case "snare_drum":
                case "snare_drums":
                case "snare":
                case "snares": return (true, "SnareDrum");
                case "cymbal":
                case "cymbals": return (true, "Cymbal");
                case "trumpet":
                case "trumpets": return (true, "Trumpet");
                case "trombone":
                case "trombones": return (true, "Trombone");
                case "tuba":
                case "tubas": return (true, "Tuba");
                case "horn":
                case "frenchhorn":
                case "frenchhorns":
                case "horns": return (true, "Horn");
                case "saxophone":
                case "sax":
                case "altosax":
                case "altosaxophone":
                case "saxophones": return (true, "Saxophone");
                case "violin":
                case "violins": return (true, "Violin");
                case "viola":
                case "violas": return (true, "Viola");
                case "cello":
                case "cellos": return (true, "Cello");
                case "doublebass":
                case "double_bass":
                case "contrabass": return (true, "DoubleBass");

                default: return (false, trackName);
            }
        }

        private static (bool, string) ProgramToStringInstrumentName(SevenBitNumber prog)
        {
            if (prog.Equals(null)) return (false, null);
            switch (prog)
            {
                case 46: return (true, "Harp");

                case 0:
                case 1: return (true, "Piano");

                case 24: return (true, "Lute");

                case 6:
                case 35:
                case 45: return (true, "Fiddle");

                case 73: return (true, "Flute");

                case 68: return (true, "Oboe");

                case 71: return (true, "Clarinet");

                case 72:
                case 79: return (true, "Fife");

                case 75: return (true, "Panpipes");

                case 47: return (true, "Timpani");

                //
                //
                //
                //

                case 56:
                case 59: return (true, "Trumpet");

                case 57: return (true, "Trombone");

                case 58: return (true, "Tuba");

                case 60:
                case 61:
                case 62:
                case 63: return (true, "Horn");

                case 64:
                case 65:
                case 66:
                case 67: return (true, "Saxophone");

                case 40: return (true, "Violin");

                case 41: return (true, "Viola");

                case 42: return (true, "Cello");

                case 43: return (true, "DoubleBass");
            }
            return (true, null);
        }
    }
}
