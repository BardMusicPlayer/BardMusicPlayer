using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMemoryParser {
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
