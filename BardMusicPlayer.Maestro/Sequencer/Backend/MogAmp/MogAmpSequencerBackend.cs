/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Maestro.Events;
using BardMusicPlayer.Maestro.Sequencer.Player;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace BardMusicPlayer.Maestro.Sequencer.Backend.MogAmp;

internal class MogAmpSequencerBackend : ISequencerBackend
{
    public EventSource SequencerBackendType { get; }

    public SequencerHandler SequencerHandler { get; set; }

    public MogAmpSequencerBackend()
    {
        SequencerBackendType = EventSource.BackEnd;
    }

    private ConcurrentDictionary<string, IPlayer> _players;

    public IReadOnlyDictionary<string, IPlayer> Players => new ReadOnlyDictionary<string, IPlayer>(_players);

    private void InitializeMogAmp()
    {
        _players = new ConcurrentDictionary<string, IPlayer>();
    }

    private void DestroyMogAmp()
    {
        _players = null;
    }

    public async Task Loop(CancellationToken token)
    {
        InitializeMogAmp();

        while (!token.IsCancellationRequested)
        {
            try
            {
                
            }
            catch (Exception)
            {
            }

            await Task.Delay(1, token);
        }

        DestroyMogAmp();
    }

    public void Dispose()
    {
        DestroyMogAmp();
        GC.SuppressFinalize(this);
    }
}
