/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.IO;

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile.Utilities
{
    internal static class XorTools
    {
        internal static byte ReadXorByte(BinaryReader reader, int xor = 0) => (byte) (reader.ReadByte() ^ (byte) xor);

        internal static short ReadXorInt16(BinaryReader reader, int xor = 0)
        {
            var data = reader.ReadBytes(2);
            if (xor != 0)
            {
                Array.Reverse(data);
                for (var i = 0; i < data.Length; i++) data[i] ^= (byte) xor;
            }

            return BitConverter.ToInt16(data, 0);
        }

        internal static int ReadXorInt32(BinaryReader reader, int xor = 0)
        {
            var data = reader.ReadBytes(4);
            if (xor != 0)
            {
                Array.Reverse(data);
                for (var i = 0; i < data.Length; i++) data[i] ^= (byte) xor;
            }

            return BitConverter.ToInt32(data, 0);
        }

        internal static uint ReadXorUInt32(BinaryReader reader, int xor = 0)
        {
            var data = reader.ReadBytes(4);
            if (xor != 0)
            {
                Array.Reverse(data);
                for (var i = 0; i < data.Length; i++) data[i] ^= (byte) xor;
            }

            return BitConverter.ToUInt32(data, 0);
        }

        internal static byte[] ReadXorBytes(BinaryReader reader, int size, int xor)
        {
            var data = reader.ReadBytes(size);
            if (xor != 0)
            {
                Array.Reverse(data);
                for (var i = 0; i < data.Length; i++) data[i] ^= (byte) xor;
            }

            return data;
        }
    }
}