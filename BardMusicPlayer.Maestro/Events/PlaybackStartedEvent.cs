/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Maestro.Events;

public sealed class PlaybackStartedEvent : MaestroEvent
{

    internal PlaybackStartedEvent()
    {
        EventType = GetType();
        Started   = true;
    }

    public bool Started;
    public override bool IsValid() => true;
}