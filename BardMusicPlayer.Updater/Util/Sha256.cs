using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace BardMusicPlayer.Updater.Util
{
    internal static class Sha256
    {
        internal static byte[] StringToBytes(string text)
        {
            text = text.ToUpper();
            var textArray = new string[text.Length / 2 + (text.Length % 2 == 0 ? 0 : 1)];
            for (var i = 0; i < textArray.Length; i++) textArray[ i ] = text.Substring(i * 2, i * 2 + 2 > text.Length ? 1 : 2);
            return textArray.Select(b => Convert.ToByte(b, 16)).ToArray();
        }

        internal static string GetChecksum(string fileName)
        {
            using var fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read);
            return GetChecksum(fileStream);
        }

        internal static string GetChecksum(byte[] fileBytes)
        {
            using var memoryStream = new MemoryStream(fileBytes);
            return GetChecksum(memoryStream);
        }

        internal static string GetChecksum(Stream stream)
        {
            using var bufferedStream = new BufferedStream(stream, 1024 * 32);
            var sha = new SHA256Managed();
            var checksum = sha.ComputeHash(bufferedStream);
            return BitConverter.ToString(checksum).Replace("-", string.Empty).ToLower();
        }
    }
}