using System;

namespace BardMusicPlayer.Seer.Reader.Memory.SignatureData {
	[Serializable]
	public class SigWorldData {
		public string world;

		public override bool Equals(object obj) {
			SigWorldData data = (obj as SigWorldData);
			if(data == null) {
				return false;
			}
			return (world == data.world);
		}
    }
}
