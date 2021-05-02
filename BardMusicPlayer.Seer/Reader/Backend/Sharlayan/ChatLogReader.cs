/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System;
using System.Collections.Generic;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan
{
    internal class ChatLogReader
	{
        public readonly List<int> Indexes = new();

        public ChatLogPointers ChatLogPointers;

        public int PreviousArrayIndex;

        public int PreviousOffset;

        private readonly MemoryHandler _memoryHandler;
        public ChatLogReader(MemoryHandler memoryHandler)
        {
            _memoryHandler = memoryHandler;
        }

        public void EnsureArrayIndexes() {
            Indexes.Clear();

            var indexes = _memoryHandler.GetByteArray(new IntPtr(ChatLogPointers.OffsetArrayStart), 4000);

            for (var i = 0; i < 1000; i++) {
                Indexes.Add(BitConverter.ToInt32(indexes, i*4));
            }
        }

        public IEnumerable<List<byte>> ResolveEntries(int offset, int length) {
            var entries = new List<List<byte>>();
            for (var i = offset; i < length; i++) {
                EnsureArrayIndexes();
                var currentOffset = Indexes[i];
                entries.Add(ResolveEntry(PreviousOffset, currentOffset));
                PreviousOffset = currentOffset;
            }
            return entries;
        }

        private List<byte> ResolveEntry(int offset, int length) => new(_memoryHandler.GetByteArray(new IntPtr(ChatLogPointers.LogStart + offset), length - offset));
    }
}
