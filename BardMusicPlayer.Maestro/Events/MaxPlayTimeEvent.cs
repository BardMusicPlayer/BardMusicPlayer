/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Maestro.Events
{
    public sealed class MaxPlayTimeEvent : MaestroEvent
    {
        internal MaxPlayTimeEvent(TimeSpan inTimeSpan, int inTick) : base(0, false)
        {
            EventType = GetType();
            timeSpan = inTimeSpan;
            tick = inTick;
        }

        public TimeSpan timeSpan { get; }

        public int tick { get; }

        public override bool IsValid() => true;
    }

}
