
using BardMusicPlayer.Seer.Reader.Memory.SignatureData;

namespace BardMusicPlayer.Seer.Reader.Memory.SignatureType {

	public class SignatureCharacterID : Signature {


		public SignatureCharacterID(Signature sig) : base(sig) { }

		public override object GetData(HookProcess process) {
			SigCharIdData data = new SigCharIdData {
				id = process.GetString(baseAddress, 0, 32),
			};
			return (object) data;
		}
    }
}