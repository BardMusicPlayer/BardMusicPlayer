/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Threading.Tasks;
using BardMusicPlayer.Common.Structs;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

#pragma warning disable 1998
namespace BardMusicPlayer.Notate.Processor.Utilities
{
    internal static partial class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNotesDictionary"></param>
        /// <param name="sourceOctaveRange"></param>
        /// <returns></returns>
        internal static async Task<Dictionary<int, Dictionary<long, Note>>> MoveNoteDictionaryToDefaultOctave(this Dictionary<int, Dictionary<long, Note>> sourceNotesDictionary, OctaveRange sourceOctaveRange)
        {
            var notesDictionary = new Dictionary<int, Dictionary<long, Note>>();
            for (var i = 0; i < 5; i++) if (sourceNotesDictionary.ContainsKey(i)) notesDictionary[i] = sourceNotesDictionary[i];
            for (var i = sourceOctaveRange.LowerNote; i <= sourceOctaveRange.UpperNote; i++)
            {
                var noteNumber = OctaveRange.C3toC6.ShiftNoteToOctave(sourceOctaveRange, i);
                foreach (var note in sourceNotesDictionary[i]) note.Value.NoteNumber = (SevenBitNumber)noteNumber;
                notesDictionary[noteNumber] = sourceNotesDictionary[i];
            }
            return notesDictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNotesDictionary"></param>
        /// <param name="sourceOctaveRange"></param>
        /// <returns></returns>
        internal static async Task<Dictionary<int, Dictionary<long, Note>>> MoveNoteDictionaryToDefaultOctave(this Task<Dictionary<int, Dictionary<long, Note>>> sourceNotesDictionary, OctaveRange sourceOctaveRange) =>
            await MoveNoteDictionaryToDefaultOctave(await sourceNotesDictionary, sourceOctaveRange);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunks"></param>
        /// <param name="tempoMap"></param>
        /// <param name="lowClamp"></param>
        /// <param name="highClamp"></param>
        /// <param name="includeToneNotes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal static async Task<Dictionary<int, Dictionary<long, Note>>> GetNoteDictionary(this List<TrackChunk> trackChunks, TempoMap tempoMap, int lowClamp = 12, int highClamp = 120, bool includeToneNotes = true, long offset = 120000)

        {
            var notesDictionary = Tools.GetEmptyNotesDictionary(lowClamp, highClamp, includeToneNotes);
            foreach (var note in trackChunks.Merge().GetNotes())
            {
                var noteNumber = note.NoteNumber;
                if (!(includeToneNotes && noteNumber < 5) && (noteNumber < lowClamp || noteNumber > highClamp)) continue;
                var timeOn = note.GetTimedNoteOnEvent().GetNoteMs(tempoMap) + offset;
                var timeOff = note.GetTimedNoteOffEvent().GetNoteMs(tempoMap) + offset;
                if (notesDictionary[noteNumber].ContainsKey(timeOn))
                {
                    var previousTimeOff = timeOn + notesDictionary[noteNumber][timeOn].Length;
                    if (previousTimeOff < timeOff) notesDictionary[noteNumber][timeOn].Length = timeOff - timeOn;
                }
                else notesDictionary[noteNumber].Add(timeOn, new Note(noteNumber, timeOff - timeOn, timeOn));
            }
            return notesDictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunk"></param>
        /// <param name="tempoMap"></param>
        /// <param name="lowClamp"></param>
        /// <param name="highClamp"></param>
        /// <param name="includeToneNotes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal static async Task<Dictionary<int, Dictionary<long, Note>>> GetNoteDictionary(this TrackChunk trackChunk, TempoMap tempoMap, int lowClamp = 12, int highClamp = 120, bool includeToneNotes = true, long offset = 120000) =>
            await GetNoteDictionary(new List<TrackChunk> { trackChunk }, tempoMap, lowClamp, highClamp, includeToneNotes, offset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunk"></param>
        /// <param name="playerCount"></param>
        /// <param name="lowClamp"></param>
        /// <param name="highClamp"></param>
        /// <param name="includeToneNotes"></param>
        /// <returns></returns>
        internal static async Task<Dictionary<int, Dictionary<int, Dictionary<long, Note>>>> GetPlayerNoteDictionary(this TrackChunk trackChunk, int playerCount, int lowClamp = 12, int highClamp = 120, bool includeToneNotes = true)
        {
            var playerNotesDictionary = Tools.GetEmptyPlayerNotesDictionary(playerCount, lowClamp, highClamp, includeToneNotes);
            using var loadBalancer = new LoadBalancer(playerCount);
            using var timedEventsManager = trackChunk.ManageTimedEvents();

            foreach (var trackEvent in timedEventsManager.Events)
            {
                switch (trackEvent.Event.EventType)
                {
                    case MidiEventType.NoteOn:
                        {
                            var note = (NoteOnEvent)trackEvent.Event;
                            if (includeToneNotes && note.NoteNumber < 5)
                            {
                                for (var player = 0; player < playerCount; player++) playerNotesDictionary[player][note.NoteNumber][trackEvent.Time] = new Note(note.NoteNumber, 25, trackEvent.Time);
                            }
                            else if (note.NoteNumber >= lowClamp && note.NoteNumber <= highClamp)
                            {
                                var (stoppedBard, stoppedNote) = loadBalancer.NotifyNoteOn(trackEvent.Time, note.Channel, note.NoteNumber, note.Velocity);
                                if (stoppedBard > -1)
                                {
                                    playerNotesDictionary[stoppedBard][stoppedNote.NoteNumber][stoppedNote.Time] = stoppedNote;
                                }
                            }
                            break;
                        }
                    case MidiEventType.NoteOff:
                        {
                            var note = (NoteOffEvent)trackEvent.Event;
                            if (note.NoteNumber >= lowClamp && note.NoteNumber <= highClamp)
                            {
                                var (stoppedBard, stoppedNote) = loadBalancer.NotifyNoteOff(trackEvent.Time, note.Channel, note.NoteNumber, note.Velocity);
                                if (stoppedBard > -1)
                                {
                                    playerNotesDictionary[stoppedBard][stoppedNote.NoteNumber][stoppedNote.Time] = stoppedNote;
                                }
                            }
                            break;
                        }
                }
            }
            return playerNotesDictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunk"></param>
        /// <param name="playerCount"></param>
        /// <param name="lowClamp"></param>
        /// <param name="highClamp"></param>
        /// <param name="includeToneNotes"></param>
        /// <returns></returns>
        internal static async Task<Dictionary<int, Dictionary<int, Dictionary<long, Note>>>> GetPlayerNoteDictionary(this Task<TrackChunk> trackChunk, int playerCount, int lowClamp = 12, int highClamp = 120, bool includeToneNotes = true) =>
            await GetPlayerNoteDictionary(await trackChunk, playerCount, lowClamp, highClamp, includeToneNotes);
    }
}
#pragma warning restore 1998