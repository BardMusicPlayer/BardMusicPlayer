/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Maestro.Old.Sequencing;

namespace BardMusicPlayer.Maestro.Old.Events;

public sealed class SongLoadedEvent : MaestroEvent
{

    internal SongLoadedEvent(int maxtracks, OldSequencer sequencer)
    {
        EventType = GetType();
        MaxTracks = maxtracks;
        _sequencer = sequencer;
    }
    private OldSequencer _sequencer;
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