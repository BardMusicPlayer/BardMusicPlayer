/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Quotidian.UtcMilliTime;

namespace BardMusicPlayer.Maestro.Events;

public abstract class MaestroEvent
{
    internal MaestroEvent(EventSource eventSource)
    {
        EventSource     = eventSource;
        TimeStamp       = Clock.Time.Now;
    }

    public long TimeStamp { get; }

    public EventSource EventSource { get; }

    public Type EventType { get; protected set; }
}
