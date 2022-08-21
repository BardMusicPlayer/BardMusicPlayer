/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

namespace BardMusicPlayer.Maestro.Events
{
    public sealed class PlaybackStartedEvent : MaestroEvent
    {

        internal PlaybackStartedEvent() : base(0, false)
        {
            EventType = GetType();
            Started = true;
        }

        public bool Started;
        public override bool IsValid() => true;
    }
}
