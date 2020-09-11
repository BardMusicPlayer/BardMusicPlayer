using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMemoryParser {
	public class HookProcess {
		private Process process;
        private IntPtr handle;
        
        // Make sure any memory locations figured out isn't in a system module and cause destruction
        private List<ProcessModule> SystemModules { get; set; } = new List<ProcessModule>();

        public IntPtr Handle {
			get { return handle; }
		}

		public IntPtr BaseAddress {
			get { return process.MainModule.BaseAddress; }
		}
		public IntPtr EndAddress {
			get { return IntPtr.Add(this.BaseAddress, process.MainModule.ModuleMemorySize); }
		}

		public ProcessModuleCollection Modules {
			get { return process.Modules; }
		}

		public HookProcess(Process proc) {
			process = proc;

            try {
                var flag = UnsafeNativeMethods.ProcessAccessFlags.PROCESS_VM_ALL;
                handle = UnsafeNativeMethods.OpenProcess(flag, false, (uint)process.Id);
            } catch (Exception) {
                handle = process.Handle;
            }

            this.GetProcessModules();
		}

        // SystemModule stuff
        internal ProcessModule GetModuleByAddress(IntPtr address) {
            try {
                for (var i = 0; i < this.SystemModules.Count; i++) {
                    ProcessModule module = this.SystemModules[i];
                    var baseAddress = module.BaseAddress.ToInt64();
                    if (baseAddress <= (long)address && baseAddress + module.ModuleMemorySize >= (long)address) {
                        return module;
                    }
                }

                return null;
            } catch (Exception) {
                return null;
            }
        }
        internal bool IsSystemModule(IntPtr address) {
            ProcessModule moduleByAddress = this.GetModuleByAddress(address);
            if (moduleByAddress != null) {
                foreach (ProcessModule module in this.SystemModules) {
                    if (module.ModuleName == moduleByAddress.ModuleName) {
                        return true;
                    }
                }
            }
            return false;
        }

        private void GetProcessModules() {
            ProcessModuleCollection modules = this.Modules;
            for (var i = 0; i < modules.Count; i++) {
                ProcessModule module = modules[i];
                this.SystemModules.Add(module);
            }
        }

        // Read memory
        public bool Peek(IntPtr address, byte[] buffer) {
            IntPtr lpNumberOfBytesRead;
            return UnsafeNativeMethods.ReadProcessMemory(this.handle, address, buffer,
                new IntPtr(buffer.Length), out lpNumberOfBytesRead);
        }
        public string GetStringFromBytes(byte[] source, int offset = 0, int size = 256) {
            var safeSize = source.Length - offset;
            if (safeSize < size) {
                size = safeSize;
            }

            byte[] bytes = new byte[size];
            Array.Copy(source, offset, bytes, 0, size);
            var realSize = 0;
            for (var i = 0; i < size; i++) {
                if (bytes[i] != 0) {
                    continue;
                }

                realSize = i;
                break;
            }

            Array.Resize(ref bytes, realSize);
            return Encoding.UTF8.GetString(bytes);
        }
        public string GetString(IntPtr address, long offset = 0, int size = 256) {
            byte[] bytes = new byte[size];
            this.Peek(new IntPtr(address.ToInt64() + offset), bytes);
            var realSize = 0;
            for (var i = 0; i < size; i++) {
                if (bytes[i] != 0) {
                    continue;
                }

                realSize = i;
                break;
            }

            Array.Resize(ref bytes, realSize);
            return Encoding.UTF8.GetString(bytes);
        }

        // Only 64 bit
        public IntPtr ReadPointer(IntPtr address, long offset = 0) {
            byte[] win64 = new byte[8];
            this.Peek(new IntPtr(address.ToInt64() + offset), win64);
            return new IntPtr(BitConverter.TryToInt64(win64, 0));
        }
        public int GetInt32(IntPtr address, long offset = 0) {
            byte[] value = new byte[4];
            this.Peek(new IntPtr(address.ToInt64() + offset), value);
            return BitConverter.TryToInt32(value, 0);
        }

        public byte GetByte(IntPtr address, long offset = 0) {
            byte[] data = new byte[1];
            this.Peek(new IntPtr(address.ToInt64() + offset), data);
            return data[0];
        }

        public byte[] GetByteArray(IntPtr address, int length) {
            byte[] data = new byte[length];
            this.Peek(address, data);
            return data;
        }

    }
}
