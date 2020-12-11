using System;
using System.Collections.Concurrent;

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

		public enum Instrument {
			Harp = 1,
			Piano = 2,
			Lute = 3,
			Fiddle = 4,
			Flute = 5,
			Oboe = 6,
			Clarinet = 7,
			Fife = 8,
			Panpipes = 9,
			Timpani = 10,
			Bongo = 11,
			BassDrum = 12,
			SnareDrum = 13,
			Cymbal = 14,
			Trumpet = 15,
			Trombone = 16,
			Tuba = 17,
			Horn = 18,
			Saxophone = 19,
			Violin = 20,
			Viola = 21,
			Cello = 22,
			DoubleBass = 23
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
		public Performance.Instrument Instrument { get; set; }

		public short Unknown2 { get; set; }

		public bool IsReady() {
			return (Status >= Performance.Status.Opened);
		}

		public bool IsSimpleInstrument() {
			return ((Type & 0x1) == 1);
		}

		public bool IsWindInstrument() {
			return (
				Instrument == Performance.Instrument.Flute ||
				Instrument == Performance.Instrument.Oboe ||
				Instrument == Performance.Instrument.Clarinet ||
				Instrument == Performance.Instrument.Fife ||
				Instrument == Performance.Instrument.Panpipes
				);
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

		// Original instrument were the instruments originally available when Performance was first added.
		public bool IsOriginalInstrument() {
			if (!Performances.IsEmpty) {
				return (Performances[0].IsSimpleInstrument());
			}
			return false;
		}

		public bool IsWindInstrument() {
			if (!Performances.IsEmpty) {
				return (Performances[0].IsWindInstrument());
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
