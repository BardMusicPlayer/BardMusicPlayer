/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Events;

public sealed class NetworkPacket : SeerEvent
{
    internal NetworkPacket(EventSource readerBackendType, byte[] message) : base(readerBackendType, 0, true)
    {
        EventType = GetType();
        Message   = message;
    }

    public byte[] Message { get; }
    public override bool IsValid() => true;
}