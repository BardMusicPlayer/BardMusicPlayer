/*
 * Copyright(c) 2021 MoogleTroupe, isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using BardMusicPlayer.Transmogrify.Song;

namespace BardMusicPlayer.Coffer
{
    public sealed class BmpPlaylistDecorator : IPlaylist
    {
        private readonly BmpPlaylist target;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target"></param>
        internal BmpPlaylistDecorator(BmpPlaylist target) => this.target = target ?? throw new NullReferenceException();

        ///<inheritdoc/>
        void IPlaylist.Add(BmpSong song)
        {
            this.target.Songs.Add(song);
        }

        ///<inheritdoc/>
        void IPlaylist.Add(int idx, BmpSong song)
        {
            this.target.Songs.Insert(idx, song);
        }

        ///<inheritdoc/>
        IEnumerator<BmpSong> IEnumerable<BmpSong>.GetEnumerator()
        {
            return this.target.Songs.GetEnumerator();
        }

        ///<inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.target.Songs.GetEnumerator();
        }

        ///<inheritdoc/>
        string IPlaylist.GetName()
        {
            return this.target.Name;
        }

        ///<inheritdoc/>
        void IPlaylist.Move(int sourceIdx, int targetIdx)
        {
            List<BmpSong> contents = this.target.Songs;
            BmpSong moveMe = contents[sourceIdx];
            contents.RemoveAt(sourceIdx);
            contents.Insert(targetIdx, moveMe);
        }

        ///<inheritdoc/>
        void IPlaylist.Remove(int idx)
        {
            this.target.Songs.RemoveAt(idx);
        }

        ///<inheritdoc/>
        void IPlaylist.SetName(string name)
        {
            this.target.Name = name ?? throw new ArgumentNullException();
        }

        ///<inheritdoc/>
        internal BmpPlaylist GetBmpPlaylist() => this.target;
    }
}
