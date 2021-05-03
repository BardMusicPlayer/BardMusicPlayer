// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.CharacterID.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.CharacterID.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
	using System;

	using Sharlayan.Core;
	using Sharlayan.Utilities;

	public static partial class Reader {
		public static bool CanGetCharacterId() {
			var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.CharacterIdKey);
			if(canRead) {
				// OTHER STUFF?
			}

			return canRead;
		}

		public static string GetCharacterId() {
			string id = "";
			if(!CanGetCharacterId() || !MemoryHandler.Instance.IsAttached) {
				return id;
			}

			var CharacterIdMap = (IntPtr) Scanner.Instance.Locations[Signatures.CharacterIdKey];

			try {
				id = MemoryHandler.Instance.GetString(CharacterIdMap, 0, 32);
			} catch(Exception ex) {
				MemoryHandler.Instance.RaiseException(Logger, ex, true);
			}

			return id;
		}
	}
}