// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Target.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.Target.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;

    using Sharlayan.Core;
    using Sharlayan.Core.Enums;
    using Sharlayan.Delegates;
    using Sharlayan.Models.ReadResults;
    using Sharlayan.Utilities;

    using BitConverter = Sharlayan.Utilities.BitConverter;

    public static partial class Reader {
        public static bool CanGetWorld() {
            var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.WorldKey);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        public static string GetWorld() {
            if (!CanGetWorld() || !MemoryHandler.Instance.IsAttached) {
                return string.Empty;
            }

			var worldMap = (IntPtr) Scanner.Instance.Locations[Signatures.WorldKey];
			try {
				string world = MemoryHandler.Instance.GetString(worldMap, 0, 16);
				return world;
			} catch(Exception ex) {
				MemoryHandler.Instance.RaiseException(Logger, ex, true);
			}
			return string.Empty;
		}
    }
}