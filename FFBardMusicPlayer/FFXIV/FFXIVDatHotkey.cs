using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// Key/Keybind - the actual key to simulate
// PerfKey/pk - PERFORMANCE_MODE_ key to get the keybind
// NoteKey/nk/byte - C+1, C#, etc
// byte key - the raw midi note byte

namespace FFBardMusicPlayer.FFXIV
{
    public class FFXIVKeybindDat : FFXIVDatFile
    {
        private class KeybindSection
        {
            public byte Type;
            public int Size;
            public byte[] Data;
        };

        private static readonly Dictionary<int, int> MainKeyMap = new Dictionary<int, int>
        {
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
            { 141, 223 }  // `
        };

        private static readonly Dictionary<string, string> OemKeyFix = new Dictionary<string, string>()
        {
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
            { "Oem1", ";" }
        };

		private static Dictionary<int, int> guitarKeyMap = new Dictionary<int, int> {
           { 27, 135 }, // ElectricGuitarClean
           { 28, 134 }, // ElectricGuitarMuted
           { 29, 136 }, // ElectricGuitarOverdriven			
           { 30, 139 }, // ElectricGuitarPowerChords
           { 31, 140 }, // ElectricGuitarSpecial
        };

        public class Keybind
        {
            public int MainKey1;
            public int ModKey1;
            public int MainKey2;
            public int ModKey2;

            public Keys GetKey() => GetKey1();

            public Keys GetKey1() => GetMain(MainKey1) | GetMod(ModKey1);

            public Keys GetKey2() => GetMain(MainKey2) | GetMod(ModKey2);

            private Keys GetMain(int key)
            {
                if (key < 130)
                {
                    return (Keys) key;
                }
                else if (MainKeyMap.ContainsKey(key))
                {
                    return (Keys) MainKeyMap[key];
                }

                return Keys.None;
            }

            private Keys GetMod(int mod)
            {
                var modKeys = Keys.None;
                if ((mod & 1) != 0)
                {
                    modKeys |= Keys.Shift;
                }

                if ((mod & 2) != 0)
                {
                    modKeys |= Keys.Control;
                }

                if ((mod & 4) != 0)
                {
                    modKeys |= Keys.Alt;
                }

                return modKeys;
            }

            public override string ToString()
            {
                var key = GetKey();
                if (key == Keys.None)
                {
                    return string.Empty;
                }
                else
                {
                    var kc = new KeysConverter();
                    var str = kc.ConvertToString(key);
                    if (OemKeyFix.ContainsKey(str))
                    {
                        str = OemKeyFix[str];
                    }

                    return str;
                }
            }
        }

        // keybindList contains map between PERFORMANCE_MODE_* to Keybind
        private readonly Dictionary<string, Keybind> keybindList = new Dictionary<string, Keybind>();

        public Keybind this[string key] => !keybindList.ContainsKey(key) 
            ? new Keybind() 
            : keybindList[key];

        public List<string> GetKeybindList() => keybindList.Keys.ToList();

        public bool ExtendedKeyboardBound
        {
            get
            {
                var all = true;
                foreach (var kk in PianoKeyMap.Values.ToList())
                {
                    if (this[kk] is Keybind kb)
                    {
                        if (kb.GetKey() == Keys.None)
                        {
                            all = false;
                            break;
                        }
                    }
                }

                return all;
            }
        }

        public static Dictionary<string, string> PianoKeyMap = new Dictionary<string, string>
        {
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
            { "C+2", "PERFORMANCE_MODE_EX_C6" }
        };

        public List<Keybind> GetPerformanceKeybinds()
        {
            var keybinds = new List<Keybind>();
            foreach (var noteKey in PianoKeyMap.Keys)
            {
                if (GetKeybindFromNoteKey(noteKey) is Keybind keybind)
                {
                    keybinds.Add(keybind);
                }
            }

            return keybinds;
        }

        public static string NoteByteToPerformanceKey(int note)
        {
            if (note >= 0 && note < PianoKeyMap.Count)
            {
                return PianoKeyMap.Keys.ToArray()[note];
            }

            return string.Empty;
        }

        public static string RawNoteByteToPianoKey(int note)
        {
            var realPianoKeyMap = new List<string>
            {
                "C", "C#", "D", "Eb", "E", "F", "F#", "G", "G#", "A", "Bb", "B"
            };
            var fn = note + 12 * 4;
            var key = fn % 12;
            var oc = fn / 12 - 1;
            return key > 0 ? $"{realPianoKeyMap[key]}{oc}" : string.Empty;
        }

        public static string NoteKeyToPerformanceKey(string nk) => PianoKeyMap.ContainsKey(nk) ? PianoKeyMap[nk] : string.Empty;

        public Keybind GetKeybindFromNoteByte(int note)
        {
            var pk = NoteByteToPerformanceKey(note);
            if (!string.IsNullOrEmpty(pk))
            {
                if (PianoKeyMap.ContainsKey(pk))
                {
                    return this[PianoKeyMap[pk]];
                }
            }

            return null;
        }

		public Keybind GetKeybindFromVoiceByte(int note)
		{
			FFXIVKeybindDat.Keybind _keybind = new FFXIVKeybindDat.Keybind();
			int key = guitarKeyMap[note];
			if (key != 0)
			{
				_keybind.MainKey1 = key;
				return _keybind;
            }
			return null;
		}

        public Keybind GetKeybindFromNoteKey(string nk)
        {
            var pk = NoteKeyToPerformanceKey(nk);
            return !string.IsNullOrEmpty(pk) ? this[pk] : null;
        }

        public void LoadKeybindDat(string charId)
        {
            var fileToLoad = $"{Program.ProgramOptions.DatPrefix}KEYBIND.DAT";
            LoadDatId(charId, fileToLoad);
        }

        protected override bool ParseDat(BinaryReader stream)
        {
            keybindList.Clear();
            if (base.ParseDat(stream))
            {
                stream.BaseStream.Seek(0x11, SeekOrigin.Begin);
                while (stream.BaseStream.Position < Header.DataSize)
                {
                    var command = ParseSection(stream);
                    var keybind = ParseSection(stream);

                    var key = Encoding.UTF8.GetString(command.Data);
                    key = key.Substring(0, key.Length - 1); // Trim off \0
                    var dat = Encoding.UTF8.GetString(keybind.Data);
                    var datKeys = dat.Split(',');
                    if (datKeys.Length == 3)
                    {
                        var key1 = datKeys[0].Split('.');
                        var key2 = datKeys[1].Split('.');
                        keybindList.Add(key, new Keybind
                        {
                            MainKey1 = int.Parse(key1[0], System.Globalization.NumberStyles.HexNumber),
                            MainKey2 = int.Parse(key2[0], System.Globalization.NumberStyles.HexNumber),
                            ModKey1  = int.Parse(key1[1], System.Globalization.NumberStyles.HexNumber),
                            ModKey2  = int.Parse(key2[1], System.Globalization.NumberStyles.HexNumber)
                        });
                    }
                }
            }

            Console.WriteLine($"Read {keybindList.Count} keys");
            return true;
        }

        private KeybindSection ParseSection(BinaryReader stream)
        {
            var headerBytes = XorTools.ReadXorBytes(stream, 3, 0x73);
            var section = new KeybindSection
            {
                Type = headerBytes[0],
                Size = headerBytes[1]
            };
            section.Data = XorTools.ReadXorBytes(stream, section.Size, 0x73);
            Array.Reverse(section.Data);
            return section;
        }
    }
}