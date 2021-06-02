using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace FFBardMusicPlayer.FFXIV
{
    public class FFXIVAddonDat : FFXIVDatFile
    {
        public class AddonStateData
        {
            // First row
            public uint Id;
            public float Xpos;
            public float Ypos;
            public float Zpos;

            // Second row
            public int UnknownState;
            public ushort Width;
            public ushort Height;
            public byte Sticky; // Sticky corner
            public byte State6;
            public byte State7;
            public byte State8;
            public int Unknown10;

            // Performance pos: 51.20175 16.94313 1
            // Performance pos: 51.27458 33.41232 4

            // Performance pos: 51.93008 66.58768 4
            // Performance pos: 52.22141 83.64929 7

            // Client: 1373 844
            public Point GetXyPoint(float w, float h)
            {
                float x = Xpos / 100.0f, y = Ypos / 100.0f;
                float s = Sticky;
                // X
                if (s % 3 == 0)
                {
                    x = w * x;
                }
                else if (s % 3 == 1)
                {
                    x = w * x - Width / 2;
                }
                else if (s % 3 == 2)
                {
                    x = w * x - Width;
                }

                // Y
                if (s >= 0 && s <= 2)
                {
                    y = h * y;
                }
                else if (s >= 3 && s <= 5)
                {
                    y = h * y - Height / 2;
                }
                else if (s >= 6 && s <= 8)
                {
                    y = h * y - Height;
                }

                return new Point((int) x, (int) y);
            }
        };

        public Dictionary<uint, AddonStateData> AddonData = new Dictionary<uint, AddonStateData>();

        public AddonStateData this[uint id] => !AddonData.ContainsKey(id) ? new AddonStateData() : AddonData[id];

        public void LoadAddonDat(string charId)
        {
            var fileToLoad = $"{Program.ProgramOptions.DatPrefix}ADDON.DAT";
            LoadDatId(charId, fileToLoad);
        }

        // 24 CB 8D CA 7E E6 3B 42 56 55 55 41 00 00 80 3F
        // 00 00 00 00 E8 04 0E 01 [01 00] 00 00 00 00 00 00 []=state3
        protected override bool ParseDat(BinaryReader stream)
        {
            var data = new Dictionary<uint, AddonStateData>(AddonData);

            AddonData.Clear();
            if (base.ParseDat(stream))
            {
                stream.BaseStream.Seek(0x60, SeekOrigin.Begin);
                AddonData = ParseBlock(stream);
            }

            return true;
        }

        private Dictionary<uint, AddonStateData> ParseBlock(BinaryReader stream)
        {
            var stateDataList = new Dictionary<uint, AddonStateData>();
            var pos = stream.BaseStream.Position;
            var numsections = stream.ReadUInt32();
            stream.BaseStream.Position = pos + 0x10;
            for (uint i = 0; i <= numsections; i++)
            {
                var ac = ParseSection(stream);
                stateDataList[ac.Id] = ac;
            }

            return stateDataList;
        }

        private AddonStateData ParseSection(BinaryReader stream)
        {
            var ac = new AddonStateData
            {
                // First row
                Id   = stream.ReadUInt32(),
                Xpos = stream.ReadSingle(),
                Ypos = stream.ReadSingle(),
                Zpos = stream.ReadSingle(),
                // Second row
                UnknownState = stream.ReadInt32(),
                Width        = stream.ReadUInt16(),
                Height       = stream.ReadUInt16(),
                Sticky       = stream.ReadByte(),
                State6       = stream.ReadByte(),
                State7       = stream.ReadByte(),
                State8       = stream.ReadByte(),
                Unknown10    = stream.ReadInt32()
            };
            return ac;
        }
    }
}