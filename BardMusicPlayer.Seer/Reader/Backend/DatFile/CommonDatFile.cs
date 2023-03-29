/*
 * Copyright(c) 2022 MoogleTroupe, sammhill
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile;

internal class CommonDatFile : IDisposable
{

    internal bool Fresh = true;
    private string _filePath;
    private int startingByte = 1360; // Shared hotbar information starts here

    private List<BarInfo> _hotbarBarInformation = new();

    internal CommonDatFile(string filePath)
    {
        _filePath = filePath;
    }

    public bool Load()
    {
        if (string.IsNullOrEmpty(_filePath)) throw new Exception("No path to COMMON.DAT file provided.");
        if (!File.Exists(_filePath)) throw new Exception("Missing COMMON.DAT file.");

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

        reader.BaseStream.Seek(startingByte, SeekOrigin.Begin);

        for (var i = 1; i < 11; i++) // 10 hotbars, indexed at 1 to match data file and game
        {
            var currentBarBytes = reader.ReadBytes(0x12); // Hotbar information is in blocks of 18 bytes
            _hotbarBarInformation.Add(new BarInfo
            {
                HotbarNumber = i,
                IsShared     = currentBarBytes[16] == 0x31
            });
        }

        return true;
    }

    public List<BarInfo> GetBars() => _hotbarBarInformation.ToList();
    public List<BarInfo> GetSharedBars() => _hotbarBarInformation.Where(x => x.IsShared).ToList();
    public List<BarInfo> GetJobBars() => _hotbarBarInformation.Where(x => !x.IsShared).ToList();

    public void Dispose()
    {
        _hotbarBarInformation.Clear();
    }
}