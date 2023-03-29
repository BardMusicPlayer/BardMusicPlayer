/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Script.BasicSharp;

internal sealed class BasicException : Exception
{
    public int line;
    public BasicException()
    {
    }

    public BasicException(string message, int line)
        : base(message)
    {
        this.line = line;
    }

    public BasicException(string message, int line, Exception inner)
        : base(message, inner)
    {
        this.line = line;
    }
}