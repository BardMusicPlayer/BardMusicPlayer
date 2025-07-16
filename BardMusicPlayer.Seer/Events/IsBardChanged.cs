/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Events
{
    public sealed class IsBardChanged : SeerEvent
    {
        internal IsBardChanged(EventSource readerBackendType, bool isBard) : base(readerBackendType)
        {
            EventType = GetType();
            IsBard = isBard;
        }

        public bool IsBard { get; }

        public override bool IsValid()
        {
            return true;
        }
    }
}