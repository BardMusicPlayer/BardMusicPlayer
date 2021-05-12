/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan
{
    internal sealed class Scanner
	{
        public MemoryHandler MemoryHandler { get; set; }

        public bool IsScanning { get; set; }

		public Dictionary<string, Signature> Locations { get; set; } = new();

        public void LoadOffsets(IEnumerable<Signature> signatures)
		{
			if (MemoryHandler.ProcessModel?.Process == null)
			{
				return;
			}
			IsScanning = true;
			Func<bool> func = delegate
			{
				var stopwatch = new Stopwatch();
				stopwatch.Start();
				var list = (signatures as List<Signature>) ?? signatures.ToList();
				if (list.Any())
				{
					foreach (var item in list)
					{
						if (item.Value == string.Empty)
						{
							Locations[item.Key] = item;
						}
						else
						{
							item.Value = item.Value.Replace("*", "?");
						}
					}
					list.RemoveAll(a => Locations.ContainsKey(a.Key));
					FindExtendedSignatures(list);
				}
				stopwatch.Stop();
                MemoryHandler.RaiseSignaturesFound(Locations, stopwatch.ElapsedMilliseconds);
				IsScanning = false;
				return true;
			};
			func.BeginInvoke(delegate
			{
			}, func);
		}

		private void FindExtendedSignatures(IEnumerable<Signature> signatures)
		{
			var notFound = new List<Signature>(signatures);
			var baseAddress = MemoryHandler.ProcessModel.Process.MainModule.BaseAddress;
			var searchEnd = IntPtr.Add(baseAddress, MemoryHandler.ProcessModel.Process.MainModule.ModuleMemorySize);
			var searchStart = baseAddress;
			ResolveLocations(baseAddress, searchStart, searchEnd, ref notFound);
        }

		private void ResolveLocations(IntPtr baseAddress, IntPtr searchStart, IntPtr searchEnd, ref List<Signature> notFound)
		{
			var array = new byte[4608];
			var list = new List<Signature>();
			var num = 0;
			while (searchStart.ToInt64() < searchEnd.ToInt64())
			{
				try
				{
					var regionSize = new IntPtr(4608);
					if (IntPtr.Add(searchStart, 4608).ToInt64() > searchEnd.ToInt64())
					{
						regionSize = (IntPtr)(searchEnd.ToInt64() - searchStart.ToInt64());
					}
					if (UnsafeNativeMethods.ReadProcessMemory(MemoryHandler.ProcessHandle, searchStart, array, regionSize, out var _))
					{
						foreach (var item in notFound)
						{
							var num2 = FindSuperSignature(array, SignatureToByte(item.Value, 63));
							if (num2 < 0)
							{
								list.Add(item);
								continue;
							}
							var pointer = new IntPtr((long)(baseAddress + num * 4096));
							item.SigScanAddress = new IntPtr(IntPtr.Add(pointer, num2 + item.Offset).ToInt64());
							if (!Locations.ContainsKey(item.Key))
							{
								Locations.Add(item.Key, item);
							}
						}
						notFound = new List<Signature>(list);
						list.Clear();
					}
					num++;
					searchStart = IntPtr.Add(searchStart, 4096);
				}
				catch (Exception ex)
				{
                    MemoryHandler?.RaiseException(ex);
				}
			}
		}

        private static int FindSuperSignature(IReadOnlyList<byte> buffer, IReadOnlyList<byte> pattern) {
            if (pattern.Count > buffer.Count)
                return -1;
            var badShift = BuildBadCharTable(pattern);
            var offset = 0;
            var last = pattern.Count - 1;
            var maxoffset = buffer.Count - pattern.Count;

            while (offset <= maxoffset) {
                int position;
                for (position = last; pattern[position] == buffer[position + offset] || pattern[position] == 63; position--) {
                    if (position == 0)
                        return offset;
                }
                offset += badShift[buffer[offset + last]];
            }

            return -1;
        }

        private static int[] BuildBadCharTable(IReadOnlyList<byte> pattern) {
            int idx;
            var last = pattern.Count - 1;
            var badShift = new int[256];
            for (idx = last; idx > 0 && pattern[idx] != 63; --idx) { }
            var diff = last - idx;
            if (diff == 0) diff = 1;
            for (idx = 0; idx <= 255; ++idx)
                badShift[idx] = diff;
            for (idx = last - diff; idx < last; ++idx)
                badShift[pattern[idx]] = last - idx;
            return badShift;
        }

		private static byte[] SignatureToByte(string signature, byte wildcard)
		{
			var array = new byte[signature.Length / 2];
			var array2 = new[]
			{
				0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
				0, 0, 0, 0, 0, 0, 0, 10, 11, 12,
				13, 14, 15
			};
			try
			{
				var num = 0;
				var num2 = 0;
				while (num2 < signature.Length)
				{
					if (signature[num2] == wildcard)
					{
						array[num] = wildcard;
					}
					else
					{
						array[num] = (byte)((array2[char.ToUpper(signature[num2]) - 48] << 4) | array2[char.ToUpper(signature[num2 + 1]) - 48]);
					}
					num2 += 2;
					num++;
				}
				return array;
			}
			catch
			{
				return null;
			}
		}
	}
}
