/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System.Diagnostics;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models
{
    internal class ProcessModel
	{
        public Process Process { get; set; }

		public int ProcessID => Process?.Id ?? (-1);

		public string ProcessName => Process?.ProcessName ?? string.Empty;
	}
}
