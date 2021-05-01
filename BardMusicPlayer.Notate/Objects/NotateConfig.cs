/*
 * MogLib/Notate/Objects/NotateConfig.cs
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ServiceStack;
using BardMusicPlayer.Common.Structs;
using Melanchall.DryWetMidi.Core;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;
using YamlDotNet.Serialization.TypeInspectors;
using static BardMusicPlayer.Notate.Objects.NotateConfig.NotateGroup;

namespace BardMusicPlayer.Notate.Objects
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NotateConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public int schemaVersion { get; internal set; } = Constants.SchemaVersion;

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue("")]
        public string title { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue("")]
        public string description { get; set; } = "";

        /// <summary>
        /// The last known midi filename this config converted.
        /// </summary>
        [DefaultValue("")]
        public string midiFileName { get; set; } = "";

        internal string lastFilePath { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public List<BardGroup> bardGroups { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        public List<SingerGroup> singerGroups { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        public abstract class NotateGroup
        {
            internal NotateGroup()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            [DefaultValue("")]
            public string description { get; set; } = "";

            /// <summary>
            /// 
            /// </summary>
            public Dictionary<int, Track> tracks { get; set; } = new();



            /// <summary>
            /// 
            /// </summary>
            public List<Modifier> postMergeModifiers { get; set; } = new();

            /// <summary>
            /// 
            /// </summary>
            public sealed class Track
            {
                /// <summary>
                /// 
                /// </summary>
                public List<Modifier> preMergeModifiers { get; set; } = new();
            }

            /// <summary>
            /// 
            /// </summary>
            public sealed class Modifier
            {
                /// <summary>
                /// 
                /// </summary>
                [DefaultValue(Type.None)]
                public Type type { get; set; } = Type.None;

                /// <summary>
                /// 
                /// </summary>
                public Dictionary<string, string> configuration { get; set; } = new();

                /// <summary>
                /// 
                /// </summary>
                public enum Type
                {
                    None = 0
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public sealed class VSTProperties
            {
                /// <summary>
                /// 
                /// </summary>
                [DefaultValue(-1)]
                public Instrument instrument { get; set; } = Instrument.None;

                /// <summary>
                /// 
                /// </summary>
                [DefaultValue(3)]
                public OctaveRange octaveRange { get; set; } = OctaveRange.C3toC6;

                /// <summary>
                /// 
                /// </summary>
                public Dictionary<int, int> noteMapper { get; set; } = new(121);
            }

            /// <summary>
            /// 
            /// </summary>
            public enum VST
            {
                VST0 = 127
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class BardGroup : NotateGroup
        {
            public BardGroup() { }
            public BardGroup (IEnumerable<int> tracks, string description = "")
            {
                this.description = description;
                foreach(var track in tracks) this.tracks.Add(track, new Track());
            }
            /// <summary>
            /// 
            /// </summary>
            [DefaultValue(1)]
            public int distributionBardCount { get; set; } = 1;

            /// <summary>
            /// 
            /// </summary>
            public Dictionary<VST, VSTProperties> instruments { get; set; } = new(7) { { VST.VST0, new VSTProperties() } };
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class SingerGroup : NotateGroup
        {
            /// <summary>
            /// 
            /// </summary>
            public Dictionary<int, string> lyricMapper { get; set; } = new(128);
        }

        /// <summary>
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static NotateConfig Open(string filePath)
        {
            var fileContents = File.ReadAllLines(filePath);
            if (fileContents.Any(line => line.ToLower().StartsWith("songs:")))
                return NotateConfigLegacy.Transform(fileContents);
            var deserializer = new DeserializerBuilder().WithTypeConverter(new NotateConfigExtensions.InstrumentConverter())
                .WithTypeConverter(new NotateConfigExtensions.OctaveRangeConverter()).IgnoreUnmatchedProperties().Build();
            var notateConfig = deserializer.Deserialize<NotateConfig>(string.Join(Environment.NewLine, fileContents));
            if (notateConfig == null) throw new BmpSchemaInvalidException();
            if (notateConfig.schemaVersion > Constants.SchemaVersion || notateConfig.schemaVersion < 3)
                throw new BmpSchemaVersionException(notateConfig.schemaVersion);
            notateConfig.lastFilePath = Path.GetDirectoryName(Path.GetFullPath(filePath));
            return notateConfig;
        }

        /// <summary>
        /// Generates a NotateConfig from the track names in a midi file
        /// </summary>
        /// <param name="filePath">The full path to the midi file</param>
        /// <returns>a NotateConfig</returns>
        public static NotateConfig GenerateConfigFromMidiFile(string filePath) => GenerateConfigFromMidiFile(File.OpenRead(filePath), Path.GetFileNameWithoutExtension(filePath), Path.GetFileName(filePath), "" + Path.GetFileName(filePath), Path.GetDirectoryName(Path.GetFullPath(filePath)));

        /// <summary>
        /// Generates a NotateConfig from the track names in a midi file
        /// </summary>
        /// <param name="stream">A stream containing the midi file</param>
        /// <param name="midiTitle"></param>
        /// <param name="midiFileName"></param>
        /// <param name="midiDescription"></param>
        /// <param name="lastFilePath"></param>
        /// <returns>a NotateConfig</returns>
        public static NotateConfig GenerateConfigFromMidiFile(Stream stream, string midiTitle = "Imported Midi File", string midiFileName = "", string midiDescription = "", string lastFilePath = "")
        {
            var notateConfig = new NotateConfig { title = midiTitle, midiFileName = midiFileName, description = midiDescription, lastFilePath = lastFilePath };
            
            var midiFile = stream.ReadAsMidiFile();

            var chunks = midiFile.GetTrackChunks().ToArray();
            var instrumentNameSplitRegex = new Regex(@"^([A-Za-z]+)([-+]\d)?");

            for (var chunkCounter = 0; chunkCounter < chunks.Length; chunkCounter++)
            {
                var trackName = chunks[chunkCounter].Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text;

                if (string.IsNullOrWhiteSpace(trackName))
                {
                    notateConfig.bardGroups.Add(new BardGroup(new[] {chunkCounter}, "Track " + chunkCounter));
                    continue;
                }

                if (trackName.Trim().ToLower().StartsWith("singer"))
                {
                    var options = trackName.Trim().Split(';');

                    var singerGroup = new SingerGroup()
                    {
                        description = "Singer Track " + chunkCounter,
                        tracks = new Dictionary<int, Track> {{chunkCounter, new Track()}}
                    };

                    foreach (var option in options)
                        if (option.Trim().ToLower().StartsWith("description="))
                            singerGroup.description = option.Trim().Remove(0, 12);

                    // TODO: parse lyric lines

                    notateConfig.singerGroups.Add(singerGroup);
                    continue;
                }


                var groups = trackName.Trim().Split('|');

                foreach (var group in groups)
                {
                    var options = group.Trim().Split(';');

                    var bardGroup = new BardGroup
                    {
                        description = "Track " + chunkCounter,
                        tracks = new Dictionary<int, Track> {{chunkCounter, new Track()}}
                    };

                    var instrumentsParsed = false;

                    foreach (var option in options)
                    {
                        if (option.Trim().ToLower().StartsWith("include="))
                        {
                            var tracksToMerge = option.Trim().Remove(0, 8).Split(',');
                            foreach (var trackToMerge in tracksToMerge)
                            {
                                if (int.TryParse(trackToMerge.Trim(), out var value) && value < chunks.Length)
                                    bardGroup.tracks[value] = new Track();
                            }
                        }
                        else if (option.Trim().ToLower().StartsWith("bards=") &&
                                 int.TryParse(option.Trim().Remove(0, 6), out var value))
                            bardGroup.distributionBardCount = value;
                        else if (option.Trim().ToLower().StartsWith("description="))
                            bardGroup.description = option.Trim().Remove(0, 12);
                        else
                        {
                            if (instrumentsParsed) continue;
                            var instrumentList = option.Trim().ToLower().Split(',');
                            for (var instrumentListCounter = OctaveRange.Tone.UpperNote;
                                instrumentListCounter >= OctaveRange.Tone.LowerNote;
                                instrumentListCounter--)
                            {
                                if (!Enum.IsDefined(typeof(VST), instrumentListCounter)) continue;
                                var vst = (VST) instrumentListCounter;
                                if (instrumentList.Length < vst.Index()) continue;
                                var instrumentListItem = instrumentList[vst.Index()].Trim().Replace("_", "").Replace(" ","");

                                switch (instrumentListItem)
                                {
                                    case "gmbassdrum":
                                        bardGroup.instruments[vst] = new VSTProperties {instrument = Instrument.BassDrum, octaveRange = OctaveRange.Mapper}.AddGmBassDrumToNoteMapper();
                                        continue;
                                    case "gmsnaredrum":
                                        bardGroup.instruments[vst] = new VSTProperties {instrument = Instrument.SnareDrum, octaveRange = OctaveRange.Mapper}.AddGmSnareDrumNoteMapper();
                                        continue;
                                    case "gmcymbal":
                                        bardGroup.instruments[vst] = new VSTProperties {instrument = Instrument.Cymbal, octaveRange = OctaveRange.Mapper}.AddGmCymbalNoteMapper();
                                        continue;
                                    case "gmbongo":
                                        bardGroup.instruments[vst] = new VSTProperties {instrument = Instrument.Bongo, octaveRange = OctaveRange.Mapper}.AddGmBongoNoteMapper();
                                        continue;
                                }

                                if (instrumentNameSplitRegex.Match(instrumentListItem) is not { } match) continue;
                                
                                var instrument = Instrument.Parse(match.Groups[1].Value);
                                var octaveRange = OctaveRange.Parse(match.Groups[2].Value);
                                bardGroup.instruments[vst] = new VSTProperties
                                    {instrument = instrument, octaveRange = octaveRange};
                            }
                            instrumentsParsed = true;
                        }
                    }
                    notateConfig.bardGroups.Add(bardGroup);
                }
            }
            return notateConfig;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class NotateConfigExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="notateConfig"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string Save(this NotateConfig notateConfig, string fileName = "")
        {

            if (fileName.IsNullOrEmpty())
            {
                if (notateConfig.midiFileName.IsNullOrEmpty() || notateConfig.lastFilePath.IsNullOrEmpty())
                    throw new BmpFileException("No fileName was specified and no folder path is known to save in.");
                fileName = notateConfig.lastFilePath + "\\" + Path.GetFileNameWithoutExtension(notateConfig.midiFileName) + ".yaml";
            }
            if (!fileName.ToLower().EndsWith(".yaml")) throw new BmpFileException("fileName does not end in .yaml");

            using var streamWriter = new StreamWriter(fileName);
            var serializer = new SerializerBuilder()
                    .WithIndentedSequences()
                    .WithTypeConverter(new InstrumentConverter())
                    .WithTypeConverter(new OctaveRangeConverter())
                    .DisableAliases()
                    .WithEmissionPhaseObjectGraphVisitor(args =>
                        new YamlIEnumerableSkipEmptyObjectGraphVisitor(args.InnerVisitor))
                    .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
                    .WithTypeInspector(next => new SortedTypeInspector(next))
                    .Build();

            serializer.Serialize(streamWriter, notateConfig);
            streamWriter.Flush();
            streamWriter.Close();
            return fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notateConfig"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<MMSong> TransmogrifyAsync(this NotateConfig notateConfig, string fileName = "")
        {
            if (fileName.IsNullOrEmpty())
            {
                if (notateConfig.midiFileName.IsNullOrEmpty() || notateConfig.lastFilePath.IsNullOrEmpty())
                    throw new BmpFileException("No fileName was specified and no folder path is known to load from.");
                fileName = notateConfig.lastFilePath + "\\" + Path.GetFileName(notateConfig.midiFileName);
            }
            if (Directory.Exists(fileName)) fileName = fileName + "\\" + notateConfig.midiFileName;
            if (!File.Exists(fileName)) throw new BmpFileException("Cannot find midi file: " + fileName);
            if (!fileName.ToLower().EndsWith(".mid") && !fileName.ToLower().EndsWith(".midi")) throw new BmpFileException("fileName does not end in .mid or .midi");
            notateConfig.midiFileName = Path.GetFileName(fileName);
            return await TransmogrifyAsync(notateConfig, File.OpenRead(fileName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notateConfig"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<MMSong> TransmogrifyAsync(this NotateConfig notateConfig, Stream stream) => Notate.Instance.Transmogrify(notateConfig, stream);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notateConfig"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static MMSong Transmogrify(this NotateConfig notateConfig, string fileName = "")
        {
            if (fileName.IsNullOrEmpty())
            {
                if (notateConfig.midiFileName.IsNullOrEmpty() || notateConfig.lastFilePath.IsNullOrEmpty())
                    throw new BmpFileException("No fileName was specified and no folder path is known to load from.");
                fileName = notateConfig.lastFilePath + "\\" + Path.GetFileName(notateConfig.midiFileName);
            }
            if (Directory.Exists(fileName)) fileName = fileName + "\\" + notateConfig.midiFileName;
            if (!File.Exists(fileName)) throw new BmpFileException("Cannot find midi file: " + fileName);
            if (!fileName.ToLower().EndsWith(".mid") && !fileName.ToLower().EndsWith(".midi")) throw new BmpFileException("fileName does not end in .mid or .midi");
            notateConfig.midiFileName = Path.GetFileName(fileName);
            return Transmogrify(notateConfig, File.OpenRead(fileName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notateConfig"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static MMSong Transmogrify(this NotateConfig notateConfig, Stream stream) => Notate.Instance.Transmogrify(notateConfig, stream);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vstProperties"></param>
        /// <returns></returns>
        public static VSTProperties AddGmBassDrumToNoteMapper(this VSTProperties vstProperties)
        {
            vstProperties.noteMapper[35] = 8;
            vstProperties.noteMapper[36] = 10;
            vstProperties.noteMapper[41] = 16;
            vstProperties.noteMapper[43] = 19;
            vstProperties.noteMapper[45] = 23;
            vstProperties.noteMapper[47] = 26;
            vstProperties.noteMapper[48] = 30;
            vstProperties.noteMapper[50] = 33;
            return vstProperties;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vstProperties"></param>
        /// <returns></returns>
        public static VSTProperties  AddGmSnareDrumNoteMapper(this VSTProperties vstProperties)
        {
            vstProperties.noteMapper[38] = 20;
            vstProperties.noteMapper[40] = 22;
            return vstProperties;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vstProperties"></param>
        /// <returns></returns>
        public static VSTProperties  AddGmCymbalNoteMapper(this VSTProperties vstProperties)
        {
            vstProperties.noteMapper[49] = 24;
            vstProperties.noteMapper[52] = 22;
            vstProperties.noteMapper[55] = 30;
            vstProperties.noteMapper[57] = 24;
            return vstProperties;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vstProperties"></param>
        /// <returns></returns>
        public static VSTProperties  AddGmBongoNoteMapper(this VSTProperties vstProperties)
        {
            vstProperties.noteMapper[60] = 23;
            vstProperties.noteMapper[61] = 20;
            return vstProperties;
        }

        internal sealed class YamlIEnumerableSkipEmptyObjectGraphVisitor : ChainedObjectGraphVisitor
        {
            public YamlIEnumerableSkipEmptyObjectGraphVisitor (IObjectGraphVisitor<IEmitter> nextVisitor) : base(nextVisitor) {}

            public override bool Enter(IObjectDescriptor value, IEmitter context)
            {
                if (value.Value is IEnumerable enumerableObject) return enumerableObject.GetEnumerator().MoveNext() && base.Enter(value, context);
                return base.Enter(value, context);
            }

            public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context) => value.Value switch
                {
                    null => false,
                    IEnumerable enumerableObject => enumerableObject.GetEnumerator().MoveNext() &&
                                                    base.EnterMapping(key, value, context),
                    _ => base.EnterMapping(key, value, context)
                };
        }
        internal sealed class SortedTypeInspector : TypeInspectorSkeleton
        {
            private readonly ITypeInspector _innerTypeInspector;
            public SortedTypeInspector(ITypeInspector innerTypeInspector) =>_innerTypeInspector = innerTypeInspector;
            public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container) => type.Name switch
                {
                    "BardGroup" => _innerTypeInspector.GetProperties(type, container)
                        .OrderByDescending(next => next.Name),
                    "SingerGroup" => _innerTypeInspector.GetProperties(type, container)
                        .OrderByDescending(next => next.Name),
                    _ => _innerTypeInspector.GetProperties(type, container)
                };
        }

        internal static int Index(this VST vst) => vst switch
        {
            VST.VST0 => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(vst), vst, null),
        };
        public class InstrumentConverter : IYamlTypeConverter
        {
            #region YamlDotNet

            // Have YamlDotNet treat this as a string during serialization.
            public bool Accepts(Type type) => type == typeof(Instrument);
            public object ReadYaml(IParser parser, Type type) => Instrument.Parse(parser.Consume<Scalar>().Value);

            public void WriteYaml(IEmitter emitter, object value, Type type) =>
                emitter.Emit(new Scalar(((Instrument) value).Name));

            #endregion
        }

        public class OctaveRangeConverter : IYamlTypeConverter
        {
            #region YamlDotNet

            // Have YamlDotNet treat this as a string during serialization.
            public bool Accepts(Type type) => type == typeof(OctaveRange);
            public object ReadYaml(IParser parser, Type type) => OctaveRange.Parse(parser.Consume<Scalar>().Value);

            public void WriteYaml(IEmitter emitter, object value, Type type) =>
                emitter.Emit(new Scalar(((OctaveRange) value).Name));

            #endregion
        }
    }
}