/*
 * Copyright(c) 2017 Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Config.JsonSettings.Modulation
{
    public class ModularityException : Exception
    {
        public ModularityException() { }
        public ModularityException(string message) : base(message) { }
        public ModularityException(string message, Exception inner) : base(message, inner) { }
    }
}