/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Seer.Utilities;

internal static class ActorIdTools
{
    internal static bool RangeOkay(uint actorId) => actorId is >= 200000000 and < 300000000;

    internal static bool RangeOkay(int actorId) => actorId is >= 200000000 and < 300000000;
}