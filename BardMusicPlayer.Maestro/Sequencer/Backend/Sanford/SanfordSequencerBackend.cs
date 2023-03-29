/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using BardMusicPlayer.Maestro.Events;
using BardMusicPlayer.Maestro.Sequencer.Player;

namespace BardMusicPlayer.Maestro.Sequencer.Backend.Sanford;

internal class SanfordSequencerBackend : ISequencerBackend
{
    public EventSource SequencerBackendType { get; }

    public SequencerHandler SequencerHandler { get; set; }

    public SanfordSequencerBackend()
    {
        SequencerBackendType = EventSource.BackEnd;
    }

    private ConcurrentDictionary<string, IPlayer> _players;

    public IReadOnlyDictionary<string, IPlayer> Players => new ReadOnlyDictionary<string, IPlayer>(_players);

    private void InitializeSanford()
    {
        _players = new ConcurrentDictionary<string, IPlayer>();
    }

    private void DestroySanford()
    {
        _players = null;
    }

    public async Task Loop(CancellationToken token)
    {
        InitializeSanford();

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

        DestroySanford();
    }

    public void Dispose()
    {
        DestroySanford();
        GC.SuppressFinalize(this);
    }
}