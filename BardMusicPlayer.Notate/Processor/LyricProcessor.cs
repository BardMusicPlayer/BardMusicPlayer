﻿/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Notate.Song;
using BardMusicPlayer.Notate.Song.Config;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Notate.Processor
{
    internal class LyricProcessor : BaseProcessor
    {
        public LyricConfig ProcessorConfig { get; set; }

        internal LyricProcessor(LyricConfig processorConfig, BmpSong song) : base(song)
        {
            ProcessorConfig = processorConfig;
        }

        public override Task<List<TrackChunk>> Process()
        {
            var trackChunk = new List<TrackChunk> { Song.TrackContainers[ProcessorConfig.Track].SourceTrackChunk }.Concat(ProcessorConfig.IncludedTracks.Select(track => Song.TrackContainers[track].SourceTrackChunk)).Merge();

            var lyricEvents = new List<TimedEvent>();

            var lyricLineCount = 0;

            var tempoMap = Song.SourceTempoMap.Clone();

            foreach (var midiEvent in trackChunk.GetTimedEvents().Where(e => e.Event.EventType == MidiEventType.Lyric))
            {
                lyricLineCount++;
                midiEvent.Time = midiEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000 + 120000;
                lyricEvents.Add(midiEvent);
            }

            var trackChunks = new List<TrackChunk>();

            for (var i = 0; i < ProcessorConfig.PlayerCount; i++)
            {
                trackChunk = new TrackChunk();
                trackChunk.AddObjects(lyricEvents);
                trackChunk.AddObjects(new List<ITimedObject>{new TimedEvent(new SequenceTrackNameEvent("lyric:"+lyricLineCount))});
                trackChunks.Add(trackChunk);
            }

            return Task.FromResult(trackChunks);
        }
    }
}
