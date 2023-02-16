/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Runtime.InteropServices;

namespace BardMusicPlayer.Grunt.Helper.Utilities;

internal static class NativeMethods
{
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        
    internal const int WM_KEYDOWN = 0x0100;
    internal const int WM_KEYUP = 0x0101;
    internal const int WM_SYSKEYDOWN = 0x104;
    internal const int WM_SYSKEYUP = 0x105;

    [DllImport("user32.dll")]
    internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll")]
    internal static extern bool CloseClipboard();

    [DllImport("user32.dll")]
    internal static extern bool SetClipboardData(uint uFormat, IntPtr data);

    internal const uint CF_UNICODETEXT = 13;

    internal static bool CopyToClipboard(this string text)
    {
        if (!OpenClipboard(IntPtr.Zero)){
            return false;
        }

        var clipboardText = Marshal.StringToHGlobalUni(text);

        SetClipboardData(CF_UNICODETEXT, clipboardText);
        CloseClipboard();

        //-------------------------------------------
        // Appears the clipboard is responsible for this.
        //-------------------------------------------
        // Marshal.FreeHGlobal(clipboardText);

        return true;
    }
}