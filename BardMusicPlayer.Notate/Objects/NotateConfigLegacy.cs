/*
 * MogLib/Notate/Objects/NotateConfigLegacy.cs
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

using System;
using System.Collections.Generic;
using BardMusicPlayer.Common.Structs;
using YamlDotNet.Serialization;
using static BardMusicPlayer.Notate.Objects.NotateConfig;

namespace BardMusicPlayer.Notate.Objects
{
    internal sealed class NotateConfigLegacy
    {
        internal static NotateConfig Transform(string[] fileContents)
        {
            var deserializer = new DeserializerBuilder().WithTypeConverter(new NotateConfigExtensions.InstrumentConverter()).WithTypeConverter(new NotateConfigExtensions.OctaveRangeConverter()).IgnoreUnmatchedProperties().Build();
            var notateConfigLegacy = deserializer.Deserialize<NotateConfigLegacy>(string.Join(Environment.NewLine, fileContents));
            if (notateConfigLegacy == null) throw new BmpSchemaInvalidException();
            var notateConfig = new NotateConfig();
            var song = notateConfigLegacy.songs[0];
            notateConfig.title = string.IsNullOrWhiteSpace(notateConfigLegacy.collection_title)
                ? song.title
                : notateConfigLegacy.collection_title;
            notateConfig.description = string.IsNullOrWhiteSpace(notateConfigLegacy.collection_description)
                ? song.description
                : notateConfigLegacy.collection_description;
            notateConfig.midiFileName = string.IsNullOrWhiteSpace(notateConfigLegacy.global_file)
                ? song.file
                : notateConfigLegacy.global_file;
            foreach (var legacyStandard in song.standard)
            {
                var bardGroup = new BardGroup
                {
                    instruments =
                    {
                        [NotateGroup.VST.VST0] = new NotateGroup.VSTProperties()
                        {
                            instrument = Instrument.Parse(legacyStandard.instrument),
                            octaveRange = OctaveRange.Parse(legacyStandard.octave)
                        }
                    },
                    distributionBardCount = legacyStandard.bards
                };
                foreach (var legacyTrack in legacyStandard.tracks)
                    bardGroup.tracks.Add(legacyTrack.Key, new NotateGroup.Track());
                notateConfig.bardGroups.Add(bardGroup);
            }

            foreach (var legacyDrum in song.drum)
            {
                if (legacyDrum.bass_drum_bards > 0)
                    notateConfig.bardGroups.Add(ConvertDrum(Instrument.BassDrum, legacyDrum,
                        int.Parse(legacyDrum.bass_drum_bards.ToString())));
                if (legacyDrum.snare_drum_bards > 0)
                    notateConfig.bardGroups.Add(ConvertDrum(Instrument.SnareDrum, legacyDrum,
                        int.Parse(legacyDrum.snare_drum_bards.ToString())));
                if (legacyDrum.cymbal_bards > 0)
                    notateConfig.bardGroups.Add(ConvertDrum(Instrument.Cymbal, legacyDrum,
                        int.Parse(legacyDrum.cymbal_bards.ToString())));
                if (legacyDrum.bongo_bards > 0)
                    notateConfig.bardGroups.Add(ConvertDrum(Instrument.Bongo, legacyDrum,
                        int.Parse(legacyDrum.bongo_bards.ToString())));
                if (legacyDrum.timpani_bards > 0)
                    notateConfig.bardGroups.Add(ConvertDrum(Instrument.Timpani, legacyDrum,
                        int.Parse(legacyDrum.timpani_bards.ToString())));
            }

            var lyricsToConvert = notateConfigLegacy.global_lyrics.Count < 1
                ? song.lyric
                : notateConfigLegacy.global_lyrics;
            foreach (var legacyLyric in lyricsToConvert) notateConfig.singerGroups.Add(ConvertSinger(legacyLyric));
            return notateConfig;
        }

        public string collection_title { get; set; } = "";
        public string collection_description { get; set; } = "";
        public string global_file { get; set; } = "";
        public List<Song> songs { get; set; } = new();
        public List<Lyric> global_lyrics { get; set; } = new();
        public sealed class Song
        {
            public string title { get; set; } = "Untitled Song";
            public string description { get; set; } = "";
            public string file { get; set; } = "";
            public List<Standard> standard { get; set; } = new();
            public List<Drum> drum { get; set; } = new();
            public List<Lyric> lyric { get; set; } = new();
        }
        public sealed class Track {}
        public sealed class Standard
        {
            public Dictionary<int,Track> tracks { get; set; } = new();
            public string instrument { get; set; } = "None";
            public int octave { get; set; } = 3;
            public int bards { get; set; } = 1;
        }
        public sealed class Drum
        {
            public Dictionary<int,Track> tracks { get; set; } = new();
            public int timpani_bards { get; set; } = 0;
            public int bongo_bards { get; set; } = 0;
            public int bass_drum_bards { get; set; } = 0;
            public int snare_drum_bards { get; set; } = 0;
            public int cymbal_bards { get; set; } = 0;
            public string map_27_drum { get; set; } = "None";
            public int map_27_note { get; set; } = 0;
            public string map_28_drum { get; set; } = "None";
            public int map_28_note { get; set; } = 0;
            public string map_29_drum { get; set; } = "None";
            public int map_29_note { get; set; } = 0;
            public string map_30_drum { get; set; } = "None";
            public int map_30_note { get; set; } = 0;
            public string map_31_drum { get; set; } = "None";
            public int map_31_note { get; set; } = 0;
            public string map_32_drum { get; set; } = "None";
            public int map_32_note { get; set; } = 0;
            public string map_33_drum { get; set; } = "None";
            public int map_33_note { get; set; } = 0;
            public string map_34_drum { get; set; } = "None";
            public int map_34_note { get; set; } = 0;
            public string map_35_drum { get; set; } = "BassDrum";
            public int map_35_note { get; set; } = 8;
            public string map_36_drum { get; set; } = "BassDrum";
            public int map_36_note { get; set; } = 10;
            public string map_37_drum { get; set; } = "None";
            public int map_37_note { get; set; } = 0;
            public string map_38_drum { get; set; } = "SnareDrum";
            public int map_38_note { get; set; } = 20;
            public string map_39_drum { get; set; } = "None";
            public int map_39_note { get; set; } = 0;
            public string map_40_drum { get; set; } = "SnareDrum";
            public int map_40_note { get; set; } = 22;
            public string map_41_drum { get; set; } = "BassDrum";
            public int map_41_note { get; set; } = 16;
            public string map_42_drum { get; set; } = "None";
            public int map_42_note { get; set; } = 0;
            public string map_43_drum { get; set; } = "BassDrum";
            public int map_43_note { get; set; } = 19;
            public string map_44_drum { get; set; } = "None";
            public int map_44_note { get; set; } = 0;
            public string map_45_drum { get; set; } = "BassDrum";
            public int map_45_note { get; set; } = 23;
            public string map_46_drum { get; set; } = "None";
            public int map_46_note { get; set; } = 0;
            public string map_47_drum { get; set; } = "BassDrum";
            public int map_47_note { get; set; } = 26;
            public string map_48_drum { get; set; } = "BassDrum";
            public int map_48_note { get; set; } = 30;
            public string map_49_drum { get; set; } = "Cymbal";
            public int map_49_note { get; set; } = 24;
            public string map_50_drum { get; set; } = "BassDrum";
            public int map_50_note { get; set; } = 33;
            public string map_51_drum { get; set; } = "None";
            public int map_51_note { get; set; } = 0;
            public string map_52_drum { get; set; } = "Cymbal";
            public int map_52_note { get; set; } = 22;
            public string map_53_drum { get; set; } = "None";
            public int map_53_note { get; set; } = 0;
            public string map_54_drum { get; set; } = "None";
            public int map_54_note { get; set; } = 0;
            public string map_55_drum { get; set; } = "Cymbal";
            public int map_55_note { get; set; } = 30;
            public string map_56_drum { get; set; } = "None";
            public int map_56_note { get; set; } = 0;
            public string map_57_drum { get; set; } = "Cymbal";
            public int map_57_note { get; set; } = 24;
            public string map_58_drum { get; set; } = "None";
            public int map_58_note { get; set; } = 0;
            public string map_59_drum { get; set; } = "None";
            public int map_59_note { get; set; } = 0;
            public string map_60_drum { get; set; } = "Bongo";
            public int map_60_note { get; set; } = 23;
            public string map_61_drum { get; set; } = "Bongo";
            public int map_61_note { get; set; } = 20;
            public string map_62_drum { get; set; } = "None";
            public int map_62_note { get; set; } = 0;
            public string map_63_drum { get; set; } = "None";
            public int map_63_note { get; set; } = 0;
            public string map_64_drum { get; set; } = "None";
            public int map_64_note { get; set; } = 0;
            public string map_65_drum { get; set; } = "None";
            public int map_65_note { get; set; } = 0;
            public string map_66_drum { get; set; } = "None";
            public int map_66_note { get; set; } = 0;
            public string map_67_drum { get; set; } = "None";
            public int map_67_note { get; set; } = 0;
            public string map_68_drum { get; set; } = "None";
            public int map_68_note { get; set; } = 0;
            public string map_69_drum { get; set; } = "None";
            public int map_69_note { get; set; } = 0;
            public string map_70_drum { get; set; } = "None";
            public int map_70_note { get; set; } = 0;
            public string map_71_drum { get; set; } = "None";
            public int map_71_note { get; set; } = 0;
            public string map_72_drum { get; set; } = "None";
            public int map_72_note { get; set; } = 0;
            public string map_73_drum { get; set; } = "None";
            public int map_73_note { get; set; } = 0;
            public string map_74_drum { get; set; } = "None";
            public int map_74_note { get; set; } = 0;
            public string map_75_drum { get; set; } = "None";
            public int map_75_note { get; set; } = 0;
            public string map_76_drum { get; set; } = "None";
            public int map_76_note { get; set; } = 0;
            public string map_77_drum { get; set; } = "None";
            public int map_77_note { get; set; } = 0;
            public string map_78_drum { get; set; } = "None";
            public int map_78_note { get; set; } = 0;
            public string map_79_drum { get; set; } = "None";
            public int map_79_note { get; set; } = 0;
            public string map_80_drum { get; set; } = "None";
            public int map_80_note { get; set; } = 0;
            public string map_81_drum { get; set; } = "None";
            public int map_81_note { get; set; } = 0;
            public string map_82_drum { get; set; } = "None";
            public int map_82_note { get; set; } = 0;
            public string map_83_drum { get; set; } = "None";
            public int map_83_note { get; set; } = 0;
            public string map_84_drum { get; set; } = "None";
            public int map_84_note { get; set; } = 0; 
            public string map_85_drum { get; set; } = "None";
            public int map_85_note { get; set; } = 0;
            public string map_86_drum { get; set; } = "None";
            public int map_86_note { get; set; } = 0;
            public string map_87_drum { get; set; } = "None";
            public int map_87_note { get; set; } = 0;
        }
        public sealed class Lyric
        {
            public string description { get; set; } = "";
            public Dictionary<int, Track> tracks { get; set; } = new();
            public string line0 { get; set; } = "";
            public string line1 { get; set; } = "";
            public string line2 { get; set; } = "";
            public string line3 { get; set; } = "";
            public string line4 { get; set; } = "";
            public string line5 { get; set; } = "";
            public string line6 { get; set; } = "";
            public string line7 { get; set; } = "";
            public string line8 { get; set; } = "";
            public string line9 { get; set; } = "";
            public string line10 { get; set; } = "";
            public string line11 { get; set; } = "";
            public string line12 { get; set; } = "";
            public string line13 { get; set; } = "";
            public string line14 { get; set; } = "";
            public string line15 { get; set; } = "";
            public string line16 { get; set; } = "";
            public string line17 { get; set; } = "";
            public string line18 { get; set; } = "";
            public string line19 { get; set; } = "";
            public string line20 { get; set; } = "";
            public string line21 { get; set; } = "";
            public string line22 { get; set; } = "";
            public string line23 { get; set; } = "";
            public string line24 { get; set; } = "";
            public string line25 { get; set; } = "";
            public string line26 { get; set; } = "";
            public string line27 { get; set; } = "";
            public string line28 { get; set; } = "";
            public string line29 { get; set; } = "";
            public string line30 { get; set; } = "";
            public string line31 { get; set; } = "";
            public string line32 { get; set; } = "";
            public string line33 { get; set; } = "";
            public string line34 { get; set; } = "";
            public string line35 { get; set; } = "";
            public string line36 { get; set; } = "";
            public string line37 { get; set; } = "";
            public string line38 { get; set; } = "";
            public string line39 { get; set; } = "";
            public string line40 { get; set; } = "";
            public string line41 { get; set; } = "";
            public string line42 { get; set; } = "";
            public string line43 { get; set; } = "";
            public string line44 { get; set; } = "";
            public string line45 { get; set; } = "";
            public string line46 { get; set; } = "";
            public string line47 { get; set; } = "";
            public string line48 { get; set; } = "";
            public string line49 { get; set; } = "";
            public string line50 { get; set; } = "";
            public string line51 { get; set; } = "";
            public string line52 { get; set; } = "";
            public string line53 { get; set; } = "";
            public string line54 { get; set; } = "";
            public string line55 { get; set; } = "";
            public string line56 { get; set; } = "";
            public string line57 { get; set; } = "";
            public string line58 { get; set; } = "";
            public string line59 { get; set; } = "";
            public string line60 { get; set; } = "";
            public string line61 { get; set; } = "";
            public string line62 { get; set; } = "";
            public string line63 { get; set; } = "";
            public string line64 { get; set; } = "";
            public string line65 { get; set; } = "";
            public string line66 { get; set; } = "";
            public string line67 { get; set; } = "";
            public string line68 { get; set; } = "";
            public string line69 { get; set; } = "";
            public string line70 { get; set; } = "";
            public string line71 { get; set; } = "";
            public string line72 { get; set; } = "";
            public string line73 { get; set; } = "";
            public string line74 { get; set; } = "";
            public string line75 { get; set; } = "";
            public string line76 { get; set; } = "";
            public string line77 { get; set; } = "";
            public string line78 { get; set; } = "";
            public string line79 { get; set; } = "";
            public string line80 { get; set; } = "";
            public string line81 { get; set; } = "";
            public string line82 { get; set; } = "";
            public string line83 { get; set; } = "";
            public string line84 { get; set; } = "";
            public string line85 { get; set; } = "";
            public string line86 { get; set; } = "";
            public string line87 { get; set; } = "";
            public string line88 { get; set; } = "";
            public string line89 { get; set; } = "";
            public string line90 { get; set; } = "";
            public string line91 { get; set; } = "";
            public string line92 { get; set; } = "";
            public string line93 { get; set; } = "";
            public string line94 { get; set; } = "";
            public string line95 { get; set; } = "";
            public string line96 { get; set; } = "";
            public string line97 { get; set; } = "";
            public string line98 { get; set; } = "";
            public string line99 { get; set; } = "";
            public string line100 { get; set; } = "";
            public string line101 { get; set; } = "";
            public string line102 { get; set; } = "";
            public string line103 { get; set; } = "";
            public string line104 { get; set; } = "";
            public string line105 { get; set; } = "";
            public string line106 { get; set; } = "";
            public string line107 { get; set; } = "";
            public string line108 { get; set; } = "";
            public string line109 { get; set; } = "";
            public string line110 { get; set; } = "";
            public string line111 { get; set; } = "";
            public string line112 { get; set; } = "";
            public string line113 { get; set; } = "";
            public string line114 { get; set; } = "";
            public string line115 { get; set; } = "";
            public string line116 { get; set; } = "";
            public string line117 { get; set; } = "";
            public string line118 { get; set; } = "";
            public string line119 { get; set; } = "";
            public string line120 { get; set; } = "";
            public string line121 { get; set; } = "";
            public string line122 { get; set; } = "";
            public string line123 { get; set; } = "";
            public string line124 { get; set; } = "";
            public string line125 { get; set; } = "";
            public string line126 { get; set; } = "";
            public string line127 { get; set; } = "";
        }
        private static BardGroup ConvertDrum(Instrument instrument, Drum drumLegacy, int bardCount)
        {
            var bardGroup = new BardGroup
            {
                instruments =
                {
                    [NotateGroup.VST.VST0] =
                        new NotateGroup.VSTProperties() {instrument = instrument, octaveRange = OctaveRange.Mapper}
                },
                distributionBardCount = bardCount
            };
            foreach (var legacyTrack in drumLegacy.tracks) bardGroup.tracks.Add(legacyTrack.Key, new NotateGroup.Track());
            if (Instrument.Parse(drumLegacy.map_27_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(27, int.Parse(drumLegacy.map_27_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_28_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(28, int.Parse(drumLegacy.map_28_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_29_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(29, int.Parse(drumLegacy.map_29_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_30_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(30, int.Parse(drumLegacy.map_30_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_31_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(31, int.Parse(drumLegacy.map_31_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_32_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(32, int.Parse(drumLegacy.map_32_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_33_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(33, int.Parse(drumLegacy.map_33_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_34_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(34, int.Parse(drumLegacy.map_34_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_35_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(35, int.Parse(drumLegacy.map_35_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_36_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(36, int.Parse(drumLegacy.map_36_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_37_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(37, int.Parse(drumLegacy.map_37_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_38_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(38, int.Parse(drumLegacy.map_38_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_39_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(39, int.Parse(drumLegacy.map_39_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_40_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(40, int.Parse(drumLegacy.map_40_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_41_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(41, int.Parse(drumLegacy.map_41_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_42_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(42, int.Parse(drumLegacy.map_42_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_43_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(43, int.Parse(drumLegacy.map_43_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_44_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(44, int.Parse(drumLegacy.map_44_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_45_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(45, int.Parse(drumLegacy.map_45_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_46_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(46, int.Parse(drumLegacy.map_46_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_47_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(47, int.Parse(drumLegacy.map_47_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_48_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(48, int.Parse(drumLegacy.map_48_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_49_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(49, int.Parse(drumLegacy.map_49_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_50_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(50, int.Parse(drumLegacy.map_50_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_51_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(51, int.Parse(drumLegacy.map_51_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_52_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(52, int.Parse(drumLegacy.map_52_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_53_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(53, int.Parse(drumLegacy.map_53_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_54_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(54, int.Parse(drumLegacy.map_54_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_55_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(55, int.Parse(drumLegacy.map_55_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_56_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(56, int.Parse(drumLegacy.map_56_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_57_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(57, int.Parse(drumLegacy.map_57_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_58_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(58, int.Parse(drumLegacy.map_58_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_59_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(59, int.Parse(drumLegacy.map_59_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_60_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(60, int.Parse(drumLegacy.map_60_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_61_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(61, int.Parse(drumLegacy.map_61_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_62_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(62, int.Parse(drumLegacy.map_62_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_63_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(63, int.Parse(drumLegacy.map_63_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_64_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(64, int.Parse(drumLegacy.map_64_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_65_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(65, int.Parse(drumLegacy.map_65_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_66_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(66, int.Parse(drumLegacy.map_66_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_67_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(67, int.Parse(drumLegacy.map_67_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_68_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(68, int.Parse(drumLegacy.map_68_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_69_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(69, int.Parse(drumLegacy.map_69_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_70_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(70, int.Parse(drumLegacy.map_70_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_71_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(71, int.Parse(drumLegacy.map_71_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_72_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(72, int.Parse(drumLegacy.map_72_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_73_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(73, int.Parse(drumLegacy.map_73_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_74_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(74, int.Parse(drumLegacy.map_74_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_75_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(75, int.Parse(drumLegacy.map_75_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_76_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(76, int.Parse(drumLegacy.map_76_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_77_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(77, int.Parse(drumLegacy.map_77_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_78_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(78, int.Parse(drumLegacy.map_78_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_79_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(79, int.Parse(drumLegacy.map_79_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_80_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(80, int.Parse(drumLegacy.map_80_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_81_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(81, int.Parse(drumLegacy.map_81_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_82_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(82, int.Parse(drumLegacy.map_82_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_83_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(83, int.Parse(drumLegacy.map_83_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_84_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(84, int.Parse(drumLegacy.map_84_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_85_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(85, int.Parse(drumLegacy.map_85_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_86_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(86, int.Parse(drumLegacy.map_86_note.ToString()));
            if (Instrument.Parse(drumLegacy.map_87_drum).Equals(instrument)) bardGroup.instruments[NotateGroup.VST.VST0].noteMapper.Add(87, int.Parse(drumLegacy.map_87_note.ToString()));
            return bardGroup;
        }
        private static SingerGroup ConvertSinger(Lyric lyricLegacy)
        {
            var singerGroup = new SingerGroup { description = lyricLegacy.description };
            foreach (var trackLegacy in lyricLegacy.tracks) singerGroup.tracks.Add(trackLegacy.Key, new NotateGroup.Track());
            singerGroup.lyricMapper = new Dictionary<int, string>(128);
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line0)) singerGroup.lyricMapper[0] = lyricLegacy.line0;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line1)) singerGroup.lyricMapper[1] = lyricLegacy.line1;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line2)) singerGroup.lyricMapper[2] = lyricLegacy.line2;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line3)) singerGroup.lyricMapper[3] = lyricLegacy.line3;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line4)) singerGroup.lyricMapper[4] = lyricLegacy.line4;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line5)) singerGroup.lyricMapper[5] = lyricLegacy.line5;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line6)) singerGroup.lyricMapper[6] = lyricLegacy.line6;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line7)) singerGroup.lyricMapper[7] = lyricLegacy.line7;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line8)) singerGroup.lyricMapper[8] = lyricLegacy.line8;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line9)) singerGroup.lyricMapper[9] = lyricLegacy.line9;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line10)) singerGroup.lyricMapper[10] = lyricLegacy.line10;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line11)) singerGroup.lyricMapper[11] = lyricLegacy.line11;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line12)) singerGroup.lyricMapper[12] = lyricLegacy.line12;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line13)) singerGroup.lyricMapper[13] = lyricLegacy.line13;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line14)) singerGroup.lyricMapper[14] = lyricLegacy.line14;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line15)) singerGroup.lyricMapper[15] = lyricLegacy.line15;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line16)) singerGroup.lyricMapper[16] = lyricLegacy.line16;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line17)) singerGroup.lyricMapper[17] = lyricLegacy.line17;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line18)) singerGroup.lyricMapper[18] = lyricLegacy.line18;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line19)) singerGroup.lyricMapper[19] = lyricLegacy.line19;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line20)) singerGroup.lyricMapper[20] = lyricLegacy.line20;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line21)) singerGroup.lyricMapper[21] = lyricLegacy.line21;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line22)) singerGroup.lyricMapper[22] = lyricLegacy.line22;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line23)) singerGroup.lyricMapper[23] = lyricLegacy.line23;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line24)) singerGroup.lyricMapper[24] = lyricLegacy.line24;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line25)) singerGroup.lyricMapper[25] = lyricLegacy.line25;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line26)) singerGroup.lyricMapper[26] = lyricLegacy.line26;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line27)) singerGroup.lyricMapper[27] = lyricLegacy.line27;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line28)) singerGroup.lyricMapper[28] = lyricLegacy.line28;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line29)) singerGroup.lyricMapper[29] = lyricLegacy.line29;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line30)) singerGroup.lyricMapper[30] = lyricLegacy.line30;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line31)) singerGroup.lyricMapper[31] = lyricLegacy.line31;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line32)) singerGroup.lyricMapper[32] = lyricLegacy.line32;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line33)) singerGroup.lyricMapper[33] = lyricLegacy.line33;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line34)) singerGroup.lyricMapper[34] = lyricLegacy.line34;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line35)) singerGroup.lyricMapper[35] = lyricLegacy.line35;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line36)) singerGroup.lyricMapper[36] = lyricLegacy.line36;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line37)) singerGroup.lyricMapper[37] = lyricLegacy.line37;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line38)) singerGroup.lyricMapper[38] = lyricLegacy.line38;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line39)) singerGroup.lyricMapper[39] = lyricLegacy.line39;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line40)) singerGroup.lyricMapper[40] = lyricLegacy.line40;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line41)) singerGroup.lyricMapper[41] = lyricLegacy.line41;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line42)) singerGroup.lyricMapper[42] = lyricLegacy.line42;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line43)) singerGroup.lyricMapper[43] = lyricLegacy.line43;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line44)) singerGroup.lyricMapper[44] = lyricLegacy.line44;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line45)) singerGroup.lyricMapper[45] = lyricLegacy.line45;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line46)) singerGroup.lyricMapper[46] = lyricLegacy.line46;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line47)) singerGroup.lyricMapper[47] = lyricLegacy.line47;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line48)) singerGroup.lyricMapper[48] = lyricLegacy.line48;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line49)) singerGroup.lyricMapper[49] = lyricLegacy.line49;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line50)) singerGroup.lyricMapper[50] = lyricLegacy.line50;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line51)) singerGroup.lyricMapper[51] = lyricLegacy.line51;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line52)) singerGroup.lyricMapper[52] = lyricLegacy.line52;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line53)) singerGroup.lyricMapper[53] = lyricLegacy.line53;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line54)) singerGroup.lyricMapper[54] = lyricLegacy.line54;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line55)) singerGroup.lyricMapper[55] = lyricLegacy.line55;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line56)) singerGroup.lyricMapper[56] = lyricLegacy.line56;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line57)) singerGroup.lyricMapper[57] = lyricLegacy.line57;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line58)) singerGroup.lyricMapper[58] = lyricLegacy.line58;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line59)) singerGroup.lyricMapper[59] = lyricLegacy.line59;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line60)) singerGroup.lyricMapper[60] = lyricLegacy.line60;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line61)) singerGroup.lyricMapper[61] = lyricLegacy.line61;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line62)) singerGroup.lyricMapper[62] = lyricLegacy.line62;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line63)) singerGroup.lyricMapper[63] = lyricLegacy.line63;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line64)) singerGroup.lyricMapper[64] = lyricLegacy.line64;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line65)) singerGroup.lyricMapper[65] = lyricLegacy.line65;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line66)) singerGroup.lyricMapper[66] = lyricLegacy.line66;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line67)) singerGroup.lyricMapper[67] = lyricLegacy.line67;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line68)) singerGroup.lyricMapper[68] = lyricLegacy.line68;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line69)) singerGroup.lyricMapper[69] = lyricLegacy.line69;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line70)) singerGroup.lyricMapper[70] = lyricLegacy.line70;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line71)) singerGroup.lyricMapper[71] = lyricLegacy.line71;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line72)) singerGroup.lyricMapper[72] = lyricLegacy.line72;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line73)) singerGroup.lyricMapper[73] = lyricLegacy.line73;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line74)) singerGroup.lyricMapper[74] = lyricLegacy.line74;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line75)) singerGroup.lyricMapper[75] = lyricLegacy.line75;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line76)) singerGroup.lyricMapper[76] = lyricLegacy.line76;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line77)) singerGroup.lyricMapper[77] = lyricLegacy.line77;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line78)) singerGroup.lyricMapper[78] = lyricLegacy.line78;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line79)) singerGroup.lyricMapper[79] = lyricLegacy.line79;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line80)) singerGroup.lyricMapper[80] = lyricLegacy.line80;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line81)) singerGroup.lyricMapper[81] = lyricLegacy.line81;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line82)) singerGroup.lyricMapper[82] = lyricLegacy.line82;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line83)) singerGroup.lyricMapper[83] = lyricLegacy.line83;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line84)) singerGroup.lyricMapper[84] = lyricLegacy.line84;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line85)) singerGroup.lyricMapper[85] = lyricLegacy.line85;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line86)) singerGroup.lyricMapper[86] = lyricLegacy.line86;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line87)) singerGroup.lyricMapper[87] = lyricLegacy.line87;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line88)) singerGroup.lyricMapper[88] = lyricLegacy.line88;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line89)) singerGroup.lyricMapper[89] = lyricLegacy.line89;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line90)) singerGroup.lyricMapper[90] = lyricLegacy.line90;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line91)) singerGroup.lyricMapper[91] = lyricLegacy.line91;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line92)) singerGroup.lyricMapper[92] = lyricLegacy.line92;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line93)) singerGroup.lyricMapper[93] = lyricLegacy.line93;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line94)) singerGroup.lyricMapper[94] = lyricLegacy.line94;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line95)) singerGroup.lyricMapper[95] = lyricLegacy.line95;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line96)) singerGroup.lyricMapper[96] = lyricLegacy.line96;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line97)) singerGroup.lyricMapper[97] = lyricLegacy.line97;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line98)) singerGroup.lyricMapper[98] = lyricLegacy.line98;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line99)) singerGroup.lyricMapper[99] = lyricLegacy.line99;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line100)) singerGroup.lyricMapper[100] = lyricLegacy.line100;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line101)) singerGroup.lyricMapper[101] = lyricLegacy.line101;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line102)) singerGroup.lyricMapper[102] = lyricLegacy.line102;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line103)) singerGroup.lyricMapper[103] = lyricLegacy.line103;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line104)) singerGroup.lyricMapper[104] = lyricLegacy.line104;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line105)) singerGroup.lyricMapper[105] = lyricLegacy.line105;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line106)) singerGroup.lyricMapper[106] = lyricLegacy.line106;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line107)) singerGroup.lyricMapper[107] = lyricLegacy.line107;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line108)) singerGroup.lyricMapper[108] = lyricLegacy.line108;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line109)) singerGroup.lyricMapper[109] = lyricLegacy.line109;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line110)) singerGroup.lyricMapper[110] = lyricLegacy.line110;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line111)) singerGroup.lyricMapper[111] = lyricLegacy.line111;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line112)) singerGroup.lyricMapper[112] = lyricLegacy.line112;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line113)) singerGroup.lyricMapper[113] = lyricLegacy.line113;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line114)) singerGroup.lyricMapper[114] = lyricLegacy.line114;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line115)) singerGroup.lyricMapper[115] = lyricLegacy.line115;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line116)) singerGroup.lyricMapper[116] = lyricLegacy.line116;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line117)) singerGroup.lyricMapper[117] = lyricLegacy.line117;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line118)) singerGroup.lyricMapper[118] = lyricLegacy.line118;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line119)) singerGroup.lyricMapper[119] = lyricLegacy.line119;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line120)) singerGroup.lyricMapper[120] = lyricLegacy.line120;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line121)) singerGroup.lyricMapper[121] = lyricLegacy.line121;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line122)) singerGroup.lyricMapper[122] = lyricLegacy.line122;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line123)) singerGroup.lyricMapper[123] = lyricLegacy.line123;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line124)) singerGroup.lyricMapper[124] = lyricLegacy.line124;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line125)) singerGroup.lyricMapper[125] = lyricLegacy.line125;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line126)) singerGroup.lyricMapper[126] = lyricLegacy.line126;
            if (!string.IsNullOrWhiteSpace(lyricLegacy.line127)) singerGroup.lyricMapper[127] = lyricLegacy.line127;
            return singerGroup;
        }
    }
}
