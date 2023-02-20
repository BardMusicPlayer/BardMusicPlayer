using BardMusicPlayer.DryWetMidi.Common;
using BardMusicPlayer.DryWetMidi.Multimedia.Clock.TickGenerator;

namespace BardMusicPlayer.DryWetMidi.Multimedia.Clock
{
    /// <summary>
    /// Holds settings for <see cref="MidiClock"/> used by a clock driven object.
    /// </summary>
    public sealed class MidiClockSettings
    {
        #region Fields

        private Func<TickGenerator.TickGenerator> _createTickGeneratorCallback = () => new HighPrecisionTickGenerator();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a callback used to create tick generator for MIDI clock.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        public Func<TickGenerator.TickGenerator> CreateTickGeneratorCallback
        {
            get { return _createTickGeneratorCallback; }
            set
            {
                ThrowIfArgument.IsNull(nameof(value), value);

                _createTickGeneratorCallback = value;
            }
        }

        #endregion
    }
}
