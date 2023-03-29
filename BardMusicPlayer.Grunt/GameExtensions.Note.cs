/*
 * Copyright(c) 2023 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian.Enums;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer;

namespace BardMusicPlayer.Grunt;

public static partial class GameExtensions
{
    /// <summary>
    /// Pushes a note in OctaveRange.C3toC6
    /// </summary>
    /// <param name="game"></param>
    /// <param name="note"></param>
    /// <param name="channel"></param>
    /// <returns></returns>
    public static async Task<bool> SendNoteOn(this Game game, int note, int channel)
    {
        if (!BmpGrunt.Instance.Started) throw new BmpGruntException("Grunt not started.");

        if (!OctaveRange.C3toC6.ValidateNoteRange(note))
#if DEBUG
            throw new BmpGruntException("Note is not in C3toC6 range.");
#else
            return false;
#endif

        if (game.InstrumentHeld.Equals(Instrument.None) || game.ChatStatus || !game.IsBard) return false;

        note -= 48;

        // TODO: Tone select for global region here
        // if (game.GameRegion == GameRegion.Global && !await SyncTapKey(game, game.InstrumentToneMenuKeys[(InstrumentToneMenuKey) channel], BmpPigeonhole.Instance.ToneKeyDelay, BmpPigeonhole.Instance.ToneKeyDelay)) return false;

        return await SyncPushKey(game, game.NoteKeys[(NoteKey) note], BmpPigeonhole.Instance.NoteKeyDelay);
    }

    /// <summary>
    /// Releases a note in OctaveRange.C3toC6
    /// </summary>
    /// <param name="game"></param>
    /// <param name="note"></param>
    /// <returns></returns>
    public static async Task<bool> SendNoteOff(this Game game, int note)
    {
        if (!BmpGrunt.Instance.Started) throw new BmpGruntException("Grunt not started.");

        if (!OctaveRange.C3toC6.ValidateNoteRange(note))
#if DEBUG
            throw new BmpGruntException("Note is not in C3toC6 range.");
#else
            return false;
#endif

        if (game.InstrumentHeld.Equals(Instrument.None) || game.ChatStatus || !game.IsBard) return false;

        note -= 48;

        return await SyncReleaseKey(game, game.NoteKeys[(NoteKey) note], BmpPigeonhole.Instance.NoteKeyDelay);
    }
}