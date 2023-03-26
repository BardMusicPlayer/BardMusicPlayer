/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Maestro.Sequencer.Player;

public partial class RemotePlayer : IPlayer
{
    public string UUID { get; private set; }

    public string Name { get; private set; }

    public string HomeWorld { get; private set; }

    PlayerType IPlayer.PlayerType => PlayerType.Remote;

    internal RemotePlayer()
    {
        UUID = Guid.NewGuid().ToString();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    bool IEquatable<IPlayer>.Equals(IPlayer other) => string.Equals(UUID, other.UUID);
}