﻿using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan;
using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan.Converters;

namespace BardMusicPlayer.DryWetMidi.Composing.Actions;

internal sealed class StepForwardAction : StepAction
{
    #region Constructor

    public StepForwardAction(ITimeSpan step)
        : base(step)
    {
    }

    #endregion

    #region Overrides

    public override PatternActionResult Invoke(long time, PatternContext context)
    {
        if (State != PatternActionState.Enabled)
            return PatternActionResult.DoNothing;

        context.SaveTime(time);
        return new PatternActionResult(time + LengthConverter.ConvertFrom(Step, time, context.TempoMap));
    }

    public override PatternAction Clone()
    {
        return new StepForwardAction(Step.Clone());
    }

    #endregion
}