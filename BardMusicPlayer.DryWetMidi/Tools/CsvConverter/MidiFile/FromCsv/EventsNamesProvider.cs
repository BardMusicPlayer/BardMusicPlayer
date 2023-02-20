using System.Reflection;
using BardMusicPlayer.DryWetMidi.Tools.CsvConverter.MidiFile.RecordTypes;

namespace BardMusicPlayer.DryWetMidi.Tools.CsvConverter.MidiFile.FromCsv
{
    internal static class EventsNamesProvider
    {
        private static readonly Dictionary<MidiFileCsvLayout, string[]> EventsNames =
            new Dictionary<MidiFileCsvLayout, string[]>
            {
                [MidiFileCsvLayout.DryWetMidi] = GetEventsNames(typeof(DryWetMidiRecordTypes.Events)),
                [MidiFileCsvLayout.MidiCsv]    = GetEventsNames(typeof(MidiCsvRecordTypes.Events))
            };

        #region Methods

        public static string[] Get(MidiFileCsvLayout layout)
        {
            return EventsNames[layout];
        }

        private static string[] GetEventsNames(Type eventNamesClassType)
        {
            return eventNamesClassType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                      .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                                      .Select(fi => fi.GetValue(null).ToString())
                                      .ToArray();
        }

        #endregion
    }
}
