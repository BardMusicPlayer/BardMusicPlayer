/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using LiteDB;
using Melanchall.DryWetMidi.Interaction;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BardMusicPlayer.Notate.Song
{
    public sealed class BmpSong
    {
        /// <summary>
        /// 
        /// </summary>
        [BsonId]
        [JsonIgnore]
        public ObjectId Id { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string[] Tags { get; set; } = new string[0];

        /// <summary>
        /// 
        /// </summary>
        public TempoMap SourceTempoMap { get; set; } = TempoMap.Default;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, TrackContainer> TrackContainers { get; set; } = new();
    }
}
