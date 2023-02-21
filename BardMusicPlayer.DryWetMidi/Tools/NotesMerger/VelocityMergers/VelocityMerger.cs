﻿using BardMusicPlayer.DryWetMidi.Common.DataTypes;

namespace BardMusicPlayer.DryWetMidi.Tools.NotesMerger.VelocityMergers;

internal abstract class VelocityMerger
{
    #region Fields

    protected SevenBitNumber _velocity;

    #endregion

    #region Properties

    public virtual SevenBitNumber Velocity => _velocity;

    #endregion

    #region Methods

    public virtual void Initialize(SevenBitNumber velocity)
    {
        _velocity = velocity;
    }

    public abstract void Merge(SevenBitNumber velocity);

    #endregion
}