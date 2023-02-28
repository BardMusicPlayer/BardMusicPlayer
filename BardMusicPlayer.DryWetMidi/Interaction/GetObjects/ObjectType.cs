namespace BardMusicPlayer.DryWetMidi.Interaction.GetObjects;

/// <summary>
/// The type of objects to get with methods of <see cref="GetObjectsUtilities"/>.
/// </summary>
[Flags]
public enum ObjectType
{
    /// <summary>
    /// Represents a timed event (see <see cref="TimedEvents.TimedEvent"/>).
    /// </summary>
    TimedEvent = 1 << 0,

    /// <summary>
    /// Represents a note (see <see cref="Notes.Note"/>).
    /// </summary>
    Note = 1 << 1,

    /// <summary>
    /// Represents a chord (see <see cref="Chords.Chord"/>).
    /// </summary>
    Chord = 1 << 2,

    /// <summary>
    /// Represents a rest (see <see cref="GetObjects.Rest"/>).
    /// </summary>
    Rest = 1 << 3,
}