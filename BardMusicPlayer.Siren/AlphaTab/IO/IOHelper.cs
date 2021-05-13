/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Siren.AlphaTab.Collections;

namespace BardMusicPlayer.Siren.AlphaTab.IO
{
    internal static class IOHelper
    {

        public static uint ReadUInt32LE(this IReadable input)
        {
            var ch1 = input.ReadByte();
            var ch2 = input.ReadByte();
            var ch3 = input.ReadByte();
            var ch4 = input.ReadByte();

            return Platform.ToUInt32((ch4 << 24) | (ch3 << 16) | (ch2 << 8) | ch1);
        }

        public static ushort ReadUInt16LE(this IReadable input)
        {
            var ch1 = input.ReadByte();
            var ch2 = input.ReadByte();

            return Platform.ToUInt16((ch2 << 8) | ch1);
        }

        public static short ReadInt16LE(this IReadable input)
        {
            var ch1 = input.ReadByte();
            var ch2 = input.ReadByte();

            return Platform.ToInt16((ch2 << 8) | ch1);
        }



        public static string Read8BitStringLength(this IReadable input, int length)
        {
            var s = new StringBuilder();
            var z = -1;
            for (var i = 0; i < length; i++)
            {
                var c = input.ReadByte();
                if (c == 0 && z == -1)
                {
                    z = i;
                }

                s.AppendChar(c);
            }

            var t = s.ToString();
            if (z >= 0)
            {
                return t.Substring(0, z);
            }

            return t;
        }


    }
}
