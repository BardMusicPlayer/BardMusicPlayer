// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.CurrentPlayer.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.CurrentPlayer.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using MogLib.Common.Structs;

namespace Sharlayan {
    using System;

	using Sharlayan.Core;
	using Sharlayan.Core.Enums;
	using Sharlayan.Models.ReadResults;
    using Sharlayan.Utilities;

	using BitConverter = Sharlayan.Utilities.BitConverter;

	public static partial class Reader {
        public static bool CanGetPerformance() {
			//var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.PerformanceLayoutKey) && Scanner.Instance.Locations.ContainsKey(Signatures.PerformanceStatusKey);
			var canRead = Scanner.Instance.Locations.ContainsKey(Signatures.PerformanceStatusKey);
			if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        public static PerformanceResult GetPerformance() {
            var result = new PerformanceResult();

            if (!CanGetPerformance() || !MemoryHandler.Instance.IsAttached) {
                return result;
            }


            try {
				var PerformanceStatusMap = (IntPtr) Scanner.Instance.Locations[Signatures.PerformanceStatusKey];

				int entrySize = 28;
				int numEntries = 99;
				byte[] performanceData = MemoryHandler.Instance.GetByteArray(Scanner.Instance.Locations[Signatures.PerformanceStatusKey], entrySize * numEntries);

				for(int i = 0; i < numEntries; i++) {
					int offset = (i * entrySize);

					//float animation = BitConverter.TryToSingle(performanceData, offset + 8);
					byte id = performanceData[offset + 12];
					//byte unknown1 = performanceData[offset + 13]; // No clue
					//byte variant = performanceData[offset + 14]; // Animation (hand to left or right)
					byte type = performanceData[offset + 15];
					byte status = performanceData[offset + 21];
					byte instrument = performanceData[offset + 22];
					//int unknown2 = BitConverter.TryToInt16(performanceData, offset + 10);

					/*if (instrument == 5)
                    {
						Debug.WriteLine($"id -> {id}");
						Debug.WriteLine($"status = {status}");
						Debug.WriteLine($"instrument = {instrument}");
					}*/

					if (id >= 0 && id <= 99) {
						PerformanceItem item = new PerformanceItem();
						//item.Animation = animation;
						//item.Unknown1 = (byte)unknown1;
						item.Id = (byte) id;
						//item.Variant = (byte) variant;
						//item.Type = (byte) type;
						item.Status = (Performance.Status) status;
						item.Instrument = instrument;

						if(!result.Performances.ContainsKey(id)) {
							result.Performances[id] = item;
						}
					}
				}
				/*
				var PerformanceLayoutMap = (IntPtr) Scanner.Instance.Locations[Signatures.PerformanceLayoutKey];

				byte layout = MemoryHandler.Instance.GetByte(PerformanceLayoutMap);
				result.Layout = layout;
				*/
            }
            catch (Exception ex) {
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }

            return result;
        }
    }
}