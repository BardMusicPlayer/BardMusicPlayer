namespace BardMusicPlayer.Synth.AlphaTab.Model
{
    /// <summary>
    /// Lists all types of how to brush multiple notes on a beat. 
    /// </summary>
    internal enum BrushType
    {
        /// <summary>
        /// No brush. 
        /// </summary>
        None,

        /// <summary>
        /// Normal brush up. 
        /// </summary>
        BrushUp,

        /// <summary>
        /// Normal brush down. 
        /// </summary>
        BrushDown,

        /// <summary>
        /// Arpeggio up. 
        /// </summary>
        ArpeggioUp,

        /// <summary>
        /// Arpeggio down. 
        /// </summary>
        ArpeggioDown
    }
}
