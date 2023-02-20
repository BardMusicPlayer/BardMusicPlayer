﻿using BardMusicPlayer.DryWetMidi.Common;
using BardMusicPlayer.DryWetMidi.Core;
using BardMusicPlayer.DryWetMidi.Core.Chunks;
using BardMusicPlayer.DryWetMidi.Core.Utilities;
using BardMusicPlayer.DryWetMidi.Interaction.GetObjects;
using BardMusicPlayer.DryWetMidi.Interaction.LengthedObject;
using BardMusicPlayer.DryWetMidi.Interaction.TempoMap;
using BardMusicPlayer.DryWetMidi.Interaction.TimedObject;
using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan;
using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan.Converters;

namespace BardMusicPlayer.DryWetMidi.Tools.Splitter
{
    public static partial class Splitter
    {
        #region Methods

        /// <summary>
        /// Splits objects within a <see cref="TrackChunk"/> by the specified step so every object will be split
        /// at points equally distanced from each other starting from the object's start time. More info in the
        /// <see href="xref:a_obj_splitting#splitobjectsbystep">Objects splitting: SplitObjectsByStep</see> article.
        /// </summary>
        /// <remarks>
        /// Nulls, objects with zero length and objects with length smaller than <paramref name="step"/>
        /// will not be split and will be returned as clones of the input objects.
        /// </remarks>
        /// <param name="trackChunk"><see cref="TrackChunk"/> to split objects within.</param>
        /// <param name="objectType">The type of objects to split.</param>
        /// <param name="step">Step to split objects by.</param>
        /// <param name="tempoMap">Tempo map used to calculate times to split by.</param>
        /// <param name="objectDetectionSettings">Settings according to which objects should be
        /// detected and built.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>One of the following errors occurred:</para>
        /// <list type="bullet">
        /// <item>
        /// <description><paramref name="trackChunk"/> is <c>null</c>.</description>
        /// </item>
        /// <item>
        /// <description><paramref name="step"/> is <c>null</c>.</description>
        /// </item>
        /// <item>
        /// <description><paramref name="tempoMap"/> is <c>null</c>.</description>
        /// </item>
        /// </list>
        /// </exception>
        public static void SplitObjectsByStep(
            this TrackChunk trackChunk,
            ObjectType objectType,
            ITimeSpan step,
            TempoMap tempoMap,
            ObjectDetectionSettings objectDetectionSettings = null)
        {
            ThrowIfArgument.IsNull(nameof(trackChunk), trackChunk);
            ThrowIfArgument.IsNull(nameof(step), step);
            ThrowIfArgument.IsNull(nameof(tempoMap), tempoMap);

            SplitTrackChunkObjects(
                trackChunk,
                objectType,
                objectDetectionSettings,
                objects => SplitObjectsByStep(objects, step, tempoMap));
        }

        /// <summary>
        /// Splits objects within a collection of <see cref="TrackChunk"/> by the specified step so every object
        /// will be split at points equally distanced from each other starting from the object's start time. More info
        /// in the <see href="xref:a_obj_splitting#splitobjectsbystep">Objects splitting: SplitObjectsByStep</see> article.
        /// </summary>
        /// <remarks>
        /// Nulls, objects with zero length and objects with length smaller than <paramref name="step"/>
        /// will not be split and will be returned as clones of the input objects.
        /// </remarks>
        /// <param name="trackChunks">A collection of <see cref="TrackChunk"/> to split objects within.</param>
        /// <param name="objectType">The type of objects to split.</param>
        /// <param name="step">Step to split objects by.</param>
        /// <param name="tempoMap">Tempo map used to calculate times to split by.</param>
        /// <param name="objectDetectionSettings">Settings according to which objects should be
        /// detected and built.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>One of the following errors occurred:</para>
        /// <list type="bullet">
        /// <item>
        /// <description><paramref name="trackChunks"/> is <c>null</c>.</description>
        /// </item>
        /// <item>
        /// <description><paramref name="step"/> is <c>null</c>.</description>
        /// </item>
        /// <item>
        /// <description><paramref name="tempoMap"/> is <c>null</c>.</description>
        /// </item>
        /// </list>
        /// </exception>
        public static void SplitObjectsByStep(
            this IEnumerable<TrackChunk> trackChunks,
            ObjectType objectType,
            ITimeSpan step,
            TempoMap tempoMap,
            ObjectDetectionSettings objectDetectionSettings = null)
        {
            ThrowIfArgument.IsNull(nameof(trackChunks), trackChunks);
            ThrowIfArgument.IsNull(nameof(step), step);
            ThrowIfArgument.IsNull(nameof(tempoMap), tempoMap);

            foreach (var trackChunk in trackChunks)
            {
                trackChunk.SplitObjectsByStep(objectType, step, tempoMap, objectDetectionSettings);
            }
        }

