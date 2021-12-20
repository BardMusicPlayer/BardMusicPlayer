/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
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
            var exitLockWaitInMs = 1000 / exitLock;

            while (exitLock > 0)
            {
                if (game.InstrumentHeld.Equals(instrumentWanted)) return true;

                if (!instrumentWanted.Equals(Instrument.None) && game.InstrumentHeld.Equals(Instrument.None))
                {
                    await SyncTapKey(game, game.InstrumentKeys[instrumentWanted]);
                    await Task.Delay(exitLockWaitInMs);
                }
                else
                {
                    await SyncTapKey(game, game.NavigationMenuKeys[NavigationMenuKey.ESC]);
                    await Task.Delay(exitLockWaitInMs);
                }

                exitLock--;
            }

            return game.InstrumentHeld.Equals(instrumentWanted);
        }

        public static bool EquipInstrumentSync(this Game game, Instrument instrumentWanted)
        {
            return Task.Run(() => EquipInstrument(game, instrumentWanted)).GetAwaiter().GetResult();
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
            if ((int) game.GameRegion < 4)
                throw new BmpGruntException("Equipping a Tone is not supported in region " + game.GameRegion);

            var exitLock = 5;

            while (exitLock > 0)
            {
                if (game.InstrumentToneHeld.Equals(instrumentToneWanted)) return true;

                if (!instrumentToneWanted.Equals(InstrumentTone.None) &&
                    game.InstrumentToneHeld.Equals(InstrumentTone.None))
                    await SyncTapKey(game, game.InstrumentToneKeys[instrumentToneWanted]);
                else
                    await SyncTapKey(game, game.NavigationMenuKeys[NavigationMenuKey.ESC]);
                exitLock--;
            }

            return game.InstrumentToneHeld.Equals(instrumentToneWanted);
        }


        public static Dictionary<int, Keys> GuitarKeyMap = new Dictionary<int, Keys> {
            { 27, Keys.OemSemicolon }, // ElectricGuitarClean
			{ 28, Keys.Oem2 }, // ElectricGuitarMuted
			{ 29, Keys.Oem3 }, // ElectricGuitarOverdriven			
			{ 30, Keys.Oem6 }, // ElectricGuitarPowerChords
			{ 31, Keys.Oem7 }, // ElectricGuitarSpecial*/
		};

        /// <summary>
        /// Switches the guitar tone by programnumber
        /// </summary>
        /// <param name="game"></param>
        /// <param name="prognumber"></param>
        /// <returns></returns>
        public static async Task<bool> GuitarByPrognumber(this Game game, int prognumber)
        {
            if ((prognumber < 27) || (prognumber > 31))
                return false;

            return await SyncTapKey(game, GuitarKeyMap[prognumber]);
        }

    }
}