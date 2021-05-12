/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models
{
    internal class ChatLogPointers
	{
		public uint LineCount { get; set; }

		public long LogEnd { get; set; }

		public long LogNext { get; set; }

		public long LogStart { get; set; }

		public long OffsetArrayEnd { get; set; }

		public long OffsetArrayPos { get; set; }

		public long OffsetArrayStart { get; set; }
	}
}
