/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Siren.AlphaTab.Collections;

namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    /// <summary>
    /// This public class describes a single track or instrument of score.
    /// It is bascially a list of staffs containing individual music notation kinds.
    /// </summary>
    internal class Track
    {
        private const int ShortNameMaxLength = 10;

        /// <summary>
        /// Gets or sets the zero-based index of this track. 
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the reference this track belongs to. 
        /// </summary>
        public Score Score { get; set; }

        /// <summary>
        /// Gets or sets the list of staffs that are defined for this track. 
        /// </summary>
        public FastList<Staff> Staves { get; set; }

        /// <summary>
        /// Gets or sets the playback information for this track. 
        /// </summary>
        public PlaybackInformation PlaybackInfo { get; set; }

        /// <summary>
        /// Gets or sets the long name of this track. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the short name of this track. 
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Track"/> class.
        /// </summary>
        /// <param name="staveCount">The stave count.</param>
        public Track(int staveCount)
        {
            Staves = new FastList<Staff>();
            EnsureStaveCount(staveCount);
            PlaybackInfo = new PlaybackInformation();
            Name = "";
            ShortName = "";
        }

        internal void EnsureStaveCount(int staveCount)
        {
            while (Staves.Count < staveCount)
            {
                AddStaff(new Staff());
            }
        }

        internal void AddStaff(Staff staff)
        {
            staff.Index = Staves.Count;
            staff.Track = this;
            Staves.Add(staff);
        }

        internal static void CopyTo(Track src, Track dst)
        {
            dst.Name = src.Name;
            dst.ShortName = src.ShortName;
            dst.Index = src.Index;
        }

        internal void Finish()
        {
            if (string.IsNullOrEmpty(ShortName))
            {
                ShortName = Name;
                if (ShortName.Length > ShortNameMaxLength)
                {
                    ShortName = ShortName.Substring(0, ShortNameMaxLength);
                }
            }

            for (int i = 0, j = Staves.Count; i < j; i++)
            {
                Staves[i].Finish();
            }
        }

        internal void ApplyLyrics(FastList<Lyrics> lyrics)
        {
            foreach (var lyric in lyrics)
            {
                lyric.Finish();
            }

            var staff = Staves[0];

            for (var li = 0; li < lyrics.Count; li++)
            {
                var lyric = lyrics[li];
                if (lyric.StartBar >= 0)
                {
                    var beat = staff.Bars[lyric.StartBar].Voices[0].Beats[0];
                    for (var ci = 0; ci < lyric.Chunks.Length && beat != null; ci++)
                    {
                        // skip rests and empty beats
                        while (beat != null && (beat.IsEmpty || beat.IsRest))
                        {
                            beat = beat.NextBeat;
                        }

                        // mismatch between chunks and beats might lead to missing beats
                        if (beat != null)
                        {
                            // initialize lyrics list for beat if required
                            if (beat.Lyrics == null)
                            {
                                beat.Lyrics = new string[lyrics.Count];
                            }

                            // assign chunk
                            beat.Lyrics[li] = lyric.Chunks[ci];
                            beat = beat.NextBeat;
                        }
                    }
                }
            }
        }
    }
}
