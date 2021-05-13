/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Seer;

namespace BardMusicPlayer.Grunt
{
    public static partial class GameExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public static bool IsDalamudHooked(this Game game)
        {
            if (!BmpGrunt.Instance.Started) throw new BmpGruntException("Grunt not started.");
            return BmpGrunt.Instance.DalamudServer.IsConnected(game.Pid);
        }
    }
}
