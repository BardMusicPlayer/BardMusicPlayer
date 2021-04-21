using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using BardMusicPlayer.Synth.AlphaTab.IO;
using BardMusicPlayer.Synth.AlphaTab.Util;

namespace BardMusicPlayer.Synth.AlphaTab.CSharp.Platform
{
    internal static partial class Platform
    {


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
            var detectedEncoding = AlphaTab.Platform.Platform.DetectEncoding(data);
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
