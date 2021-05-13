﻿/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Siren.AlphaTab.Model;

namespace BardMusicPlayer.Siren.AlphaTab.Audio
{
    internal static class MidiUtils
    {
        /// <summary>
        /// The amount of ticks per quarter note used within this midi system.
        /// (Pulses Per Quarter Note)
        /// </summary>
        public const int QuarterTime = 960;

        private const int MinVelocity = 15;
        private const int VelocityIncrement = 16;

        /// <summary>
        /// Converts the given midi tick duration into milliseconds. 
        /// </summary>
        /// <param name="ticks">The duration in midi ticks</param>
        /// <param name="tempo">The current tempo in BPM.</param>
        /// <returns>The converted duration in milliseconds. </returns>
        public static int TicksToMillis(int ticks, int tempo)
        {
            return (int)(ticks * (60000.0 / (tempo * QuarterTime)));
        }

        /// <summary>
        /// Converts the given midi tick duration into milliseconds. 
        /// </summary>
        /// <param name="millis">The duration in milliseconds</param>
        /// <param name="tempo">The current tempo in BPM.</param>
        /// <returns>The converted duration in midi ticks. </returns>
        public static int MillisToTicks(int millis, int tempo)
        {
            return (int)(millis / (60000.0 / (tempo * QuarterTime)));
        }

        /// <summary>
        /// Converts a duration value to its ticks equivalent.
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static int ToTicks(this Duration duration)
        {
            return ValueToTicks((int)duration);
        }

        /// <summary>
        /// Converts a numerical value to its ticks equivalent.
        /// </summary>
        /// <param name="duration">the numerical proportion to convert. (i.E. timesignature denominator, note duration,...)</param>
        /// <returns></returns>
        public static int ValueToTicks(int duration)
        {
            float denomninator = duration;
            if (denomninator < 0)
            {
                denomninator = 1 / -denomninator;
            }

            return (int)(QuarterTime * (4.0 / denomninator));
        }

        public static int ApplyDot(int ticks, bool doubleDotted)
        {
            if (doubleDotted)
            {
                return ticks + ticks / 4 * 3;
            }

            return ticks + ticks / 2;
        }

        public static int ApplyTuplet(int ticks, int numerator, int denominator)
        {
            return ticks * denominator / numerator;
        }

        public static int RemoveTuplet(int ticks, int numerator, int denominator)
        {
            return ticks * numerator / denominator;
        }

        public static int DynamicToVelocity(DynamicValue dyn)
        {
            return MinVelocity + (int)dyn * VelocityIncrement;
        }
    }
}
