/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using BardMusicPlayer.Maestro.Events;
using BardMusicPlayer.Seer;

namespace BardMusicPlayer.Maestro.Old.Events;

public sealed class SpeedShiftEvent : MaestroEvent
{
    internal SpeedShiftEvent(Game g, float speedShift, bool isHost = false)
    {
        EventType  = GetType();
        SpeedShift = speedShift;
        game       = g;
        IsHost     = isHost;
    }

    public Game game { get; }
    public float SpeedShift { get; }
    public bool IsHost { get; }
    public override bool IsValid() => true;
}