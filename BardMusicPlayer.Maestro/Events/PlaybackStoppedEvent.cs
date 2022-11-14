/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Maestro.Events
{
    public sealed class PlaybackStoppedEvent : MaestroEvent
    {

        internal PlaybackStoppedEvent() : base(0, false)
        {
            EventType = GetType();
            Stopped = true;
        }

        public bool Stopped;
        public override bool IsValid() => true;
    }
}
