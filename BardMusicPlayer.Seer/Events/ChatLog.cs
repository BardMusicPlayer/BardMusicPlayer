/*
 * Copyright(c) 2025 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Core.Interfaces;

namespace BardMusicPlayer.Seer.Events
{
    public sealed class ChatLog : SeerEvent
    {
        internal ChatLog(EventSource readerBackendType, Game game, IChatLogItem item) : base(readerBackendType)
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

        public override bool IsValid()
        {
            return true;
        }
    }
}