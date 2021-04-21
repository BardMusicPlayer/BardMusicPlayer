using System;

namespace BardMusicPlayer.Seer.Reader.Memory.SignatureData {
	[Serializable]
	public class SigChatInputData {
		public string text = string.Empty;
		public bool open = false;
		public override bool Equals(object obj) {
			SigChatInputData data = (obj as SigChatInputData);
			if (data == null) {
				return false;
			}
			return (this.text != data.text) || (this.open != data.open);
		}
	}
}
