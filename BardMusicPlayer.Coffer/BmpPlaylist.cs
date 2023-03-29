/*
 * Copyright(c) 2023 MoogleTroupe, isaki, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Transmogrify.Song;
using LiteDB;

namespace BardMusicPlayer.Coffer;

public sealed class BmpPlaylist
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string Name { get; set; }

    [BsonRef(Constants.SONG_COL_NAME)]
    public List<BmpSong> Songs { get; set; }
}