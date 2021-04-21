/*
 * MogLib/Common/Objects/MMSongLegacy.cs
 *
 * Copyright (C) 2021  MoogleTroupe
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System.Collections.Generic;
using BardMusicPlayer.Common;
using BardMusicPlayer.Common.Structs;
using ServiceStack.Text;
using static BardMusicPlayer.Notate.Objects.NotateConfig.NotateGroup;

namespace BardMusicPlayer.Notate.Objects
{
    internal sealed class MMSongLegacy
    {
        internal static MMSong DeserializeFromString(string json)
        {
            using (JsConfig.With(new Config
            {
                TextCase = TextCase.CamelCase,
                PropertyConvention = PropertyConvention.Lenient,
                IncludePublicFields = false,
                ExcludeDefaultValues = false,
            }))
            {
                var legacyMmSong = JsonSerializer.DeserializeFromString<MMSongLegacy>(json);
                if (legacyMmSong == null) throw new BmpSchemaInvalidException();
                if (legacyMmSong.schemaVersion < 1 || legacyMmSong.schemaVersion > 2) throw new BmpSchemaVersionException(legacyMmSong.schemaVersion);
                var mmSong = new MMSong();
                var legacySong = legacyMmSong.songs[0];
                mmSong.title = string.IsNullOrWhiteSpace(legacyMmSong.title) ? legacySong.title : legacyMmSong.title;
                mmSong.description = string.IsNullOrWhiteSpace(legacyMmSong.description) ? legacySong.description : legacyMmSong.description;
                mmSong.tags = legacyMmSong.tags.Length == 0 ? legacySong.tags : legacyMmSong.tags;
                foreach (var legacyBard in legacySong.bards)
                {
                    MMSong.Bard bard = new() {instruments = new Dictionary<VST, Instrument> {{VST.VST0, legacyBard.instrument}}};
                    bard.sequence.Append(legacyBard.sequence);
                    mmSong.bards.Add(bard);
                }
                foreach (var legacyLyric in legacySong.lyrics)
                {
                    MMSong.Singer singer = new() {description = legacyLyric.description};
                    singer.sequence.Append(legacyLyric.sequence);
                    singer.lines.Append(legacyLyric.lines);
                    mmSong.singers.Add(singer);
                }

                var midiFileStream = mmSong.GetMidiFile();
                mmSong.sourceMidiFile = midiFileStream.ToArray();
                mmSong.notateConfig = NotateConfig.GenerateConfigFromMidiFile(midiFileStream.Rewind());

                return mmSong;
            }
        }
        public string title { get; internal set; } = "";
        public string description { get; internal set; } = "";
        public int schemaVersion { get; internal set; } = 2;
        public string[] tags { get; internal set; } = new string[0];
        public List<Song> songs { get; internal set; } = new();
        public sealed class Song
        {
            public string title { get; internal set; } = "";
            public string description { get; internal set; } = "";
            public string[] tags { get; internal set; } = new string[0];
            public List<Bard> bards { get; internal set; } = new();
            public List<Lyric> lyrics { get; internal set; } = new();
            public sealed class Bard
            {
                public Instrument instrument { get; internal set; } = 0;
                public Dictionary<long, int> sequence { get; internal set; } = new();
            }
            public sealed class Lyric
            {
                public string description { get; internal set; } = "";
                public Dictionary<int, string> lines { get; internal set; } = new();
                public Dictionary<long, int> sequence { get; internal set; } = new();
            }
        }
    }
}
