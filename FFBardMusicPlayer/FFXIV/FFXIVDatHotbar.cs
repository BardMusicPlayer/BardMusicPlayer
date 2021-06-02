using System.Collections.Generic;
using System.IO;
using FFBardMusicCommon;

// Key/Keybind - the actual key to simulate
// PerfKey/pk - PERFORMANCE_MODE_ key to get the keybind
// NoteKey/nk/byte - C+1, C#, etc
// byte key - the raw midi note byte

namespace FFBardMusicPlayer.FFXIV
{
    public class FFXIVHotbarDat : FFXIVDatFile
    {
        #region Hotbar classes

        public class HotbarSection
        {
            internal byte Action; // Higher level? 0D for 60-70 spells
            internal byte Flag;
            internal byte Unk1;
            internal byte Unk2;
            internal byte Job;
            internal byte Hotbar;
            internal byte Slot;
            internal byte Type;
        };

        public class HotbarSlots : Dictionary<int, HotbarJobSlot>
        {
        }

        public class HotbarSlot : HotbarSection
        {
            public int HotbarOutput => Hotbar + 1;

            public int SlotOutput
            {
                get
                {
                    var fslot = Slot + 1;

                    var ss = fslot % 10;
                    if (fslot > 10)
                    {
                        ss += fslot / 10 * 10 - 1;
                    }

                    return ss;
                }
            }

            public override string ToString() => $"HOTBAR_{HotbarOutput}_{SlotOutput:X}";
        }

        public class HotbarJobSlots : Dictionary<int, HotbarSlot>
        {
        }

        public class HotbarJobSlot
        {
            public HotbarJobSlots JobSlots = new HotbarJobSlots();

            public HotbarSlot this[int i]
            {
                get
                {
                    if (!JobSlots.ContainsKey(i))
                    {
                        JobSlots[i] = new HotbarSlot();
                    }

                    return JobSlots[i];
                }
                set => JobSlots[i] = value;
            }
        }

        public class HotbarRows : Dictionary<int, HotbarRow>
        {
        }

        public class HotbarRow
        {
            public HotbarSlots Slots = new HotbarSlots();

            public HotbarJobSlot this[int i]
            {
                get
                {
                    if (!Slots.ContainsKey(i))
                    {
                        Slots[i] = new HotbarJobSlot();
                    }

                    return Slots[i];
                }
                set => Slots[i] = value;
            }
        }

        public class HotbarData
        {
            public HotbarRows Rows = new HotbarRows();

            public HotbarRow this[int i]
            {
                get
                {
                    if (!Rows.ContainsKey(i))
                    {
                        Rows[i] = new HotbarRow();
                    }

                    return Rows[i];
                }
                set => Rows[i] = value;
            }

            public void Clear() { Rows.Clear(); }
        }

        #endregion

        private readonly HotbarData hotbarData = new HotbarData();

        public void LoadHotbarDat(string charId)
        {
            var fileToLoad = $"{Program.ProgramOptions.DatPrefix}HOTBAR.DAT";
            LoadDatId(charId, fileToLoad);
        }

        public List<HotbarSlot> GetSlotsFromType(int type)
        {
            var slots = new List<HotbarSlot>();
            foreach (var row in hotbarData.Rows.Values)
            {
                foreach (var jobSlot in row.Slots.Values)
                {
                    foreach (var slot in jobSlot.JobSlots.Values)
                    {
                        if (slot.Type == type)
                        {
                            slots.Add(slot);
                        }
                    }
                }
            }

            return slots;
        }

        public string GetHotbarSlotKeyMap(int hnum, int snum, int jnum)
        {
            if (hotbarData.Rows.ContainsKey(hnum))
            {
                var row = hotbarData[hnum];
                if (row.Slots.ContainsKey(snum))
                {
                    var slot = row[snum][jnum];
                    return slot.ToString();
                }
            }

            return null;
        }

        public string GetInstrumentKeyMap(Instrument ins)
        {
            var slots = GetSlotsFromType(0x1D);
            foreach (var slot in slots)
            {
                if (slot.Action == (int) ins)
                {
                    return slot.ToString();
                }
            }

            return string.Empty;
        }

        protected override bool ParseDat(BinaryReader stream)
        {
            hotbarData.Clear();
            if (base.ParseDat(stream))
            {
                stream.BaseStream.Seek(0x10, SeekOrigin.Begin);
                while (stream.BaseStream.Position < Header.DataSize)
                {
                    var ac = ParseSection(stream);
                    if (ac.Job <= 0x23)
                    {
                        if (ac.Type == 0x1D)
                        {
                            //Console.WriteLine(string.Format("{0} ({1}): {2} {3}", ac.ToString(), ac.job, ac.action, ac.type));
                        }

                        hotbarData[ac.Hotbar][ac.Slot][ac.Job] = ac;
                    }
                }
            }

            return true;
        }

        private HotbarSlot ParseSection(BinaryReader stream)
        {
            byte xor = 0x31;
            var ac = new HotbarSlot
            {
                Action = XorTools.ReadXorByte(stream, xor),
                Flag   = XorTools.ReadXorByte(stream, xor),
                Unk1   = XorTools.ReadXorByte(stream, xor),
                Unk2   = XorTools.ReadXorByte(stream, xor),
                Job    = XorTools.ReadXorByte(stream, xor),
                Hotbar = XorTools.ReadXorByte(stream, xor),
                Slot   = XorTools.ReadXorByte(stream, xor),
                Type   = XorTools.ReadXorByte(stream, xor)
            };
            return ac;
        }
    }
}