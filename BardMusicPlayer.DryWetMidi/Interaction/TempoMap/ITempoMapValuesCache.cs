namespace BardMusicPlayer.DryWetMidi.Interaction.TempoMap
{
    internal interface ITempoMapValuesCache
    {
        #region Properties

        IEnumerable<TempoMapLine> InvalidateOnLines { get; }

        #endregion

        #region Methods

        void Invalidate(TempoMap tempoMap);

        #endregion
    }
}
