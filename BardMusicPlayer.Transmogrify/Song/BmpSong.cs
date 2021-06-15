/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Transmogrify.Processor.Utilities;
using BardMusicPlayer.Transmogrify.Song.Config;
using BardMusicPlayer.Transmogrify.Song.Utilities;
using LiteDB;
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
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<BmpSong> OpenMidiFile(string path)
        {
            var timer = new Stopwatch();
            timer.Start();

            if (!File.Exists(path)) throw new BmpTransmogrifyException("File " + path + " does not exist!");

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

            timer.Stop();

            var timeTaken = timer.Elapsed;
            Console.WriteLine("Time taken: " + timeTaken.ToString(@"m\:ss\.fff"));
            return Task.FromResult(song);
        }
    }
}
