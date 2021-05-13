/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.IO;
using BardMusicPlayer.Quotidian.Enums;
using BardMusicPlayer.Seer.Reader.Backend.DatFile.Objects;
using BardMusicPlayer.Seer.Reader.Backend.DatFile.Utilities;

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile {
    internal class KeybindDatFile : IDisposable {

        internal bool Fresh = true;
        private string _filePath;
        internal KeybindDatFile(string filePath)
        {
            _filePath = filePath;
        }

        internal bool Load()
        {
            if (string.IsNullOrEmpty(_filePath)) throw new FileFormatException("No path to KEYBIND.DAT file provided.");

            if (!File.Exists(_filePath)) throw new FileFormatException("Missing KEYBIND.DAT file.");

            using var fileStream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var memStream = new MemoryStream();
            if (fileStream.CanRead && fileStream.CanSeek)
            {
                fileStream.CopyTo(memStream);
            }

            fileStream.Dispose();
            if (memStream.Length == 0)
            {
                memStream.Dispose();
                return false;
            }

            using var reader = new BinaryReader(memStream);
            reader.BaseStream.Seek(0x04, SeekOrigin.Begin);

            var fileSize = XorTools.ReadXorInt32(reader);
            var dataSize = XorTools.ReadXorInt32(reader) + 16;

            var sourceSize = reader.BaseStream.Length;

            if (sourceSize - fileSize != 32)
            {
                reader.Dispose();
                memStream.Dispose();
                throw new FileFormatException("Invalid KEYBIND.DAT size.");
            }

            reader.BaseStream.Seek(0x60, SeekOrigin.Begin);
            try
            {
                reader.BaseStream.Seek(0x11, SeekOrigin.Begin);
                while (reader.BaseStream.Position < dataSize)
                {
                    var command = ParseSection(reader);
                    var keybind = ParseSection(reader);

                    var key = System.Text.Encoding.UTF8.GetString(command.Data);
                    key = key.Substring(0, key.Length - 1); // Trim off \0
                    var dat = System.Text.Encoding.UTF8.GetString(keybind.Data);
                    var datKeys = dat.Split(',');
                    if (datKeys.Length != 3) continue;
                    var key1 = datKeys[0].Split('.');
                    var key2 = datKeys[1].Split('.');
                    KeybindList.Add(key, new Keybind
                    {
                        MainKey1 = int.Parse(key1[0], System.Globalization.NumberStyles.HexNumber),
                        MainKey2 = int.Parse(key2[0], System.Globalization.NumberStyles.HexNumber),
                        ModKey1 = int.Parse(key1[1], System.Globalization.NumberStyles.HexNumber),
                        ModKey2 = int.Parse(key2[1], System.Globalization.NumberStyles.HexNumber),
                    });
                }
            }
            catch (Exception ex)
            {
                throw new FileFormatException("Invalid HOTBAR.DAT format: " + ex.Message);
            }
            finally
            {
                reader.Dispose();
                memStream.Dispose();
            }
            return true;
        }

        public readonly Dictionary<string, Keybind> KeybindList = new();
		public Keybind this[string key] => !KeybindList.ContainsKey(key) ? new Keybind() : KeybindList[key];

        public Keys GetKeybindFromKeyString(string nk) => !string.IsNullOrEmpty(nk) ? this[nk].GetKey() : Keys.None;

        private static KeybindSection ParseSection(BinaryReader stream) {
			var headerBytes = XorTools.ReadXorBytes(stream, 3, 0x73);
			var section = new KeybindSection {
				Type = headerBytes[0],
				Size = headerBytes[1],
			};
			section.Data = XorTools.ReadXorBytes(stream, section.Size, 0x73);
			Array.Reverse(section.Data);
			return section;
		}
        ~KeybindDatFile() => Dispose();
        public void Dispose()
        {
            if (KeybindList == null) return;
            foreach(var keyBind in KeybindList.Values) keyBind?.Dispose();
            KeybindList.Clear();
        }
    }
}