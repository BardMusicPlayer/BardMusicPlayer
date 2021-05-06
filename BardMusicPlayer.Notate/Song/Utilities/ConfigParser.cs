/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Common.Structs;
using BardMusicPlayer.Notate.Song.Config;
using Melanchall.DryWetMidi.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BardMusicPlayer.Notate.Song.Config.Interfaces;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Notate.Song.Utilities
{
    internal static class ConfigParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunk"></param>
        /// <param name="configContainers"></param>
        /// <returns></returns>
        internal static void WriteConfigs(this TrackChunk trackChunk, Dictionary<int, ConfigContainer> configContainers)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunk"></param>
        /// <param name="trackNumber"></param>
        /// <param name="song"></param>
        /// <returns></returns>
        internal static Dictionary<long, ConfigContainer> ReadConfigs(this TrackChunk trackChunk, int trackNumber, BmpSong song)
        {
            // TODO - Verbose logging of all parsing operations.

            var configContainers = new Dictionary<long, ConfigContainer>();

            if (trackChunk.GetNotes().Count == 0) return configContainers;

            var trackName = (trackChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text ?? "").Replace(" ", "").ToLower();

            if (trackName.StartsWith("ignore")) return configContainers;

            var groups = trackName.Split('|');

            

            var modifier = new Regex(@"^([A-Za-z0-9]+)([-+]\d)?");

            for(var groupCounter = 0; groupCounter < groups.Length; groupCounter++)
            {
                var configContainer = new ConfigContainer();
                var fields = groups[groupCounter].Split(';');

                if (fields.Length == 0) continue;

                // bmp 2.x style group name
                if (fields[0].StartsWith("manualtone:") || fields[0].StartsWith("autotone:") || fields[0].StartsWith("drumtone:") || fields[0].StartsWith("lyric:"))
                {
                    var subfields = fields[0].Split(':');

                    if (subfields.Length < 2) continue;

                    if (subfields[0].Equals("manualtone"))
                    {
                        // TODO
                        return configContainers;
                    } 
                    
                    else if (subfields[0].Equals("autotone"))
                    {
                        var config = (AutoToneConfig)(configContainer.Config = new AutoToneConfig { Track = trackNumber });
                        var instrumentAndOctaveRange = modifier.Match(subfields[1]);

                        if (instrumentAndOctaveRange.Success)
                        {
                            if (instrumentAndOctaveRange.Groups[1].Success) config.AutoToneInstrumentGroup = AutoToneInstrumentGroup.Parse(instrumentAndOctaveRange.Groups[1].Value);
                            if (config.AutoToneInstrumentGroup.Equals(AutoToneInstrumentGroup.Invalid)) config.AutoToneInstrumentGroup = AutoToneInstrumentGroup.Lute1Harp3Piano1;
                            if (instrumentAndOctaveRange.Groups[2].Success) config.AutoToneOctaveRange = AutoToneOctaveRange.Parse(instrumentAndOctaveRange.Groups[2].Value);
                            if (config.AutoToneOctaveRange.Equals(AutoToneOctaveRange.Invalid)) config.AutoToneOctaveRange = AutoToneOctaveRange.C2toC7;
                        }

                        ParseAdditionalOptions(config, song, fields);

                        configContainers.Add(groupCounter, configContainer);
                    }

                    else if (subfields[0].Equals("drumtone"))
                    {
                        // TODO
                        return configContainers;
                    }

                    else if (subfields[0].Equals("lyric") && subfields[1].Equals("default"))
                    {
                        var config = (LyricConfig)(configContainer.Config = new LyricConfig { Track = trackNumber });
                        ParseAdditionalOptions(config, song, fields);
                        configContainers.Add(groupCounter, configContainer);
                    }
                }

                // bmp 1.x style group name
                else
                {
                    var config = (ClassicConfig)(configContainer.Config = new ClassicConfig { Track = trackNumber });
                    
                    var instrumentAndOctaveRange = modifier.Match(fields[0]);

                    if (instrumentAndOctaveRange.Success)
                    {
                        if (instrumentAndOctaveRange.Groups[1].Success) config.Instrument = Instrument.Parse(instrumentAndOctaveRange.Groups[1].Value);
                        if (config.Instrument.Equals(Instrument.None)) config.Instrument = Instrument.Harp;
                        if (instrumentAndOctaveRange.Groups[2].Success) config.OctaveRange = OctaveRange.Parse(instrumentAndOctaveRange.Groups[2].Value);
                        if (config.OctaveRange.Equals(OctaveRange.Invalid)) config.OctaveRange = OctaveRange.C3toC6;
                    }

                    ParseAdditionalOptions(config, song, fields);

                    configContainers.Add(groupCounter, configContainer);
                }
            }

            // TODO: let's expose the "default" track configuration as UI setting.
            if (configContainers.Count == 0) configContainers.Add(0, new ConfigContainer { Config = new ClassicConfig { Track = trackNumber } });

            return configContainers;
        }

        /// <summary>
        /// Parses tracks to merge in, and bards to load balance distribute to.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="song"></param>
        /// <param name="fields"></param>
        private static void ParseAdditionalOptions(IConfig config, BmpSong song, IReadOnlyList<string> fields)
        {
            for (var fieldCounter = 1; fieldCounter < fields.Count; fieldCounter++)
            {
                if (fields[fieldCounter].StartsWith("include="))
                {
                    var tracksToMerge = fields[fieldCounter].Remove(0, 8).Split(',');
                    foreach (var trackToMerge in tracksToMerge)
                    {
                        if (int.TryParse(trackToMerge, out var value) && value < song.TrackContainers.Count)
                            config.IncludedTracks.Add(value);
                    }
                }
                else if (fields[fieldCounter].StartsWith("bards=") && int.TryParse(fields[fieldCounter].Remove(0, 6), out var value) && value > 0 && value < 17)
                    config.PlayerCount = value;
            }
        }
    }
}
