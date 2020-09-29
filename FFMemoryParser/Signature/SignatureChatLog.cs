// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.ChatLog.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.ChatLog.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FFMemoryParser {

    [Serializable]
    public class ChatLogItem {
        public byte[] Bytes { get; set; }
        public string Code { get; set; }
        public string Combined { get; set; }
        public bool JP { get; set; }
        public string Line { get; set; }
        public string Raw { get; set; }
        public DateTime TimeStamp { get; set; }
    };

    [Serializable]
    public class ChatLogList : List<ChatLogItem> { }
    
    [Serializable]
    public class SigChatLogData {
        public ChatLogList chatMessages = new ChatLogList();
	};

    public class SignatureChatLog : Signature {
        ChatLogReader chatReader = new ChatLogReader();

        int previousArrayIndex = 0;
        int previousOffset = 0;
        public SignatureChatLog(Signature sig) : base(sig) { }

        public override object GetData(HookProcess process) {
            var result = new SigChatLogData();

            chatReader.PreviousArrayIndex = previousArrayIndex;
            chatReader.PreviousOffset = previousOffset;

            if (baseAddress.ToInt64() <= 20) {
                return result;
            }

            List<List<byte>> buffered = new List<List<byte>>();

            try {
                chatReader.Indexes.Clear();
                chatReader.ChatLogPointers = new ChatLogPointers {
                    LineCount = (uint) process.GetUInt(baseAddress),
                    OffsetArrayStart = process.GetUInt(baseAddress, Offsets["OffsetArrayStart"]),
                    OffsetArrayPos = process.GetUInt(baseAddress, Offsets["OffsetArrayPos"]),
                    OffsetArrayEnd = process.GetUInt(baseAddress, Offsets["OffsetArrayEnd"]),
                    LogStart = process.GetUInt(baseAddress, Offsets["LogStart"]),
                    LogNext = process.GetUInt(baseAddress, Offsets["LogNext"]),
                    LogEnd = process.GetUInt(baseAddress, Offsets["LogEnd"]),
                };

                chatReader.EnsureArrayIndexes(process);

                // Convenience
                ChatLogPointers ptrs = chatReader.ChatLogPointers;

                var currentArrayIndex = (ptrs.OffsetArrayPos - ptrs.OffsetArrayStart) / 4;
                if (chatReader.ChatLogFirstRun) {
                    chatReader.ChatLogFirstRun = false;
                    chatReader.PreviousOffset = chatReader.Indexes[(int) currentArrayIndex - 1];
                    chatReader.PreviousArrayIndex = (int) currentArrayIndex - 1;
                }
                else {
                    if (currentArrayIndex < chatReader.PreviousArrayIndex) {
                        buffered.AddRange(chatReader.ResolveEntries(process, chatReader.PreviousArrayIndex, 1000));
                        chatReader.PreviousOffset = 0;
                        chatReader.PreviousArrayIndex = 0;
                    }

                    if (chatReader.PreviousArrayIndex < currentArrayIndex) {
                        buffered.AddRange(chatReader.ResolveEntries(process, chatReader.PreviousArrayIndex, (int) currentArrayIndex));
                    }

                    chatReader.PreviousArrayIndex = (int) currentArrayIndex;
                }
            }
            catch (Exception ex) {
                return null;
            }

            foreach (List<byte> bytes in buffered.Where(b => b.Count > 0)) {
                try {
                    ChatLogItem chatLogEntry = ChatEntry.Process(bytes.ToArray());
                    if (Regex.IsMatch(chatLogEntry.Combined, @"[\w\d]{4}::?.+")) {
                        result.chatMessages.Add(chatLogEntry);
                    }
                }
                catch (Exception ex) {
                }
            }

            previousArrayIndex = chatReader.PreviousArrayIndex;
            previousOffset = chatReader.PreviousOffset;

            return result;
        }

        private class ChatLogReader {
            public readonly List<int> Indexes = new List<int>();

            public bool ChatLogFirstRun = true;

            public ChatLogPointers ChatLogPointers;

            public int PreviousArrayIndex;

            public int PreviousOffset;

            public void EnsureArrayIndexes(HookProcess process) {
                Indexes.Clear();
                for (var i = 0; i < 1000; i++) {
                    Indexes.Add((int) process.GetUInt(new IntPtr(ChatLogPointers.OffsetArrayStart + i * 4)));
                }
            }

            public IEnumerable<List<byte>> ResolveEntries(HookProcess process, int offset, int length) {
                List<List<byte>> entries = new List<List<byte>>();
                for (var i = offset; i < length; i++) {
                    EnsureArrayIndexes(process);
                    var currentOffset = Indexes[i];
                    entries.Add(ResolveEntry(process, PreviousOffset, currentOffset));
                    PreviousOffset = currentOffset;
                }

                return entries;
            }

            private List<byte> ResolveEntry(HookProcess process, int offset, int length) {
                return new List<byte>(process.GetByteArray(new IntPtr(ChatLogPointers.LogStart + offset), length - offset));
            }
        }
        internal class ChatLogPointers {
            public uint LineCount { get; set; }
            public long LogEnd { get; set; }
            public long LogNext { get; set; }
            public long LogStart { get; set; }
            public long OffsetArrayEnd { get; set; }
            public long OffsetArrayPos { get; set; }
            public long OffsetArrayStart { get; set; }
        }

    }
}