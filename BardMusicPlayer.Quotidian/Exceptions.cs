/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Quotidian;

public class BmpException : Exception
{
    public BmpException()
    {
    }

    public BmpException(string message) : base(message)
    {
    }

    public BmpException(string message, Exception inner) : base(message, inner)
    {
    }
}