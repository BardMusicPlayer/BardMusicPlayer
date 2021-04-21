/*
 * MogLib/Common/Objects/MMSong.cs
 *
 * Copyright (C) 2021  MoogleTroupe
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using ServiceStack;
using BardMusicPlayer.Common;
using BardMusicPlayer.Common.Structs;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using ServiceStack.Text;
using static BardMusicPlayer.Notate.Objects.NotateConfig.NotateGroup;
using Note = Melanchall.DryWetMidi.Interaction.Note;

namespace BardMusicPlayer.Notate.Objects
{
    public sealed class MMSong
    {
        public int schemaVersion { get; internal set; } = Constants.SchemaVersion;
        public NotateConfig notateConfig { get; internal set; } = new();
        public byte[] sourceMidiFile { get; internal set; } = new byte[0];
        public string title { get; internal set; } = "";
        public string description { get; internal set; } = "";
        public string[] tags { get; internal set; } = new string[0];
        public List<Bard> bards { get; internal set; } = new();
        public List<Singer> singers { get; internal set; } = new();

        public abstract class Group
        {
            internal Group()
            {
            }

            public string description { get; internal set; } = "";
            public Dictionary<long, int> sequence { get; internal set; } = new();
        }

        public sealed class Bard : Group
        {
            public Dictionary<VST, Instrument> instruments { get; internal set; } = new() { { VST.VST0, Instrument.None } };
        }

        public sealed class Singer : Group
        {
            public Dictionary<int, string> lines { get; internal set; } = new();
        }

        /// <summary>
        /// Decompresses and Deserializes a MMSong from a File
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static MMSong Open(string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return DecompressAndDeserialize(stream, filePath);
        }

        /// <summary>
        /// Decompresses and Deserializes a MMSong from a Stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static MMSong DecompressAndDeserialize(Stream stream, string filePath = "")
        {
            using var compressor = new GZipStream(stream, CompressionMode.Decompress);
            using var reader = new StreamReader(compressor);
            using (JsConfig.With(new Config
            {
                TextCase = TextCase.CamelCase,
                PropertyConvention = PropertyConvention.Lenient,
                IncludePublicFields = false,
                ExcludeDefaultValues = false,
            }))
            {
                var sb = new StringBuilder();
                while (!reader.EndOfStream) sb.Append(reader.ReadLine());
                var json = sb.ToString();
                var mmSong = JsonSerializer.DeserializeFromString<MMSong>(json);
                if (mmSong == null) throw new BmpSchemaInvalidException();
                switch (mmSong.schemaVersion)
                {
                    case > Constants.SchemaVersion:
                        throw new BmpSchemaVersionException(mmSong.schemaVersion);
                    case < 3:
                        mmSong = MMSongLegacy.DeserializeFromString(json);
                        mmSong.notateConfig.lastFilePath = Path.GetDirectoryName(Path.GetFullPath(filePath));
                        return mmSong;
                    default:
                        mmSong.notateConfig.lastFilePath = Path.GetDirectoryName(Path.GetFullPath(filePath));
                        return mmSong;
                }
            }
        }
    }

    public static class MMSongExtensions
    {
        /// <summary>
        /// Serializes and Compresses this MMSong into a File
        /// </summary>
        /// <param name="mmSong"></param>
        /// <param name="fileName"></param> 
        public static string Save(this MMSong mmSong, string fileName = "")
        {
            if (fileName.IsNullOrEmpty())
            {
                if (mmSong.notateConfig.midiFileName.IsNullOrEmpty() || mmSong.notateConfig.lastFilePath.IsNullOrEmpty())
                    throw new BmpFileException("No fileName was specified and no folder path is known to save in.");
                fileName = mmSong.notateConfig.lastFilePath + "\\" + Path.GetFileNameWithoutExtension(mmSong.notateConfig.midiFileName) + ".mmsong";
            }
            if (!fileName.ToLower().EndsWith(".mmsong")) throw new BmpFileException("fileName does not end in .mmsong");
            using var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            SerializeAndCompress(mmSong, stream);
            return fileName;    
        }

        /// <summary>
        /// Serializes and Compresses this MMSong into a Stream
        /// </summary>
        /// <param name="mmSong"></param>
        /// <param name="stream"></param> 
        public static void SerializeAndCompress(this MMSong mmSong, Stream stream)
        {
            using var compressor = new GZipStream(stream, CompressionLevel.Optimal);
            using (JsConfig.With(new Config
            {
                TextCase = TextCase.CamelCase,
                PropertyConvention = PropertyConvention.Lenient,
                IncludePublicFields = false,
                ExcludeDefaultValues = false,
            }))
            {
                JsonSerializer.SerializeToStream(mmSong, compressor);
            }
        }

        /// <summary>
        /// Get a MemoryStream containing the source Midi file used to create this MMSong
        /// </summary>
        /// <param name="mmSong"></param>
        /// <returns></returns>
        public static MemoryStream GetSourceMidiFile(this MMSong mmSong) => new (mmSong.sourceMidiFile);

        /// <summary>
        /// Get a MemoryStream containing a type 1 Midi file created from this MMSong
        /// </summary>
        /// <param name="mmSong"></param>
        /// <param name="includeSingerTracks">Include Singer tracks</param>
        /// <param name="forceOldStyleOctaves">if notes should all be put in the c3-c6 range</param>
        /// <param name="applyInstrumentOffsets">if instruments should have timing offsets applied. Note, this generated midi file should *not* be re-imported if this is used.</param>
        public static MemoryStream GetMidiFile(this MMSong mmSong, bool includeSingerTracks = true, bool forceOldStyleOctaves = false, bool applyInstrumentOffsets = false)
        {
            var sequence = new MidiFile();
            var chunkZero = new TrackChunk();
            var vst = 0;
            Dictionary<Instrument, int> vstMapping = new();
            using (var timedEventsManager = chunkZero.ManageTimedEvents())
            {
                timedEventsManager.Events.AddEvent(new SequenceTrackNameEvent("Generated by MogLib schemaVersion " + Constants.SchemaVersion), time: 0);
                foreach (var instrument in mmSong.bards.SelectMany(bard => bard.instruments.Values).Distinct().ToList())
                {
                    if (vst == 9) vst = 10;
                    if (vst == 16) vst = 15;
                    else timedEventsManager.Events.AddEvent(new ProgramChangeEvent((SevenBitNumber)instrument.MidiProgramChangeCode) {Channel = (FourBitNumber)vst}, 0);
                    vstMapping.Add(instrument, vst);
                    vst++;
                }
            }
            sequence.Chunks.Add(chunkZero);

            foreach (var bard in mmSong.bards)
            {
                var notes = new List<Note>();
                var failure = false;
                long lastTime = 0;
                var lastNote = 254;
                var currentInstrument = bard.instruments[VST.VST0];
                var channel = vstMapping[currentInstrument];
                var trackName = new StringBuilder();
                foreach (var bardVst in bard.instruments)
                {
                    if (trackName.Length > 0) trackName.Append(",");
                    trackName.Append(bardVst.Value.GetDefaultTrackName());
                }
                foreach (var sEvent in bard.sequence.Where(_ => !failure))
                {
                    if (lastNote == 254)
                    {
                        switch (sEvent.Value)
                        {
                            case > 120: // VST Switch
                                currentInstrument = bard.instruments[(VST) sEvent.Value];
                                channel = vstMapping[currentInstrument];
                                notes.Add(new Note((SevenBitNumber) sEvent.Value, 25, sEvent.Key * 25)
                                {
                                    Channel = (FourBitNumber) channel,
                                    Velocity = (SevenBitNumber) 1,
                                    OffVelocity = (SevenBitNumber) 0
                                });
                                break;
                            case <= 60 and >= 24 when sEvent.Key * 25 % 100 == 50 || sEvent.Key * 25 % 100 == 0:
                                lastNote = sEvent.Value;
                                if(forceOldStyleOctaves ? !OctaveRange.C3toC6.TryShiftNoteToOctave(OctaveRange.C1toC4, ref lastNote) : !currentInstrument.TryShiftNoteToDefaultOctave(OctaveRange.C1toC4, ref lastNote)) failure = true;
                                lastTime = sEvent.Key * 25;
                                break;
                            default:
                                failure = true;
                                break;
                        }
                    }
                    else
                    {
                        if (sEvent.Value == 254)
                        {
                            var dur = sEvent.Key * 25 - lastTime;
                            notes.Add(new Note((SevenBitNumber) lastNote, dur, applyInstrumentOffsets ? currentInstrument.SampleOffset + lastTime : lastTime)
                            {
                                Channel = (FourBitNumber) channel,
                                Velocity = (SevenBitNumber) 127,
                                OffVelocity = (SevenBitNumber) 0
                            });
                            lastNote = 254;
                            lastTime = sEvent.Key * 25;
                        }
                        else failure = true;
                    }
                }
                if (failure) throw new BmpException("Error exporting MMSong to Midi");
                var currentChunk = new TrackChunk(new SequenceTrackNameEvent(trackName.ToString()));
                currentChunk.AddObjects(notes);
                sequence.Chunks.Add(currentChunk);
            }
            if (includeSingerTracks)
            {
                foreach (var singer in mmSong.singers)
                {
                    var currentChunk = new TrackChunk(new SequenceTrackNameEvent("Singer" + (string.IsNullOrWhiteSpace(singer.description)? "" : ";description=" + singer.description)));
                    var notes = new List<Note>();
                    using (var timedEventsManager = currentChunk.ManageTimedEvents())
                    {
                        foreach (var sEvent in singer.sequence)
                        {
                            notes.Add(new Note((SevenBitNumber) sEvent.Value, 25, sEvent.Key * 25)
                            {
                                Channel = (FourBitNumber) 0,
                                Velocity = (SevenBitNumber) 1,
                                OffVelocity = (SevenBitNumber) 0
                            });
                            timedEventsManager.Events.AddEvent(new LyricEvent(singer.lines[sEvent.Value]), sEvent.Key * 25);
                        }
                    }
                    currentChunk.AddObjects(notes);
                    sequence.Chunks.Add(currentChunk);
                }
            }

            sequence.ReplaceTempoMap(TempoMap.Create(Tempo.FromBeatsPerMinute(100)));
            sequence.TimeDivision = new TicksPerQuarterNoteTimeDivision(600);

            var midiFile = new MemoryStream();
            sequence.Write(midiFile, MidiFileFormat.MultiTrack,
                new WritingSettings { TextEncoding = Encoding.ASCII });
            midiFile.Flush();
            midiFile.Position = 0;
            return midiFile;
        }
    }
}
