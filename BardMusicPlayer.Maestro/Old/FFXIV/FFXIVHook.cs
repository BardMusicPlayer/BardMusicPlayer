/*
 * Copyright(c) 2022 Parulina, sabihoshi, trotlinebeercan, troy-f, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.Quotidian.Enums;

namespace BardMusicPlayer.Maestro.Old.FFXIV;

public class FFXIVHook
{

    #region WIN32

    private const int WH_KEYBOARD_LL = 13;
    private const int WH_GETMESSAGE = 3;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    public const int WM_SYSKEYDOWN = 0x104;
    public const int WM_SYSKEYUP = 0x105;
    private const int WM_CHAR = 0x0102;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_LBUTTONUP = 0x0202;

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, MessageProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetMessageExtraInfo();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern uint MapVirtualKey(uint uCode, uint uMapType);

    [DllImport("user32.dll")]
    internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll")]
    internal static extern bool CloseClipboard();

    [DllImport("user32.dll")]
    internal static extern bool SetClipboardData(uint uFormat, IntPtr data);

    private struct INPUT
    {
        public int type;
        public InputUnion un;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)]
        public MOUSEINPUT mi;
        [FieldOffset(0)]
        public KEYBDINPUT ki;
        [FieldOffset(0)]
        public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public uint uMsg;
        public ushort wParamL;
        public ushort wParamH;
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetClientRect(HandleRef hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool ClientToScreen(HandleRef hWnd, ref POINT lpPoint);


    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public int Width()
        {
            return Right - Left;
        }
        public int Height()
        {
            return Bottom - Top;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public delegate IntPtr MessageProc(int nCode, IntPtr wParam, IntPtr lParam);

    #endregion

    public List<Keys> lastPerformanceKeys = new();

    private IntPtr mainWindowHandle;
    private static MessageProc proc;
    private IntPtr _hookID = IntPtr.Zero;

    public EventHandler<Keys> OnKeyPressed;

    public Process Process { get; private set; }

    public bool Hook(Process process, bool useCallback = true)
    {
        if (process == null)
        {
            return false;
        }
        if (_hookID != IntPtr.Zero)
        {
            Unhook();
        }
        Process = process;
        mainWindowHandle = process.MainWindowHandle;

        if (useCallback)
        {
            proc = HookCallback;
            _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, proc, IntPtr.Zero, 0);
            return _hookID != IntPtr.Zero;
        }
        return true;
    }
    public void Unhook()
    {
        if (_hookID != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookID);
            Process = null;
            _hookID = IntPtr.Zero;
        }
    }

    public void FocusWindow()
    {
        SetForegroundWindow(mainWindowHandle);
    }
    public void SendTimedSyncKey(Keys key, bool modifier = true, bool sendDown = true, bool sendUp = true)
    {
        Task.Run(() =>
        {
            var key2 = key & ~Keys.Control & key & ~Keys.Shift & key & ~Keys.Alt;
            if (sendDown)
            {
                if (modifier)
                {
                    for (var i = 0; i < 5; i++)
                    {
                        if ((key & Keys.Control) == Keys.Control)
                        {
                            SendMessage(mainWindowHandle, WM_KEYDOWN, (IntPtr)Keys.ControlKey, (IntPtr)0);
                        }
                        if ((key & Keys.Alt) == Keys.Alt)
                        {
                            SendMessage(mainWindowHandle, WM_SYSKEYDOWN, (IntPtr)Keys.AltKey, (IntPtr)0);
                        }
                        if ((key & Keys.Shift) == Keys.Shift)
                        {
                            SendMessage(mainWindowHandle, WM_KEYDOWN, (IntPtr)Keys.ShiftKey, (IntPtr)0);
                        }
                        Thread.Sleep(5);
                    }
                }
                SendMessage(mainWindowHandle, WM_KEYDOWN, (IntPtr)key2, (IntPtr)0);
                Thread.Sleep(50);
            }
            if (sendUp)
            {
                SendMessage(mainWindowHandle, WM_KEYUP, (IntPtr)key2, (IntPtr)0);
                if (modifier)
                {
                    if ((key & Keys.Shift) == Keys.Shift)
                    {
                        Thread.Sleep(5);
                        SendMessage(mainWindowHandle, WM_KEYUP, (IntPtr)Keys.ShiftKey, (IntPtr)0);
                    }
                    if ((key & Keys.Alt) == Keys.Alt)
                    {
                        Thread.Sleep(5);
                        SendMessage(mainWindowHandle, WM_SYSKEYUP, (IntPtr)Keys.AltKey, (IntPtr)0);
                    }
                    if ((key & Keys.Control) == Keys.Control)
                    {
                        Thread.Sleep(5);
                        SendMessage(mainWindowHandle, WM_KEYUP, (IntPtr)Keys.ControlKey, (IntPtr)0);
                    }
                }
            }
        });
    }
    public void SendSyncKey(Keys key, bool modifier = true, bool sendDown = true, bool sendUp = true)
    {
        var key2 = key & ~Keys.Control & key & ~Keys.Shift & key & ~Keys.Alt;
        if (sendDown)
        {
            if (modifier)
            {
                if ((key & Keys.Control) == Keys.Control)
                    SendMessage(mainWindowHandle, WM_KEYDOWN, (IntPtr)Keys.ControlKey, (IntPtr)0);
                if ((key & Keys.Alt) == Keys.Alt)
                    SendMessage(mainWindowHandle, WM_KEYDOWN, (IntPtr)Keys.AltKey, (IntPtr)0);
                if ((key & Keys.Shift) == Keys.Shift)
                    SendMessage(mainWindowHandle, WM_KEYDOWN, (IntPtr)Keys.ShiftKey, (IntPtr)0);
            }
            SendMessage(mainWindowHandle, WM_KEYDOWN, (IntPtr)key2, (IntPtr)0);
        }
        if (sendUp)
        {
            SendMessage(mainWindowHandle, WM_KEYUP, (IntPtr)key2, (IntPtr)0);
            if (modifier)
            {
                if ((key & Keys.Control) == Keys.Control)
                    SendMessage(mainWindowHandle, WM_KEYUP, (IntPtr)Keys.ControlKey, (IntPtr)0);
                if ((key & Keys.Alt) == Keys.Alt)
                    SendMessage(mainWindowHandle, WM_KEYUP, (IntPtr)Keys.AltKey, (IntPtr)0);
                if ((key & Keys.Shift) == Keys.Shift)
                    SendMessage(mainWindowHandle, WM_KEYUP, (IntPtr)Keys.ShiftKey, (IntPtr)0);
            }
        }
    }

    public void SendAsyncKey(Keys key, bool modifier = true, bool sendDown = true, bool sendUp = true)
    {
        var key2 = key & ~Keys.Control & key & ~Keys.Shift;
        if (sendDown)
        {
            if (modifier)
            {
                if ((key & Keys.Control) == Keys.Control)
                {
                    PostMessage(mainWindowHandle, WM_KEYDOWN, (IntPtr)Keys.ControlKey, (IntPtr)0);
                }
                if ((key & Keys.Shift) == Keys.Shift)
                {
                    PostMessage(mainWindowHandle, WM_KEYDOWN, (IntPtr)Keys.ShiftKey, (IntPtr)0);
                }
            }
            PostMessage(mainWindowHandle, WM_KEYDOWN, (IntPtr)key2, (IntPtr)0);
        }
        if (sendUp)
        {
            PostMessage(mainWindowHandle, WM_KEYUP, (IntPtr)key2, (IntPtr)0);
            if (modifier)
            {
                if ((key & Keys.Control) == Keys.Control)
                {
                    PostMessage(mainWindowHandle, WM_KEYUP, (IntPtr)Keys.ControlKey, (IntPtr)0);
                }
                if ((key & Keys.Shift) == Keys.Shift)
                {
                    PostMessage(mainWindowHandle, WM_KEYUP, (IntPtr)Keys.ShiftKey, (IntPtr)0);
                }
            }
        }
    }

    public void SendKeyStroke(Keys keyDown = Keys.None, Keys modDown = Keys.None, Keys keyUp = Keys.None, Keys modUp = Keys.None)
    {
        if (keyDown != Keys.None)
        {
            SendAsyncKey(keyDown | modDown, true, true, false);
        }
        if (keyUp != Keys.None)
        {
            SendAsyncKey(keyDown | modDown, true, false);
        }
    }

    public void SendKeyInput(IEnumerable<KEYBDINPUT> KeybdInput)
    {
        var keyList = KeybdInput.Select(input => new INPUT { type = 1, un = new InputUnion { ki = input } }).ToList();
        SendInput((uint)keyList.Count, keyList.ToArray(), Marshal.SizeOf(typeof(INPUT)));
    }

    public void SendAsyncChar(char charInput)
    {
        SendMessage(mainWindowHandle, WM_CHAR, (IntPtr)charInput, (IntPtr)0);
    }

    public bool CopyToClipboard(string text)
    {
        if (GetForegroundWindow() != mainWindowHandle)
        {
            SetForegroundWindow(mainWindowHandle);
        }

        if (!OpenClipboard(IntPtr.Zero))
        {
            return false;
        }

        var clipboardText = Marshal.StringToHGlobalUni(text);

        SetClipboardData(13, clipboardText);
        CloseClipboard();
        Task.Delay(50).Wait();
        SendSyncKey((int)Keys.Control + Keys.V);
        return true;
    }

    public void SendString(string input)
    {
        if (GetForegroundWindow() != mainWindowHandle)
        {
            SetForegroundWindow(mainWindowHandle);
        }
        var keyList = new List<INPUT>();
        foreach (short c in input)
        {
            if (c > 10)
            {
                var keyDown = new INPUT
                {
                    type = 1,
                    un = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = 0,
                            wScan = (ushort)c,
                            dwFlags = 0x0004
                        }
                    }
                };
                keyList.Add(keyDown);

                var keyUp = new INPUT
                {
                    type = 1,
                    un = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = 0,
                            wScan = (ushort)c,
                            dwFlags = 0x0004 | 0x0002
                        }
                    }
                };
                keyList.Add(keyUp);
            }
        }
        SendInput((uint)keyList.Count, keyList.ToArray(), Marshal.SizeOf(typeof(INPUT)));
    }

    public void SendAsyncKeybind(Keys keybind)
    {
        SendAsyncKey(keybind);
    }
    public void SendSyncKeybind(Keys keybind)
    {
        SendSyncKey(keybind);
    }

    public void SendTimedSyncKeybind(Keys keybind)
    {
        SendTimedSyncKey(keybind);
    }

    public void SendKeybindDown(Keys keybind)
    {
        if (keybind == Keys.None)
            return;

        SendAsyncKey(keybind, true, true, false);

        if (!lastPerformanceKeys.Contains(keybind))
        {
            lastPerformanceKeys.Add(keybind);
        }
    }

    public void SendKeybindUp(Keys keybind)
    {
        if (keybind == Keys.None)
            return;

        SendAsyncKey(keybind, true, false);

        if (lastPerformanceKeys.Contains(keybind))
        {
            lastPerformanceKeys.Remove(keybind);
        }
    }

    public void ClearLastPerformanceKeybinds()
    {
        foreach (var keybind in lastPerformanceKeys.ToArray())
        {
            SendSyncKey(keybind, true, false);
        }
        lastPerformanceKeys.Clear();
    }

    public RECT GetClientRect()
    {
        return GetClientRect(new HandleRef(this, mainWindowHandle), out var rect) ? rect : new RECT();
    }

    public bool GetScreenFromClientPoint(ref POINT point)
    {
        return ClientToScreen(new HandleRef(this, mainWindowHandle), ref point);
    }

    public IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (GetForegroundWindow() == mainWindowHandle)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                var vkCode = Marshal.ReadInt32(lParam);
                OnKeyPressed?.Invoke(this, (Keys)vkCode);
            }
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }
}