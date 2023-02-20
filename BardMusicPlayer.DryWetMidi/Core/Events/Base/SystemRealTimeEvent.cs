namespace BardMusicPlayer.DryWetMidi.Core.Events.Base
{
    /// <summary>
    /// Represents a system real-time event.
    /// </summary>
    /// <remarks>
    /// MIDI system realtime messages are messages that are not specific to a MIDI channel but
    /// prompt all devices on the MIDI system to respond and to do so in real time.
    /// </remarks>
    public abstract class SystemRealTimeEvent : MidiEvent
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemRealTimeEvent"/> with the specified event type.
        /// </summary>
        /// <param name="eventType">The type of event.</param>
        protected SystemRealTimeEvent(MidiEventType eventType)
            : base(eventType)
        {
        }

        #endregion

        #region Overrides

        internal sealed override void Read(MidiReader reader, ReadingSettings.ReadingSettings settings, int size)
        {
        }

        internal sealed override void Write(MidiWriter writer, WritingSettings.WritingSettings settings)
        {
        }

        internal sealed override int GetSize(WritingSettings.WritingSettings settings)
        {
            return 0;
        }

        #endregion
    }
}
