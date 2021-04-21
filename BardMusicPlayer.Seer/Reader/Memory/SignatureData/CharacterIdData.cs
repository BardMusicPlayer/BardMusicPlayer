using System;

namespace BardMusicPlayer.Seer.Reader.Memory.SignatureData {
	[Serializable]
	public class SigCharIdData {
		public string id;
		public override bool Equals(object obj) {
			SigCharIdData data = (obj as SigCharIdData);
			if (data == null) {
				return false;
			}
			return (id == data.id);
		}
	}
}
