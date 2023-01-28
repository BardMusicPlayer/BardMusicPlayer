/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.Grunt.Helper.Utilities;
using BardMusicPlayer.Quotidian.Enums;
using BardMusicPlayer.Seer;

namespace BardMusicPlayer.Grunt;

public static partial class GameExtensions
{
    private static readonly SemaphoreSlim LyricSemaphoreSlim = new (1,1);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="game"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static async Task<bool> SendLyricLine(this Game game, string text)
    {
        if (!BmpGrunt.Instance.Started) throw new BmpGruntException("Grunt not started.");

        if (BmpGrunt.Instance.DalamudServer.IsConnected(game.Pid))
            return BmpGrunt.Instance.DalamudServer.SendChat(game.Pid, text);

        var sent = false;

        // We lock here to allow lyrics from multiple bards to not have collisions in the windows clipboard.
        await LyricSemaphoreSlim.WaitAsync();
        try
        {
            var tcs = new TaskCompletionSource<bool>();
            var clipboardThread = new Thread(() => SendLyricLineClipBoardTask(tcs, game, text));
            clipboardThread.SetApartmentState(ApartmentState.STA);
            clipboardThread.Start();
            sent = await tcs.Task;
        }
        catch (Exception)
        {
            // TODO: Log errors
        }
        finally
        {
            LyricSemaphoreSlim.Release();
        }

        return sent;
    }

    private static void SendLyricLineClipBoardTask(TaskCompletionSource<bool> tcs, Game game, string text)
    {
        try {
            if (!game.ChatStatus && !SyncTapKey(game, Keys.Enter).GetAwaiter().GetResult())
            {
                tcs.SetResult(false);
                return;
            }

            text.CopyToClipboard();

            var result = true;

            if (!SyncTapKey(game, (int) Keys.Control + Keys.V).GetAwaiter().GetResult())
            {
                result = false;
            }

            else if (!SyncTapKey(game, Keys.Enter).GetAwaiter().GetResult())
            {
                result = false;
            }

            tcs.SetResult(result);
        }
        catch(Exception) {
            // TODO: Log errors
            tcs.SetResult(false);
        }
    }
}