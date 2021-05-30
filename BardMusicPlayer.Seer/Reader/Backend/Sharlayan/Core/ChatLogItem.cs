/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Core.Interfaces;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Core
{
    internal class ChatLogItem : IChatLogItem
    {
        public byte[] Bytes { get; set; }

        public string Code { get; set; }

        public string Combined { get; set; }

        public bool JP { get; set; }

        public string Line { get; set; }

        public string Raw { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}