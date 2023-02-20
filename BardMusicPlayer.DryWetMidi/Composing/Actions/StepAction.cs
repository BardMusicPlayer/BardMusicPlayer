using BardMusicPlayer.DryWetMidi.Interaction;

namespace BardMusicPlayer.DryWetMidi.Composing
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
