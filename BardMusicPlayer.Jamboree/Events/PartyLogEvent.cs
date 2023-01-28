﻿namespace BardMusicPlayer.Jamboree.Events
{
    /// <summary>
    /// Called only on host side
    /// </summary>
    public sealed class PartyLogEvent : JamboreeEvent
    {
        /// <summary>
        /// on host, when a party and token was created
        /// </summary>
        /// <param name="token"></param>
        internal PartyLogEvent(string logstring)
        {
            EventType = GetType();
            LogString = logstring;
        }

        /// <summary>
        /// the base64 token for the clients to join
        /// </summary>
        public string LogString { get; }

        public override bool IsValid() => true;
    }

}
