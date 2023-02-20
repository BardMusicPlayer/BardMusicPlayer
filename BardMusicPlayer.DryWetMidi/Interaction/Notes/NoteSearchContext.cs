﻿using BardMusicPlayer.DryWetMidi.Core;

namespace BardMusicPlayer.DryWetMidi.Interaction
{
    /// <summary>
    /// Defines a context to search notes within. The default value is <see cref="SingleEventsCollection"/>.
    /// More info in the
    /// <see href="xref:a_getting_objects#notesearchcontext">Getting objects: GetNotes: Settings: NoteSearchContext</see>
    /// article.
    /// </summary>
    /// <seealso cref="NoteDetectionSettings"/>
    /// <seealso cref="NotesManagingUtilities"/>
    public enum NoteSearchContext
    {
        /// <summary>
        /// A note should be detected only within single events collection (<see cref="EventsCollection"/>
        /// or <see cref="TrackChunk"/>). It means MIDI events that make up a note should be present
        /// in one events collection.
        /// </summary>
        SingleEventsCollection = 0,

        /// <summary>
        /// A note can be detected within all events collection (<see cref="EventsCollection"/>
        /// or <see cref="TrackChunk"/>) of the source (<see cref="MidiFile"/> for example). It means
        /// MIDI events that make up a note can be present in different events collections.
        /// </summary>
        AllEventsCollections
    }
}
