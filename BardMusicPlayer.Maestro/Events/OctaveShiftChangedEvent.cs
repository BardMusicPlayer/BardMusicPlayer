/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Seer;

namespace BardMusicPlayer.Maestro.Events;

public sealed class OctaveShiftChangedEvent : MaestroEvent
{
    internal OctaveShiftChangedEvent(Game g, int octaveShift, bool isHost=false)
    {
        EventType   = GetType();
        OctaveShift = octaveShift;
        game        = g;
        IsHost      = isHost;
    }

    public Game game { get; }
    public int OctaveShift { get; }
    public bool IsHost { get; }
    public override bool IsValid() => true;
}