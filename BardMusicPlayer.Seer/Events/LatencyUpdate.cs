/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
*/

namespace BardMusicPlayer.Seer.Events
{
    public sealed class LatencyUpdate : SeerEvent
    {
        internal LatencyUpdate(EventSource readerBackendType, long milis) : base(readerBackendType, 100, true)
        {
            EventType = GetType();
            LatencyMilis = milis;
        }

        public long LatencyMilis { get; }
        public override bool IsValid() => true;
    }
}