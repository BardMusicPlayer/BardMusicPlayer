using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMemoryParser {
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
