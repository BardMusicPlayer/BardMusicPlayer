/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using LiteDB;
using Melanchall.DryWetMidi.Interaction;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Notate.Processor.Utilities;
using BardMusicPlayer.Notate.Song.Config;
using BardMusicPlayer.Notate.Song.Utilities;
using Melanchall.DryWetMidi.Core;

namespace BardMusicPlayer.Notate.Song
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
        public List<string> Tags { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        public TempoMap SourceTempoMap { get; set; } = TempoMap.Default;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<long, TrackContainer> TrackContainers { get; set; } = new();

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
                var notes = trackChunk.GetNotes();
                var newTrackChunk = new TrackChunk(new SequenceTrackNameEvent(trackChunk.Events.OfType<SequenceTrackNameEvent>().First().Text));
                var newNotes = new List<Note>();
                foreach (var note in notes)
                {
                    if (note.Time - delta < 0) continue;
                    note.Time -= delta;
                    newNotes.Add(note);
                }
                newTrackChunk.AddObjects(newNotes);
                midiFile.Chunks.Add(newTrackChunk);
            }
            midiFile.ReplaceTempoMap(Tools.GetMsTempoMap());
            return Task.FromResult(midiFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<BmpSong> OpenMidiFile(string path)
        {
            var timer = new Stopwatch();
            timer.Start();

            if (!File.Exists(path)) throw new BmpNotateException("File " + path + " does not exist!");

            using var fileStream = File.OpenRead(path);

            var midiFile = fileStream.ReadAsMidiFile();

            fileStream.Dispose();

            var song = new BmpSong
            {
                Title = Path.GetFileNameWithoutExtension(path),
                SourceTempoMap = midiFile.GetTempoMap().Clone(),
                TrackContainers = new Dictionary<long, TrackContainer>()
            };

            var trackChunkArray = midiFile.GetTrackChunks().ToArray();

            for (var i = 0; i < midiFile.GetTrackChunks().Count(); i++) song.TrackContainers[i] = new TrackContainer
            {
                SourceTrackChunk = (TrackChunk) trackChunkArray[i].Clone()
            };
            for (var i = 0; i < midiFile.GetTrackChunks().Count(); i++) 
            {
                song.TrackContainers[i].ConfigContainers = song.TrackContainers[i].SourceTrackChunk.ReadConfigs(i, song);
            }

            Parallel.For(0, song.TrackContainers.Count, async i =>
            {
                Parallel.For(0, song.TrackContainers[i].ConfigContainers.Count, async j =>
                {
                    switch (song.TrackContainers[i].ConfigContainers[j].Config)
                    {
                        case ClassicConfig classicConfig:
                            Console.WriteLine("Processing: Track:" + i + " ConfigContainer:" + j + " ConfigType:" +
                                              classicConfig.GetType() +
                                              " Instrument:" + classicConfig.Instrument + " OctaveRange:" +
                                              classicConfig.OctaveRange + " PlayerCount:" + classicConfig.PlayerCount +
                                              " IncludeTracks:" + string.Join(",", classicConfig.IncludedTracks));
                            song.TrackContainers[i].ConfigContainers[j].ProccesedTrackChunks =
                                await song.TrackContainers[i].ConfigContainers[j].RefreshTrackChunks(song);
                            break;
                        case AutoToneConfig autoToneConfig:
                            Console.WriteLine("Processing: Track:" + i + " ConfigContainer:" + j + " ConfigType:" +
                                              autoToneConfig.GetType() +
                                             " AutoToneInstrumentGroup:" + autoToneConfig.AutoToneInstrumentGroup + " OctaveRange:" +
                                              autoToneConfig.AutoToneOctaveRange + " PlayerCount:" + autoToneConfig.PlayerCount +
                                             " IncludeTracks:" + string.Join(",", autoToneConfig.IncludedTracks));
                            song.TrackContainers[i].ConfigContainers[j].ProccesedTrackChunks =
                                await song.TrackContainers[i].ConfigContainers[j].RefreshTrackChunks(song);
                            break;
                        default:
                            Console.WriteLine("error unknown config.");
                            break;
                    }
                });
            });

            timer.Stop();

            var timeTaken = timer.Elapsed;
            Console.WriteLine("Time taken: " + timeTaken.ToString(@"m\:ss\.fff"));
            return Task.FromResult(song);
        }
    }
}
