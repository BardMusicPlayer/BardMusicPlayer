/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Threading.Tasks;
using BardMusicPlayer.Quotidian.Enums;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer;

namespace BardMusicPlayer.Grunt
{
    public static partial class GameExtensions
    {
        /// <summary>
        /// Equips an instrument
        /// </summary>
        /// <param name="game"></param>
        /// <param name="instrumentWanted"></param>
        /// <returns></returns>
        public static async Task<bool> EquipInstrument(this Game game, Instrument instrumentWanted)
        {
            if (!BmpGrunt.Instance.Started) throw new BmpGruntException("Grunt not started.");

            if (!game.IsBard) return false;

            // TODO, call EquipTone for regions that no longer support this.
            // if (game.GameRegion == GameRegion.Global) return await EquipTone(game, instrument.InstrumentTone);

            var exitLock = 5;

            while (exitLock > 0)
            {
                if (game.InstrumentHeld.Equals(instrumentWanted)) return true;

                if (!instrumentWanted.Equals(Instrument.None) && game.InstrumentHeld.Equals(Instrument.None))
                {
                    await SyncTapKey(game, game.InstrumentKeys[instrumentWanted]);
                    await Task.Delay(1000);
                }
                else
                {
                    await SyncTapKey(game, game.NavigationMenuKeys[NavigationMenuKey.ESC]);
                    await Task.Delay(1000);
                }
                exitLock--;
            }

            return game.InstrumentHeld.Equals(instrumentWanted);
        }

        /// <summary>
        /// Equips an instrument tone
        /// </summary>
        /// <param name="game"></param>
        /// <param name="instrumentToneWanted"></param>
        /// <returns></returns>
        public static async Task<bool> EquipTone(this Game game, InstrumentTone instrumentToneWanted)
        {
            if (!BmpGrunt.Instance.Started) throw new BmpGruntException("Grunt not started.");

            if (!game.IsBard) return false;

            // TODO for 5.55
            if ((int) game.GameRegion < 4) throw new BmpGruntException("Equipping a Tone is not supported in region " + game.GameRegion);

            var exitLock = 5;

            while (exitLock > 0)
            {
                if (game.InstrumentToneHeld.Equals(instrumentToneWanted)) return true;

                if (!instrumentToneWanted.Equals(InstrumentTone.None) && game.InstrumentToneHeld.Equals(InstrumentTone.None))
                {
                    await SyncTapKey(game, game.InstrumentToneKeys[instrumentToneWanted]);
                }
                else
                {
                    await SyncTapKey(game, game.NavigationMenuKeys[NavigationMenuKey.ESC]);
                }
                exitLock--;
            }

            return game.InstrumentToneHeld.Equals(instrumentToneWanted);
        }
    }
}
