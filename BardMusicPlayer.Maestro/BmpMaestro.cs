/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Maestro.Sequencer;

namespace BardMusicPlayer.Maestro;

public partial class BmpMaestro
{
    private static readonly Lazy<BmpMaestro> LazyInstance = new(() => new BmpMaestro());
    internal SequencerHandler _sequencerHandler;

    /// <summary>
    /// 
    /// </summary>
    public bool Started { get; private set; }

    private BmpMaestro()
    {
    }

    public static BmpMaestro Instance => LazyInstance.Value;

    /// <summary>
    /// Start Maestro.
    /// </summary>
    public void Start(SequencerType sequencerType)
    {
        if (Started) return;
        _sequencerHandler = new SequencerHandler(sequencerType);
        Started           = true;
    }

    /// <summary>
    /// Stop Maestro.
    /// </summary>
    public void Stop()
    {
        if (!Started) return;
        _sequencerHandler.Dispose();
        Started = false;
    }

    ~BmpMaestro() => Dispose();
    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }
}