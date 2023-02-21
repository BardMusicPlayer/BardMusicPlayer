﻿using BardMusicPlayer.DryWetMidi.Core.Events.Meta;
using BardMusicPlayer.DryWetMidi.Interaction.TimedEvents;

namespace BardMusicPlayer.DryWetMidi.Composing.Actions;

internal sealed class AddTextEventAction<TEvent> : PatternAction
    where TEvent : BaseTextEvent
{
    #region Constructor

    public AddTextEventAction(string text)
    {
        Text = text;
    }

    #endregion

    #region Properties

    public string Text { get; }

    #endregion

    #region Overrides

    public override PatternActionResult Invoke(long time, PatternContext context)
    {
        if (State != PatternActionState.Enabled)
            return PatternActionResult.DoNothing;

        var textEvent = (BaseTextEvent)Activator.CreateInstance(typeof(TEvent), Text);
        var timedEvent = new TimedEvent(textEvent, time);

        return new PatternActionResult(time, new[] { timedEvent });
    }

    public override PatternAction Clone()
    {
        return new AddTextEventAction<TEvent>(Text);
    }

    #endregion
}