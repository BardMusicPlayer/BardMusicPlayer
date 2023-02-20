using BardMusicPlayer.DryWetMidi.Core.Events.Base;
using BardMusicPlayer.DryWetMidi.Core.Events.Info;
using BardMusicPlayer.DryWetMidi.Core.Events.SysEx;

namespace BardMusicPlayer.DryWetMidi.Core.Events.Readers
{
    internal sealed class SysExEventReader : IEventReader
    {
        #region IEventReader

        public MidiEvent Read(MidiReader reader, ReadingSettings.ReadingSettings settings, byte currentStatusByte)
        {
            var size = reader.ReadVlqNumber();

            //

            SysExEvent sysExEvent = null;

            switch (currentStatusByte)
            {
                case EventStatusBytes.Global.NormalSysEx:
                    sysExEvent = new NormalSysExEvent();
                    break;
                case EventStatusBytes.Global.EscapeSysEx:
                    sysExEvent = new EscapeSysExEvent();
                    break;
            }

            //

            sysExEvent.Read(reader, settings, size);
            return sysExEvent;
        }

        #endregion
    }
}
