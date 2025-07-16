/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Core.Interfaces
{
    internal interface IChatLogItem
    {
        byte[] Bytes { get; set; }

        string Code { get; set; }

        string Combined { get; set; }

        bool JP { get; set; }

        string Line { get; set; }

        string Raw { get; set; }

        DateTime TimeStamp { get; set; }
    }
}