/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Seer.Events
{
    public sealed class ChatLog : SeerEvent
    {
        internal ChatLog(EventSource readerBackendType, Game game, Reader.Backend.Sharlayan.Core.ChatLogItem item) : base(readerBackendType, 0, false)
        {
            EventType = GetType();
            ChatLogGame = game;
            ChatLogTimeStamp = item.TimeStamp;
            ChatLogCode = item.Code;
            ChatLogLine = item.Line;
        }

        public Game ChatLogGame { get; }
        public DateTime ChatLogTimeStamp { get; }
        public string ChatLogCode { get; }
        public string ChatLogLine { get; }

        public override bool IsValid() => true;
    }
}