/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using BardMusicPlayer.Seer.Utilities;

namespace BardMusicPlayer.Seer.Events
{
    public sealed class ActorIdChanged : SeerEvent
    {
        internal ActorIdChanged(EventSource readerBackendType, uint actorId) : base(readerBackendType)
        {
            EventType = GetType();
            ActorId = actorId;
        }

        public uint ActorId { get; }

        public override bool IsValid()
        {
            return ActorIdTools.RangeOkay(ActorId);
        }
    }
}