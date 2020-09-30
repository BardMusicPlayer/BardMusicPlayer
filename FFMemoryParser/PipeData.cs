using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMemoryParser {

	[Serializable]
	public class PipeData {
		public string id = string.Empty;
		public byte[] data;
		public PipeData(string i, byte[] d) {
			id = i;
			data = d;
		}
	}
}
