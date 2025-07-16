/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Seer.Events
{
    public class SeerExceptionEvent : SeerEvent
    {
        internal SeerExceptionEvent(Exception exception, EventSource eventSource = EventSource.Seer) : base(eventSource)
        {
            EventType = GetType();
            Exception = exception;
        }

        public Exception Exception { get; }

        public override bool IsValid()
        {
            return true;
        }
    }

    public sealed class GameExceptionEvent : SeerExceptionEvent
    {
        internal GameExceptionEvent(Game game, int pid, Exception exception) : base(exception, EventSource.Game)
        {
            EventType = GetType();
            Game = game;
            Pid = pid;
        }

        public int Pid { get; }

        public override bool IsValid()
        {
            return true;
        }
    }

    public sealed class BackendExceptionEvent : SeerExceptionEvent
    {
        internal BackendExceptionEvent(EventSource readerBackendType, Exception exception) : base(exception,
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