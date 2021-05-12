/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using BardMusicPlayer.Notate.Song;

namespace BardMusicPlayer.Catalog
{
    public interface IPlaylist : IEnumerable<BmpSong>
    {
        /// <summary>
        /// This adds a song to the playlist.
        /// </summary>
        /// <param name="song"></param>
        void Add(BmpSong song);

        /// <summary>
        /// This song adds a song to the playlist at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="song"></param>
        void Add(int idx, BmpSong song);

        /// <summary>
        /// This moves a song within the playlist to the given index.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        void Move(int source, int target);

        /// <summary>
        /// This removes the song at the given index.
        /// </summary>
        /// <param name="idx"></param>
        void Remove(int idx);

        /// <summary>
        /// Returns the name of the playlist.
        /// </summary>
        /// <returns></returns>
        string GetName();

        /// <summary>
        /// Sets the name of the playlist.
        /// </summary>
        /// <param name="name"></param>
        void SetName(string name);
    }
}
