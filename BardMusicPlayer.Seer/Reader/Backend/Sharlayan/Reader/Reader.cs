/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Reader
{
    internal partial class Reader
    {
        public Reader(MemoryHandler memoryHandler)
        {
            Scanner = memoryHandler.Scanner;
            MemoryHandler = memoryHandler;
            MemoryHandler.Reader = this;
            _chatLogReader = new ChatLogReader(memoryHandler);
        }

        public Scanner Scanner { get; set; }

        public MemoryHandler MemoryHandler { get; set; }
    }
}