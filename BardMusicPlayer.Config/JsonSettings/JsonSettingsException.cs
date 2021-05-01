/*
 * Copyright(c) 2017 Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Config.JsonSettings
{
    public class JsonSettingsException : Exception
    {
        public JsonSettingsException() { }
        public JsonSettingsException(string message) : base(message) { }
        public JsonSettingsException(string message, Exception inner) : base(message, inner) { }
    }
}