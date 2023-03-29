/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Pigeonhole;

public class BmpPigeonholeException : Exception
{
    public BmpPigeonholeException(string message) : base(message) { }
    public BmpPigeonholeException(string message, Exception inner) : base(message, inner) { }
}