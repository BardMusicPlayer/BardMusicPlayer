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
                if (fields[0].StartsWith("vst:") || fields[0].StartsWith("notetone:") || fields[0].StartsWith("autotone:") || fields[0].StartsWith("drumtone:") || fields[0].Equals("drumtone") || fields[0].StartsWith("octavetone") || fields[0].Equals("lyric"))
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

                        case "notetone" when subfields.Length < 2:
                            BmpLog.W(BmpLog.Source.Transmogrify, "Skipping NoteTone on track " + trackNumber + " due to the configuration not specifying a tone.");
                            continue;
                        case "notetone":
                        {
                            var noteToneConfig = (NoteToneProcessorConfig)(configContainer.ProcessorConfig = new NoteToneProcessorConfig { Track = trackNumber });
                            noteToneConfig.InstrumentTone = InstrumentTone.Parse(subfields[1]);
                            if (noteToneConfig.InstrumentTone.Equals(InstrumentTone.None))
                            {
                                BmpLog.W(BmpLog.Source.Transmogrify, "Skipping NoteTone on track " + trackNumber + " due to the configuration specifying an invalid tone.");
                                continue;
                            }
                            var noteToneSubConfigurations = 0;
                            if (subfields.Length > 2)
                            {
                                subfields = subfields.Skip(2).ToArray();
                                foreach (var mapping in subfields)
                                {
                                    var split = mapping.Split(',');
                                    if (split.Length != 3)
                                    {
                                        BmpLog.W(BmpLog.Source.Transmogrify, "Skipping invalid NoteTone mapping \"" + mapping + "\" on track " + trackNumber);
                                        continue;
                                    }
                                    if (!int.TryParse(split[0], out var sourceNote) || sourceNote > 120 || sourceNote < 12)
                                    {
                                        BmpLog.W(BmpLog.Source.Transmogrify, "Skipping invalid NoteTone mapping \"" + mapping + "\" on track " + trackNumber + " because source note \"" + split[0] + "\" is more then 120 or less then 12");
                                        continue;
                                    }
                                    if (!int.TryParse(split[1], out var toneIndex) || toneIndex < -1 || toneIndex > 4 || (toneIndex == 0 && noteToneConfig.InstrumentTone.Tone0.Equals(Instrument.None)) || (toneIndex == 1 && noteToneConfig.InstrumentTone.Tone1.Equals(Instrument.None)) || (toneIndex == 2 && noteToneConfig.InstrumentTone.Tone2.Equals(Instrument.None)) || (toneIndex == 3 && noteToneConfig.InstrumentTone.Tone3.Equals(Instrument.None)) || (toneIndex == 4 && noteToneConfig.InstrumentTone.Tone4.Equals(Instrument.None)))
                                    {
                                        BmpLog.W(BmpLog.Source.Transmogrify, "Skipping invalid NoteTone mapping \"" + mapping + "\" on track " + trackNumber + " because \"" + split[1] + "\" is not a valid tone number for Tone " + noteToneConfig.InstrumentTone.Name);
                                        continue;
                                    }
                                    if (!int.TryParse(split[2], out var destinationNote) || destinationNote < -1 || destinationNote > 36)
                                    {
                                        BmpLog.W(BmpLog.Source.Transmogrify, "Skipping invalid NoteTone mapping \"" + mapping + "\" on track " + trackNumber + " because destination note \"" + split[2] + "\" is more then 36 or less then -1");
                                        continue;
                                    }
                                    noteToneConfig.Mapper[sourceNote] = (toneIndex, destinationNote);
                                    noteToneSubConfigurations++;
                                }
                            }
                            if (noteToneSubConfigurations == 0)
                            {
                                BmpLog.W(BmpLog.Source.Transmogrify, "Skipping NoteTone on track " + trackNumber + " because no mappings are specified.");
                                continue;
                            }
                            ParseAdditionalOptions(trackNumber, noteToneConfig, song, fields);
                            BmpLog.I(BmpLog.Source.Transmogrify, "Found NoteTone Config Group " + noteToneConfig.InstrumentTone.Name + " with " + noteToneSubConfigurations + " mappings on track " + noteToneConfig.Track + " ;bards=" + noteToneConfig.PlayerCount + ";include=" + string.Join(",",noteToneConfig.IncludedTracks));
                            configContainers.Add(groupCounter, configContainer);
                            continue;
                        }

                        case "octavetone" when subfields.Length < 2:
                            BmpLog.W(BmpLog.Source.Transmogrify, "Skipping OctaveTone on track " + trackNumber + " due to the configuration not specifying a tone.");
                            continue;
                        case "octavetone":
                            var octaveToneConfig = (OctaveToneProcessorConfig)(configContainer.ProcessorConfig = new OctaveToneProcessorConfig { Track = trackNumber });
                            octaveToneConfig.InstrumentTone = InstrumentTone.Parse(subfields[1]);
                            if (octaveToneConfig.InstrumentTone.Equals(InstrumentTone.None))
                            {
                                BmpLog.W(BmpLog.Source.Transmogrify, "Skipping OctaveTone on track " + trackNumber + " due to the configuration specifying an invalid tone.");
                                continue;
                            }
                            var octaveToneSubConfigurations = 0;
                            if (subfields.Length > 2)
                            {
                                subfields = subfields.Skip(2).ToArray();
                                foreach (var mapping in subfields)
                                {
                                    var split = mapping.Split(',');
                                    if (split.Length != 3)
                                    {
                                        BmpLog.W(BmpLog.Source.Transmogrify, "Skipping invalid OctaveTone mapping \"" + mapping + "\" on track " + trackNumber);
                                        continue;
                                    }
                                    if (!int.TryParse(split[0], out var sourceOctave) || sourceOctave > 8 || sourceOctave < 0)
                                    {
                                        BmpLog.W(BmpLog.Source.Transmogrify, "Skipping invalid OctaveTone mapping \"" + mapping + "\" on track " + trackNumber + " because source octave \"" + split[0] + "\" is more then 8 or less then 0");
                                        continue;
                                    }
                                    if (!int.TryParse(split[1], out var toneIndex) || toneIndex < -1 || toneIndex > 4 || (toneIndex == 0 && octaveToneConfig.InstrumentTone.Tone0.Equals(Instrument.None)) || (toneIndex == 1 && octaveToneConfig.InstrumentTone.Tone1.Equals(Instrument.None)) || (toneIndex == 2 && octaveToneConfig.InstrumentTone.Tone2.Equals(Instrument.None)) || (toneIndex == 3 && octaveToneConfig.InstrumentTone.Tone3.Equals(Instrument.None)) || (toneIndex == 4 && octaveToneConfig.InstrumentTone.Tone4.Equals(Instrument.None)))
                                    {
                                        BmpLog.W(BmpLog.Source.Transmogrify, "Skipping invalid OctaveTone mapping \"" + mapping + "\" on track " + trackNumber + " because \"" + split[1] + "\" is not a valid tone number for Tone " + octaveToneConfig.InstrumentTone.Name);
                                        continue;
                                    }
                                    if (!int.TryParse(split[2], out var destinationOctave) || destinationOctave < -1 || destinationOctave > 3)
                                    {
                                        BmpLog.W(BmpLog.Source.Transmogrify, "Skipping invalid OctaveTone mapping \"" + mapping + "\" on track " + trackNumber + " because destination octave \"" + split[2] + "\" is more then 3 or less then -1");
                                        continue;
                                    }
                                    octaveToneConfig.Mapper[sourceOctave] = (toneIndex, destinationOctave);
                                    octaveToneSubConfigurations++;
                                }
                            }
                            if (octaveToneSubConfigurations == 0)
                            {
                                BmpLog.W(BmpLog.Source.Transmogrify, "Skipping OctaveTone on track " + trackNumber + " because no mappings are specified.");
                                continue;
                            }
                            ParseAdditionalOptions(trackNumber, octaveToneConfig, song, fields);
                            BmpLog.I(BmpLog.Source.Transmogrify, "Found OctaveTone Config Group " + octaveToneConfig.InstrumentTone.Name + " with " + octaveToneSubConfigurations + " mappings on track " + octaveToneConfig.Track + " ;bards=" + octaveToneConfig.PlayerCount + ";include=" + string.Join(",",octaveToneConfig.IncludedTracks));
                            configContainers.Add(groupCounter, configContainer);
                            continue;

                        case "autotone" when subfields.Length < 2:
                            BmpLog.W(BmpLog.Source.Transmogrify, "Skipping AutoTone on track " + trackNumber + " due to the configuration not specifying an autotone group.");
                            continue;
                        case "autotone":
                        {
                            var autoToneConfig = (AutoToneProcessorConfig)(configContainer.ProcessorConfig = new AutoToneProcessorConfig { Track = trackNumber });
                            var instrumentAndOctaveRange = modifier.Match(subfields[1]);
                            if (!instrumentAndOctaveRange.Success)
                            {
                                BmpLog.W(BmpLog.Source.Transmogrify, "Skipping AutoTone on track " + trackNumber + " due to the configuration specifying an invalid autotone group.");
                                continue;
                            }
                            if (instrumentAndOctaveRange.Groups[1].Success) autoToneConfig.AutoToneInstrumentGroup = AutoToneInstrumentGroup.Parse(instrumentAndOctaveRange.Groups[1].Value);
                            if (autoToneConfig.AutoToneInstrumentGroup.Equals(AutoToneInstrumentGroup.Invalid))
                            {
                                BmpLog.W(BmpLog.Source.Transmogrify, "Skipping AutoTone on track " + trackNumber + " due to the configuration specifying an invalid autotone group.");
                                continue;
                            }
                            if (instrumentAndOctaveRange.Groups[2].Success) autoToneConfig.AutoToneOctaveRange = AutoToneOctaveRange.Parse(instrumentAndOctaveRange.Groups[2].Value);
                            if (autoToneConfig.AutoToneOctaveRange.Equals(AutoToneOctaveRange.Invalid)) autoToneConfig.AutoToneOctaveRange = AutoToneOctaveRange.C2toC7;
                            ParseAdditionalOptions(trackNumber, autoToneConfig, song, fields);
                            BmpLog.I(BmpLog.Source.Transmogrify, "Found AutoTone Config Group " + autoToneConfig.AutoToneInstrumentGroup.Name + " OctaveRange " + autoToneConfig.AutoToneOctaveRange.Name +" on track " + autoToneConfig.Track + " ;bards=" + autoToneConfig.PlayerCount + ";include=" + string.Join(",",autoToneConfig.IncludedTracks));
                            configContainers.Add(groupCounter, configContainer);
                            continue;
                        }

                        case "drumtone":
                            var drumToneConfig = (DrumToneProcessorConfig)(configContainer.ProcessorConfig = new DrumToneProcessorConfig { Track = trackNumber });
                            var drumToneSubConfigurations = 0;
                            if (subfields.Length > 1)
                            {
                                subfields = subfields.Skip(1).ToArray();
                                foreach (var mapping in subfields)
                                {
                                    var split = mapping.Split(',');
                                    if (split.Length != 3)
                                    {
                                        BmpLog.W(BmpLog.Source.Transmogrify, "Skipping invalid DrumTone mapping \"" + mapping + "\" on track " + trackNumber);
                                        continue;
                                    }
                                    if (!int.TryParse(split[0], out var sourceNote) || sourceNote > 87 || sourceNote < 27)
                                    {
                                        BmpLog.W(BmpLog.Source.Transmogrify, "Skipping invalid DrumTone mapping \"" + mapping + "\" on track " + trackNumber + " because source note \"" + split[0] + "\" is more then 87 or less then 27");
                                        continue;
                                    }
                                    if (!int.TryParse(split[1], out var toneIndex) || toneIndex < -1 || toneIndex > 4 || (toneIndex == 0 && InstrumentTone.Drums.Tone0.Equals(Instrument.None)) || (toneIndex == 1 && InstrumentTone.Drums.Tone1.Equals(Instrument.None)) || (toneIndex == 2 && InstrumentTone.Drums.Tone2.Equals(Instrument.None)) || (toneIndex == 3 && InstrumentTone.Drums.Tone3.Equals(Instrument.None)) || (toneIndex == 4 && InstrumentTone.Drums.Tone4.Equals(Instrument.None)))
                                    {
                                        BmpLog.W(BmpLog.Source.Transmogrify, "Skipping invalid DrumTone mapping \"" + mapping + "\" on track " + trackNumber + " because \"" + split[1] + "\" is not a valid tone number for Tone " + InstrumentTone.Drums.Name);
                                        continue;
                                    }
                                    if (!int.TryParse(split[2], out var destinationNote) || destinationNote < -1 || destinationNote > 36)
                                    {
                                        BmpLog.W(BmpLog.Source.Transmogrify, "Skipping invalid DrumTone mapping \"" + mapping + "\" on track " + trackNumber + " because destination note \"" + split[2] + "\" is more then 36 or less then -1");
                                        continue;
                                    }
                                    drumToneConfig.Mapper[sourceNote] = (toneIndex, destinationNote);
                                    drumToneSubConfigurations++;
                                }
                            }
                            ParseAdditionalOptions(trackNumber, drumToneConfig, song, fields);
                            BmpLog.I(BmpLog.Source.Transmogrify, "Found DrumTone Config Group with " + drumToneSubConfigurations + " override mappings on track " + drumToneConfig.Track + " ;bards=" + drumToneConfig.PlayerCount + ";include=" + string.Join(",",drumToneConfig.IncludedTracks));
                            configContainers.Add(groupCounter, configContainer);
                            continue;

                        case "lyric":
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
                BmpLog.I(BmpLog.Source.Transmogrify, "Found 0 configurations on track " + trackNumber + ", and the keyword \"Ignore\" is not in the track title. Adding a default AutoTone.");
                configContainers.Add(0, new ConfigContainer { ProcessorConfig = new AutoToneProcessorConfig { Track = trackNumber } });
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
