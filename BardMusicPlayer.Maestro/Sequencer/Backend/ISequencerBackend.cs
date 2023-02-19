/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Maestro.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BardMusicPlayer.Maestro.Sequencer.Backend;

internal interface ISequencerBackend : IDisposable
{
    EventSource SequencerBackendType { get; }

    SequencerHandler SequencerHandler { get; set; }

    /// <summary>
    /// Dictionary of Players with thier uuid as the dictionary key. A Player may be a LocalPlayer or may be a RemotePlayer.
    /// </summary>
    IReadOnlyDictionary<string, IPlayer> Players { get; }

    Task Loop(CancellationToken token);
}
