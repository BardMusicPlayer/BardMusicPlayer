/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile.Objects
{
    internal class HotbarJobSlot : IDisposable
    {
        public Dictionary<int, HotbarSlot> JobSlots = new();

        public HotbarSlot this[int i] {
            get {
                if(!JobSlots.ContainsKey(i)) {
                    JobSlots[i] = new HotbarSlot();
                }
                return JobSlots[i];
            }
            set => JobSlots[i] = value;
        }

        ~HotbarJobSlot() => Dispose();
        public void Dispose()
        {
            if (JobSlots == null) return;
            foreach(var slot in JobSlots.Values) slot?.Dispose();
            JobSlots.Clear();
        }
    }
}
