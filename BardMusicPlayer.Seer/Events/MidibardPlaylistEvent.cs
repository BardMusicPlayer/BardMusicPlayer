/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Seer.Events
{
    public sealed class MidibardPlaylistEvent : SeerEvent
    {
        internal MidibardPlaylistEvent(EventSource readerBackendType, Game game, int song) : base(readerBackendType, 0, false)
        {
            EventType = GetType();
            ChatLogGame = game;
            Song = song;
        }

        public Game ChatLogGame { get; }
        public DateTime ChatLogTimeStamp { get; }
        public int Song { get; }

        public override bool IsValid() => true;
    }
}