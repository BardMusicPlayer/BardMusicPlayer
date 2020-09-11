// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Target.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.Target.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;

namespace FFMemoryParser {
	public class SigCharIdData {
		public string id;
	}

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