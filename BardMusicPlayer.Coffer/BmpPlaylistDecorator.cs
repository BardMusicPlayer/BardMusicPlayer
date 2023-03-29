/*
 * Copyright(c) 2023 MoogleTroupe, isaki, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections;
using BardMusicPlayer.Transmogrify.Song;

namespace BardMusicPlayer.Coffer;

public sealed class BmpPlaylistDecorator : IPlaylist
{
    private readonly BmpPlaylist target;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="target"></param>
    internal BmpPlaylistDecorator(BmpPlaylist target) => this.target = target ?? throw new NullReferenceException();

    ///<inheritdoc/>
    void IPlaylist.Add(BmpSong song) { target.Songs.Add(song); }

    ///<inheritdoc/>
    void IPlaylist.Add(int idx, BmpSong song) { target.Songs.Insert(idx, song); }

    ///<inheritdoc/>
    IEnumerator<BmpSong> IEnumerable<BmpSong>.GetEnumerator() { return target.Songs.GetEnumerator(); }

    ///<inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() { return target.Songs.GetEnumerator(); }

    ///<inheritdoc/>
    string IPlaylist.GetName() { return target.Name; }

    ///<inheritdoc/>
    void IPlaylist.Move(int sourceIdx, int targetIdx)
    {
        var contents = target.Songs;
        var count = contents.Count;
        if (sourceIdx < 0 || sourceIdx >= count)
        {
            return;
        }

        if (targetIdx < 0)
        {
            targetIdx = 0;
        }
        else if (targetIdx >= count)
        {
            targetIdx = count - 1;
        }

        var moveMe = contents[sourceIdx];
        contents.RemoveAt(sourceIdx);
        contents.Insert(targetIdx, moveMe);
    }

    ///<inheritdoc/>
    void IPlaylist.Remove(int idx) { target.Songs.RemoveAt(idx); }

    /// <inheritdoc />
    void IPlaylist.Remove(BmpSong song) { target.Songs.Remove(song); }

    ///<inheritdoc/>
    void IPlaylist.SetName(string name) { target.Name = name ?? throw new ArgumentNullException(); }

    internal BmpPlaylist GetBmpPlaylist() => target;
}