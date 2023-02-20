﻿using BardMusicPlayer.DryWetMidi.Common;
using BardMusicPlayer.DryWetMidi.Core;
using BardMusicPlayer.DryWetMidi.Interaction.TempoMap;
using BardMusicPlayer.DryWetMidi.Tools.Splitter.Settings;

namespace BardMusicPlayer.DryWetMidi.Tools.Splitter
{
    public static partial class Splitter
    {
        #region Methods

        /// <summary>
        /// Splits <see cref="MidiFile"/> by chunks within it.
        /// </summary>
        /// <param name="settings">Settings accoridng to which MIDI file should be split.</param>
        /// <param name="midiFile"><see cref="MidiFile"/> to split.</param>
        /// <returns>Collection of <see cref="MidiFile"/> where each file contains single chunk from
        /// the original file.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="midiFile"/> is <c>null</c>.</exception>
        public static IEnumerable<MidiFile> SplitByChunks(this MidiFile midiFile, SplitFileByChunksSettings settings = null)
        {
            ThrowIfArgument.IsNull(nameof(midiFile), midiFile);

            settings = settings ?? new SplitFileByChunksSettings();
            
            var tempoMap = settings.PreserveTempoMap
                ? midiFile.GetTempoMap()
                : null;

            foreach (var midiChunk in midiFile.Chunks.Where(c => settings.Filter?.Invoke(c) != false))
            {
                var newFile = new MidiFile(midiChunk.Clone())
                {
                    TimeDivision = midiFile.TimeDivision.Clone()
                };

                if (settings.PreserveTempoMap)
                    newFile.ReplaceTempoMap(tempoMap);

                yield return newFile;
            }
        }

        #endregion
    }
}
