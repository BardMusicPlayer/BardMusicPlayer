/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Quotidian.Enums;
using BardMusicPlayer.Seer;

namespace BardMusicPlayer.Grunt;

public static partial class GameExtensions
{
    /// <summary>
    /// Blindly pushes buttons to request ensemble mode. Task takes approximately 700 milliseconds to complete.
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    public static async Task<bool> RequestEnsemble(this Game game)
    {
        if (!BmpGrunt.Instance.Started) throw new BmpGruntException("Grunt not started.");

        await Task.Delay(200);
        if (!await SyncTapKey(game, game.NavigationMenuKeys[NavigationMenuKey.VIRTUAL_PAD_SELECT])) return false;
        if (!await SyncTapKey(game, game.NavigationMenuKeys[NavigationMenuKey.LEFT])) return false;
        if (!await SyncTapKey(game, game.NavigationMenuKeys[NavigationMenuKey.OK])) return false;

        await Task.Delay(200);
        if (!await SyncTapKey(game, game.NavigationMenuKeys[NavigationMenuKey.OK])) return false;

        await Task.Delay(200);
        return await SyncTapKey(game, game.NavigationMenuKeys[NavigationMenuKey.OK]);
    }

    /// <summary>
    /// Blindly pushes buttons to accept ensemble mode. Task takes approximately 450 milliseconds to complete.
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    public static async Task<bool> AcceptEnsemble(this Game game)
    {
        if (!BmpGrunt.Instance.Started) throw new BmpGruntException("Grunt not started.");

        await Task.Delay(200);
        if (!await SyncTapKey(game, game.NavigationMenuKeys[NavigationMenuKey.OK])) return false;

        await Task.Delay(200);
        return await SyncTapKey(game, game.NavigationMenuKeys[NavigationMenuKey.OK]);
    }
}