/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Maestro.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BardMusicPlayer.Maestro.Sequencer.Backend.MogAmp;

internal class MogAmpSequencerBackend : ISequencerBackend
{
    public EventSource SequencerBackendType { get; }

    public SequencerHandler SequencerHandler { get; set; }

    public MogAmpSequencerBackend(int sleepTimeInMs)
    {
        SequencerBackendType = EventSource.MogAmp;
    }

    private void InitializeMogAmp()
    {

    }

    private void DestroyMogAmp()
    {

    }

    public async Task Loop(CancellationToken token)
    {
        InitializeMogAmp();

        while (!token.IsCancellationRequested)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
            }

            await Task.Delay(100, token);
        }

        DestroyMogAmp();
    }

    public void Dispose()
    {
        DestroyMogAmp();
        GC.SuppressFinalize(this);
    }
}
