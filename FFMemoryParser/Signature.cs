using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMemoryParser {
	public class Signature {
		public IntPtr baseAddress;

		public string Type;
		public string Key;
		private string sigValue;
		public string Value {
			get { return sigValue; }
			set {
				sigValue = value.Replace("*", "?");
			}
		}
		public List<long> PointerPath;
		public Dictionary<string, int> Offsets;

		public int Offset {
			get {
				return Value.Length / 2;
			}
		}
		public Signature() { }
		public Signature(Signature sig) {
			this.baseAddress = sig.baseAddress;
			this.Key = sig.Key;
			this.sigValue = sig.sigValue;
			this.PointerPath = sig.PointerPath;
			this.Offsets = sig.Offsets;
		}

		public virtual object GetData(HookProcess process) {
			return null;
		}

		public bool ResolvePointerPath(HookProcess process) {
			IntPtr nextAddress = baseAddress;
			bool asmStart = false;
			foreach (var offset in PointerPath) {
				try {
					baseAddress = new IntPtr(nextAddress.ToInt64() + offset);
					if (baseAddress == IntPtr.Zero) {
						return false;
					}

					if (!asmStart) {
						nextAddress = baseAddress + process.GetInt32(new IntPtr(baseAddress.ToInt64())) + 4;
						asmStart = true;
					} else {
						nextAddress = process.ReadPointer(baseAddress);
					}
				} catch {
					return false;
				}
			}
			return true;
		}
	}
}
