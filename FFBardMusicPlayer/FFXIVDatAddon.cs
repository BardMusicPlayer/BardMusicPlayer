using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFBardMusicPlayer {
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

			public void GetXYPos(float w, float h, out float ox, out float oy) {
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
				ox = x;
				oy = y;
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

		public void LoadAddonDat(string charId) {
			String fileToLoad = Program.programOptions.DatPrefix + "ADDON.DAT";
			LoadDatId(charId, fileToLoad);
		}
		// 24 CB 8D CA 7E E6 3B 42 56 55 55 41 00 00 80 3F
		// 00 00 00 00 E8 04 0E 01 [01 00] 00 00 00 00 00 00 []=state3
		protected override bool ParseDat(BinaryReader stream) {

			Dictionary<uint, AddonStateData> data = new Dictionary<uint, AddonStateData>(addonData);

			addonData.Clear();
			if(base.ParseDat(stream)) {

				stream.BaseStream.Seek(0x70, SeekOrigin.Begin);
				while(stream.BaseStream.Position < header.dataSize) {
					AddonStateData ac = ParseSection(stream);
					if(ac.id == 0) {
						continue;
					}
					//if(ac.unknown1 == 0x24){
					if(ac.id == 0xCB) {
						// Performance dialog...
						//Console.WriteLine(string.Format("{0} {1} x: {2} y: {3}, state3: {4}", ac.unknown1, ac.unknown2, ac.xpos, ac.ypos, ac.state3));
					}
					/*
					foreach(AddonStateData d in data.Values) {
						if(d.id == ac.id) {
							if((int)d.xpos != (int)ac.xpos || (int)d.ypos != (int)ac.ypos) {
								Console.WriteLine(string.Format("{0} : x: {1} y: {2}", ac.id.ToString("X"), ac.xpos, ac.ypos));
							}
							if((d.state1 != ac.state1) || (d.state2 != ac.state2) || (d.state3 != ac.state3) || (d.state4 != ac.state4)) {
								Console.WriteLine(string.Format("State1-4 {0} {1} {2} {3}", ac.state1, ac.state2, ac.state3, ac.state4));
							}
							if((d.sticky != ac.sticky) || (d.state6 != ac.state6) || (d.state7 != ac.state7) || (d.state8 != ac.state8)) {
								Console.WriteLine(string.Format("State5-8 {0} {1} {2} {3}", ac.sticky, ac.state6, ac.state7, ac.state8));
							}
						}
					}
					*/
					addonData[ac.id] = ac;
					if(stream.BaseStream.Position >= 0x25f0) {
						break;
					}
				}
			}
			return true;
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
