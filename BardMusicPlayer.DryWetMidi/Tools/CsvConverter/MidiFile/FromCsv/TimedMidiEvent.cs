using BardMusicPlayer.DryWetMidi.Core.Events.Base;
using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan;

namespace BardMusicPlayer.DryWetMidi.Tools.CsvConverter.MidiFile.FromCsv
{
    internal sealed class TimedMidiEvent
    {
        #region Constructor

        public TimedMidiEvent(ITimeSpan time, MidiEvent midiEvent)
        {
            Time = time;
            Event = midiEvent;
        }

        #endregion

        #region Properties

        public ITimeSpan Time { get; }

        public MidiEvent Event { get; }

        #endregion
    }
}
