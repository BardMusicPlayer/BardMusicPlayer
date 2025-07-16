/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Events
{
    public sealed class EnsembleStarted : SeerEvent
    {
        internal EnsembleStarted(EventSource readerBackendType, long timestamp = -1) : base(readerBackendType, 100,
            true)
        {
            EventType = GetType();
            NetTimeStamp = timestamp;
        }

        public long NetTimeStamp { get; }

        public override bool IsValid()
        {
            return true;
        }
    }
}