        /// <summary>
        /// Splits objects within a <see cref="MidiFile"/> by the specified step so every object will be split
        /// at points equally distanced from each other starting from the object's start time. More info in the
        /// <see href="xref:a_obj_splitting#splitobjectsbystep">Objects splitting: SplitObjectsByStep</see> article.
        /// </summary>
        /// <remarks>
        /// Nulls, objects with zero length and objects with length smaller than <paramref name="step"/>
        /// will not be split and will be returned as clones of the input objects.
        /// </remarks>
        /// <param name="midiFile"><see cref="MidiFile"/> to split objects within.</param>
        /// <param name="objectType">The type of objects to split.</param>
        /// <param name="step">Step to split objects by.</param>
        /// <param name="objectDetectionSettings">Settings according to which objects should be
        /// detected and built.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>One of the following errors occurred:</para>
        /// <list type="bullet">
        /// <item>
        /// <description><paramref name="midiFile"/> is <c>null</c>.</description>
        /// </item>
        /// <item>
        /// <description><paramref name="step"/> is <c>null</c>.</description>
        /// </item>
        /// </list>
        /// </exception>
        public static void SplitObjectsByStep(
            this MidiFile midiFile,
            ObjectType objectType,
            ITimeSpan step,
            ObjectDetectionSettings objectDetectionSettings = null)
        {
            ThrowIfArgument.IsNull(nameof(midiFile), midiFile);
            ThrowIfArgument.IsNull(nameof(step), step);

            var tempoMap = midiFile.GetTempoMap();
            midiFile
                .GetTrackChunks()
                .SplitObjectsByStep(objectType, step, tempoMap, objectDetectionSettings);
        }

        /// <summary>
        /// Splits objects by the specified step so every object will be split at points
        /// equally distanced from each other starting from the object's start time. More info in the
        /// <see href="xref:a_obj_splitting#splitobjectsbystep">Objects splitting: SplitObjectsByStep</see> article.
        /// </summary>
        /// <remarks>
        /// Nulls, objects with zero length and objects with length smaller than <paramref name="step"/>
        /// will not be split and will be returned as clones of the input objects.
        /// </remarks>
        /// <param name="objects">Objects to split.</param>
        /// <param name="step">Step to split objects by.</param>
        /// <param name="tempoMap">Tempo map used to calculate times to split by.</param>
        /// <returns>Objects that are result of splitting <paramref name="objects"/> going in the same
        /// order as elements of <paramref name="objects"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>One of the following errors occurred:</para>
        /// <list type="bullet">
        /// <item>
        /// <description><paramref name="objects"/> is <c>null</c>.</description>
        /// </item>
        /// <item>
        /// <description><paramref name="step"/> is <c>null</c>.</description>
        /// </item>
        /// <item>
        /// <description><paramref name="tempoMap"/> is <c>null</c>.</description>
        /// </item>
        /// </list>
        /// </exception>
        public static IEnumerable<ITimedObject> SplitObjectsByStep(this IEnumerable<ITimedObject> objects, ITimeSpan step, TempoMap tempoMap)
        {
            ThrowIfArgument.IsNull(nameof(objects), objects);
            ThrowIfArgument.IsNull(nameof(step), step);
            ThrowIfArgument.IsNull(nameof(tempoMap), tempoMap);

            foreach (var obj in objects)
            {
                if (obj == null)
                {
                    yield return null;
                    continue;
                }

                var lengthedObject = obj as ILengthedObject;
                if (lengthedObject == null)
                {
                    yield return obj;
                    continue;
                }

                if (lengthedObject.Length == 0)
                {
                    yield return (ILengthedObject)obj.Clone();
                    continue;
                }

                var startTime = obj.Time;
                var endTime = startTime + lengthedObject.Length;

                var time = startTime;
                var tail = (ILengthedObject)obj.Clone();

                while (time < endTime && tail != null)
                {
                    var convertedStep = LengthConverter.ConvertFrom(step, time, tempoMap);
                    if (convertedStep == 0)
                        throw new InvalidOperationException("Step is too small.");

                    time += convertedStep;

                    var parts = tail.Split(time);
                    yield return parts.LeftPart;

                    tail = parts.RightPart;
                }
            }
        }

        #endregion
    }
}
