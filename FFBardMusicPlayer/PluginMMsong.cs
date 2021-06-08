using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using FFBardMusicCommon;

namespace FFBardMusicPlayer
{
    internal class PluginMMsong
    {
        internal static MidiFile Load(string filePath)
        {
            try
            {
                var mmSong = JsonExtensions.DeserializeFromFileCompressed<MmSong>(filePath);

                if (mmSong.SchemaVersion < 1 && mmSong.SchemaVersion > 3)
                {
                    throw new FileFormatException("Error: This mmsong file format is not understood.");
                }
                // For now, just play the first available song.
                // if (mmSong.songs.Count != 1) throw new FileFormatException("Error: BMP currently only supports mmsong files with 1 song in them.");

                var song = mmSong.Songs[0];

                var sequence = new MidiFile();
                sequence.Chunks.Add(new TrackChunk());
                sequence.TimeDivision = new TicksPerQuarterNoteTimeDivision(600);
                using (var tempoMapManager = sequence.ManageTempoMap())
                {
                    tempoMapManager.SetTempo(0, Tempo.FromBeatsPerMinute(100));
                }

                foreach (var bard in song.Bards)
                {
                    var notes = new List<Note>();
                    var failure = false;

                    switch (bard.Instrument)
                    {
                        case Instrument.Cymbal:
                        case Instrument.Trumpet:
                        case Instrument.Trombone:
                        case Instrument.Horn:
                        case Instrument.Tuba:
                        case Instrument.Saxophone:
                        case Instrument.Violin:
                        case Instrument.Viola:
                        case Instrument.Cello:
                        case Instrument.DoubleBass:
                        case Instrument.ElectricGuitarClean:
                        case Instrument.ElectricGuitarMuted:
                        case Instrument.ElectricGuitarOverdriven:
                        case Instrument.ElectricGuitarPowerChords:
                        case Instrument.ElectricGuitarSpecial:
                            bard.Sequence = bard.Sequence.ToDictionary(
                                x => x.Key + 2,
                                x => x.Value);
                            break;
                    }

                    if (bard.Sequence.Count % 2 == 0)
                    {
                        long lastTime = 0;
                        var lastNote = 254;
                        foreach (var sEvent in bard.Sequence.Where(sEvent => !failure))
                        {
                            if (lastNote == 254)
                            {
                                if (sEvent.Value <= 60 && sEvent.Value >= 24 &&
                                    (sEvent.Key * 25 % 100 == 50 || sEvent.Key * 25 % 100 == 0))
                                {
                                    lastNote = sEvent.Value + 24;
                                    lastTime = sEvent.Key * 25;
                                }
                                else
                                {
                                    failure = true;
                                }
                            }
                            else
                            {
                                if (sEvent.Value == 254)
                                {
                                    var dur = sEvent.Key * 25 - lastTime;
                                    notes.Add(new Note((SevenBitNumber) lastNote, dur, lastTime)
                                    {
                                        Channel     = (FourBitNumber) 14,
                                        Velocity    = (SevenBitNumber) 127,
                                        OffVelocity = (SevenBitNumber) 0
                                    });
                                    lastNote = 254;
                                    lastTime = sEvent.Key * 25;
                                }
                                else
                                {
                                    failure = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        failure = true;
                    }

                    if (failure)
                    {
                        throw new FileFormatException("Error: This mmsong file is corrupted");
                    }

                    var currentChunk = new TrackChunk(new SequenceTrackNameEvent(bard.Instrument.ToString()));
                    currentChunk.AddNotes(notes);
                    notes = null;
                    sequence.Chunks.Add(currentChunk);
                    currentChunk = null;
                }

                using (var manager = new TimedEventsManager(sequence.GetTrackChunks().First().Events))
                {
                    manager.Events.Add(new TimedEvent(new MarkerEvent(),
                        sequence.GetDuration<MetricTimeSpan>().TotalMicroseconds / 1000 + 100));
                }

                return sequence;
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
                {
                    return DeserializeCompressed<T>(fs, settings);
                }
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

        private class MmSong
        {
            public List<Song> Songs { get; set; } = new List<Song>();

            public int SchemaVersion { get; set; }

            public class Song
            {
                public string Title { get; set; }

                public List<Bard> Bards { get; set; } = new List<Bard>();
            }

            public class Bard
            {
                public Instrument Instrument { get; set; }

                public Dictionary<long, int> Sequence { get; set; } = new Dictionary<long, int>();
            }
        }
    }
}