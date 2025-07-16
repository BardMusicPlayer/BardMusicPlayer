/*
 * Copyright(c) 2025 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System.Collections.Generic;
using BardMusicPlayer.Seer.Utilities;

namespace BardMusicPlayer.Seer.Events
{
    public sealed class PartyLeaderChanged : SeerEvent
    {
        internal PartyLeaderChanged(EventSource readerBackendType, KeyValuePair<uint, string> partyLeader) : base(
            readerBackendType)
        {
            EventType = GetType();
            PartyLeader = partyLeader;
        }

        public KeyValuePair<uint, string> PartyLeader { get; set; }

        public override bool IsValid()
        {
            return ActorIdTools.RangeOkay(PartyLeader.Key);
        }
    }
}