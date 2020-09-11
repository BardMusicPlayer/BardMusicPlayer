// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.CurrentPlayer.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.CurrentPlayer.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace FFMemoryParser {

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
		}
	}
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
	}
	public class SignaturePerformance : Signature {

		public SignaturePerformance(Signature sig) : base(sig) { }
        public override object GetData(HookProcess process) {
            
			var result = new SigPerfData();

			int entrySize = 12;
			int numEntries = 99;
			byte[] performanceData = process.GetByteArray(baseAddress, entrySize * numEntries);

			for(int i = 0; i < numEntries; i++) {
				int offset = (i * entrySize);

				float animation = BitConverter.TryToSingle(performanceData, offset+0);
				byte id = performanceData[offset + 4];
				byte unknown1 = performanceData[offset + 5];
				byte variant = performanceData[offset + 6]; // Animation (hand to left or right)
				byte type = performanceData[offset + 7];
				byte status = performanceData[offset + 8];
				byte instrument = performanceData[offset + 9];
				int unknown2 = BitConverter.TryToInt16(performanceData, offset + 10);

				if(id >= 0 && id <= 99) {
					PerformanceData item = new PerformanceData();
					item.Animation = animation;
					item.Unknown1 = (byte)unknown1;
					item.Id = (byte) id;
					item.Variant = (byte) variant;
					item.Type = (byte) type;
					item.Status = (Performance.Status) status;
					item.Instrument = (Performance.Instrument) instrument;

					if(!result.Performances.ContainsKey(id)) {
						result.Performances[id] = item;
					}
				}
			}
            return result;
        }
    }
}