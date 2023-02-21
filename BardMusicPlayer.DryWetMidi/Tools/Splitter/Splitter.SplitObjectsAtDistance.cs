﻿using System.ComponentModel;
using BardMusicPlayer.DryWetMidi.Common;
using BardMusicPlayer.DryWetMidi.Core;
using BardMusicPlayer.DryWetMidi.Core.Chunks;
using BardMusicPlayer.DryWetMidi.Core.Utilities;
using BardMusicPlayer.DryWetMidi.Interaction.GetObjects;
using BardMusicPlayer.DryWetMidi.Interaction.LengthedObject;
using BardMusicPlayer.DryWetMidi.Interaction.TempoMap;
using BardMusicPlayer.DryWetMidi.Interaction.TimedObject;
using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan;
using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan.Converters;
using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan.Representations;
using BardMusicPlayer.DryWetMidi.Tools.Common;

namespace BardMusicPlayer.DryWetMidi.Tools.Splitter;

public static partial class Splitter
{
    #region Constants

    private const double ZeroRatio = 0.0;
    private const double FullLengthRatio = 1.0;

    #endregion

    #region Methods

    /// <summary>
    /// Splits objects within a <see cref="TrackChunk"/> at the specified distance from an object's start or end.
    /// More info in the <see href="xref:a_obj_splitting#splitobjectsatdistance">Objects splitting: SplitObjectsAtDistance</see>
    /// article.
    /// </summary>
    /// <param name="trackChunk"><see cref="TrackChunk"/> to split objects within.</param>
    /// <param name="objectType">The type of objects to split.</param>
    /// <param name="distance">Distance to split objects at.</param>
    /// <param name="from">Point of an object <paramref name="distance"/> should be measured from.</param>
    /// <param name="tempoMap">Tempo map used for distances calculations.</param>
    /// <param name="objectDetectionSettings">Settings according to which objects should be
    /// detected and built.</param>
    /// <exception cref="ArgumentNullException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="trackChunk"/> is <c>null</c>.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="distance"/> is <c>null</c>.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="tempoMap"/> is <c>null</c>.</description>
    /// </item>
    /// </list>
    /// </exception>
    /// <exception cref="InvalidEnumArgumentException"><paramref name="from"/> specified an invalid value.</exception>
    public static void SplitObjectsAtDistance(
        this TrackChunk trackChunk,
        ObjectType objectType,
        ITimeSpan distance,
        LengthedObjectTarget from,
        TempoMap tempoMap,
        ObjectDetectionSettings objectDetectionSettings = null)
    {
        ThrowIfArgument.IsNull(nameof(trackChunk), trackChunk);
        ThrowIfArgument.IsNull(nameof(distance), distance);
        ThrowIfArgument.IsInvalidEnumValue(nameof(from), from);
        ThrowIfArgument.IsNull(nameof(tempoMap), tempoMap);

        SplitTrackChunkObjects(
            trackChunk,
            objectType,
            objectDetectionSettings,
            objects => SplitObjectsAtDistance(objects, distance, from, tempoMap));
    }

    /// <summary>
    /// Splits objects within a collection of <see cref="TrackChunk"/> at the specified distance from an
    /// object's start or end. More info in the
    /// <see href="xref:a_obj_splitting#splitobjectsatdistance">Objects splitting: SplitObjectsAtDistance</see>
    /// article.
    /// </summary>
    /// <param name="trackChunks">A collection of <see cref="TrackChunk"/> to split objects within.</param>
    /// <param name="objectType">The type of objects to split.</param>
    /// <param name="distance">Distance to split objects at.</param>
    /// <param name="from">Point of an object <paramref name="distance"/> should be measured from.</param>
    /// <param name="tempoMap">Tempo map used for distances calculations.</param>
    /// <param name="objectDetectionSettings">Settings according to which objects should be
    /// detected and built.</param>
    /// <exception cref="ArgumentNullException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="trackChunks"/> is <c>null</c>.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="distance"/> is <c>null</c>.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="tempoMap"/> is <c>null</c>.</description>
    /// </item>
    /// </list>
    /// </exception>
    /// <exception cref="InvalidEnumArgumentException"><paramref name="from"/> specified an invalid value.</exception>
    public static void SplitObjectsAtDistance(
        this IEnumerable<TrackChunk> trackChunks,
        ObjectType objectType,
        ITimeSpan distance,
        LengthedObjectTarget from,
        TempoMap tempoMap,
        ObjectDetectionSettings objectDetectionSettings = null)
    {
        ThrowIfArgument.IsNull(nameof(trackChunks), trackChunks);
        ThrowIfArgument.IsNull(nameof(distance), distance);
        ThrowIfArgument.IsInvalidEnumValue(nameof(from), from);
        ThrowIfArgument.IsNull(nameof(tempoMap), tempoMap);

        foreach (var trackChunk in trackChunks)
        {
            trackChunk.SplitObjectsAtDistance(objectType, distance, from, tempoMap, objectDetectionSettings);
        }
    }

