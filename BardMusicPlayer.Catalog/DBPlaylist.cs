﻿using BardMusicPlayer.Notate.Objects;
using LiteDB;
using System.Collections.Generic;

namespace BardMusicPlayer.Catalog
{
    public sealed class DBPlaylist
    {
        [BsonId]
        public ObjectId Id { get; set; } = null;

        public string Name { get; set; }

        [BsonRef(Constants.SONG_COL_NAME)]
        public List<MMSong> Songs { get; set; }
    }
}
