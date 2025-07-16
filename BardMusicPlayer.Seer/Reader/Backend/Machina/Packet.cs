/*
 * Copyright(c) 2025 GiR-Zippo, 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;

namespace BardMusicPlayer.Seer.Reader.Backend.Machina
{
    internal sealed partial class Packet : IDisposable
    {
        private readonly Dictionary<ulong, uint> _contentId2ActorId = new();
        private readonly ReaderHandler _machinaReader;

        internal Packet(ReaderHandler readerHandler)
        {
            _machinaReader = readerHandler;
        }

        public void Dispose()
        {
            _contentId2ActorId.Clear();
        }

        private static bool ValidTimeSig(byte timeSig)
        {
            return timeSig is > 1 and < 8;
        }

        private static bool ValidTempo(byte tempo)
        {
            return tempo is > 29 and < 201;
        }

        ~Packet()
        {
            Dispose();
        }
    }
}