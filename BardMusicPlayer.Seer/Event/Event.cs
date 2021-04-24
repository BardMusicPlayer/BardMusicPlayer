using System;

namespace BardMusicPlayer.Seer.Event
{
    internal abstract class Event
    {
        internal Event(Type eventType)
        {
            this.EventType = eventType;
        }
        public Type EventType { get; private set; }

        /// <summary>
        /// Used to determine if the Reader was able to successfully obtain the
        /// data it was expecting to grab, and the Event is safe to use.
        /// </summary>
        /// <returns>True, if the Event should be used to update data.</returns>
        public abstract bool IsValid();
    }
}
