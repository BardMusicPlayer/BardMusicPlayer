#region

using System;

#endregion

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