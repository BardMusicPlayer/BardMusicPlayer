/*
 * Copyright(c) 2022 MoogleTroupe, isaki, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using BardMusicPlayer.Transmogrify.Song;

namespace BardMusicPlayer.Coffer;

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
    /// This removes the specified <see cref="BmpSong"/>.
    /// </summary>
    /// <param name="song">The song the remove</param>
    void Remove(BmpSong song);

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