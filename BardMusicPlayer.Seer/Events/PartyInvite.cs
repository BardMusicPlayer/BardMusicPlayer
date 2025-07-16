/*
 * Copyright(c) 2025 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Events
{
    public sealed class PartyInvite : SeerEvent
    {
        internal PartyInvite(EventSource readerBackendType) : base(
            readerBackendType)
        {
            EventType = GetType();
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}