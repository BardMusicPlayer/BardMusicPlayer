﻿/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
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

        public override bool IsValid() => ActorIdTools.RangeOkay(ActorId);
    }
}
