/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer.Reader.Backend.DatFile.Objects;
using BardMusicPlayer.Seer.Reader.Backend.DatFile.Utilities;

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile {
    internal class HotbarDatFile : IDisposable {

        internal bool Fresh = true;
        private string _filePath;

        internal HotbarDatFile(string filePath)
        {
            _filePath = filePath;
        }

        internal bool Load()
        {
            if (string.IsNullOrEmpty(_filePath)) throw new FileFormatException("No path to HOTBAR.DAT file provided.");
            if (!File.Exists(_filePath)) throw new FileFormatException("Missing HOTBAR.DAT file.");

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
                throw new FileFormatException("Invalid HOTBAR.DAT size.");
            }

            reader.BaseStream.Seek(0x60, SeekOrigin.Begin);
            try
            {
                reader.BaseStream.Seek(0x10, SeekOrigin.Begin);
                while (reader.BaseStream.Position < dataSize)
                {
                    var ac = ParseSection(reader);
                    if (ac.Job <= 0x23)
                    {
                        if (ac.Type == 0x1D)
                        {
                            //Console.WriteLine(string.Format("{0} ({1}): {2} {3}", ac.ToString(), ac.job, ac.action, ac.type));
                        }
                        _hotbarData[ac.Hotbar][ac.Slot][ac.Job] = ac;
                    }
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

        private readonly HotbarData _hotbarData = new();

		public List<HotbarSlot> GetSlotsFromType(SlotType type) => GetSlotsFromType((int) type);
		public List<HotbarSlot> GetSlotsFromType(int type) => (from row in _hotbarData.Rows.Values from jobSlot in row.Slots.Values from slot in jobSlot.JobSlots.Values where slot.Type == type select slot).ToList();

        internal enum SlotType
        {
			Unknown,
			Instrument = 0x1D,
            InstrumentTone = Unknown
        }

        public string GetInstrumentToneKeyMap(InstrumentTone instrumentTone) {
            var slots = GetSlotsFromType(SlotType.InstrumentTone);
            foreach (var slot in slots.Where(slot => slot.Action == instrumentTone))
            {
                return slot.ToString();
            }
            return string.Empty;
        }

		public string GetInstrumentKeyMap(Instrument instrument) {
            var slots = GetSlotsFromType(SlotType.Instrument);
			foreach (var slot in slots.Where(slot => slot.Action == instrument))
            {
                return slot.ToString();
            }
			return string.Empty;
		}

		private static HotbarSlot ParseSection(BinaryReader stream) {
			const byte xor = 0x31;
			var ac = new HotbarSlot {
				Action = XorTools.ReadXorByte(stream, xor),
				Flag = XorTools.ReadXorByte(stream, xor),
				Unk1 = XorTools.ReadXorByte(stream, xor),
				Unk2 = XorTools.ReadXorByte(stream, xor),
				Job = XorTools.ReadXorByte(stream, xor),
				Hotbar = XorTools.ReadXorByte(stream, xor),
				Slot = XorTools.ReadXorByte(stream, xor),
				Type = XorTools.ReadXorByte(stream, xor),
			};
			return ac;
		}
        ~HotbarDatFile() => Dispose();
        public void Dispose()
        {
            _hotbarData?.Dispose();
        }
    }
}