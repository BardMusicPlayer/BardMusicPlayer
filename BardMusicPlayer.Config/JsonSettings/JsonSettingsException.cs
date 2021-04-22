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