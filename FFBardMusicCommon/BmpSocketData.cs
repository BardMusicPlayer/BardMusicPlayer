using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace FFBardMusicCommon {

	public class BmpPacket {
		public static string Serialize(Object o) {
			return JsonConvert.SerializeObject(o);
		}
		public static T Deserialize<T>(string p) {
			return JsonConvert.DeserializeObject<T> (p);
		}

		public enum PacketId {
			PacketConduct,
		}
		public PacketId packetId = 0;
		public uint actorId = 0;
	}
	

	public class BmpConductorData : BmpPacket {
		public string conductorName;
		public BmpConductorData(string name) {
			conductorName = name;
			packetId = PacketId.PacketConduct;
		}
	}

	public class BmpPlayerStatusData : BmpPacket {
		public bool play;
		public int tick;
		public bool loop;
	}

	public class BmpPlayerFileData : BmpPacket {
		public string filename;
		public byte[] data;
	}

	public class BmpPerformerSettingData : BmpPacket {
		public float ss;
		public int os;
		public int tn;
	}

	public class BmpPerformerDelayData : BmpPacket {
		public int delay;
	}

	public class BmpCharacterData : BmpPacket {
		int jobId;
	}

}
