using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan;

namespace BardMusicPlayer.DryWetMidi.Composing.Actions
{
    internal abstract class StepAction : PatternAction
    {
        #region Constructor

        public StepAction(ITimeSpan step)
        {
            Step = step;
        }

        #endregion

        #region Properties

        public ITimeSpan Step { get; }

        #endregion
    }
}
