using BardMusicPlayer.Common.Structs;
using BardMusicPlayer.Notate.Song.Config;
using Melanchall.DryWetMidi.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
        internal static Dictionary<int, ConfigContainer> ReadConfigs(this TrackChunk trackChunk, int trackNumber, BmpSong song)
        {
            var configContainers = new Dictionary<int, ConfigContainer>();

            var trackName = trackChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text;

            var groups = trackName.Replace(" ", "").ToLower().Split('|');

            for(var groupCounter = 0; groupCounter < groups.Length; groupCounter++)
            {
                var configContainer = new ConfigContainer();
                var fields = groups[groupCounter].Split(';');

                if (fields.Length == 0) continue;

                // bmp 2.x style group name
                if (fields[0].StartsWith("tone:") || fields[0].StartsWith("autotone:"))
                {
                    // TODO bmp 2.x stuff.
                    continue;
                }

                // bmp 1.x style group name
                else
                {
                    ClassicConfig config = (ClassicConfig)(configContainer.Config = new ClassicConfig() { Track = trackNumber });

                    var instrumentNameSplitRegex = new Regex(@"^([A-Za-z]+)([-+]\d)?");

                    var instrumentAndOctaveRange = instrumentNameSplitRegex.Match(fields[0]);

                    if (instrumentAndOctaveRange.Success)
                    {
                        if (instrumentAndOctaveRange.Groups[1].Success) config.Instrument = Instrument.Parse(instrumentAndOctaveRange.Groups[1].Value);
                        if (config.Instrument.Equals(Instrument.None)) config.Instrument = Instrument.Piano;
                        if (instrumentAndOctaveRange.Groups[2].Success) config.OctaveRange = OctaveRange.Parse(instrumentAndOctaveRange.Groups[2].Value);
                        if (config.OctaveRange.Equals(OctaveRange.Invalid)) config.OctaveRange = OctaveRange.C3toC6;
                    }

                    for (var fieldCounter = 1; fieldCounter < fields.Length; fieldCounter++)
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

                    configContainers.Add(groupCounter, configContainer);
                    continue;
                }
            }

            // No configuration matches default a single group to bmp 1.x piano on c3. Maybe this can be replaced with bmp 2.x autotone soon?
            if (configContainers.Count == 0) configContainers.Add(0, new ConfigContainer { Config = new ClassicConfig() { Track = trackNumber } });

            return configContainers;
        }
    }
}
