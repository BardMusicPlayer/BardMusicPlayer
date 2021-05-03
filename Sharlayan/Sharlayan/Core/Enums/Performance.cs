// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Performance.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Performance.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Core.Enums {
	public class Performance {

		public enum Status : byte {
			Closed = 0,
			Loading = 1,
			Opened = 2,
			SwitchingNote = 3,
			HoldingNote = 4
		}

	}
}
