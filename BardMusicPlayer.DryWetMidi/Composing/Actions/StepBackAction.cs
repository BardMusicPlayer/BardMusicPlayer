﻿using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan;
using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan.Converters;
using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan.Representations;

namespace BardMusicPlayer.DryWetMidi.Composing.Actions
{
    internal sealed class StepBackAction : StepAction
    {
        #region Constructor

        public StepBackAction(ITimeSpan step)
            : base(step)
        {
        }

        #endregion

        #region Overrides

        public override PatternActionResult Invoke(long time, PatternContext context)
        {
            if (State != PatternActionState.Enabled)
                return PatternActionResult.DoNothing;

            var tempoMap = context.TempoMap;

            context.SaveTime(time);

            var convertedTime = TimeConverter.ConvertFrom(((MidiTimeSpan)time).Subtract(Step, TimeSpanMode.TimeLength), tempoMap);
            return new PatternActionResult(Math.Max(convertedTime, 0));
        }

        public override PatternAction Clone()
        {
            return new StepBackAction(Step.Clone());
        }

        #endregion
    }
}
