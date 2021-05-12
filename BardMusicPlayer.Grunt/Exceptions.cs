/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Common;

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
