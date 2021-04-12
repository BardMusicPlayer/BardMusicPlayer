using System;
using System.Collections.Concurrent;
using MogLib.Common.Structs;

namespace FFMemoryParser {
	[Serializable]
	public class Performance {
		public enum Status : byte {
			Closed = 0,
			Loading = 1,
			Opened = 2,
			SwitchingNote = 3,
			HoldingNote = 4
		}
	}
	[Serializable]
	public class PerformanceData {

		public float Animation { get; set; }
		public byte Unknown1 { get; set; }
		public byte Id { get; set; }
		public byte Variant { get; set; }
		public byte Type { get; set; }
		public Performance.Status Status { get; set; }
		public Instrument Instrument { get; set; }

		public short Unknown2 { get; set; }

		public bool IsReady() {
			return (Status >= Performance.Status.Opened);
		}
	}
	[Serializable]
	public class SigPerfData {
		public ConcurrentDictionary<uint, PerformanceData> Performances { get; } = new ConcurrentDictionary<uint, PerformanceData>();

		// Convenience funcs for local player
		public bool IsUp() {
			if (!Performances.IsEmpty) {

				return (Performances[0].Status >= Performance.Status.Opened);
			}
			return false;
		}

		public override bool Equals(object obj) {
			SigPerfData data = (obj as SigPerfData);
			if(data == null) {
				return false;
			}
			int perf1c = 0;
			int perf2c = 0;
			for (uint i = 0; i < Performances.Count; i++) {
				perf1c += (int)Performances[i].Instrument;
				perf2c += (int)data.Performances[i].Instrument;
			}
			return (perf1c == perf2c);
		}
	}
}