    /// <summary>
    /// Splits objects within a <see cref="MidiFile"/> at the specified distance from an object's start or end.
    /// More info in the <see href="xref:a_obj_splitting#splitobjectsatdistance">Objects splitting: SplitObjectsAtDistance</see>
    /// article.
    /// </summary>
    /// <param name="midiFile"><see cref="MidiFile"/> to split objects within.</param>
    /// <param name="objectType">The type of objects to split.</param>
    /// <param name="distance">Distance to split objects at.</param>
    /// <param name="from">Point of an object <paramref name="distance"/> should be measured from.</param>
    /// <param name="objectDetectionSettings">Settings according to which objects should be
    /// detected and built.</param>
    /// <exception cref="ArgumentNullException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="midiFile"/> is <c>null</c>.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="distance"/> is <c>null</c>.</description>
    /// </item>
    /// </list>
    /// </exception>
    /// <exception cref="InvalidEnumArgumentException"><paramref name="from"/> specified an invalid value.</exception>
    public static void SplitObjectsAtDistance(
        this MidiFile midiFile,
        ObjectType objectType,
        ITimeSpan distance,
        LengthedObjectTarget from,
        ObjectDetectionSettings objectDetectionSettings = null)
    {
        ThrowIfArgument.IsNull(nameof(midiFile), midiFile);
        ThrowIfArgument.IsNull(nameof(distance), distance);
        ThrowIfArgument.IsInvalidEnumValue(nameof(from), from);

        var tempoMap = midiFile.GetTempoMap();

        midiFile.GetTrackChunks().SplitObjectsAtDistance(objectType, distance, from, tempoMap, objectDetectionSettings);
    }

    /// <summary>
    /// Splits objects within a <see cref="TrackChunk"/> by the specified ratio of an object's length measuring
    /// it from the object's start or end. For example, 0.5 means splitting at the center of an object. More info
    /// in the <see href="xref:a_obj_splitting#splitobjectsatdistance">Objects splitting: SplitObjectsAtDistance</see>
    /// article.
    /// </summary>
    /// <param name="trackChunk"><see cref="TrackChunk"/> to split objects within.</param>
    /// <param name="objectType">The type of objects to split.</param>
    /// <param name="ratio">Ratio of an object's length to split by. Valid values are from 0 to 1.</param>
    /// <param name="lengthType">The type an object's length should be processed according to.</param>
    /// <param name="from">Point of an object distance should be measured from.</param>
    /// <param name="tempoMap">Tempo map used for distances calculations.</param>
    /// <param name="objectDetectionSettings">Settings according to which objects should be
    /// detected and built.</param>
    /// <exception cref="ArgumentNullException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="trackChunk"/> is <c>null</c>.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="tempoMap"/> is <c>null</c>.</description>
    /// </item>
    /// </list>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="ratio"/> is out of valid range.</exception>
    /// <exception cref="InvalidEnumArgumentException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="lengthType"/> specified an invalid value.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="from"/> specified an invalid value.</description>
    /// </item>
    /// </list>
    /// </exception>
    public static void SplitObjectsAtDistance(
        this TrackChunk trackChunk,
        ObjectType objectType,
        double ratio,
        TimeSpanType lengthType,
        LengthedObjectTarget from,
        TempoMap tempoMap,
        ObjectDetectionSettings objectDetectionSettings = null)
    {
        ThrowIfArgument.IsNull(nameof(trackChunk), trackChunk);
        ThrowIfArgument.IsOutOfRange(nameof(ratio),
            ratio,
            ZeroRatio,
            FullLengthRatio,
            $"Ratio is out of [{ZeroRatio}; {FullLengthRatio}] range.");
        ThrowIfArgument.IsInvalidEnumValue(nameof(lengthType), lengthType);
        ThrowIfArgument.IsInvalidEnumValue(nameof(from), from);
        ThrowIfArgument.IsNull(nameof(tempoMap), tempoMap);

        SplitTrackChunkObjects(
            trackChunk,
            objectType,
            objectDetectionSettings,
            objects => SplitObjectsAtDistance(objects, ratio, lengthType, from, tempoMap));
    }

