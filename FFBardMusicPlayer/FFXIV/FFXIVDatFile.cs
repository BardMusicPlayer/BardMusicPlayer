using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace FFBardMusicPlayer.FFXIV
{
    // Key/Keybind - the actual key to simulate
// PerfKey/pk - PERFORMANCE_MODE_ key to get the keybind
// NoteKey/nk/byte - C+1, C#, etc
// byte key - the raw midi note byte

    public class FFXIVDatFile
    {
        public EventHandler OnFileLoad;
        private readonly FileSystemWatcher fileWatcher = new FileSystemWatcher();
        private MemoryStream memStream = new MemoryStream();
        protected DatHeader Header = new DatHeader();

        protected class DatHeader
        {
            public int FileSize;
            public int DataSize;
        };

        public FFXIVDatFile() { fileWatcher.Changed += OnDatFileChange; }

        protected void OnDatFileChange(object o, FileSystemEventArgs e)
        {
            Console.WriteLine($"Change, reload [{e.Name}].");
            LoadDat(e.FullPath);
        }

        public static List<string> GetIdList()
        {
            var ids = new List<string>();
            var doc = FFXIVDocsResolver.GetPath();
            var dirPath = Path.Combine(new[] { doc, "My Games" });
            foreach (var dir in Directory.GetDirectories(dirPath, "FINAL FANTASY XIV - *"))
            {
                foreach (var dir2 in Directory.GetDirectories(dir, "FFXIV_CHR*", SearchOption.TopDirectoryOnly))
                {
                    var ffxivDir = Path.GetFileName(dir2);
                    ids.Add(ffxivDir);
                }
            }

            return ids;
        }

        protected string FindFFXIVDatFile(string charId, string file)
        {
            var doc = FFXIVDocsResolver.GetPath();
            var dirPath = Path.Combine(new[] { doc, "My Games" });
            foreach (var dir in Directory.GetDirectories(dirPath, "FINAL FANTASY XIV - *"))
            {
                var path = Path.Combine(new[] { dir, charId, file });
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return string.Empty;
        }

        public void LoadDatId(string charId, string file) { LoadDat(FindFFXIVDatFile(charId, file)); }

        protected void LoadDat(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            fileWatcher.EnableRaisingEvents = false;
            if (File.Exists(filePath))
            {
                // Check for open file
                Stream fileStream;
                while (true)
                {
                    try
                    {
                        fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        break;
                    }
                    catch (Exception)
                    {
                    }

                    Thread.Sleep(100);
                }

                memStream = new MemoryStream();
                if (fileStream.CanRead && fileStream.CanSeek)
                {
                    fileStream.CopyTo(memStream);
                    fileStream.Close();
                }

                if (memStream != null && memStream.Length != 0)
                {
                    using (var reader = new BinaryReader(memStream))
                    {
                        ParseDat(reader);
                    }
                }

                fileWatcher.Path                = Path.GetDirectoryName(filePath);
                fileWatcher.Filter              = Path.GetFileName(filePath);
                fileWatcher.EnableRaisingEvents = true;

                OnFileLoad?.Invoke(this, EventArgs.Empty);
            }
        }

        protected virtual bool ParseDat(BinaryReader stream)
        {
            stream.BaseStream.Seek(0x04, SeekOrigin.Begin);
            Header = new DatHeader
            {
                FileSize = XorTools.ReadXorInt32(stream),
                DataSize = XorTools.ReadXorInt32(stream) + 16
            };
            return stream.BaseStream.Length - Header.FileSize == 32;
        }
    }

    public static class XorTools
    {
        public static byte ReadXorByte(BinaryReader reader, int xor = 0) => (byte) (reader.ReadByte() ^ (byte) xor);

        public static short ReadXorInt16(BinaryReader reader, int xor = 0)
        {
            var data = reader.ReadBytes(2);
            if (xor != 0)
            {
                Array.Reverse(data);
                for (var i = 0; i < data.Length; i++)
                {
                    data[i] ^= (byte) xor;
                }
            }

            return BitConverter.ToInt16(data, 0);
        }

        public static int ReadXorInt32(BinaryReader reader, int xor = 0)
        {
            var data = reader.ReadBytes(4);
            if (xor != 0)
            {
                Array.Reverse(data);
                for (var i = 0; i < data.Length; i++)
                {
                    data[i] ^= (byte) xor;
                }
            }

            return BitConverter.ToInt32(data, 0);
        }

        public static uint ReadXorUInt32(BinaryReader reader, int xor = 0)
        {
            var data = reader.ReadBytes(4);
            if (xor != 0)
            {
                Array.Reverse(data);
                for (var i = 0; i < data.Length; i++)
                {
                    data[i] ^= (byte) xor;
                }
            }

            return BitConverter.ToUInt32(data, 0);
        }

        public static byte[] ReadXorBytes(BinaryReader reader, int size, int xor)
        {
            var data = reader.ReadBytes(size);
            if (xor != 0)
            {
                Array.Reverse(data);
                for (var i = 0; i < data.Length; i++)
                {
                    data[i] ^= (byte) xor;
                }
            }

            return data;
        }
    }
}