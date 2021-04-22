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