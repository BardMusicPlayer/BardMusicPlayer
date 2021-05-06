/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using BardMusicPlayer.Notate.Song;
using BardMusicPlayer.Notate.Song.Config;
using Melanchall.DryWetMidi.Core;

namespace BardMusicPlayer.Notate.Processor
{
    internal class ManualToneProcessor : BaseProcessor
    {
        public ManualToneConfig Config { get; set; }

        internal ManualToneProcessor(ManualToneConfig config, BmpSong song) : base(song)
        {
            Config = config;
        }

        public override async Task<List<TrackChunk>> Process()
        {
            throw new System.NotImplementedException();
        }
    }
}
