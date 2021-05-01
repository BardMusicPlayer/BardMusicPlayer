/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System;
using System.Runtime.InteropServices;
using System.Text;
// ReSharper disable InconsistentNaming

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan
{
	internal static class UnsafeNativeMethods
	{
		public enum ProcessAccessFlags
		{
			PROCESS_VM_ALL = 2035711,
			PROCESS_VM_READ = 0x10
		}

		public struct MEMORY_BASIC_INFORMATION
		{
			public IntPtr BaseAddress;

			public IntPtr AllocationBase;

			public uint AllocationProtect;

			public IntPtr RegionSize;

			public uint State;

			public uint Protect;

			public uint Type;

			public override string ToString()
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat($"BaseAddress:{BaseAddress}{Environment.NewLine}");
				stringBuilder.AppendFormat($"AllocationBase:{AllocationBase}{Environment.NewLine}");
				stringBuilder.AppendFormat($"AllocationProtect:{AllocationProtect}{Environment.NewLine}");
				stringBuilder.AppendFormat($"RegionSize:{RegionSize}{Environment.NewLine}");
				stringBuilder.AppendFormat($"State:{State}{Environment.NewLine}");
				stringBuilder.AppendFormat($"Protect:{Protect}{Environment.NewLine}");
				stringBuilder.AppendFormat($"Type:{Type}{Environment.NewLine}");
				return stringBuilder.ToString();
			}
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr lpBaseAddress, [In][Out] byte[] lpBuffer, IntPtr regionSize, out IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr lpBaseAddress, [In][Out] IntPtr lpBuffer, IntPtr regionSize, out IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll")]
		public static extern int VirtualQueryEx(IntPtr processHandle, IntPtr lpBaseAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);
	}
}
