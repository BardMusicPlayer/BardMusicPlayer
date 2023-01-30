/*
 * Copyright(c) 2022 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Machina.FFXIV;
using Machina.FFXIV.Oodle;
using Machina.Infrastructure;

namespace BardMusicPlayer.Seer.Utilities;

internal class MachinaManager : IDisposable
{
    internal static MachinaManager Instance => LazyInstance.Value;

    private static readonly Lazy<MachinaManager> LazyInstance = new(() => new MachinaManager());

    private MachinaManager()
    {
        _lock = new object();

        Trace.UseGlobalLock = false;
        Trace.Listeners.Add(new MachinaLogger());

        _monitor = new FFXIVNetworkMonitor
        {
            MonitorType         = NetworkMonitorType.RawSocket,
            OodlePath           = BmpSeer.Instance.Games.Values.First().GamePath + @"\game\ffxiv_dx11.exe",
            OodleImplementation = OodleImplementation.FfxivUdp
        };
        _monitor.MessageReceivedEventHandler += MessageReceivedEventHandler;
    }

    private static readonly List<int> Lengths = new() {48, 56, 88, 656, 664, 928, 3576 };
    private readonly FFXIVNetworkMonitor _monitor;
    private readonly object _lock;
    private bool _monitorRunning;

    internal delegate void MessageReceivedHandler(int processId, byte[] message);

    internal event MessageReceivedHandler MessageReceived;

    internal void AddGame(int pid)
    {
        lock (_lock)
        {
            if (_monitorRunning)
            {
                _monitor.Stop();
                _monitorRunning = false;
            }

            _monitor.ProcessIDList.Add((uint) pid);
            _monitor.Start();
            _monitorRunning = true;
        }
    }

    internal void RemoveGame(int pid)
    {
        lock (_lock)
        {
            if (_monitorRunning)
            {
                _monitor.Stop();
                _monitorRunning = false;
            }

            _monitor.ProcessIDList.Remove((uint) pid);
            if (_monitor.ProcessIDList.Count <= 0) return;

            _monitor.Start();
            _monitorRunning = true;
        }
    }

    private void MessageReceivedEventHandler(TCPConnection connection, long epoch, byte[] message)
    {
        if (Lengths.Contains(message.Length)) 
            //if (message.Length > 28)
            MessageReceived?.Invoke((int) connection.ProcessId, message);
    }

    ~MachinaManager() { Dispose(); }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_monitorRunning)
            {
                _monitor.Stop();
                _monitorRunning = false;
            }

            _monitor.ProcessIDList.Clear();
            _monitor.MessageReceivedEventHandler -= MessageReceivedEventHandler;
        }
    }
}