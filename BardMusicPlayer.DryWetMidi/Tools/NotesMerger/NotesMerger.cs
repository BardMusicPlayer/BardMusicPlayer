﻿using BardMusicPlayer.DryWetMidi.Common;
using BardMusicPlayer.DryWetMidi.Interaction.Notes;
using BardMusicPlayer.DryWetMidi.Interaction.ObjectId;
using BardMusicPlayer.DryWetMidi.Interaction.TempoMap;
using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan.Converters;
using BardMusicPlayer.DryWetMidi.Interaction.TimeSpan.Representations;
using BardMusicPlayer.DryWetMidi.Tools.Merger;
using BardMusicPlayer.DryWetMidi.Tools.NotesMerger.VelocityMergers;

namespace BardMusicPlayer.DryWetMidi.Tools.NotesMerger
{
    /// <summary>
    /// Provides methods to merge nearby notes.
    /// </summary>
    [Obsolete("OBS17")]
    public sealed class NotesMerger
    {
        #region Nested classes

        private sealed class NoteHolder
        {
            #region Fields

            private readonly Note _note;

            private readonly VelocityMerger _velocityMerger;
            private readonly VelocityMerger _offVelocityMerger;

            #endregion

            #region Constructor

            public NoteHolder(Note note, VelocityMerger velocityMerger, VelocityMerger offVelocityMerger)
            {
                _note = note;

                _velocityMerger = velocityMerger;
                _offVelocityMerger = offVelocityMerger;

                _velocityMerger.Initialize(note.Velocity);
                _offVelocityMerger.Initialize(note.OffVelocity);

                EndTime = _note.EndTime;
            }

            #endregion

            #region Properties

            public long EndTime { get; set; }

            #endregion

            #region Methods

            public void MergeVelocities(Note note)
            {
                _velocityMerger.Merge(note.Velocity);
                _offVelocityMerger.Merge(note.OffVelocity);
            }

            public Note GetResultNote()
            {
                _note.Length = EndTime - _note.Time;

                _note.Velocity = _velocityMerger.Velocity;
                _note.OffVelocity = _offVelocityMerger.Velocity;

                return _note;
            }

            #endregion
        }

        #endregion

        #region Constants

        private static readonly Dictionary<VelocityMergingPolicy, Func<VelocityMerger>> VelocityMergers =
            new Dictionary<VelocityMergingPolicy, Func<VelocityMerger>>
            {
                [VelocityMergingPolicy.First] = () => new FirstVelocityMerger(),
                [VelocityMergingPolicy.Last] = () => new LastVelocityMerger(),
                [VelocityMergingPolicy.Min] = () => new MinVelocityMerger(),
                [VelocityMergingPolicy.Max] = () => new MaxVelocityMerger(),
                [VelocityMergingPolicy.Average] = () => new AverageVelocityMerger()
            };

        #endregion

        #region Methods

        /// <summary>
        /// Merges nearby notes in the specified collection of notes.
        /// </summary>
        /// <param name="notes">Collection of notes to merge notes in.</param>
        /// <param name="tempoMap">Tempo map used to calculate distances between notes.</param>
        /// <param name="settings">Settings according to which notes should be merged.</param>
        /// <returns>Collection of notes which produced from the input one by merging nearby notes.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="notes"/> is <c>null</c>.</exception>
        public IEnumerable<Note> Merge(IEnumerable<Note> notes, TempoMap tempoMap, NotesMergingSettings settings = null)
        {
            ThrowIfArgument.IsNull(nameof(notes), notes);

            settings = settings ?? new NotesMergingSettings();

            var currentNotes = new Dictionary<NoteId, NoteHolder>();
            var toleranceType = settings.Tolerance.GetType();

            foreach (var note in notes.Where(n => n != null).OrderBy(n => n.Time))
            {
                var noteId = (NoteId)note.GetObjectId();

                NoteHolder currentNoteHolder;
                if (!currentNotes.TryGetValue(noteId, out currentNoteHolder))
                {
                    currentNotes.Add(noteId, CreateNoteHolder(note, settings));
                    continue;
                }

                var currentEndTime = currentNoteHolder.EndTime;
                var distance = Math.Max(0, note.Time - currentEndTime);
                var convertedDistance = LengthConverter.ConvertTo((MidiTimeSpan)distance,
                                                                  toleranceType,
                                                                  currentEndTime,
                                                                  tempoMap);

                if (convertedDistance.CompareTo(settings.Tolerance) <= 0)
                {
                    var endTime = Math.Max(note.EndTime, currentEndTime);
                    currentNoteHolder.EndTime = endTime;
                    currentNoteHolder.MergeVelocities(note);
                }
                else
                {
                    yield return currentNotes[noteId].GetResultNote();
                    currentNotes[noteId] = CreateNoteHolder(note, settings);
                }
            }

            foreach (var note in currentNotes.Values)
            {
                yield return note.GetResultNote();
            }
        }

        private static NoteHolder CreateNoteHolder(Note note, NotesMergingSettings settings)
        {
            return new NoteHolder((Note)note.Clone(),
                                  VelocityMergers[settings.VelocityMergingPolicy](),
                                  VelocityMergers[settings.OffVelocityMergingPolicy]());
        }

        #endregion
    }
}
