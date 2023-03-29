/*
 * Copyright(c) 2022 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer.Reader.Backend;

internal interface IReaderBackend : IDisposable
{
    EventSource ReaderBackendType { get; }

    ReaderHandler ReaderHandler { get; set; }

    int SleepTimeInMs { get; set; }

    Task Loop(CancellationToken token);
}