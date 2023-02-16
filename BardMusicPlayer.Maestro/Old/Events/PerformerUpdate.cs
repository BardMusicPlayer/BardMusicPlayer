/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Maestro.Old.Events;

public sealed class PerformerUpdate : MaestroEvent
{

    internal PerformerUpdate()
    {
        EventType = GetType();
    }

    public override bool IsValid() => true;
}