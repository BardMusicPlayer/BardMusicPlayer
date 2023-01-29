/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Quotidian;

namespace BardMusicPlayer.Script
{
    public sealed class BmpScriptException : BmpException
    {
        internal BmpScriptException()
        {
        }
        internal BmpScriptException(string message) : base(message)
        {
        }
    }
}