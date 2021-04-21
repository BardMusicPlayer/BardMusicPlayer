
using BardMusicPlayer.Seer.Reader.Memory.SignatureData;

namespace BardMusicPlayer.Seer.Reader.Memory.SignatureType {

	public class SignatureWorld : Signature {

		public SignatureWorld(Signature sig) : base(sig) { }

		public override object GetData(HookProcess process) {
			SigWorldData data = new SigWorldData {
				world = process.GetString(baseAddress, 0, 16)
			};
			return (object) data;
		}
    }
}