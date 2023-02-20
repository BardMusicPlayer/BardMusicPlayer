/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Maestro.Sequencer.Backend;
using BardMusicPlayer.Maestro.Sequencer.Backend.MogAmp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BardMusicPlayer.Maestro.Sequencer;

internal class SequencerHandler : IDisposable
{
    private readonly ISequencerBackend _sequencerBackend;
    private CancellationTokenSource _cts;
    private Task _task;

    internal SequencerHandler(SequencerType sequencerType)
    {
        switch (sequencerType)
        {
            case SequencerType.MogAmp:
                _sequencerBackend = new MogAmpSequencerBackend();
                break;
        }
        
        _sequencerBackend.SequencerHandler = this;
        StartBackend();
    }

    ~SequencerHandler() { Dispose(); }

    public void Dispose()
    {
        StopBackend();
        _sequencerBackend.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Starts the internal IBackend thread.
    /// </summary>
    internal void StartBackend()
    {
        if (_task != null)
            throw new BmpMaestroBackendAlreadyRunningException(_sequencerBackend.SequencerBackendType);

        _cts  = new CancellationTokenSource();
        _task = Task.Factory.StartNew(() => _sequencerBackend.Loop(_cts.Token), TaskCreationOptions.LongRunning);
    }

    /// <summary>
    /// Stops the internal IBackend thread.
    /// </summary>
    internal void StopBackend()
    {
        if (_task == null) return;

        _cts.Cancel();
        _task = null;
    }
}