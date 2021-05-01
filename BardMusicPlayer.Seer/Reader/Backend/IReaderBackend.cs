/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Threading;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer.Reader.Backend
{
    internal interface IReaderBackend : IDisposable
    {
        EventSource ReaderBackendType { get; }
        ReaderHandler ReaderHandler { get; set; }
        CancellationToken CancellationToken { get; set; }
        int SleepTimeInMs { get; set; }
        void Loop();
    }
}
