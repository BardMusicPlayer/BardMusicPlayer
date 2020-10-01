﻿using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using static Sharlayan.Core.Enums.Performance;

namespace FFBardMusicPlayer
{
    internal class MMsongUtil
    {
        internal static MemoryStream Load(string filePath, int index)
        {
            try
            {
                MMSong mmSong = JsonExtensions.DeserializeFromFileCompressed<MMSong>(filePath);

                if (mmSong.schemaVersion != 1) throw new FileFormatException("Error: This mmsong file format is not understood.");

                MMSong.Song song = mmSong.songs[index];

                MidiFile sequence = new MidiFile();
                sequence.Chunks.Add(new TrackChunk());
                sequence.TimeDivision = new TicksPerQuarterNoteTimeDivision(600);
                using (TempoMapManager tempoMapManager = sequence.ManageTempoMap()) tempoMapManager.SetTempo(0, Tempo.FromBeatsPerMinute(100));

                foreach (MMSong.Bard bard in song.bards)
                {
                    List<Note> notes = new List<Note>();
                    bool failure = false;
                    
                    if (bard.sequence.Count % 2 == 0)
                    {
                        long lastTime = 0;
                        int lastNote = 254;
                        foreach (KeyValuePair<long, int> sEvent in bard.sequence)
                        {
                            if (!failure)
                            {
                                if (lastNote == 254)
                                {
                                    if (sEvent.Value <= 60 && sEvent.Value >= 24 && ((sEvent.Key * 25 % 100) == 50 || (sEvent.Key * 25) % 100 == 0))
                                    {
                                        lastNote = sEvent.Value + 24;
                                        lastTime = sEvent.Key * 25;
                                    }
                                    else failure = true;
                                }
                                else
                                {
                                    if (sEvent.Value == 254)
                                    {
                                        long dur = (sEvent.Key * 25) - lastTime;
                                        notes.Add(new Note((SevenBitNumber)lastNote, dur, lastTime)
                                        {
                                            Channel = (FourBitNumber)14,
                                            Velocity = (SevenBitNumber)(int)127,
                                            OffVelocity = (SevenBitNumber)(int)0
                                        });
                                        lastNote = 254;
                                        lastTime = sEvent.Key * 25;
                                    }
                                    else failure = true;
                                }
                            }
                        }
                    }
                    else failure = true;

                    if (failure) throw new FileFormatException("Error: This mmsong file is corrupted");

                    TrackChunk currentChunk = new TrackChunk(new SequenceTrackNameEvent(bard.instrument.ToString()));
                    currentChunk.AddNotes(notes);
                    notes = null;
                    sequence.Chunks.Add(currentChunk);
                    currentChunk = null;
                }

                var stream = new MemoryStream();
                sequence.Write(stream, MidiFileFormat.MultiTrack, new WritingSettings { CompressionPolicy = CompressionPolicy.NoCompression });
                stream.Flush();
                stream.Position = 0;
                sequence = null;
                return stream;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static class JsonExtensions
        {
            public static T DeserializeFromFileCompressed<T>(string path, JsonSerializerSettings settings = null)
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    return DeserializeCompressed<T>(fs, settings);
            }

            public static T DeserializeCompressed<T>(Stream stream, JsonSerializerSettings settings = null)
            {
                using (var compressor = new GZipStream(stream, CompressionMode.Decompress))
                using (var reader = new StreamReader(compressor))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var serializer = JsonSerializer.CreateDefault(settings);
                    return serializer.Deserialize<T>(jsonReader);
                }
            }
        }

        private class MMSong
        {
            public List<Song> songs { get; set; } = new List<Song>();
            public int schemaVersion { get; set; }
            public class Song
            {
                public List<Bard> bards { get; set; } = new List<Bard>();
            }
            public class Bard
            {
                public Instrument instrument { get; set; }
                public Dictionary<long, int> sequence { get; set; } = new Dictionary<long, int>();
            }
        }
    }
}