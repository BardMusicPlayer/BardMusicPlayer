/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

#region

using BardMusicPlayer.Siren.AlphaTab.Audio;
using BardMusicPlayer.Siren.AlphaTab.Collections;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    /// <summary>
    ///     A voice represents a group of beats
    ///     that can be played during a bar.
    /// </summary>
    internal abstract class Voice
    {
        private FastDictionary<int, Beat> _beatLookup;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Voice" /> class.
        /// </summary>
        protected Voice()
        {
            Beats = new FastList<Beat>();
            IsEmpty = true;
        }

        /// <summary>
        ///     Gets or sets the zero-based index of this voice within the bar.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        ///     Gets or sets the reference to the bar this voice belongs to.
        /// </summary>
        public Bar Bar { get; set; }

        /// <summary>
        ///     Gets or sets the list of beats contained in this voice.
        /// </summary>
        public FastList<Beat> Beats { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this voice is empty.
        /// </summary>
        public bool IsEmpty { get; set; }

        internal static void CopyTo(Voice src, Voice dst)
        {
            dst.Index = src.Index;
            dst.IsEmpty = src.IsEmpty;
        }

        internal void InsertBeat(Beat after, Beat newBeat)
        {
            newBeat.NextBeat = after.NextBeat;
            if (newBeat.NextBeat != null) newBeat.NextBeat.PreviousBeat = newBeat;

            newBeat.PreviousBeat = after;
            newBeat.Voice = this;
            after.NextBeat = newBeat;
            Beats.InsertAt(after.Index + 1, newBeat);
        }


        internal void AddBeat(Beat beat)
        {
            beat.Voice = this;
            beat.Index = Beats.Count;
            Beats.Add(beat);
            if (!beat.IsEmpty) IsEmpty = false;
        }

        private void Chain(Beat beat)
        {
            if (Bar == null) return;

            if (beat.Index < Beats.Count - 1)
            {
                beat.NextBeat = Beats[beat.Index + 1];
                beat.NextBeat.PreviousBeat = beat;
            }
            else if (beat.IsLastOfVoice && beat.Voice.Bar.NextBar != null)
            {
                var nextVoice = Bar.NextBar.Voices[Index];
                if (nextVoice.Beats.Count > 0)
                {
                    beat.NextBeat = nextVoice.Beats[0];
                    beat.NextBeat.PreviousBeat = beat;
                }
                else
                {
                    beat.NextBeat.PreviousBeat = beat;
                }
            }
        }

        internal void AddGraceBeat(Beat beat)
        {
            if (Beats.Count == 0)
            {
                AddBeat(beat);
                return;
            }

            // remove last beat
            var lastBeat = Beats[Beats.Count - 1];
            Beats.RemoveAt(Beats.Count - 1);

            // insert grace beat
            AddBeat(beat);
            // reinsert last beat
            AddBeat(lastBeat);

            IsEmpty = false;
        }

        internal Beat GetBeatAtDisplayStart(int displayStart)
        {
            return _beatLookup.ContainsKey(displayStart) ? _beatLookup[displayStart] : null;
        }

        internal void Finish()
        {
            _beatLookup = new FastDictionary<int, Beat>();
            for (var index = 0; index < Beats.Count; index++)
            {
                var beat = Beats[index];
                beat.Index = index;
                Chain(beat);
            }

            var currentDisplayTick = 0;
            var currentPlaybackTick = 0;
            for (var i = 0; i < Beats.Count; i++)
            {
                var beat = Beats[i];
                beat.Index = i;
                beat.Finish();

                if (beat.GraceType is GraceType.None or GraceType.BendGrace)
                {
                    beat.DisplayStart = currentDisplayTick;
                    beat.PlaybackStart = currentPlaybackTick;
                    currentDisplayTick += beat.DisplayDuration;
                    currentPlaybackTick += beat.PlaybackDuration;
                }
                else
                {
                    if (beat.PreviousBeat == null || beat.PreviousBeat.GraceType == GraceType.None)
                    {
                        // find note which is not a grace note
                        var nonGrace = beat;
                        var numberOfGraceBeats = 0;
                        while (nonGrace != null && nonGrace.GraceType != GraceType.None)
                        {
                            nonGrace = nonGrace.NextBeat;
                            numberOfGraceBeats++;
                        }

                        var graceDuration = Duration.Eighth;
                        var stolenDuration = 0;
                        graceDuration = numberOfGraceBeats switch
                        {
                            1 => Duration.Eighth,
                            2 => Duration.Sixteenth,
                            _ => Duration.ThirtySecond
                        };

                        nonGrace?.UpdateDurations();

                        // grace beats have 1/4 size of the non grace beat preceeding them
                        var perGraceDisplayDuration = beat.PreviousBeat == null
                            ? Duration.ThirtySecond.ToTicks()
                            : beat.PreviousBeat.DisplayDuration / 4 / numberOfGraceBeats;

                        // move all grace beats
                        var graceBeat = Beats[i];
                        for (var j = 0; j < numberOfGraceBeats && graceBeat != null; j++)
                        {
                            graceBeat.Duration = graceDuration;
                            graceBeat.UpdateDurations();

                            graceBeat.DisplayStart =
                                currentDisplayTick - (numberOfGraceBeats - j + 1) * perGraceDisplayDuration;
                            graceBeat.DisplayDuration = perGraceDisplayDuration;

                            stolenDuration += graceBeat.PlaybackDuration;

                            graceBeat = graceBeat.NextBeat;
                        }

                        // steal needed duration from beat duration
                        if (beat.GraceType == GraceType.BeforeBeat)
                        {
                            if (beat.PreviousBeat != null) beat.PreviousBeat.PlaybackDuration -= stolenDuration;

                            currentPlaybackTick -= stolenDuration;
                        }
                        else if (nonGrace != null && beat.GraceType == GraceType.OnBeat)
                        {
                            nonGrace.PlaybackDuration -= stolenDuration;
                        }
                    }

                    beat.PlaybackStart = currentPlaybackTick;
                    currentPlaybackTick = beat.PlaybackStart + beat.PlaybackDuration;
                }

                beat.FinishTuplet();
                _beatLookup[beat.DisplayStart] = beat;
            }
        }

        public int CalculateDuration()
        {
            if (IsEmpty || Beats.Count == 0) return 0;

            var lastBeat = Beats[Beats.Count - 1];
            return lastBeat.PlaybackStart + lastBeat.PlaybackDuration;
        }
    }
}