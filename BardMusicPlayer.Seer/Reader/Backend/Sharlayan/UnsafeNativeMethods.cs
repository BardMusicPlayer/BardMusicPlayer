/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System;
using System.Runtime.InteropServices;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan
{
	internal static class UnsafeNativeMethods
	{
		public enum ProcessAccessFlags
		{
			PROCESS_VM_ALL = 2035711,
			PROCESS_VM_READ = 0x10
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr lpBaseAddress, [In][Out] byte[] lpBuffer, IntPtr regionSize, out IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr lpBaseAddress, [In][Out] IntPtr lpBuffer, IntPtr regionSize, out IntPtr lpNumberOfBytesRead);
	}
}
