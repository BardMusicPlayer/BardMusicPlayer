﻿using System.ComponentModel;
using BardMusicPlayer.DryWetMidi.Common;
using BardMusicPlayer.DryWetMidi.Common.DataTypes;
using BardMusicPlayer.DryWetMidi.Core.Events.Channel;
using BardMusicPlayer.DryWetMidi.Interaction.TimedEvents;
using BardMusicPlayer.DryWetMidi.Interaction.TimedObject;
using BardMusicPlayer.DryWetMidi.Interaction.Utilities.ThrowIf;

namespace BardMusicPlayer.DryWetMidi.Interaction.Parameters;

/// <summary>
/// Represents parameter (RPN or NRPN) encoded as series of Control Change events.
/// </summary>
public abstract class Parameter : ITimedObject, INotifyTimeChanged
{
    #region Events

    /// <summary>
    /// Occurs when the time of an object has been changed.
    /// </summary>
    public event EventHandler<TimeChangedEventArgs> TimeChanged;

    #endregion

    #region Fields

    private long _time;
    private ParameterValueType _valueType = ParameterValueType.Exact;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the channel of the current parameter. This channel is in fact
    /// the channel of Control Change events that represent the parameter.
    /// </summary>
    public FourBitNumber Channel { get; set; }

    /// <summary>
    /// Gets or sets the type of the current parameter's value.
    /// </summary>
    /// <exception cref="InvalidEnumArgumentException"><paramref name="value"/> specified an invalid value.</exception>
    public ParameterValueType ValueType
    {
        get { return _valueType; }
        set
        {
            ThrowIfArgument.IsInvalidEnumValue(nameof(value), _valueType);

            _valueType = value;
        }
    }

    /// <summary>
    /// Gets or sets absolute time of the parameter data in units defined by the time division of a MIDI file.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is negative.</exception>
    public long Time
    {
        get { return _time; }
        set
        {
            ThrowIfTimeArgument.IsNegative(nameof(value), value);

            var oldTime = Time;
            if (value == oldTime)
                return;

            _time = value;

            TimeChanged?.Invoke(this, new TimeChangedEventArgs(oldTime, value));
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Clones object by creating a copy of it.
    /// </summary>
    /// <returns>Copy of the object.</returns>
    public abstract ITimedObject Clone();

    /// <summary>
    /// Returns the collection of <see cref="TimedEvent"/> objects that represent the current
    /// parameter. In fact, each <see cref="TimedEvent"/> object will contain <see cref="ControlChangeEvent"/> event.
    /// </summary>
    /// <returns>Collection of <see cref="TimedEvent"/> objects that represent the current
    /// parameter.</returns>
    public abstract IEnumerable<TimedEvent> GetTimedEvents();

    #endregion
}