// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Target.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.Target.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using FFBardMusicCommon;
using System;

namespace FFMemoryParser {

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