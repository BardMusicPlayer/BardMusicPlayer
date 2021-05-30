/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Core.Enums;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Reader
{
    internal partial class Reader
    {
        public bool CanGetPerformance() => Scanner.Locations.ContainsKey(Signatures.PerformanceStatusKey);

        public Instrument GetPerformance()
        {
            var result = Instrument.None;
            if (!CanGetPerformance() || !MemoryHandler.IsAttached) return result;

            try
            {
                var performanceData = MemoryHandler.GetByteArray(Scanner.Locations[Signatures.PerformanceStatusKey],
                    MemoryHandler.Structures.PerformanceInfo.SourceSize);

                var status = (Performance.Status) performanceData[MemoryHandler.Structures.PerformanceInfo.Status];
                var instrument = Instrument.Parse(performanceData[MemoryHandler.Structures.PerformanceInfo.Instrument]);

                switch (status)
                {
                    case Performance.Status.Closed:
                    case Performance.Status.Loading:
                        return Instrument.None;

                    case Performance.Status.Opened:
                    case Performance.Status.SwitchingNote:
                    case Performance.Status.HoldingNote:
                        return instrument > Instrument.None ? instrument : Instrument.None;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                MemoryHandler?.RaiseException(ex);
            }

            return result;
        }
    }
}