/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System;
using System.Collections.Generic;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan
{
    internal sealed class ChatLogReader
    {
        private const int BUFFER_SIZE = 4000;
        private readonly MemoryHandler _memoryHandler;
        public readonly List<int> Indexes = new();
        public ChatLogPointers ChatLogPointers;
        public int PreviousArrayIndex;
        public int PreviousOffset;

        public ChatLogReader(MemoryHandler memoryHandler)
        {
            _memoryHandler = memoryHandler;
        }

        public void EnsureArrayIndexes()
        {
            Indexes.Clear();

            var indexes = _memoryHandler.GetByteArray(new IntPtr(ChatLogPointers.OffsetArrayStart), BUFFER_SIZE);

            for (var i = 0; i < BUFFER_SIZE; i += 4) Indexes.Add(BitConverter.ToInt32(indexes, i));
        }

        public IEnumerable<List<byte>> ResolveEntries(int offset, int length)
        {
            var entries = new List<List<byte>>();
            for (var i = offset; i < length; i++)
            {
                EnsureArrayIndexes();
                var currentOffset = Indexes[i];
                entries.Add(ResolveEntry(PreviousOffset, currentOffset));
                PreviousOffset = currentOffset;
            }

            return entries;
        }

        private List<byte> ResolveEntry(int offset, int length)
        {
            return new List<byte>(_memoryHandler.GetByteArray(new IntPtr(ChatLogPointers.LogStart + offset),
                length - offset));
        }
    }
}