
using BardMusicPlayer.Seer.Reader.Memory.SignatureData;

namespace BardMusicPlayer.Seer.Reader.Memory.SignatureType {

	public class SignaturePerformance : Signature {

		public SignaturePerformance(Signature sig) : base(sig) { }
        public override object GetData(HookProcess process) {
            
			var result = new SigPerfData();

			int entrySize = Offsets["OffsetEntrySize"];
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