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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MogLib.Common.Structs;
using MogLib.Notate.Objects;

namespace FFBardMusicPlayer
{
    class DryWetUtil
    {
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
                    lastFile.Write(oldfile, MidiFileFormat.MultiTrack,
                        new WritingSettings { TextEncoding = Encoding.ASCII });
                    oldfile.Flush();
                    oldfile.Position = 0;
                    return oldfile;
                }

                if (Path.GetExtension(filePath).ToLower().Equals(".mmsong"))
                {
                    var mmSongStream = MMSong.Open(filePath).GetMidiFile(false, false);
                    lastFile = MidiFile.Read(mmSongStream);
                    lastMD5 = md5;
                    mmSongStream.Position = 0;
                    return mmSongStream;
                }
                
                midiFile = MidiFile.Read(filePath, new ReadingSettings
                {
                    TextEncoding = Encoding.UTF8,
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

                var trackZeroName = midiFile.GetTrackChunks().First().Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text;

                if (!string.IsNullOrEmpty(trackZeroName) && (trackZeroName.ToLower().Contains("mogamp") || trackZeroName.ToLower().Contains("mognotate")))
                {
                    var notateConfig = NotateConfig.GenerateConfigFromMidiFile(filePath);
                    var mmSongStream = notateConfig.Transmogrify().GetMidiFile(false, false);
                    lastFile = MidiFile.Read(mmSongStream);
                    lastMD5 = md5;
                    mmSongStream.Position = 0;
                    return mmSongStream;
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

                            trackName = Instrument.Parse(trackName);

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
    }
}