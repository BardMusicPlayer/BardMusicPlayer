/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BardMusicPlayer.Common.UtcMilliTime;
using BardMusicPlayer.Config;
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
                        game.Dispose();
                        _games.Remove(game.Pid);
                    }

                    foreach (var process in processes)
                    {
                        // Add new games.
                        if (_games.ContainsKey(process.Id) || process.HasExited || !process.Responding) continue;

                        // Adding a game spikes the cpu when sharlayan scans memory.
                        var timeNow = Clock.Time.Now;
                        if (coolDown + BmpConfig.Instance.SeerGameScanCooldown > timeNow) continue;
                        coolDown = timeNow;
                        _games.Add(process.Id, new Game(process));
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
