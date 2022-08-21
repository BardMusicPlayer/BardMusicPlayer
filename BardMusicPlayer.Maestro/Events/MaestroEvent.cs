/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using BardMusicPlayer.Quotidian.UtcMilliTime;
using System;

namespace BardMusicPlayer.Maestro.Events
{
    public abstract class MaestroEvent
    {
        internal MaestroEvent(byte dedupeThreshold = 0, bool highPriority = false)
        {
            DedupeThreshold = dedupeThreshold;
            HighPriority = highPriority;
            TimeStamp = Clock.Time.Now;
        }

        public long TimeStamp { get; }

        internal byte DedupeThreshold { get; }

        internal bool HighPriority { get; }

        public Type EventType { get; protected set; }

        /// <summary>
        /// Used to determine if the Reader was able to successfully obtain the
        /// data it was expecting to grab, and the Event is safe to use.
        /// </summary>
        /// <returns>True, if the Event should be used to update data.</returns>
        public abstract bool IsValid();
    }
}
