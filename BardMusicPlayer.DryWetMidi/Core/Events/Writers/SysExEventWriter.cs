using BardMusicPlayer.DryWetMidi.Common.DataTypes;
using BardMusicPlayer.DryWetMidi.Core.Events.Base;
using BardMusicPlayer.DryWetMidi.Core.Events.Info;

namespace BardMusicPlayer.DryWetMidi.Core.Events.Writers
{
    internal sealed class SysExEventWriter : IEventWriter
    {
        #region IEventWriter

        public void Write(MidiEvent midiEvent, MidiWriter writer, WritingSettings.WritingSettings settings, bool writeStatusByte)
        {
            if (writeStatusByte)
            {
                var statusByte = GetStatusByte(midiEvent);
                writer.WriteByte(statusByte);
            }

            //

            var contentSize = midiEvent.GetSize(settings);
            writer.WriteVlqNumber(contentSize);
            midiEvent.Write(writer, settings);
        }

        public int CalculateSize(MidiEvent midiEvent, WritingSettings.WritingSettings settings, bool writeStatusByte)
        {
            var contentSize = midiEvent.GetSize(settings);
            return (writeStatusByte ? 1 : 0) + contentSize.GetVlqLength() + contentSize;
        }

        public byte GetStatusByte(MidiEvent midiEvent)
        {
            switch (midiEvent.EventType)
            {
                case MidiEventType.NormalSysEx:
                    return EventStatusBytes.Global.NormalSysEx;
                case MidiEventType.EscapeSysEx:
                    return EventStatusBytes.Global.EscapeSysEx;
            }

            return 0;
        }

        #endregion
    }
}
