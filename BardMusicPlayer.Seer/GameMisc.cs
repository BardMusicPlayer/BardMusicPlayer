/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace BardMusicPlayer.Seer;

#region Enums
public enum HandleType
{
    Unknown = 0,
    Other = 1,
    Mutant = 15,
}

public enum OBJECT_INFORMATION_CLASS
{
    ObjectBasicInformation,
    ObjectNameInformation,
    ObjectTypeInformation,
    ObjectAllTypesInformation,
    ObjectHandleInformation,
}

public enum SYSTEM_INFORMATION_CLASS
{
    SystemBasicInformation = 0,
    SystemPerformanceInformation = 2,
    SystemTimeOfDayInformation = 3,
    SystemProcessInformation = 5,
    SystemProcessorPerformanceInformation = 8,
    SystemHandleInformation = 16,        // 0x00000010
    SystemInterruptInformation = 23,     // 0x00000017
    SystemExceptionInformation = 33,     // 0x00000021
    SystemRegistryQuotaInformation = 37, // 0x00000025
    SystemLookasideInformation = 45,     // 0x0000002D
}

public enum NT_STATUS
{
    STATUS_BUFFER_OVERFLOW = -2147483643,      // 0x80000005
    STATUS_INFO_LENGTH_MISMATCH = -1073741820, // 0xC0000004
    STATUS_SUCCESS = 0,
}

public enum SpecialWindowHandles
{
    HWND_TOP = 0,
    HWND_BOTTOM = 1,
    HWND_TOPMOST = -1,
    HWND_NOTOPMOST = -2
}

[Flags]
public enum WindowStyles : uint
{
    WS_BORDER = 0x800000,
    WS_CAPTION = 0xc00000,
    WS_CHILD = 0x40000000,
    WS_CLIPCHILDREN = 0x2000000,
    WS_CLIPSIBLINGS = 0x4000000,
    WS_DISABLED = 0x8000000,
    WS_DLGFRAME = 0x400000,
    WS_GROUP = 0x20000,
    WS_HSCROLL = 0x100000,
    WS_MAXIMIZE = 0x1000000,
    WS_MAXIMIZEBOX = 0x10000,
    WS_MINIMIZE = 0x20000000,
    WS_MINIMIZEBOX = 0x20000,
    WS_OVERLAPPED = 0x0,
    WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
    WS_POPUP = 0x80000000u,
    WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
    WS_SIZEFRAME = 0x40000,
    WS_SYSMENU = 0x80000,
    WS_TABSTOP = 0x10000,
    WS_VISIBLE = 0x10000000,
    WS_VSCROLL = 0x200000
}

[Flags]
public enum SetWindowPosFlags : uint
{
    SWP_ASYNCWINDOWPOS = 0x4000,
    SWP_DEFERERASE = 0x2000,
    SWP_DRAWFRAME = 0x0020,
    SWP_FRAMECHANGED = 0x0020,
    SWP_HIDEWINDOW = 0x0080,
    SWP_NOACTIVATE = 0x0010,
    SWP_NOCOPYBITS = 0x0100,
    SWP_NOMOVE = 0x0002,
    SWP_NOOWNERZORDER = 0x0200,
    SWP_NOREDRAW = 0x0008,
    SWP_NOREPOSITION = 0x0200,
    SWP_NOSENDCHANGING = 0x0400,
    SWP_NOSIZE = 0x0001,
    SWP_NOZORDER = 0x0004,
    SWP_SHOWWINDOW = 0x0040
}

public enum GWL
{
    GWL_WNDPROC = (-4),
    GWL_HINSTANCE = (-6),
    GWL_HWNDPARENT = (-8),
    GWL_STYLE = (-16),
    GWL_EXSTYLE = (-20),
    GWL_USERDATA = (-21),
    GWL_ID = (-12)
}
#endregion

