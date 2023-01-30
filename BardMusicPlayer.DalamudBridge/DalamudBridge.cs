/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

#region

using System;
using BardMusicPlayer.DalamudBridge.Helper.Dalamud;
using BardMusicPlayer.Seer;

#endregion

namespace BardMusicPlayer.DalamudBridge;

public sealed partial class DalamudBridge
{
    private static readonly Lazy<DalamudBridge> LazyInstance = new(static () => new DalamudBridge());

    internal DalamudServer DalamudServer;

    private DalamudBridge()
    {
    }

    /// <summary>
    /// </summary>
    public bool Started { get; private set; }

    public static DalamudBridge Instance => LazyInstance.Value;

    /// <summary>
    ///     Start Grunt.
    /// </summary>
    public void Start()
    {
        if (Started)
            return;

        if (!BmpSeer.Instance.Started)
            throw new DalamudBridgeException("DalamudBridge requires Seer to be running.");

        DalamudServer = new DalamudServer();
        StartEventsHandler();
        Started = true;
    }

    /// <summary>
    ///     Stop Grunt.
    /// </summary>
    public void Stop()
    {
        if (!Started) return;

        StopEventsHandler();
        DalamudServer?.Dispose();
        DalamudServer = null;
        Started       = false;
    }


    public void ActionToQueue(DalamudBridgeCommandStruct data)
    {
        if (!Started) return;

        PublishEvent(data);
    }


    ~DalamudBridge()
    {
        Dispose();
    }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }
}