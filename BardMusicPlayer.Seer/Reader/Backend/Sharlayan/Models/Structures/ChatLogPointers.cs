/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models.Structures
{
    internal class ChatLogPointers
	{
		public int LogEnd { get; set; }

		public int LogNext { get; set; }

		public int LogStart { get; set; }

		public int OffsetArrayEnd { get; set; }

		public int OffsetArrayPos { get; set; }

		public int OffsetArrayStart { get; set; }
	}
}
