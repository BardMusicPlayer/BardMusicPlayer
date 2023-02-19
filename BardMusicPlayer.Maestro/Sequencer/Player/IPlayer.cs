/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Maestro.Sequencer.Player
{
    internal interface IPlayer : IDisposable, IEquatable<IPlayer>
    {
        string UUID { get; }
    }
}
