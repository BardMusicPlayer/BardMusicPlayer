/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Diagnostics;
using System.Threading;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer
{
    public partial class Seer
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
                            _games.Add(process.Id, new Game(process));
                        }
                    }
                }
                catch (Exception ex)
                {
                    PublishEvent(new SeerExceptionEvent(ex));
                }

                Thread.Sleep(200);
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
