/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using BardMusicPlayer.Notate.Song;
using Melanchall.DryWetMidi.Core;

namespace BardMusicPlayer.Notate.Processor
{
    internal class AutoToneProcessor : BaseProcessor
    {
        public AutoToneProcessor(BmpSong song) : base(song)
        {
        }

        public override Task<List<TrackChunk>> Process()
        {
            throw new System.NotImplementedException();
        }
    }
}
