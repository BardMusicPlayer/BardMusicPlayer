/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Grunt.Helper.Utilities;
using BardMusicPlayer.Quotidian.Enums;
using BardMusicPlayer.Seer;

namespace BardMusicPlayer.Grunt;

public static partial class GameExtensions
{
    /// <summary>
    /// Named opposite of behavior, this runs async in this thread, sync to sendmessage the ffxiv game.
    /// Note that in addition to the delay value, using any modifier key will add an mandatory 25ms delay loop to allow the game to capture said modifier input.
    /// </summary>
    /// <param name="game"></param>
    /// <param name="key"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public static async Task<bool> SyncPushKey(this Game game, Keys key, int delay = 25)
    {
        if (!BmpGrunt.Instance.Started) throw new BmpGruntException("Grunt not started.");
            
        var sent = false;
        try
        {
            var baseKey = key & ~Keys.Control & key & ~Keys.Shift & key & ~Keys.Alt;

            if (baseKey != key)
            {
                for (var i = 0; i < 5; i++)
                {
                    if ((key & Keys.Control) == Keys.Control) NativeMethods.SendMessage(game.Process.MainWindowHandle, NativeMethods.WM_KEYDOWN, (IntPtr) Keys.ControlKey, (IntPtr) 0);
                    if ((key & Keys.Alt) == Keys.Alt) NativeMethods.SendMessage(game.Process.MainWindowHandle, NativeMethods.WM_SYSKEYDOWN, (IntPtr) Keys.AltKey, (IntPtr) 0);
                    if ((key & Keys.Shift) == Keys.Shift) NativeMethods.SendMessage(game.Process.MainWindowHandle, NativeMethods.WM_KEYDOWN, (IntPtr) Keys.ShiftKey, (IntPtr) 0);
                    await Task.Delay(5);
                }
            }

            NativeMethods.SendMessage(game.Process.MainWindowHandle, NativeMethods.WM_KEYDOWN, (IntPtr) baseKey, (IntPtr) 0);
            await Task.Delay(delay);

            sent = true;
        }
        catch (Exception)
        {
            // TODO: log error message.
        }

        return sent;
    }

    /// <summary>
    /// Named opposite of behavior, this runs async in this thread, sync to sendmessage the ffxiv game.
    /// Note that in addition to the delay value, using any modifier key will add a mandatory 5ms delay per modifier allow the game to capture said modifier input.
    /// </summary>
    /// <param name="game"></param>
    /// <param name="key"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public static async Task<bool> SyncReleaseKey(this Game game, Keys key, int delay = 25)
    {
        if (!BmpGrunt.Instance.Started) throw new BmpGruntException("Grunt not started.");
            
        var sent = false;
            
        try
        {
            var baseKey = key & ~Keys.Control & key & ~Keys.Shift & key & ~Keys.Alt;

            NativeMethods.SendMessage(game.Process.MainWindowHandle, NativeMethods.WM_KEYUP, (IntPtr) baseKey, (IntPtr) 0);

            if (baseKey != key)
            {
                if ((key & Keys.Shift) == Keys.Shift)
                {
                    await Task.Delay(5);
                    NativeMethods.SendMessage(game.Process.MainWindowHandle, NativeMethods.WM_KEYUP, (IntPtr) Keys.ShiftKey, (IntPtr) 0);
                }

                if ((key & Keys.Alt) == Keys.Alt)
                {
                    await Task.Delay(5);
                    NativeMethods.SendMessage(game.Process.MainWindowHandle, NativeMethods.WM_SYSKEYUP, (IntPtr) Keys.AltKey, (IntPtr) 0);
                }

                if ((key & Keys.Control) == Keys.Control)
                {
                    await Task.Delay(5);
                    NativeMethods.SendMessage(game.Process.MainWindowHandle, NativeMethods.WM_KEYUP, (IntPtr) Keys.ControlKey, (IntPtr) 0);
                }
            }

            await Task.Delay(delay);

            sent = true;
        }
        catch (Exception)
        {
            // TODO: log error message.
        }

        return sent;
    }

    /// <summary>
    /// Named opposite of behavior, this runs async in this thread, sync to sendmessage the ffxiv game.
    /// Note that in addition to the delay values, using modifier keys will add 30-45ms extra delay depending on how many modifier keys are used.
    /// </summary>
    /// <param name="game"></param>
    /// <param name="key"></param>
    /// <param name="downDelay"></param>
    /// <param name="upDelay"></param>
    /// <returns></returns>
    public static async Task<bool> SyncTapKey(this Game game, Keys key, int downDelay = 25, int upDelay = 25) => await SyncPushKey(game, key, downDelay) && await SyncReleaseKey(game, key, upDelay);
        
    /// <summary>
    /// Named opposite of behavior, this runs synced in this thread, async to postmessage the ffxiv game.
    /// </summary>
    /// <param name="game"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool ASyncPushKey(this Game game, Keys key)
    {
        if (!BmpGrunt.Instance.Started) throw new BmpGruntException("Grunt not started.");
            
        var sent = false;
            
        try
        {
            if (key != (key & ~Keys.Control & key & ~Keys.Shift & key & ~Keys.Alt)) throw new BmpGruntException("ASync Key methods do not accept modifier keys.");

            NativeMethods.PostMessage(game.Process.MainWindowHandle, NativeMethods.WM_KEYDOWN, (IntPtr) key, (IntPtr) 0);

            sent = true;
        }
        catch (Exception)
        {
            // TODO: log error message.
        }

        return sent;
    }

    /// <summary>
    /// Named opposite of behavior, this runs synced in this thread, async to postmessage the ffxiv game.
    /// </summary>
    /// <param name="game"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool ASyncReleaseKey(this Game game, Keys key)
    {
        if (!BmpGrunt.Instance.Started) throw new BmpGruntException("Grunt not started.");
            
        var sent = false;
            
        try
        {
            if (key != (key & ~Keys.Control & key & ~Keys.Shift & key & ~Keys.Alt)) throw new BmpGruntException("ASync Key methods do not accept modifier keys.");

            NativeMethods.PostMessage(game.Process.MainWindowHandle, NativeMethods.WM_KEYUP, (IntPtr) key, (IntPtr) 0);

            sent = true;
        }
        catch (Exception)
        {
            // TODO: log error message.
        }

        return sent;
    }
}