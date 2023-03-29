/*
 * Copyright(c) 2023 MoogleTroupe, trotlinebeercan, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer.Events;
using BardMusicPlayer.Seer.Utilities;

namespace BardMusicPlayer.Seer;

public partial class BmpSeer : IDisposable
{
    private static readonly Lazy<BmpSeer> LazyInstance = new(() => new BmpSeer());

    /// <summary>
    /// 
    /// </summary>
    public bool Started { get; private set; }

    private readonly ConcurrentDictionary<int, Game> _games;

    private BmpSeer() { _games = new ConcurrentDictionary<int, Game>(); }

    public static BmpSeer Instance => LazyInstance.Value;

    /// <summary>
    /// Current active games
    /// </summary>
    public IReadOnlyDictionary<int, Game> Games => new ReadOnlyDictionary<int, Game>(_games);

    /// <summary>
    /// Configure the firewall for Machina
    /// </summary>
    /// <param name="appName">This application name.</param>
    /// <returns>true if successful</returns>
    public bool SetupFirewall(string appName)
    {
        try
        {
            if (Environment.GetEnvironmentVariable("WINEPREFIX") != null) return true;
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName               = "netsh";
            psi.UseShellExecute        = false;
            psi.RedirectStandardError  = true;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow         = true;
            psi.WindowStyle            = ProcessWindowStyle.Hidden;
            psi.Arguments              = $@"advfirewall firewall delete rule name=""{appName}""";

            Process proc = Process.Start(psi);
            proc.WaitForExit();

            psi = new ProcessStartInfo();
            psi.FileName = "netsh";
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.Arguments = $@"advfirewall firewall add rule name=""{appName}"" dir=in action=allow profile=any program=""{Process.GetCurrentProcess().MainModule.FileName}""";

            proc = Process.Start(psi);
            proc.WaitForExit();

            return true;
        }
        catch (Exception ex)
        {
            PublishEvent(new SeerExceptionEvent(ex));
            return false;
        }
    }

    /// <summary>
    /// Unconfigure the firewall for Machina
    /// </summary>
    /// <param name="appName">This application name.</param>
    /// <returns>true if successful</returns>
    public bool DestroyFirewall(string appName)
    {
        try
        {
            if (Environment.GetEnvironmentVariable("WINEPREFIX") != null) return true;
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName               = "netsh";
            psi.UseShellExecute        = false;
            psi.RedirectStandardError  = true;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow         = true;
            psi.WindowStyle            = ProcessWindowStyle.Hidden;
            psi.Arguments              = $@"advfirewall firewall delete rule name=""{appName}""";

            Process proc = Process.Start(psi);
            proc.WaitForExit();

            return true;
        }
        catch (Exception ex)
        {
            PublishEvent(new SeerExceptionEvent(ex));
            return false;
        }
    }

    /// <summary>
    /// Start Seer monitoring.
    /// </summary>
    public void Start()
    {
        if (Started) return;

        if (!BmpPigeonhole.Initialized) throw new BmpSeerException("Seer requires Pigeonhole to be initialized.");

        StartEventsHandler();
        StartProcessWatcher();
        Started = true;
    }

    /// <summary>
    /// Stop Seer monitoring.
    /// </summary>
    public void Stop()
    {
        if (!Started) return;

        StopProcessWatcher();
        StopEventsHandler();

        foreach (var game in _games.Values)
        {
            game?.Dispose();
        }

        _games.Clear();

        Started = false;
    }

    ~BmpSeer() { Dispose(); }

    public void Dispose()
    {
        Stop();
        MachinaManager.Instance.Dispose();
        GC.SuppressFinalize(this);
    }
}