using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using BardMusicPlayer.Common.Enums;

// Key/Keybind - the actual key to simulate
// PerfKey/pk - PERFORMANCE_MODE_ key to get the keybind
// NoteKey/nk/byte - C+1, C#, etc
// byte key - the raw midi note byte

namespace BardMusicPlayer.Seer.Reader.Dat {
	public class FFXIVKeybindDat : FFXIVDatFile {

		class KeybindSection {
			public byte type;
			public Int32 size;
			public byte[] data;
		};

		private static Dictionary<int, int> mainKeyMap = new Dictionary<int, int> {
			{ 130, 187 }, // =
			{ 131, 188 }, // ,
			{ 132, 189 }, // ~
			{ 133, 190 }, // .
			{ 134, 191 }, // /
			{ 135, 186 }, // ;
			{ 136, 192 }, // '
			{ 137, 219 }, // [
			{ 138, 220 }, // \
			{ 139, 221 }, // ]
			{ 140, 222 }, // #
			{ 141, 223 }, // `
		};
		static Dictionary<string, string> oemKeyFix = new Dictionary<string, string>(){
			{ "OemQuestion", "?" },
			{ "Oemplus", "+" },
			{ "Oem5", "\\" },
			{ "OemPeriod", "." },
			{ "Oemcomma", "," },
			{ "OemMinus", "-" },
			{ "Oem8", "`" },
			{ "OemOpenBrackets", "[" },
			{ "Oem6", "]" },
			{ "Oem7", "#" },
			{ "Oemtilde", "'" },
			{ "Oem1", ";" },
		};

		public class Keybind {
			public int mainKey1 = 0;
			public int modKey1 = 0;
			public int mainKey2 = 0;
			public int modKey2 = 0;
            
			public Keys GetKey() {
				return GetKey1();
			}

			public Keys GetKey1() {
				return GetMain(mainKey1) | GetMod(modKey1);
			}
			public Keys GetKey2() {
				return GetMain(mainKey2) | GetMod(modKey2);
			}

			private Keys GetMain(int key) {
				if(key < 130) {
					return (Keys) key;
				} else if(mainKeyMap.ContainsKey(key)) {
					return (Keys) mainKeyMap[key];
				}
				return Keys.None;
			}
			private Keys GetMod(int mod) {
				Keys modKeys = Keys.None;
				if((mod & 1) != 0) {
					modKeys |= Keys.Shift;
				}
				if((mod & 2) != 0) {
					modKeys |= Keys.Control;
				}
				if((mod & 4) != 0) {
					modKeys |= Keys.Alt;
				}
				return modKeys;
			}

			public override string ToString() {
				Keys key = GetKey();
				if(key == Keys.None) {
					return string.Empty;
				} else {
                    string str = key.ToString();
					if(oemKeyFix.ContainsKey(str)) {
						str = oemKeyFix[str];
					}
					return str;
				}
			}
		}

		// keybindList contains map between PERFORMANCE_MODE_* to Keybind
		Dictionary<string, Keybind> keybindList = new Dictionary<string, Keybind>();
		public Keybind this[string key] {
			get {
				if(!keybindList.ContainsKey(key)) {
					return new Keybind();
				}
				return keybindList[key];
			}
		}

		public List<string> GetKeybindList() {
			return keybindList.Keys.ToList();
		}

		public bool ExtendedKeyboardBound {
			get {
				bool all = true;
				foreach(string kk in FFXIVKeybindDat.pianoKeyMap.Values.ToList()) {
					if(this[kk] is Keybind kb) {
						if(kb.GetKey() == Keys.None) {
							all = false;
							break;
						}
					}
				}
				return all;
			}
		}

		public static Dictionary<string, string> pianoKeyMap = new Dictionary<string, string> {
			{ "C-1", "PERFORMANCE_MODE_EX_C3" }, { "C#-1", "PERFORMANCE_MODE_EX_C3_SHARP" },
			{ "D-1", "PERFORMANCE_MODE_EX_D3" }, { "Eb-1", "PERFORMANCE_MODE_EX_D3_SHARP" },
			{ "E-1", "PERFORMANCE_MODE_EX_E3" },
			{ "F-1", "PERFORMANCE_MODE_EX_F3" }, { "F#-1", "PERFORMANCE_MODE_EX_F3_SHARP" },
			{ "G-1", "PERFORMANCE_MODE_EX_G3" }, { "G#-1", "PERFORMANCE_MODE_EX_G3_SHARP" },
			{ "A-1", "PERFORMANCE_MODE_EX_A3" }, { "Bb-1", "PERFORMANCE_MODE_EX_A3_SHARP" },
			{ "B-1", "PERFORMANCE_MODE_EX_B3" },
			{ "C", "PERFORMANCE_MODE_EX_C4" }, { "C#", "PERFORMANCE_MODE_EX_C4_SHARP" },
			{ "D", "PERFORMANCE_MODE_EX_D4" }, { "Eb", "PERFORMANCE_MODE_EX_D4_SHARP" },
			{ "E", "PERFORMANCE_MODE_EX_E4" },
			{ "F", "PERFORMANCE_MODE_EX_F4" }, { "F#", "PERFORMANCE_MODE_EX_F4_SHARP" },
			{ "G", "PERFORMANCE_MODE_EX_G4" }, { "G#", "PERFORMANCE_MODE_EX_G4_SHARP" },
			{ "A", "PERFORMANCE_MODE_EX_A4" }, { "Bb", "PERFORMANCE_MODE_EX_A4_SHARP" },
			{ "B", "PERFORMANCE_MODE_EX_B4" },
			{ "C+1", "PERFORMANCE_MODE_EX_C5" }, { "C#+1", "PERFORMANCE_MODE_EX_C5_SHARP" },
			{ "D+1", "PERFORMANCE_MODE_EX_D5" }, { "Eb+1", "PERFORMANCE_MODE_EX_D5_SHARP" },
			{ "E+1", "PERFORMANCE_MODE_EX_E5" },
			{ "F+1", "PERFORMANCE_MODE_EX_F5" }, { "F#+1", "PERFORMANCE_MODE_EX_F5_SHARP" },
			{ "G+1", "PERFORMANCE_MODE_EX_G5" }, { "G#+1", "PERFORMANCE_MODE_EX_G5_SHARP" },
			{ "A+1", "PERFORMANCE_MODE_EX_A5" }, { "Bb+1", "PERFORMANCE_MODE_EX_A5_SHARP" },
			{ "B+1", "PERFORMANCE_MODE_EX_B5" },
			{ "C+2", "PERFORMANCE_MODE_EX_C6" },
		};


