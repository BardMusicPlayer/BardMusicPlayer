/*
 * Copyright(c) 2023 MoogleTroupe, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Diagnostics;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian.UtcMilliTime;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer;

public partial class BmpSeer
{
    private CancellationTokenSource _watcherTokenSource;

    private void StartProcessWatcher()
    {
        _watcherTokenSource = new CancellationTokenSource();

        Task.Factory.StartNew(() => RunProcessWatcher(_watcherTokenSource.Token), TaskCreationOptions.LongRunning);
    }

    private async Task RunProcessWatcher(CancellationToken token)
    {
        var coolDown = 0L;
        var processes = new List<Process>();

        while (!_watcherTokenSource.IsCancellationRequested)
        {
            try
            {
                // Clear the list of processes from previous iterations
                processes.Clear();

                // Get new processes and add them to the list
                processes.AddRange(Process.GetProcessesByName("ffxiv_dx11"));

                // Sort processes by creation time
                processes = processes.OrderBy(p => p.StartTime).ToList();

                // Remove games that are no longer running
                foreach (var game in _games.Values.Where(game =>
                             !token.IsCancellationRequested && (game.Process is null || game.Process.HasExited ||
                                                                !game.Process.Responding || processes.All(process => process.Id != game.Pid))))
                {
                    _games.TryRemove(game.Pid, out _);
                    game.Dispose();
                }

                foreach (var process in processes)
                {
                    if (token.IsCancellationRequested)
                        break;

                    // Add new games.
                    if (process is null || _games.ContainsKey(process.Id) || process.HasExited ||
                        !process.Responding)
                        continue;

                    // Adding a game spikes the cpu when sharlayan scans memory.
                    var timeNow = Clock.Time.Now;
                    if (coolDown + BmpPigeonhole.Instance.SeerGameScanCooldown > timeNow)
                        continue;

                    coolDown = timeNow;

                    var game = new Game(process);
                    if (!game.Initialize())
                    {
                        game.Dispose();
                    }
                    else
                    {
                        if (!_games.TryAdd(process.Id, game))
                            game.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                PublishEvent(new SeerExceptionEvent(ex));
            }

            await Task.Delay(5000, token);
        }
    }

    private void StopProcessWatcher() { _watcherTokenSource.Cancel(); }
}