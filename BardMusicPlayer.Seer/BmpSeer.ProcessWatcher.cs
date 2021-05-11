/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Diagnostics;
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
                    foreach (var process in Process.GetProcessesByName("ffxiv_dx11"))
                    {
                        // Remove dead games if we were watching them.
                        if (process.HasExited || process.Responding == false && _games.ContainsKey(process.Id))
                        {
                            _games[process.Id]?.Dispose();
                            _games.Remove(process.Id);
                        }

                        // Add new games.
                        else if (!_games.ContainsKey(process.Id) && !process.HasExited && process.Responding)
                        {
                            // Adding a game spikes the cpu when sharlayan scans memory.
                            var timeNow = Clock.Time.Now.ToUtcMilliTime() / 1000;
                            if (coolDown + BmpConfig.Instance.SeerGameScanCooldown > timeNow) continue;
                            coolDown = timeNow;
                            _games.Add(process.Id, new Game(process));
                        }
                    }
                }
                catch (Exception ex)
                {
                    PublishEvent(new SeerExceptionEvent(ex));
                }

                Thread.Sleep(BmpConfig.Instance.SeerGameScanCooldown);
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
