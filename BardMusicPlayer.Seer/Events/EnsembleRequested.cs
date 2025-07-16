/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Events
{
    public sealed class EnsembleRequested : SeerEvent
    {
        internal EnsembleRequested(EventSource readerBackendType) : base(readerBackendType, 100)
        {
            EventType = GetType();
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}