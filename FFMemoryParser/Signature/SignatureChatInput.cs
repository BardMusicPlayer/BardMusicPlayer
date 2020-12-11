using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMemoryParser {
    public class SignatureChatInput : Signature {
        public SignatureChatInput(Signature sig) : base(sig) { }
        public override object GetData(HookProcess process) {
            IntPtr pointer = (IntPtr) process.GetInt64(baseAddress);
            string chattext = string.Empty;
            if(pointer != IntPtr.Zero) {
                int len = process.GetInt32(pointer, Offsets["OffsetInputLength"]);
                if(len <= 501) { // ???
                    IntPtr pointer2 = (IntPtr) process.GetInt64(pointer, Offsets["OffsetInputText"]);
                    if(pointer2 != IntPtr.Zero) {
                        chattext = process.GetString(pointer2, 0, len);
					}
				}
			}
            SigChatInputData data = new SigChatInputData {
                open = (pointer != IntPtr.Zero),
                text = chattext,
            };
            return (object)data;
        }
    }
}
