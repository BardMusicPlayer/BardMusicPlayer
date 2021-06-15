/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BardMusicPlayer.Transmogrify.Song;
using Melanchall.DryWetMidi.Core;

namespace BardMusicPlayer.Transmogrify.Processor
{
    internal abstract class BaseProcessor : IDisposable
    {
        protected BaseProcessor(BmpSong song)
        {
            Song = song;
        }

        protected BmpSong Song { get; set; }

        public abstract Task<List<TrackChunk>> Process();

        ~BaseProcessor() => Dispose();
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
