/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BardMusicPlayer.Quotidian;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Transmogrify.Song.Config;
using BardMusicPlayer.Transmogrify.Song.Config.Interfaces;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Transmogrify.Song.Utilities
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
            var configContainers = new Dictionary<long, ConfigContainer>();

            if (trackChunk.GetNotes().Count == 0 && trackChunk.GetTimedEvents().All(x => x.Event.EventType != MidiEventType.Lyric))
            {
                BmpLog.I(BmpLog.Source.Transmogrify, "Skipping track " + trackNumber + " as it contains no notes and contains no lyric events.");
                return configContainers;
            }

            var trackName = (trackChunk.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text ?? "").Replace(" ", "").ToLower();

            if (trackName.Contains("ignore"))
            {
                BmpLog.I(BmpLog.Source.Transmogrify, "Skipping track " + trackNumber + " as the track title contains \"Ignore\"");
                return configContainers;
            }

            var groups = trackName.Split('|');

            var modifier = new Regex(@"^([A-Za-z0-9]+)([-+]\d)?");

            for(var groupCounter = 0; groupCounter < groups.Length; groupCounter++)
            {
                var configContainer = new ConfigContainer();
                var fields = groups[groupCounter].Split(';');

                if (fields.Length == 0) continue;

                // bmp 2.x style group name
                if (fields[0].StartsWith("vst:") || fields[0].Equals("lyrics"))
                {
                    var subfields = fields[0].Split(':');

                    switch (subfields[0])
                    {
                        case "vst" when subfields.Length < 2:
                            BmpLog.W(BmpLog.Source.Transmogrify, "Skipping VST on track " + trackNumber + " due to the configuration not specifying a tone.");
                            continue;
                        case "vst":
                            var manualToneConfig = (VSTProcessorConfig)(configContainer.ProcessorConfig = new VSTProcessorConfig { Track = trackNumber });
                            manualToneConfig.InstrumentTone = InstrumentTone.Parse(subfields[1]);
                            if (manualToneConfig.InstrumentTone.Equals(InstrumentTone.None))
                            {
                                BmpLog.W(BmpLog.Source.Transmogrify, "Skipping VST on track " + trackNumber + " due to the configuration specifying an invalid tone.");
                                continue;
                            }
                            if (subfields.Length > 2)
                            {
                                var shifts = subfields[2].Split(',');
                                foreach (var shift in shifts)
                                {
                                    var toneIndexAndOctaveRange = modifier.Match(shift);
                                    if (!toneIndexAndOctaveRange.Success)
                                    {
                                        BmpLog.W(BmpLog.Source.Transmogrify, "Skipping invalid VST octave setting \"" + shift + "\" on track " + trackNumber);
                                        continue;
                                    }
                                    if (!toneIndexAndOctaveRange.Groups[1].Success)
                                    {
                                        BmpLog.W(BmpLog.Source.Transmogrify, "Skipping invalid VST octave setting \"" + shift + "\" on track " + trackNumber + " because \"" + toneIndexAndOctaveRange.Groups[1].Value + "\" is not a valid tone number");
                                        continue;
                                    }
                                    if (!int.TryParse(toneIndexAndOctaveRange.Groups[1].Value, out var toneIndex) || toneIndex < 0 || toneIndex > 4)
                                    {
                                        BmpLog.W(BmpLog.Source.Transmogrify, "Skipping invalid VST octave setting \"" + shift + "\" on track " + trackNumber + " because \"" + toneIndexAndOctaveRange.Groups[1].Value + "\" is not a valid tone number");
                                        continue;
                                    }
                                    var octaveRange = OctaveRange.C3toC6;
                                    if (toneIndexAndOctaveRange.Groups[2].Success) octaveRange = OctaveRange.Parse(toneIndexAndOctaveRange.Groups[2].Value);
                                    if (octaveRange.Equals(OctaveRange.Invalid)) octaveRange = OctaveRange.C3toC6;
                                    manualToneConfig.OctaveRanges[toneIndex] = octaveRange;
                                }
                            }
                            ParseAdditionalOptions(trackNumber, manualToneConfig, song, fields);
                            BmpLog.I(BmpLog.Source.Transmogrify, "Found VST Config Group with on track " + manualToneConfig.Track + " ;bards=" + manualToneConfig.PlayerCount + ";include=" + string.Join(",",manualToneConfig.IncludedTracks));
                            configContainers.Add(groupCounter, configContainer);
                            continue;

                        case "lyrics":
                            var lyricConfig = (LyricProcessorConfig)(configContainer.ProcessorConfig = new LyricProcessorConfig { Track = trackNumber });
                            ParseAdditionalOptions(trackNumber, lyricConfig, song, fields);
                            BmpLog.I(BmpLog.Source.Transmogrify, "Found Lyric Config on track " + lyricConfig.Track + " ;bards=" + lyricConfig.PlayerCount + ";include=" + string.Join(",",lyricConfig.IncludedTracks));
                            configContainers.Add(groupCounter, configContainer);
                            continue;
                    }
                } 

                // bmp 1.x style group name
                else
                {
                    var classicConfig = (ClassicProcessorConfig)(configContainer.ProcessorConfig = new ClassicProcessorConfig { Track = trackNumber });
                    var instrumentAndOctaveRange = modifier.Match(fields[0]);
                    if (!instrumentAndOctaveRange.Success) continue; // Invalid Instrument name.
                    if (instrumentAndOctaveRange.Groups[1].Success) classicConfig.Instrument = Instrument.Parse(instrumentAndOctaveRange.Groups[1].Value);
                    if (classicConfig.Instrument.Equals(Instrument.None)) continue;  // Invalid Instrument name.
                    if (instrumentAndOctaveRange.Groups[2].Success) classicConfig.OctaveRange = OctaveRange.Parse(instrumentAndOctaveRange.Groups[2].Value);
                    if (classicConfig.OctaveRange.Equals(OctaveRange.Invalid)) classicConfig.OctaveRange = OctaveRange.C3toC6;
                    ParseAdditionalOptions(trackNumber, classicConfig, song, fields);
                    BmpLog.I(BmpLog.Source.Transmogrify, "Found Classic Config Instrument " + classicConfig.Instrument.Name + " OctaveRange " + classicConfig.OctaveRange.Name +" on track " + classicConfig.Track + " ;bards=" + classicConfig.PlayerCount + ";include=" + string.Join(",",classicConfig.IncludedTracks));
                    configContainers.Add(groupCounter, configContainer);
                }
            }

            if (configContainers.Count == 0)
            {
                BmpLog.I(BmpLog.Source.Transmogrify, "Found 0 configurations on track " + trackNumber + ", and the keyword \"Ignore\" is not in the track title. Adding a default harp.");
                configContainers.Add(0, new ConfigContainer { ProcessorConfig = new ClassicProcessorConfig { Track = trackNumber } });
            }

            return configContainers;
        }

        /// <summary>
        /// Parses tracks to merge in, and bards to load balance distribute to.
        /// </summary>
        /// <param name="trackNumber"></param>
        /// <param name="processorConfig"></param>
        /// <param name="song"></param>
        /// <param name="fields"></param>
        private static void ParseAdditionalOptions(int trackNumber, IProcessorConfig processorConfig, BmpSong song, IReadOnlyList<string> fields)
        {
            for (var fieldCounter = 1; fieldCounter < fields.Count; fieldCounter++)
            {
                if (fields[fieldCounter].StartsWith("include="))
                {
                    var tracksToMerge = fields[fieldCounter].Remove(0, 8).Split(',');
                    foreach (var trackToMerge in tracksToMerge)
                    {
                        if (int.TryParse(trackToMerge, out var value) && value != trackNumber && value > -1 && value < song.TrackContainers.Count)
                            processorConfig.IncludedTracks.Add(value);
                    }
                }
                else if (fields[fieldCounter].StartsWith("bards=") && int.TryParse(fields[fieldCounter].Remove(0, 6), out var value) && value > 0 && value < 17)
                    processorConfig.PlayerCount = value;
            }
        }
    }
}
