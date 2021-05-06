/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Notate.Processor.Utilities
{
    internal static partial class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="note"></param>
        /// <param name="tempoMap"></param>
        /// <returns></returns>
        internal static long GetNoteMs(this TimedEvent note, TempoMap tempoMap) => note.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds / 1000;
    }
}