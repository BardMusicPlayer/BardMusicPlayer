/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BardMusicPlayer.Quotidian.Enums;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Events;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models.Structures;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Utilities;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan
{
    internal class MemoryHandler
	{
        private bool _isNewInstance = true;

		internal Scanner Scanner { get; }

        internal Reader.Reader Reader { get; set; }

        internal bool IsAttached { get; set; }
		
		internal GameRegion GameRegion { get; }

		internal IntPtr ProcessHandle { get; set; }

		internal ProcessModel ProcessModel { get; set; }

		internal StructuresContainer Structures { get; set; }
		
		private List<ProcessModule> SystemModules { get; } = new();

        public event EventHandler<ExceptionEvent> ExceptionEvent = delegate
		{
		};

		public event EventHandler<SignaturesFoundEvent> SignaturesFoundEvent = delegate
		{
		};

		~MemoryHandler()
		{
			UnsetProcess();
		}

		public MemoryHandler(Scanner scanner, GameRegion gameRegion)
        {
            GameRegion = gameRegion;
			Scanner = scanner;
            Scanner.MemoryHandler = this;
        }

		public byte GetByte(IntPtr address, long offset = 0L)
		{
			var array = new byte[1];
			Peek(new IntPtr(address.ToInt64() + offset), array);
			return array[0];
		}

		public byte[] GetByteArray(IntPtr address, int length)
        {
            if (length < 1) return new byte[0];
            var array = new byte[length];
            Peek(address, array);
            return array;
		}

		public short GetInt16(IntPtr address, long offset = 0L)
		{
			var array = new byte[2];
			Peek(new IntPtr(address.ToInt64() + offset), array);
			return SBitConverter.TryToInt16(array, 0);
		}

		public int GetInt32(IntPtr address, long offset = 0L)
		{
			var array = new byte[4];
			Peek(new IntPtr(address.ToInt64() + offset), array);
			return SBitConverter.TryToInt32(array, 0);
		}

		public long GetInt64(IntPtr address, long offset = 0L)
		{
			var array = new byte[8];
			Peek(new IntPtr(address.ToInt64() + offset), array);
			return SBitConverter.TryToInt64(array, 0);
		}

		public long GetPlatformInt(IntPtr address, long offset = 0L)
		{
			var array = new byte[8];
			Peek(new IntPtr(address.ToInt64() + offset), array);
			return GetPlatformIntFromBytes(array);
		}

		public long GetPlatformIntFromBytes(byte[] source, int index = 0) => SBitConverter.TryToInt64(source, index);

		public long GetPlatformUInt(IntPtr address, long offset = 0L)
		{
			var array = new byte[8];
			Peek(new IntPtr(address.ToInt64() + offset), array);
			return GetPlatformUIntFromBytes(array);
		}

		public long GetPlatformUIntFromBytes(byte[] source, int index = 0) => (long) SBitConverter.TryToUInt64(source, index);
		public IntPtr GetStaticAddress(long offset) => new(ProcessModel.Process.MainModule.BaseAddress.ToInt64() + offset);

        public string GetString(IntPtr address, long offset = 0L, int size = 256)
		{
			var array = new byte[size];
			Peek(new IntPtr(address.ToInt64() + offset), array);
			var newSize = 0;
			for (var i = 0; i < size; i++)
            {
                if (array[i] != 0) continue;
                newSize = i;
                break;
            }
			Array.Resize(ref array, newSize);
			return Encoding.UTF8.GetString(array);
		}

		public string GetStringFromBytes(byte[] source, int offset = 0, int size = 256)
		{
			var num = source.Length - offset;
			if (num < size) size = num;
            var array = new byte[size];
			Array.Copy(source, offset, array, 0, size);
			var newSize = 0;
			for (var i = 0; i < size; i++)
            {
                if (array[i] != 0) continue;
                newSize = i;
                break;
            }
			Array.Resize(ref array, newSize);
			return Encoding.UTF8.GetString(array);
		}

		public T GetStructure<T>(IntPtr address, int offset = 0)
		{
			var intPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(T)));
			UnsafeNativeMethods.ReadProcessMemory(ProcessModel.Process.Handle, address + offset, intPtr, new IntPtr(Marshal.SizeOf(typeof(T))), out var _);
			var result = (T)Marshal.PtrToStructure(intPtr, typeof(T));
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		public ushort GetUInt16(IntPtr address, long offset = 0L)
		{
			var array = new byte[4];
			Peek(new IntPtr(address.ToInt64() + offset), array);
			return SBitConverter.TryToUInt16(array, 0);
		}

		public uint GetUInt32(IntPtr address, long offset = 0L)
		{
			var array = new byte[4];
			Peek(new IntPtr(address.ToInt64() + offset), array);
			return SBitConverter.TryToUInt32(array, 0);
		}

		public ulong GetUInt64(IntPtr address, long offset = 0L)
		{
			var array = new byte[8];
			Peek(new IntPtr(address.ToInt64() + offset), array);
			return SBitConverter.TryToUInt32(array, 0);
		}

		public bool Peek(IntPtr address, byte[] buffer) => UnsafeNativeMethods.ReadProcessMemory(ProcessHandle, address, buffer, new IntPtr(buffer.Length), out _);

        public IntPtr ReadPointer(IntPtr address, long offset = 0L)
        {
            var array = new byte[8];
            Peek(new IntPtr(address.ToInt64() + offset), array);
            return new IntPtr(SBitConverter.TryToInt64(array, 0));
        }

        public IntPtr ResolvePointerPath(IEnumerable<long> path, IntPtr baseAddress, bool IsASMSignature = false)
		{
			var intPtr = baseAddress;
			foreach (var item in path)
			{
				try
				{
					baseAddress = new IntPtr(intPtr.ToInt64() + item);
					if (baseAddress == IntPtr.Zero)
					{
						return IntPtr.Zero;
					}
					if (IsASMSignature)
					{
						intPtr = baseAddress + GetInt32(new IntPtr(baseAddress.ToInt64()), 0L) + 4;
						IsASMSignature = false;
					}
					else
					{
						intPtr = ReadPointer(baseAddress, 0L);
					}
				}
				catch
				{
					return IntPtr.Zero;
				}
			}
			return baseAddress;
		}

		public void SetProcess(ProcessModel processModel)
		{
			ProcessModel = processModel;

			UnsetProcess();
			try
			{
				ProcessHandle = UnsafeNativeMethods.OpenProcess(UnsafeNativeMethods.ProcessAccessFlags.PROCESS_VM_ALL, false, (uint)ProcessModel.ProcessID);
			}
			catch (Exception)
			{
				ProcessHandle = processModel.Process.Handle;
			}
			finally
			{
				IsAttached = true;
			}
			if (_isNewInstance)
			{
				_isNewInstance = false;
				ResolveMemoryStructures();
			}
			SystemModules.Clear();
			GetProcessModules();
			Scanner.Locations.Clear();
			Scanner.LoadOffsets(Signatures.Resolve(this));
		}

		public void UnsetProcess()
		{
			try
			{
				if (IsAttached) UnsafeNativeMethods.CloseHandle(ProcessHandle);
            }
			catch (Exception)
			{
				// Ignored
			}
			finally
			{
                ProcessHandle = IntPtr.Zero;
				IsAttached = false;
			}
		}

		internal ProcessModule GetModuleByAddress(IntPtr address)
		{
			try
            {
                return (from processModule in SystemModules let num = processModule.BaseAddress.ToInt64() where num <= (long) address && num + processModule.ModuleMemorySize >= (long) address select processModule).FirstOrDefault();
            }
			catch (Exception)
			{
				return null;
			}
		}

        internal void ResolveMemoryStructures()
		{
			Structures = new APIHelper(this).GetStructures();
		}

		protected internal virtual void RaiseException(Exception ex)
		{
			ExceptionEvent?.Invoke(this, new ExceptionEvent(this, ex));
		}

		protected internal virtual void RaiseSignaturesFound(Dictionary<string, Signature> signatures, long processingTime)
		{
			SignaturesFoundEvent?.Invoke(this, new SignaturesFoundEvent(this, signatures, processingTime));
		}

		private void GetProcessModules()
		{
			var modules = ProcessModel.Process.Modules;
			for (var i = 0; i < modules.Count; i++)
			{
				var item = modules[i];
				SystemModules.Add(item);
			}
		}
	}
}