		public List<Keybind> GetPerformanceKeybinds() {
			List<Keybind> keybinds = new List<Keybind>();
			foreach(string noteKey in pianoKeyMap.Keys) {
				if(GetKeybindFromNoteKey(noteKey) is Keybind keybind) {
					keybinds.Add(keybind);
				}
			}
			return keybinds;
		}

		public static string NoteByteToPerformanceKey(int note) {
			if(note >= 0 && note < pianoKeyMap.Count) {
				return pianoKeyMap.Keys.ToArray()[note];
			}
			return string.Empty;
		}

		public static string RawNoteByteToPianoKey(int note) {
			List<string> realPianoKeyMap = new List<string> {
				"C", "C#", "D", "Eb", "E", "F", "F#", "G", "G#", "A", "Bb", "B"
			};
			int fn = note + (12 * 4);
			int key = fn % 12;
			int oc = ((int) (fn / 12)) - 1;
			if(key > 0) {
				return string.Format("{0}{1}", realPianoKeyMap[key], oc);
			}
			return string.Empty;
		}

		public static string NoteKeyToPerformanceKey(string nk) {
			if(pianoKeyMap.ContainsKey(nk)) {
				return pianoKeyMap[nk];
			}
			return string.Empty;
		}

		public Keybind GetKeybindFromNoteByte(int note) {
			string pk = NoteByteToPerformanceKey(note);
			if(!string.IsNullOrEmpty(pk)) {
				if(pianoKeyMap.ContainsKey(pk)) {
					return this[pianoKeyMap[pk]];
				}
			}
			return null;
		}
		public Keybind GetKeybindFromNoteKey(string nk) {
			string pk = NoteKeyToPerformanceKey(nk);
			if(!string.IsNullOrEmpty(pk)) {
				return this[pk];
			}
			return null;
		}

		public void LoadKeybindDat(string charId)
        {
            String fileToLoad = ""; // TODO fix this path "KEYBIND.DAT";
			LoadDatId(charId, fileToLoad);
		}

		protected override bool ParseDat(BinaryReader stream) {
			keybindList.Clear();
			if(base.ParseDat(stream)) {
				stream.BaseStream.Seek(0x11, SeekOrigin.Begin);
				while(stream.BaseStream.Position < header.dataSize) {
					KeybindSection command = ParseSection(stream);
					KeybindSection keybind = ParseSection(stream);

					string key = System.Text.Encoding.UTF8.GetString(command.data);
					key = key.Substring(0, key.Length - 1); // Trim off \0
					string dat = System.Text.Encoding.UTF8.GetString(keybind.data);
					string[] datKeys = dat.Split(',');
					if(datKeys.Length == 3) {
						string[] key1 = datKeys[0].Split('.');
						string[] key2 = datKeys[1].Split('.');
						keybindList.Add(key, new Keybind {
							mainKey1 = int.Parse(key1[0], System.Globalization.NumberStyles.HexNumber),
							mainKey2 = int.Parse(key2[0], System.Globalization.NumberStyles.HexNumber),
							modKey1 = int.Parse(key1[1], System.Globalization.NumberStyles.HexNumber),
							modKey2 = int.Parse(key2[1], System.Globalization.NumberStyles.HexNumber),
						});
					}
				}
			}
			Console.WriteLine("Read " + keybindList.Count + " keys");
			return true;
		}
		private KeybindSection ParseSection(BinaryReader stream) {
			byte[] headerBytes = XorTools.ReadXorBytes(stream, 3, 0x73);
			KeybindSection section = new KeybindSection {
				type = headerBytes[0],
				size = headerBytes[1],
			};
			section.data = XorTools.ReadXorBytes(stream, section.size, 0x73);
			Array.Reverse(section.data);
			return section;
		}
	}
}