﻿using BardMusicPlayer.DryWetMidi.Common;
using BardMusicPlayer.DryWetMidi.Common.DataTypes;
using BardMusicPlayer.DryWetMidi.Core.Events.Base;

namespace BardMusicPlayer.DryWetMidi.Core.Utilities;

/// <summary>
/// Provides useful methods to manipulate an instance of the <see cref="MidiFile"/>.
/// </summary>
public static class MidiFileUtilities
{
    #region Methods

    /// <summary>
    /// Gets all channel numbers presented in the specified <see cref="MidiFile"/>.
    /// </summary>
    /// <param name="midiFile"><see cref="MidiFile"/> to get channels of.</param>
    /// <returns>Collection of channel numbers presented in the <paramref name="midiFile"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="midiFile"/> is <c>null</c>.</exception>
    public static IEnumerable<FourBitNumber> GetChannels(this MidiFile midiFile)
    {
        ThrowIfArgument.IsNull(nameof(midiFile), midiFile);

        return midiFile.GetTrackChunks().GetChannels();
    }

    internal static IEnumerable<MidiEvent> GetEvents(this MidiFile midiFile)
    {
        return midiFile.GetTrackChunks().SelectMany(c => c.Events);
    }

    #endregion
}