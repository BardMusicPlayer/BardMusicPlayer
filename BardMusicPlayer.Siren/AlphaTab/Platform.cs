/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using BardMusicPlayer.Siren.AlphaTab.IO;
using BardMusicPlayer.Siren.AlphaTab.Util;

namespace BardMusicPlayer.Siren.AlphaTab
{
    internal static class Platform
    {
        public static bool IsStringNumber(string s, bool allowSign = true)
        {
            if (s.Length == 0)
            {
                return false;
            }

            var c = s[0];
            return IsCharNumber(c, allowSign);
        }

        public static bool IsCharNumber(int c, bool allowSign = true)
        {
            return allowSign && c == 0x2D || c >= 0x30 && c <= 0x39;
        }

        public static bool IsWhiteSpace(int c)
        {
            return c == 0x20 || c == 0x0B || c == 0x0D || c == 0x0A || c == 0x09;
        }

        public static bool IsAlmostEqualTo(this float a, float b)
        {
            return Math.Abs(a - b) < 0.00001f;
        }

        public static string ToHexString(int n, int digits = 0)
        {
            var s = "";
            const string hexChars = "0123456789ABCDEF";
            do
            {
                s = StringFromCharCode((int)hexChars[n & 15]) + s;
                n >>= 4;
            } while (n > 0);

            while (s.Length < digits)
            {
                s = "0" + s;
            }

            return s;
        }

        public static uint ToUInt32(int i)
        {
            return (uint)i;
        }

        public static short ToInt16(int i)
        {
            return (short)i;
        }

        public static ushort ToUInt16(int i)
        {
            return (ushort)i;
        }

        public static byte ToUInt8(int i)
        {
            return (byte)i;
        }

        internal static string DetectEncoding(byte[] data)
        {
            if (data.Length > 2 && data[0] == 0xFE && data[1] == 0xFF)
            {
                return "utf-16be";
            }

            if (data.Length > 2 && data[0] == 0xFF && data[1] == 0xFE)
            {
                return "utf-16le";
            }

            if (data.Length > 4 && data[0] == 0x00 && data[1] == 0x00 && data[2] == 0xFE && data[3] == 0xFF)
            {
                return "utf-32be";
            }

            if (data.Length > 4 && data[0] == 0xFF && data[1] == 0xFE && data[2] == 0x00 && data[3] == 0x00)
            {
                return "utf-32le";
            }

            return null;
        }

        public static void Log(LogLevel logLevel, string category, string msg, object details = null)
        {
            Trace.WriteLine($"[AlphaTab][{category}][{logLevel}] {msg} {details}", "AlphaTab");
        }

        public static float Log2(float s)
        {
            return (float)Math.Log(s, 2);
        }

        public static int ParseInt(string s)
        {
            float f;
            if (!float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out f))
            {
                return int.MinValue;
            }

            return (int)f;
        }

        public static int[] CloneArray(int[] array)
        {
            return (int[])array.Clone();
        }

        public static void BlockCopy(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        {
            Buffer.BlockCopy(src, srcOffset, dst, dstOffset, count);
        }

        public static string StringFromCharCode(int c)
        {
            return ((char)c).ToString();
        }
        
        public static sbyte ReadSignedByte(this IReadable readable)
        {
            return unchecked((sbyte)(byte)readable.ReadByte());
        }

        public static string ToString(byte[] data, string encoding)
        {
            var detectedEncoding = Platform.DetectEncoding(data);
            if (detectedEncoding != null)
            {
                encoding = detectedEncoding;
            }

            if (encoding == null)
            {
                encoding = "utf-8";
            }

            Encoding enc;
            try
            {
                enc = Encoding.GetEncoding(encoding);
            }
            catch
            {
                enc = Encoding.UTF8;
            }

            return enc.GetString(data, 0, data.Length);
        }
        
        public static void ClearIntArray(int[] array)
        {
            Array.Clear(array, 0, array.Length);
        }
        
        public static void ArrayCopy<T>(T[] src, int srcOffset, T[] dst, int dstOffset, int count)
        {
            Array.Copy(src, srcOffset, dst, dstOffset, count);
        }
        
        public static long GetCurrentMilliseconds()
        {
            return Stopwatch.GetTimestamp();
        }
    }
}