#region Structs
[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left, Top, Right, Bottom;

    public RECT(int left, int top, int right, int bottom)
    {
        Left   = left;
        Top    = top;
        Right  = right;
        Bottom = bottom;
    }

    public RECT(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

    public int X
    {
        get { return Left; }
        set { Right -= (Left - value); Left = value; }
    }

    public int Y
    {
        get { return Top; }
        set { Bottom -= (Top - value); Top = value; }
    }

    public int Height
    {
        get { return Bottom - Top; }
        set { Bottom = value + Top; }
    }

    public int Width
    {
        get { return Right - Left; }
        set { Right = value + Left; }
    }

    public Point Location
    {
        get { return new Point(Left, Top); }
        set { X = value.X; Y = value.Y; }
    }

    public Size Size
    {
        get { return new Size(Width, Height); }
        set { Width = value.Width; Height = value.Height; }
    }

    public static implicit operator Rectangle(RECT r)
    {
        return new Rectangle(r.Left, r.Top, r.Width, r.Height);
    }

    public static implicit operator RECT(Rectangle r)
    {
        return new RECT(r);
    }

    public static bool operator ==(RECT r1, RECT r2)
    {
        return r1.Equals(r2);
    }

    public static bool operator !=(RECT r1, RECT r2)
    {
        return !r1.Equals(r2);
    }

    public bool Equals(RECT r)
    {
        return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
    }

    public override bool Equals(object obj)
    {
        if (obj is RECT)
            return Equals((RECT)obj);
        else if (obj is Rectangle)
            return Equals(new RECT((Rectangle)obj));
        return false;
    }

    public override int GetHashCode()
    {
        return ((Rectangle)this).GetHashCode();
    }

    public override string ToString()
    {
        return string.Format(CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
    }
}
#endregion

public struct SystemHandleEntry
{
    public int OwnerProcessId;
    public byte ObjectTypeNumber;
    public byte Flags;
    public ushort Handle;
    public IntPtr Object;
    public int GrantedAccess;
}

public static class SystemHelper
{
    [DllImport("ntdll.dll")]
    public static extern NT_STATUS NtQueryObject(
        [In] IntPtr Handle,
        [In] OBJECT_INFORMATION_CLASS ObjectInformationClass,
        [In] IntPtr ObjectInformation,
        [In] int ObjectInformationLength,
        out int ReturnLength);

    [DllImport("ntdll.dll")]
    public static extern NT_STATUS NtQuerySystemInformation(
        [In] SYSTEM_INFORMATION_CLASS SystemInformationClass,
        [In] IntPtr SystemInformation,
        [In] int SystemInformationLength,
        out int ReturnLength);

    //[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle([In] IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DuplicateHandle(
        [In] IntPtr hSourceProcessHandle,
        [In] IntPtr hSourceHandle,
        [In] IntPtr hTargetProcessHandle,
        out IntPtr lpTargetHandle,
        [In] int dwDesiredAccess,
        [MarshalAs(UnmanagedType.Bool), In] bool bInheritHandle,
        [In] int dwOptions);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DuplicateHandle(
        [In] IntPtr hSourceProcessHandle,
        [In] IntPtr hSourceHandle,
        [In] IntPtr hTargetProcessHandle,
        [Out] IntPtr lpTargetHandle,
        [In] int dwDesiredAccess,
        [MarshalAs(UnmanagedType.Bool), In] bool bInheritHandle,
        [In] int dwOptions);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr OpenProcess(
        [In] int dwDesiredAccess,
        [MarshalAs(UnmanagedType.Bool), In] bool bInheritHandle,
        [In] int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint QueryDosDevice(
        string lpDeviceName,
        StringBuilder lpTargetPath,
        int ucchMax);

    [DllImport("user32.dll")]
    public static extern int SetWindowText(IntPtr hWnd, string text);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

    [DllImport("user32.dll")]
    public static extern bool AdjustWindowRect(ref RECT lpRect, uint dwStyle, bool bMenu);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, UInt32 uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int SendMessageA(IntPtr hWnd, int wMsg, int wParam, int lParam);

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

}

public class HandleInfo
{
    private static Dictionary<byte, string> _rawTypeMap = new Dictionary<byte, string>();
    private string _name;
    private string _typeStr;
    private HandleType _type;
    private bool _typeAndNameAttempted;

    public int ProcessId { get; private set; }

    public ushort Handle { get; private set; }

    public int GrantedAccess { get; private set; }

    public byte RawType { get; private set; }

    public HandleInfo(int processId, ushort handle, int grantedAccess, byte rawType)
    {
        ProcessId     = processId;
        Handle        = handle;
        GrantedAccess = grantedAccess;
        RawType       = rawType;
    }

    public string Name
    {
        get
        {
            if (_name == null)
                initTypeAndName();
            return _name;
        }
    }

    public HandleType Type
    {
        get
        {
            if (_typeStr == null)
                initType();
            return _type;
        }
    }

    public void CloseRemote() => SystemHelper.DuplicateHandle(SystemHelper.OpenProcess(64, true, ProcessId), (IntPtr)(int)Handle, IntPtr.Zero, IntPtr.Zero, 0, false, 1);

    private void initType()
    {
        if (_rawTypeMap.ContainsKey(RawType))
        {
            _typeStr = _rawTypeMap[RawType];
            _type    = HandleTypeFromString(_typeStr);
        }
        else
            initTypeAndName();
    }

    private void initTypeAndName()
    {
        if (_typeAndNameAttempted)
            return;
        _typeAndNameAttempted = true;
        IntPtr ProcPtr = IntPtr.Zero;
        IntPtr lpTargetHandle = IntPtr.Zero;
        try
        {
            ProcPtr = SystemHelper.OpenProcess(64, true, ProcessId);
            if (!SystemHelper.DuplicateHandle(ProcPtr, (IntPtr)(int)Handle, SystemHelper.GetCurrentProcess(), out lpTargetHandle, 0, false, 2))
                return;
            if (_rawTypeMap.ContainsKey(RawType))
            {
                _typeStr = _rawTypeMap[RawType];
            }
            else
            {
                int ReturnLength;
                int QueryStatus = (int)SystemHelper.NtQueryObject(lpTargetHandle, OBJECT_INFORMATION_CLASS.ObjectTypeInformation, IntPtr.Zero, 0, out ReturnLength);
                IntPtr AllocMemPtr = IntPtr.Zero;
                try
                {
                    AllocMemPtr = Marshal.AllocHGlobal(ReturnLength);
                    if (SystemHelper.NtQueryObject(lpTargetHandle, OBJECT_INFORMATION_CLASS.ObjectTypeInformation, AllocMemPtr, ReturnLength, out ReturnLength) != NT_STATUS.STATUS_SUCCESS)
                        return;
                    _typeStr = Marshal.PtrToStringUni((IntPtr)((long)AllocMemPtr + 88L + 2 * IntPtr.Size));
                    lock (_rawTypeMap)
                        _rawTypeMap[RawType] = _typeStr;
                }
                finally
                {
                    Marshal.FreeHGlobal(AllocMemPtr);
                }
            }
            _type = HandleTypeFromString(_typeStr);
            if (_typeStr == null || GrantedAccess == 1180063 || GrantedAccess == 1180041 || GrantedAccess == 1179785)
                return;
            int ReturnLength1;
            int num4 = (int)SystemHelper.NtQueryObject(lpTargetHandle, OBJECT_INFORMATION_CLASS.ObjectNameInformation, IntPtr.Zero, 0, out ReturnLength1);
            IntPtr num5 = IntPtr.Zero;
            try
            {
                num5 = Marshal.AllocHGlobal(ReturnLength1);
                if (SystemHelper.NtQueryObject(lpTargetHandle, OBJECT_INFORMATION_CLASS.ObjectNameInformation, num5, ReturnLength1, out ReturnLength1) != NT_STATUS.STATUS_SUCCESS)
                    return;
                _name = Marshal.PtrToStringUni(num5 + (2 * IntPtr.Size));
                if (!(_typeStr == "File") && !(_typeStr == "Directory"))
                    return;
                _name = GetRegularFileNameFromDevice(_name);
            }
            finally
            {
                Marshal.FreeHGlobal(num5);
            }
        }
        finally
        {
            SystemHelper.CloseHandle(ProcPtr);
            if (lpTargetHandle != IntPtr.Zero)
                SystemHelper.CloseHandle(lpTargetHandle);
        }
    }

    private static string GetRegularFileNameFromDevice(string strRawName)
    {
        string fileNameFromDevice = strRawName;
        foreach (string logicalDrive in Environment.GetLogicalDrives())
        {
            StringBuilder lpTargetPath = new StringBuilder(260);
            if (SystemHelper.QueryDosDevice(logicalDrive.Substring(0, 2), lpTargetPath, 260) == 0U)
                return strRawName;
            string oldValue = lpTargetPath.ToString();
            if (fileNameFromDevice.StartsWith(oldValue))
            {
                fileNameFromDevice = fileNameFromDevice.Replace(oldValue, logicalDrive.Substring(0, 2));
                break;
            }
        }
        return fileNameFromDevice;
    }

    public static HandleType HandleTypeFromString(string typeStr)
    {
        switch (typeStr)
        {
            case "Mutant":
                return HandleType.Mutant;
            case null:
                return HandleType.Unknown;
            default:
                return HandleType.Other;
        }
    }
}

public static class MutantHandler
{
    private static readonly HashSet<string> ValidMutexs = new HashSet<string>()
    {
        "\\BaseNamedObjects\\6AA83AB5-BAC4-4a36-9F66-A309770760CB_ffxiv_game00",
        "\\BaseNamedObjects\\6AA83AB5-BAC4-4a36-9F66-A309770760CB_ffxiv_game01"
    };

    public static HashSet<HandleInfo> GetMutants(int procid)
    {
        HashSet<HandleInfo> mutants = new HashSet<HandleInfo>();
        int SystemInformationLength = 65536;
        IntPtr SystemInformationPtr = IntPtr.Zero;
        try
        {
            bool running = true;
            while (running)
            {
                SystemInformationPtr = Marshal.AllocHGlobal(SystemInformationLength);
                switch (SystemHelper.NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS.SystemHandleInformation, SystemInformationPtr, SystemInformationLength, out int ReturnLength))
                {
                    case NT_STATUS.STATUS_INFO_LENGTH_MISMATCH:
                        SystemInformationLength = Math.Max(SystemInformationLength, ReturnLength);
                        Marshal.FreeHGlobal(SystemInformationPtr);
                        SystemInformationPtr = IntPtr.Zero;
                        continue;
                    case NT_STATUS.STATUS_SUCCESS:
                        long i = IntPtr.Size == 4 ? Marshal.ReadInt32(SystemInformationPtr) : (int)Marshal.ReadInt64(SystemInformationPtr);
                        long size = IntPtr.Size;
                        int SystemHandleEntrySize = Marshal.SizeOf(typeof(SystemHandleEntry));
                        for (int index = 0; index < i; ++index)
                        {
                            SystemHandleEntry structure = (SystemHandleEntry)Marshal.PtrToStructure((IntPtr)((long)SystemInformationPtr + size), typeof(SystemHandleEntry));
                            if (structure.OwnerProcessId == procid)
                            {
                                HandleInfo handleInfo = new HandleInfo(structure.OwnerProcessId, structure.Handle, structure.GrantedAccess, structure.ObjectTypeNumber);
                                if (handleInfo.Type == HandleType.Mutant)
                                    mutants.Add(handleInfo);
                            }
                            size += SystemHandleEntrySize;
                        }
                        running = false;
                        break;
                    default:
                        throw new Exception("Failed to retrieve system handle information.");
                }
            }
        }
        finally
        {
            if (SystemInformationPtr != IntPtr.Zero)
                Marshal.FreeHGlobal(SystemInformationPtr);
        }
        return mutants;
    }

    public static void KillMutant(Process proc)
    {
        foreach (HandleInfo mutant in GetMutants(proc.Id))
        {
            if (ValidMutexs.Contains(mutant.Name))
            {
                mutant.CloseRemote();
                break;
            }
        }
    }
}

public static class WindowSizer
{
    public static void SetWindowSize(Process proc, int width, int height, int pos_X = 0, int pos_Y = 0, bool move = false)
    {
        if (proc.Handle == IntPtr.Zero)
            return;
            
        //Resize window
        RECT clientRect = new RECT(0, 0, width, height);

        int cx = clientRect.Right - clientRect.Left;
        int cy = clientRect.Bottom - clientRect.Top;

        SystemHelper.AdjustWindowRect(ref clientRect, (uint)SystemHelper.GetWindowLongPtr(proc.MainWindowHandle, (int)GWL.GWL_STYLE), false);
        if (move)
            SystemHelper.SetWindowPos(proc.MainWindowHandle, new IntPtr((int)SpecialWindowHandles.HWND_TOP), pos_X, pos_Y, cx, cy, (SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOSENDCHANGING | SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_FRAMECHANGED));
        else
            SystemHelper.SetWindowPos(proc.MainWindowHandle, new IntPtr((int)SpecialWindowHandles.HWND_TOP), 0, 0, cx, cy, (SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOSENDCHANGING | SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_FRAMECHANGED));
        SystemHelper.SendMessageA(proc.MainWindowHandle, 0x0232, 0, 0); //WM_EXITSIZEMOVE
    }
}

public sealed partial class Game
{
    public void KillMutant()
    {
        MutantHandler.KillMutant(Process);
    }

    public void SetClientWindowName(string text)
    {
        SystemHelper.SetWindowText(Process.MainWindowHandle, text);
    }

    public void SetWindowPosAndSize(int x, int y, int sz_x, int sz_y, bool move = false)
    {
        WindowSizer.SetWindowSize(Process, sz_x, sz_y, x, y, move);
    }
}