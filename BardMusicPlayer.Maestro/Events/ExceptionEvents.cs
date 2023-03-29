/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Maestro.Events;

public class MaestroExceptionEvent : MaestroEvent
{
    internal MaestroExceptionEvent(Exception exception, EventSource eventSource = EventSource.Maestro) : base(eventSource)
    {
        EventType = GetType();
        Exception = exception;
    }

    public Exception Exception { get; }
}

public sealed class BackendExceptionEvent : MaestroExceptionEvent
{
    internal BackendExceptionEvent(EventSource sequencerBackendType, Exception exception) : base(exception,
        sequencerBackendType)
    {
        EventType = GetType();
    }
}