using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace BardMusicPlayer.Seer.Reader.Dat {
	public class FFXIVAddonDat : FFXIVDatFile {

		public class AddonStateData {
			// First row
			public uint id;
			public float xpos;
			public float ypos;
			public float zpos;
			// Second row
			public int unknownState;
			public ushort width;
			public ushort height;
			public byte sticky; // Sticky corner
			public byte state6;
			public byte state7;
			public byte state8;
			public int unknown10;

			// Performance pos: 51.20175 16.94313 1
			// Performance pos: 51.27458 33.41232 4

			// Performance pos: 51.93008 66.58768 4
			// Performance pos: 52.22141 83.64929 7

			// Client: 1373 844
			public Point GetXYPoint(float w, float h) {
				float x = xpos / 100.0f, y = ypos / 100.0f;
				float s = sticky;
				// X
				if(s % 3 == 0) {
					x = (w * x);
				} else if(s % 3 == 1) {
					x = (w * x) - width / 2;
				} else if(s % 3 == 2) {
					x = (w * x) - width;
				}
				// Y
				if(s >= 0 && s <= 2) {
					y = (h * y);
				} else if(s >= 3 && s <= 5) {
					y = (h * y) - height / 2;
				} else if(s >= 6 && s <= 8) {
					y = (h * y) - height;
				}
				return new Point((int) x, (int) y);
			}
		};

		public Dictionary<uint, AddonStateData> addonData = new Dictionary<uint, AddonStateData>();
		public AddonStateData this[uint id] {
			get {
				if(!addonData.ContainsKey(id)) {
					return new AddonStateData();
				}
				return addonData[id];
			}
		}

		public void LoadAddonDat(string charId)
        {
            String fileToLoad = ""; // TODO fix this path "ADDON.DAT";
			LoadDatId(charId, fileToLoad);
		}
		// 24 CB 8D CA 7E E6 3B 42 56 55 55 41 00 00 80 3F
		// 00 00 00 00 E8 04 0E 01 [01 00] 00 00 00 00 00 00 []=state3
		protected override bool ParseDat(BinaryReader stream) {

			Dictionary<uint, AddonStateData> data = new Dictionary<uint, AddonStateData>(addonData);

			addonData.Clear();
			if(base.ParseDat(stream)) {

				stream.BaseStream.Seek(0x60, SeekOrigin.Begin);
				addonData = this.ParseBlock(stream);
			}
			return true;
		}

		private Dictionary<uint, AddonStateData> ParseBlock(BinaryReader stream) {
			Dictionary<uint, AddonStateData> stateDataList = new Dictionary<uint, AddonStateData>();
			long pos = stream.BaseStream.Position;
			uint numsections = stream.ReadUInt32();
			stream.BaseStream.Position = pos + 0x10;
			for(uint i = 0; i <= numsections; i++) {
				AddonStateData ac = this.ParseSection(stream);
				stateDataList[ac.id] = ac;
			}
			return stateDataList;
		}

		private AddonStateData ParseSection(BinaryReader stream) {
			AddonStateData ac = new AddonStateData {
				// First row
				id = stream.ReadUInt32(),
				xpos = stream.ReadSingle(),
				ypos = stream.ReadSingle(),
				zpos = stream.ReadSingle(),
				// Second row
				unknownState = stream.ReadInt32(),
				width = stream.ReadUInt16(),
				height = stream.ReadUInt16(),
				sticky = stream.ReadByte(),
				state6 = stream.ReadByte(),
				state7 = stream.ReadByte(),
				state8 = stream.ReadByte(),
				unknown10 = stream.ReadInt32()

			};
			return ac;
		}
	}
}
