/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Maestro.Sequencing;
using System.Collections.Generic;
using System.Linq;

namespace BardMusicPlayer.Maestro.Events
{
    public sealed class SongLoadedEvent : MaestroEvent
    {

        internal SongLoadedEvent(int maxtracks, Sequencer sequencer)
        {
            EventType = GetType();
            MaxTracks = maxtracks;
            _sequencer = sequencer;
        }
        private Sequencer _sequencer;
        public int MaxTracks { get; }
        public int TotalNoteCount => _sequencer.notesPlayedCount.Values.Sum();

        public List<int> CurrentNoteCountForTracks
        {
            get
            {
                return _sequencer.notesPlayedCount.Select(s => s.Key.Count).ToList();
            }
        }

        public override bool IsValid() => true;
    }
}
