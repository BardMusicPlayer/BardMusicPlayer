using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FFBardMusicPlayer
{
    class DryWetUtil
    {

        public static Sequence ScrubFile(string filePath)
        {
            MidiFile midiFile;
            IEnumerable<TrackChunk> originalTrackChunks;
            TempoMap tempoMap;

            MidiFile newMidiFile;
            ConcurrentDictionary<int, TrackChunk> newTrackChunks;

            Sequence sequence = null;

            try
            {
                midiFile = MidiFile.Read(filePath, new ReadingSettings
                {
                    ReaderSettings = new ReaderSettings
                    {
                        ReadFromMemory = true
                    }
                });

                bool explode = false;

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
                        else if (fileFormat == MidiFileFormat.SingleTrack)
                        {
                            explode = true;
                        }
                    }
                    catch (Exception exception) when (exception is UnknownFileFormatException || exception is InvalidOperationException)
                    {
                        throw exception;
                    }
                }
                #endregion

                if (explode || midiFile.Chunks.Count == 1) originalTrackChunks = midiFile.GetTrackChunks().First().Explode();
                else originalTrackChunks = midiFile.GetTrackChunks();

                tempoMap = midiFile.GetTempoMap();
                newTrackChunks = new ConcurrentDictionary<int, TrackChunk>();

                bool skipFirst = originalTrackChunks.First().GetNotes().Count() > 0 ? false : true;

                long firstNote = originalTrackChunks.GetNotes().First().GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000;

                Parallel.ForEach(originalTrackChunks, (originalChunk, loopState, index) =>
                {
                    if (!(skipFirst && index == 0))
                    {
                        int noteVelocity = int.Parse(index.ToString()) + (skipFirst ? 0 : 1);

                        Dictionary<int, Dictionary<long, Note>> allNoteEvents = new Dictionary<int, Dictionary<long, Note>>();
                        for (int i = 0; i < 127; i++) allNoteEvents.Add(i, new Dictionary<long, Note>());

                        // Fill the track dictionary and remove duplicate notes
                        foreach (Note note in originalChunk.GetNotes())
                        {
                            long noteOnMS = 1000 + note.GetTimedNoteOnEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000 - firstNote;
                            long noteOffMS = 1000 + note.GetTimedNoteOffEvent().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000 - firstNote;
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
                                if (previousNote.Length < note.Length) allNoteEvents[noteNumber][noteOnMS] = newNote; // keep the longest of all duplicates
                            }
                            else allNoteEvents[noteNumber].Add(noteOnMS, newNote);
                        }

                        // Merge all the dictionaries into one collection
                        TrackChunk newChunk = new TrackChunk();
                        for (int i = 0; i < 127; i++)
                        {
                            long lastNoteTimeStamp = -1;
                            Dictionary<long, Note> noteEvents = allNoteEvents[i];
                            foreach (var noteEvent in noteEvents)
                            {
                                if (lastNoteTimeStamp >= 0 && noteEvents[lastNoteTimeStamp].Length + lastNoteTimeStamp >= noteEvent.Key)
                                {
                                    noteEvents[lastNoteTimeStamp].Length = noteEvents[lastNoteTimeStamp].Length - (noteEvents[lastNoteTimeStamp].Length + lastNoteTimeStamp + 1 - noteEvent.Key);
                                }

                                lastNoteTimeStamp = noteEvent.Key;
                            }
                            newChunk.AddNotes(noteEvents.Values);
                            noteEvents = null;
                        }
                        allNoteEvents = null;

                        // auto arpeggiate
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

                        // Discover the instrument name from the track title, and from program changes if that fails
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
                                (bool success, string parsedTrackName) = TrackNameToInstrumentName(trackName);

                                if (success) trackName = parsedTrackName;
                                else
                                {
                                    var originalInstrument = originalChunk.Events.OfType<ProgramChangeEvent>().FirstOrDefault()?.ProgramNumber;
                                    if (!(originalInstrument is null) && originalInstrument.Equals(typeof(SevenBitNumber))) (success, parsedTrackName) = ProgramToInstrumentName((SevenBitNumber)originalInstrument);
                                    if (success) trackName = parsedTrackName;
                                }

                                if (octaveShift > 0) trackName = trackName + "+" + octaveShift;
                                else if (octaveShift < 0) trackName = trackName + octaveShift;
                            }
                        }

                        newChunk = new TrackChunk(new SequenceTrackNameEvent(trackName));
                        newChunk.AddNotes(fixedNotes);
                        fixedNotes = null;

                        newTrackChunks.TryAdd(noteVelocity, newChunk);
                    }
                });

                // Fill a midi file with the new track chunks
                newMidiFile = new MidiFile();
                newMidiFile.Chunks.Add(new TrackChunk());
                newMidiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision(600);
                using (TempoMapManager tempoManager = newMidiFile.ManageTempoMap()) tempoManager.SetTempo(0, Tempo.FromBeatsPerMinute(100));
                newMidiFile.Chunks.AddRange(newTrackChunks.Values);
#if DEBUG
                newMidiFile.Write("drywet-scrubber-debug.mid", true, MidiFileFormat.MultiTrack, new WritingSettings { CompressionPolicy = CompressionPolicy.NoCompression });
#endif
                // Write the midi file out into a memory stream and pass that to sanford to create a sanford sequence object
                using (var stream = new MemoryStream())
                {
                    newMidiFile.Write(stream, MidiFileFormat.MultiTrack, new WritingSettings { CompressionPolicy = CompressionPolicy.NoCompression });
                    stream.Flush();
                    stream.Position = 0;
                    sequence = new Sequence(stream);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                newTrackChunks = null;
                newMidiFile = null;

                tempoMap = null;
                originalTrackChunks = null;
                midiFile = null;
            }

            return sequence;
        }

        private static (bool, string) TrackNameToInstrumentName(string trackName)
        {
            if (string.IsNullOrEmpty(trackName)) return (false, null);
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
                default: return (false, trackName);
            }
        }

        private static (bool, string) ProgramToInstrumentName(SevenBitNumber prog)
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
            }
            return (true, null);
        }


    }
}
