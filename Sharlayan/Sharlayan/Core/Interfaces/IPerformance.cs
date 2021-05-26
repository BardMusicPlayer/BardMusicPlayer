// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPerformance.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   IPerformance.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using FFBardMusicCommon;

namespace Sharlayan.Core.Interfaces {
	using System.Collections.Generic;

	using Sharlayan.Core.Enums;

	public interface IPerformance {
		
		float Animation { get; set; }

		byte Unknown1 { get; set; }

		byte Id { get; set; }

		byte Variant { get; set; }

		byte Type { get; set; }

		Performance.Status Status { get; set; }

		Instrument Instrument { get; set; }

		short Unknown2 { get; set; }
	}
}