/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Utilities
{
    internal static class SBitConverter
    {
        public static bool TryToBoolean(byte[] value, int index)
        {
            try
            {
                return BitConverter.ToBoolean(value, index);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static char TryToChar(byte[] value, int index)
        {
            try
            {
                return BitConverter.ToChar(value, index);
            }
            catch (Exception)
            {
                return '\0';
            }
        }

        public static double TryToDouble(byte[] value, int index)
        {
            try
            {
                return BitConverter.ToDouble(value, index);
            }
            catch (Exception)
            {
                return 0.0;
            }
        }

        public static long TryToDoubleToInt64Bits(double value)
        {
            try
            {
                return BitConverter.DoubleToInt64Bits(value);
            }
            catch (Exception)
            {
                return 0L;
            }
        }

        public static short TryToInt16(byte[] value, int index)
        {
            try
            {
                return BitConverter.ToInt16(value, index);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int TryToInt32(byte[] value, int index)
        {
            try
            {
                return BitConverter.ToInt32(value, index);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static long TryToInt64(byte[] value, int index)
        {
            try
            {
                return BitConverter.ToInt64(value, index);
            }
            catch (Exception)
            {
                return 0L;
            }
        }

        public static double TryToInt64BitsToDouble(long value)
        {
            try
            {
                return BitConverter.Int64BitsToDouble(value);
            }
            catch (Exception)
            {
                return 0.0;
            }
        }

        public static float TryToSingle(byte[] value, int index)
        {
            try
            {
                return BitConverter.ToSingle(value, index);
            }
            catch (Exception)
            {
                return 0f;
            }
        }

        public static string TryToString(byte[] value, int index)
        {
            try
            {
                return BitConverter.ToString(value, index);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static ushort TryToUInt16(byte[] value, int index)
        {
            try
            {
                return BitConverter.ToUInt16(value, index);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static uint TryToUInt32(byte[] value, int index)
        {
            try
            {
                return BitConverter.ToUInt32(value, index);
            }
            catch (Exception)
            {
                return 0u;
            }
        }

        public static ulong TryToUInt64(byte[] value, int index)
        {
            try
            {
                return BitConverter.ToUInt64(value, index);
            }
            catch (Exception)
            {
                return 0uL;
            }
        }
    }
}