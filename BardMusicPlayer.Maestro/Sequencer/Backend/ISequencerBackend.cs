/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Maestro.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BardMusicPlayer.Maestro.Sequencer.Backend;

internal interface ISequencerBackend : IDisposable
{
    EventSource SequencerBackendType { get; }

    SequencerHandler SequencerHandler { get; set; }

    Task Loop(CancellationToken token);
}
