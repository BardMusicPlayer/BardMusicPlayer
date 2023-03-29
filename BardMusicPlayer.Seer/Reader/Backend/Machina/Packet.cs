/*
 * Copyright(c) 2022 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Reader.Backend.Machina;

internal partial class Packet : IDisposable
{
    private MachinaReaderBackend _machinaReader;

    internal Packet(MachinaReaderBackend machinaReader) { _machinaReader = machinaReader; }

    private static bool ValidTimeSig(byte timeSig) => timeSig is > 1 and < 8;

    private static bool ValidTempo(byte tempo) => tempo is > 29 and < 201;

    private Dictionary<ulong, uint> _contentId2ActorId = new();

    ~Packet() { Dispose(); }

    public void Dispose() { _contentId2ActorId.Clear(); }
}