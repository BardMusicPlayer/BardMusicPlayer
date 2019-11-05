using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using static Sharlayan.Core.Enums.Performance;

// Key/Keybind - the actual key to simulate
// PerfKey/pk - PERFORMANCE_MODE_ key to get the keybind
// NoteKey/nk/byte - C+1, C#, etc
// byte key - the raw midi note byte

namespace FFBardMusicPlayer {
	public class FFXIVHotbarDat : FFXIVDatFile {

		#region Hotbar classes
		public class HotbarSection {
			public byte action = 0; // Higher level? 0D for 60-70 spells
			public byte flag = 0;
			public byte unk1 = 0;
			public byte unk2 = 0;
			public byte job = 0;
			public byte hotbar = 0;
			public byte slot = 0;
			public byte type = 0;
		};


		public class HotbarSlots : Dictionary<int, HotbarJobSlot> { }
		public class HotbarSlot : HotbarSection {

			public int Hotbar {
				get { return (hotbar + 1); }
			}
			public int Slot {
				get {
					int fslot = (slot + 1);

					int ss = (fslot % 10);
					if(fslot > 10) {
						ss += (fslot / 10 * 10) - 1;
					}
					return ss;
				}
			}
			public override string ToString() {
				return string.Format("HOTBAR_{0}_{1:X}", Hotbar, Slot);
			}
		}

		public class HotbarJobSlots : Dictionary<int, HotbarSlot> { }
		public class HotbarJobSlot {
			public HotbarJobSlots jobSlots = new HotbarJobSlots();
			public HotbarSlot this[int i] {
				get {
					if(!jobSlots.ContainsKey(i)) {
						jobSlots[i] = new HotbarSlot();
					}
					return jobSlots[i];
				}
				set {
					jobSlots[i] = value;
				}
			}
		}

		public class HotbarRows : Dictionary<int, HotbarRow> { }
		public class HotbarRow {
			public HotbarSlots slots = new HotbarSlots();
			public HotbarJobSlot this[int i] {
				get {
					if(!slots.ContainsKey(i)) {
						slots[i] = new HotbarJobSlot();
					}
					return slots[i];
				}
				set {
					slots[i] = value;
				}
			}
		}

		public class HotbarData {
			public HotbarRows rows = new HotbarRows();
			public HotbarRow this[int i] {
				get {
					if(!rows.ContainsKey(i)) {
						rows[i] = new HotbarRow();
					}
					return rows[i];
				}
				set {
					rows[i] = value;
				}
			}

			public void Clear() {
				rows.Clear();
			}
		}

		#endregion

		HotbarData hotbarData = new HotbarData();

		public void LoadHotbarDat(string charId) {
            String fileToLoad = Program.programOptions.DatPrefix + "HOTBAR.DAT";
			LoadDatId(charId, fileToLoad);
		}

		public List<HotbarSlot> GetSlotsFromType(int type) {

			List<HotbarSlot> slots = new List<HotbarSlot>();
			foreach(HotbarRow row in hotbarData.rows.Values) {
				foreach(HotbarJobSlot jobSlot in row.slots.Values) {
					foreach(HotbarSlot slot in jobSlot.jobSlots.Values) {
						if(slot.type == type) {
							slots.Add(slot);
						}
					}
				}
			}
			return slots;
		}
		
		public string GetHotbarSlotKeyMap(int hnum, int snum, int jnum) {
			if(hotbarData.rows.ContainsKey(hnum)) {
				HotbarRow row = hotbarData[hnum];
				if(row.slots.ContainsKey(snum)) {
					HotbarSlot slot = row[snum][jnum];
					return slot.ToString();
				}
			}
			return null;
		}

		public string GetInstrumentKeyMap(Instrument ins) {

			List<FFXIVHotbarDat.HotbarSlot> slots = this.GetSlotsFromType(0x1D);
			foreach(FFXIVHotbarDat.HotbarSlot slot in slots) {
				if(slot.action == (int) ins) {
					return slot.ToString();
				}
			}

			return string.Empty;
		}

		protected override bool ParseDat(BinaryReader stream) {

			hotbarData.Clear();
			if(base.ParseDat(stream)) {

				stream.BaseStream.Seek(0x10, SeekOrigin.Begin);
				while(stream.BaseStream.Position < header.dataSize) {
					HotbarSlot ac = ParseSection(stream);
					if(ac.job <= 0x23) {
						if(ac.type == 0x1D) {
							//Console.WriteLine(string.Format("{0} ({1}): {2} {3}", ac.ToString(), ac.job, ac.action, ac.type));
						}
						hotbarData[ac.hotbar][ac.slot][ac.job] = ac;
					}
				}
			}
			return true;
		}
		private HotbarSlot ParseSection(BinaryReader stream) {
			byte xor = 0x31;
			HotbarSlot ac = new HotbarSlot {
				action = XorTools.ReadXorByte(stream, xor),
				flag = XorTools.ReadXorByte(stream, xor),
				unk1 = XorTools.ReadXorByte(stream, xor),
				unk2 = XorTools.ReadXorByte(stream, xor),
				job = XorTools.ReadXorByte(stream, xor),
				hotbar = XorTools.ReadXorByte(stream, xor),
				slot = XorTools.ReadXorByte(stream, xor),
				type = XorTools.ReadXorByte(stream, xor),
			};
			return ac;
		}
	}
}