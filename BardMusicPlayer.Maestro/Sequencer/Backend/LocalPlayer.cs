/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Maestro.Sequencer.Backend
{
    public partial class LocalPlayer : IPlayer
    {
        public string UUID { get; private set; }

        internal LocalPlayer()
        {
            UUID = Guid.NewGuid().ToString();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        bool IEquatable<IPlayer>.Equals(IPlayer other) => string.Equals(UUID, other.UUID);
    }
}