    /// <summary>
    /// Splits objects within a collection of <see cref="TrackChunk"/> by the specified ratio of an object's
    /// length measuring it from the object's start or end. For example, 0.5 means splitting at the center of an object.
    /// More info in the <see href="xref:a_obj_splitting#splitobjectsatdistance">Objects splitting: SplitObjectsAtDistance</see>
    /// article.
    /// </summary>
    /// <param name="trackChunks">A collection of <see cref="TrackChunk"/> to split objects within.</param>
    /// <param name="objectType">The type of objects to split.</param>
    /// <param name="ratio">Ratio of an object's length to split by. Valid values are from 0 to 1.</param>
    /// <param name="lengthType">The type an object's length should be processed according to.</param>
    /// <param name="from">Point of an object distance should be measured from.</param>
    /// <param name="tempoMap">Tempo map used for distances calculations.</param>
    /// <param name="objectDetectionSettings">Settings according to which objects should be
    /// detected and built.</param>
    /// <exception cref="ArgumentNullException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="trackChunks"/> is <c>null</c>.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="tempoMap"/> is <c>null</c>.</description>
    /// </item>
    /// </list>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="ratio"/> is out of valid range.</exception>
    /// <exception cref="InvalidEnumArgumentException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="lengthType"/> specified an invalid value.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="from"/> specified an invalid value.</description>
    /// </item>
    /// </list>
    /// </exception>
    public static void SplitObjectsAtDistance(
        this IEnumerable<TrackChunk> trackChunks,
        ObjectType objectType,
        double ratio,
        TimeSpanType lengthType,
        LengthedObjectTarget from,
        TempoMap tempoMap,
        ObjectDetectionSettings objectDetectionSettings = null)
    {
        ThrowIfArgument.IsNull(nameof(trackChunks), trackChunks);
        ThrowIfArgument.IsOutOfRange(nameof(ratio),
            ratio,
            ZeroRatio,
            FullLengthRatio,
            $"Ratio is out of [{ZeroRatio}; {FullLengthRatio}] range.");
        ThrowIfArgument.IsInvalidEnumValue(nameof(lengthType), lengthType);
        ThrowIfArgument.IsInvalidEnumValue(nameof(from), from);
        ThrowIfArgument.IsNull(nameof(tempoMap), tempoMap);

        foreach (var trackChunk in trackChunks)
        {
            trackChunk.SplitObjectsAtDistance(objectType, ratio, lengthType, from, tempoMap, objectDetectionSettings);
        }
    }

    /// <summary>
    /// Splits objects within a <see cref="MidiFile"/> by the specified ratio of an object's length measuring
    /// it from the object's start or end. For example, 0.5 means splitting at the center of an object. More info
    /// in the <see href="xref:a_obj_splitting#splitobjectsatdistance">Objects splitting: SplitObjectsAtDistance</see>
    /// article.
    /// </summary>
    /// <param name="midiFile"><see cref="MidiFile"/> to split objects within.</param>
    /// <param name="objectType">The type of objects to split.</param>
    /// <param name="ratio">Ratio of an object's length to split by. Valid values are from 0 to 1.</param>
    /// <param name="lengthType">The type an object's length should be processed according to.</param>
    /// <param name="from">Point of an object distance should be measured from.</param>
    /// <param name="objectDetectionSettings">Settings according to which objects should be
    /// detected and built.</param>
    /// <exception cref="ArgumentNullException"><paramref name="midiFile"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="ratio"/> is out of valid range.</exception>
    /// <exception cref="InvalidEnumArgumentException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="lengthType"/> specified an invalid value.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="from"/> specified an invalid value.</description>
    /// </item>
    /// </list>
    /// </exception>
    public static void SplitObjectsAtDistance(
        this MidiFile midiFile,
        ObjectType objectType,
        double ratio,
        TimeSpanType lengthType,
        LengthedObjectTarget from,
        ObjectDetectionSettings objectDetectionSettings = null)
    {
        ThrowIfArgument.IsNull(nameof(midiFile), midiFile);
        ThrowIfArgument.IsOutOfRange(nameof(ratio),
            ratio,
            ZeroRatio,
            FullLengthRatio,
            $"Ratio is out of [{ZeroRatio}; {FullLengthRatio}] range.");
        ThrowIfArgument.IsInvalidEnumValue(nameof(lengthType), lengthType);
        ThrowIfArgument.IsInvalidEnumValue(nameof(from), from);

        var tempoMap = midiFile.GetTempoMap();

        midiFile.GetTrackChunks().SplitObjectsAtDistance(objectType, ratio, lengthType, from, tempoMap, objectDetectionSettings);
    }

