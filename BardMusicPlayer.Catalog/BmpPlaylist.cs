/*
 * Copyright(c) 2021 MoogleTroupe, isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using LiteDB;
using System.Collections.Generic;
using BardMusicPlayer.Notate.Song;

namespace BardMusicPlayer.Catalog
{
    public sealed class BmpPlaylist
    {
        [BsonId]
        public ObjectId Id { get; set; } = null;

        public string Name { get; set; }

        [BsonRef(Constants.SONG_COL_NAME)]
        public List<BmpSong> Songs { get; set; }
    }
}
