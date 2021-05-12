/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Events
{
    public sealed class ChatStatusChanged : SeerEvent
    {
        internal ChatStatusChanged(EventSource readerBackendType, bool chatStatus) : base(readerBackendType, 0, true)
        {
            EventType = GetType();
            ChatStatus = chatStatus;
        }

        public bool ChatStatus { get; }

        public override bool IsValid() => true;
    }
}
