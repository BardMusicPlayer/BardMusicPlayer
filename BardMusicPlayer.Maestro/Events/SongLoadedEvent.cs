/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Maestro.Sequencing;
using System.Collections.Generic;

namespace BardMusicPlayer.Maestro.Events
{
    public sealed class SongLoadedEvent : MaestroEvent
    {

        internal SongLoadedEvent(int maxtracks, Sequencer sequencer) : base(0, false)
        {
            EventType = GetType();
            MaxTracks = maxtracks;
            _sequencer = sequencer;
        }
        private Sequencer _sequencer;
        public int MaxTracks { get; }
        public int TotalNoteCount
        {
            get
            {
                int sum = 0;
                foreach (int s in _sequencer.notesPlayedCount.Values)
                    sum += s;
                return sum;
            }
        }
        public List<int> CurrentNoteCountForTracks
        {
            get
            {
                List<int> t = new List<int>();
                foreach (var s in _sequencer.notesPlayedCount)
                    t.Add(s.Key.Count);
                return t;
            }
        }

        public override bool IsValid() => true;
    }
}
