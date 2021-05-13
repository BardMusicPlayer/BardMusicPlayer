/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Utilities
{
    internal static class ActorIdTools
    {
        internal static bool RangeOkay(uint actorId) => actorId >= 200000000 && actorId < 300000000;
        internal static bool RangeOkay(int actorId) => actorId >= 200000000 && actorId < 300000000;
    }
}
