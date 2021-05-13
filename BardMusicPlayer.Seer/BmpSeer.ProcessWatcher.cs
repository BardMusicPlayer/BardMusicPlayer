/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian.UtcMilliTime;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer
{
    public partial class BmpSeer
    {
        private bool _shouldRunProcessWatcherThread;
        private Thread _processWatcherThread;

        private void StartProcessWatcher()
        {
            _shouldRunProcessWatcherThread = true;
            _processWatcherThread = new Thread(RunProcessWatcher) { IsBackground = true };
            _processWatcherThread.Start();
        }

        private void RunProcessWatcher()
        {
            long coolDown = 0;
            while (_shouldRunProcessWatcherThread)
            {
                try
                {
                    var processes = Process.GetProcessesByName("ffxiv_dx11");

                    foreach (var game in _games.Values.Where(game => game.Process is null || game.Process.HasExited || !game.Process.Responding || processes.All(process => process.Id != game.Pid)))
                    {
                        _games.TryRemove(game.Pid, out _);
                        game?.Dispose();
                    }

                    foreach (var process in processes)
                    {
                        // Add new games.
                        if (process is null || _games.ContainsKey(process.Id) || process.HasExited || !process.Responding) continue;

                        // Adding a game spikes the cpu when sharlayan scans memory.
                        var timeNow = Clock.Time.Now;
                        if (coolDown + BmpPigeonhole.Instance.SeerGameScanCooldown > timeNow) continue;
                        coolDown = timeNow;
                        var game = new Game(process);
                        if (!_games.TryAdd(process.Id, game) || !game.Initialize()) game.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    PublishEvent(new SeerExceptionEvent(ex));
                }

                Thread.Sleep(1);
            }
        }

        private void StopProcessWatcher()
        {
            _shouldRunProcessWatcherThread = false;
            if (_processWatcherThread != null && !_processWatcherThread.Join(500)) _processWatcherThread.Abort();
            _processWatcherThread = null;
        }
    }
}
