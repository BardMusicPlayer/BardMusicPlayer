using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMemoryParser {
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
