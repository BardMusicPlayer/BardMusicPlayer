/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BardMusicPlayer.Seer.Utilities;

namespace BardMusicPlayer.Seer.Events
{
    public sealed class PartyMembersChanged : SeerEvent
    {
        internal PartyMembersChanged(EventSource readerBackendType, IDictionary<uint, string> partyMembers) : base(
            readerBackendType)
        {
            EventType = GetType();
            PartyMembers = new ReadOnlyDictionary<uint, string>(partyMembers);
        }

        public IReadOnlyDictionary<uint, string> PartyMembers { get; set; }

        public override bool IsValid()
        {
            return PartyMembers.Count is 0 or > 1 and < 9 &&
                   PartyMembers.Keys.All(ActorIdTools.RangeOkay) && !PartyMembers.Values.Any(string.IsNullOrEmpty);
        }
    }
}