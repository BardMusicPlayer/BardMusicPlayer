/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Collections.Generic;

namespace BardMusicPlayer.Notate.Processor.Utilities
{
    internal static class Tools
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerCount"></param>
        /// <param name="lowClamp"></param>
        /// <param name="highClamp"></param>
        /// <returns></returns>
        internal static Dictionary<int, Dictionary<int, Dictionary<long, Note>>> GetEmptyPlayerNotesDictionary(int playerCount, int lowClamp = 12, int highClamp = 120)
        {
            var playerNotesDictionary = new Dictionary<int, Dictionary<int, Dictionary<long, Note>>>();

            for (var i = 0; i < playerCount; i++)
            {
                playerNotesDictionary.Add(i, GetEmptyNotesDictionary(lowClamp, highClamp));
            }
            return playerNotesDictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lowClamp"></param>
        /// <param name="highClamp"></param>
        /// <returns></returns>
        internal static Dictionary<int, Dictionary<long, Note>> GetEmptyNotesDictionary(int lowClamp = 12, int highClamp = 120)
        {
            if (lowClamp < 12 || highClamp > 120) throw new BmpNotateException("Clamp out of range.");
            var notesDictionary = new Dictionary<int, Dictionary<long, Note>>();
            for (var j = 0; j < 5; j++) notesDictionary[j] = new Dictionary<long, Note>();
            for (var j = lowClamp; j <= highClamp; j++) notesDictionary[j] = new Dictionary<long, Note>();
            return notesDictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static TempoMap GetMsTempoMap() => TempoMap.Create(new TicksPerQuarterNoteTimeDivision(600), Tempo.FromBeatsPerMinute(100));
    }
}