    /// <summary>
    /// Splits objects at the specified distance from an object's start or end. More info in the
    /// <see href="xref:a_obj_splitting#splitobjectsatdistance">Objects splitting: SplitObjectsAtDistance</see>
    /// article.
    /// </summary>
    /// <param name="objects">Objects to split.</param>
    /// <param name="distance">Distance to split objects at.</param>
    /// <param name="from">Point of an object <paramref name="distance"/> should be measured from.</param>
    /// <param name="tempoMap">Tempo map used for distances calculations.</param>
    /// <returns>Objects that are result of splitting <paramref name="objects"/> going in the same
    /// order as elements of <paramref name="objects"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="objects"/> is <c>null</c>.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="distance"/> is <c>null</c>.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="tempoMap"/> is <c>null</c>.</description>
    /// </item>
    /// </list>
    /// </exception>
    /// <exception cref="InvalidEnumArgumentException"><paramref name="from"/> specified an invalid value.</exception>
    public static IEnumerable<ITimedObject> SplitObjectsAtDistance(this IEnumerable<ITimedObject> objects, ITimeSpan distance, LengthedObjectTarget from, TempoMap tempoMap)
    {
        ThrowIfArgument.IsNull(nameof(objects), objects);
        ThrowIfArgument.IsNull(nameof(distance), distance);
        ThrowIfArgument.IsInvalidEnumValue(nameof(from), from);
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

            var parts = SplitObjectAtDistance(lengthedObject, distance, from, tempoMap);

            if (parts.LeftPart != null)
                yield return parts.LeftPart;

            if (parts.RightPart != null)
                yield return parts.RightPart;
        }
    }

    /// <summary>
    /// Splits objects by the specified ratio of an object's length measuring it from
    /// the object's start or end. For example, 0.5 means splitting at the center of an object. More info in the
    /// <see href="xref:a_obj_splitting#splitobjectsatdistance">Objects splitting: SplitObjectsAtDistance</see>
    /// article.
    /// </summary>
    /// <param name="objects">Objects to split.</param>
    /// <param name="ratio">Ratio of an object's length to split by. Valid values are from 0 to 1.</param>
    /// <param name="lengthType">The type an object's length should be processed according to.</param>
    /// <param name="from">Point of an object distance should be measured from.</param>
    /// <param name="tempoMap">Tempo map used for distances calculations.</param>
    /// <returns>Objects that are result of splitting <paramref name="objects"/> going in the same
    /// order as elements of <paramref name="objects"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="objects"/> is <c>null</c>.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="tempoMap"/> is <c>null</c>.</description>
    /// </item>
    /// </list>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="ratio"/> is out of valid range.</exception>
    /// <exception cref="InvalidEnumArgumentException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="lengthType"/> specified an invalid value.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="from"/> specified an invalid value.</description>
    /// </item>
    /// </list>
    /// </exception>
    public static IEnumerable<ITimedObject> SplitObjectsAtDistance(this IEnumerable<ITimedObject> objects, double ratio, TimeSpanType lengthType, LengthedObjectTarget from, TempoMap tempoMap)
    {
        ThrowIfArgument.IsNull(nameof(objects), objects);
        ThrowIfArgument.IsOutOfRange(nameof(ratio),
            ratio,
            ZeroRatio,
            FullLengthRatio,
            $"Ratio is out of [{ZeroRatio}; {FullLengthRatio}] range.");
        ThrowIfArgument.IsInvalidEnumValue(nameof(lengthType), lengthType);
        ThrowIfArgument.IsInvalidEnumValue(nameof(from), from);
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

            var distance = lengthedObject.LengthAs(lengthType, tempoMap).Multiply(ratio);
            var parts = SplitObjectAtDistance(lengthedObject, distance, from, tempoMap);

            if (parts.LeftPart != null)
                yield return parts.LeftPart;

            if (parts.RightPart != null)
                yield return parts.RightPart;
        }
    }

    private static SplitLengthedObject SplitObjectAtDistance(ILengthedObject obj, ITimeSpan distance, LengthedObjectTarget from, TempoMap tempoMap)
    {
        var time = from == LengthedObjectTarget.Start
            ? ((MidiTimeSpan)obj.Time).Add(distance, TimeSpanMode.TimeLength)
            : ((MidiTimeSpan)obj.EndTime).Subtract(distance, TimeSpanMode.TimeLength);

        return obj.Split(TimeConverter.ConvertFrom(time, tempoMap));
    }

    #endregion
}