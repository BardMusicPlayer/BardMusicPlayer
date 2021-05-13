/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Quotidian;

namespace BardMusicPlayer.Grunt
{
    public class BmpGruntException : BmpException
    {
        internal BmpGruntException() : base()
        {
        }
        internal BmpGruntException(string message) : base(message)
        {
        }
    }
}
