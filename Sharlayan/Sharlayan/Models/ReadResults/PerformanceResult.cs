// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerformanceResult.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   PerformanceResult.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
	using Sharlayan.Core;
	using Sharlayan.Core.Enums;
	using System.Collections.Concurrent;

	public class PerformanceResult {

		public ConcurrentDictionary<uint, PerformanceItem> Performances { get; } = new ConcurrentDictionary<uint, PerformanceItem>();

		public bool IsUp() {
			if(!Performances.IsEmpty) {

                return (Performances[0].Status >= Performance.Status.Opened);
			}
			return false;
		}

		// Original instrument were the instruments originally available when Performance was first added.
		public bool IsOriginalInstrument() {
			if(!Performances.IsEmpty) {
				return (Performances[0].IsSimpleInstrument());
			}
			return false;
		}

		public bool IsWindInstrument() {
			if(!Performances.IsEmpty) {
				return (Performances[0].IsWindInstrument());
			}
			return false;
		}
	}
}