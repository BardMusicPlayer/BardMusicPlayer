﻿using BardMusicPlayer.DryWetMidi.Interaction.TimedEvents;
using BardMusicPlayer.DryWetMidi.Standards.GeneralMidi;

namespace BardMusicPlayer.DryWetMidi.Composing.Actions.SetProgram;

internal sealed class SetGeneralMidiProgramAction : PatternAction
{
    #region Constructor

    public SetGeneralMidiProgramAction(GeneralMidiProgram program)
    {
        Program = program;
    }

    #endregion

    #region Properties

    public GeneralMidiProgram Program { get; }

    #endregion

    #region Overrides

    public override PatternActionResult Invoke(long time, PatternContext context)
    {
        if (State != PatternActionState.Enabled)
            return PatternActionResult.DoNothing;

        var programEvent = Program.GetProgramEvent(context.Channel);
        var timedEvent = new TimedEvent(programEvent, time);

        return new PatternActionResult(time, new[] { timedEvent });
    }

    public override PatternAction Clone()
    {
        return new SetGeneralMidiProgramAction(Program);
    }

    #endregion
}