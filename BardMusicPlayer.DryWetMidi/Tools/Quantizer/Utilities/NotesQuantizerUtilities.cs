﻿using BardMusicPlayer.DryWetMidi.Common;
using BardMusicPlayer.DryWetMidi.Core;
using BardMusicPlayer.DryWetMidi.Core.Chunks;
using BardMusicPlayer.DryWetMidi.Core.Utilities;
using BardMusicPlayer.DryWetMidi.Interaction.GetObjects;
using BardMusicPlayer.DryWetMidi.Interaction.Grid;
using BardMusicPlayer.DryWetMidi.Interaction.Notes;
using BardMusicPlayer.DryWetMidi.Interaction.TempoMap;
using BardMusicPlayer.DryWetMidi.Tools.Common;

namespace BardMusicPlayer.DryWetMidi.Tools.Quantizer.Utilities;

/// <summary>
/// Provides methods to quantize notes time.
/// </summary>
public static class NotesQuantizerUtilities
{
    #region Methods

    /// <summary>
    /// Quantizes notes contained in the specified <see cref="TrackChunk"/>.
    /// </summary>
    /// <param name="trackChunk"><see cref="TrackChunk"/> to quantize notes in.</param>
    /// <param name="grid">Grid to quantize objects by.</param>
    /// <param name="tempoMap">Tempo map used to calculate times to quantize by.</param>
    /// <param name="settings">Settings according to which notes should be quantized.</param>
    /// <exception cref="ArgumentNullException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="trackChunk"/> is <c>null</c>.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="grid"/> is <c>null</c>.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="tempoMap"/> is <c>null</c>.</description>
    /// </item>
    /// </list>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description>Note is going to be moved beyond zero.</description>
    /// </item>
    /// <item>
    /// <description>Note's end is going to be moved beyond the note's fixed end.</description>
    /// </item>
    /// </list>
    /// </exception>
    [Obsolete("OBS13")]
    public static void QuantizeNotes(this TrackChunk trackChunk, IGrid grid, TempoMap tempoMap, NotesQuantizingSettings settings = null)
    {
        ThrowIfArgument.IsNull(nameof(trackChunk), trackChunk);
        ThrowIfArgument.IsNull(nameof(grid), grid);
        ThrowIfArgument.IsNull(nameof(tempoMap), tempoMap);

        trackChunk.QuantizeObjects(
            ObjectType.Note,
            grid,
            tempoMap,
            GetSettings(settings),
            new ObjectDetectionSettings
            {
                NoteDetectionSettings = settings.NoteDetectionSettings
            });
    }

    /// <summary>
    /// Quantizes notes contained in the specified collection of <see cref="TrackChunk"/>.
    /// </summary>
    /// <param name="trackChunks">Collection of <see cref="TrackChunk"/> to quantize notes in.</param>
    /// <param name="grid">Grid to quantize objects by.</param>
    /// <param name="tempoMap">Tempo map used to calculate times to quantize by.</param>
    /// <param name="settings">Settings according to which notes should be quantized.</param>
    /// <exception cref="ArgumentNullException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="trackChunks"/> is <c>null</c>.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="grid"/> is <c>null</c>.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="tempoMap"/> is <c>null</c>.</description>
    /// </item>
    /// </list>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description>Note is going to be moved beyond zero.</description>
    /// </item>
    /// <item>
    /// <description>Note's end is going to be moved beyond the note's fixed end.</description>
    /// </item>
    /// </list>
    /// </exception>
    [Obsolete("OBS13")]
    public static void QuantizeNotes(this IEnumerable<TrackChunk> trackChunks, IGrid grid, TempoMap tempoMap, NotesQuantizingSettings settings = null)
    {
        ThrowIfArgument.IsNull(nameof(trackChunks), trackChunks);
        ThrowIfArgument.IsNull(nameof(grid), grid);
        ThrowIfArgument.IsNull(nameof(tempoMap), tempoMap);

        foreach (var trackChunk in trackChunks)
        {
            trackChunk.QuantizeNotes(grid, tempoMap, settings);
        }
    }

    /// <summary>
    /// Quantizes notes contained in the specified <see cref="MidiFile"/>.
    /// </summary>
    /// <param name="midiFile"><see cref="MidiFile"/> to quantize notes in.</param>
    /// <param name="grid">Grid to quantize objects by.</param>
    /// <param name="settings">Settings according to which notes should be quantized.</param>
    /// <exception cref="ArgumentNullException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="midiFile"/> is <c>null</c>.</description>
    /// </item>
    /// <item>
    /// <description><paramref name="grid"/> is <c>null</c>.</description>
    /// </item>
    /// </list>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// <para>One of the following errors occured:</para>
    /// <list type="bullet">
    /// <item>
    /// <description>Note is going to be moved beyond zero.</description>
    /// </item>
    /// <item>
    /// <description>Note's end is going to be moved beyond the note's fixed end.</description>
    /// </item>
    /// </list>
    /// </exception>
    [Obsolete("OBS13")]
    public static void QuantizeNotes(this MidiFile midiFile, IGrid grid, NotesQuantizingSettings settings = null)
    {
        ThrowIfArgument.IsNull(nameof(midiFile), midiFile);
        ThrowIfArgument.IsNull(nameof(grid), grid);

        var tempoMap = midiFile.GetTempoMap();

        midiFile.GetTrackChunks().QuantizeNotes(grid, tempoMap, settings);
    }

    [Obsolete("Obsolete")]
    private static QuantizingSettings GetSettings(NotesQuantizingSettings settings) => new QuantizingSettings
    {
        DistanceCalculationType = settings.DistanceCalculationType,
        QuantizingLevel = settings.QuantizingLevel,
        Filter = obj => settings.Filter((Note)obj),
        LengthType = settings.LengthType,
        Target = settings.QuantizingTarget == LengthedObjectTarget.Start ? QuantizerTarget.Start : QuantizerTarget.End,
        QuantizingBeyondZeroPolicy = settings.QuantizingBeyondZeroPolicy,
        QuantizingBeyondFixedEndPolicy = settings.QuantizingBeyondFixedEndPolicy,
        FixOppositeEnd = settings.FixOppositeEnd
    };

    #endregion
}