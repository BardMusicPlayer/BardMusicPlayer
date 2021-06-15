/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Quotidian.Structs;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Transmogrify.Processor.Utilities
{
    internal static partial class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNotesDictionary"></param>
        /// <param name="sourceOctaveRange"></param>
        /// <returns></returns>
        internal static Task<Dictionary<int, Dictionary<long, Note>>> MoveNoteDictionaryToDefaultOctave(this Dictionary<int, Dictionary<long, Note>> sourceNotesDictionary, OctaveRange sourceOctaveRange)
        {
            var notesDictionary = new Dictionary<int, Dictionary<long, Note>>(sourceNotesDictionary.Count);
            for (var i = 0; i < 5; i++) if (sourceNotesDictionary.ContainsKey(i)) notesDictionary[i] = sourceNotesDictionary[i];
            for (var i = sourceOctaveRange.LowerNote; i <= sourceOctaveRange.UpperNote; i++)
            {
                if (!sourceNotesDictionary.ContainsKey(i)) continue;
                var noteNumber = OctaveRange.C3toC6.ShiftNoteToOctave(sourceOctaveRange, i);
                foreach (var note in sourceNotesDictionary[i]) note.Value.NoteNumber = (SevenBitNumber) noteNumber;
                notesDictionary[noteNumber] = sourceNotesDictionary[i];
            }
            return Task.FromResult(notesDictionary);
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
        /// <param name="tone"></param>
        /// <param name="mapper"></param>
        /// <param name="lowClamp"></param>
        /// <param name="highClamp"></param>
        /// <param name="startingChannel"></param>
        /// <param name="readTones"></param>
        /// <param name="noteSampleOffset"></param>
        /// <returns></returns>
        internal static Task<Dictionary<int, Dictionary<long, Note>>> GetNoteDictionary(this List<TrackChunk> trackChunks, TempoMap tempoMap, InstrumentTone tone, Dictionary<int, (int,int)> mapper, int lowClamp = 12, int highClamp = 120, int startingChannel = 0, bool readTones = true, int noteSampleOffset = 0)
        {
            if (lowClamp >= highClamp) return Task.FromResult(new Dictionary<int, Dictionary<long, Note>>());

            var notesDictionary = Tools.GetEmptyNotesDictionary(lowClamp, highClamp, trackChunks.Count);

            var currentChannel = startingChannel;
            tempoMap = tempoMap.Clone();

            foreach (var note in trackChunks.Merge().GetNotes())
            {
                var noteNumber = note.NoteNumber;
                if (!(noteNumber < 5) && (noteNumber < lowClamp || noteNumber > highClamp)) continue;

                if (noteNumber < 5)
                {
                    if (readTones) currentChannel = noteNumber;
                    continue;
                }

                var timeOn = note.GetTimedNoteOnEvent().GetNoteMs(tempoMap) + 120000 + 
                             tone.GetInstrumentFromChannel(currentChannel).SampleOffset + 
                             tone.GetInstrumentFromChannel(currentChannel)
                                 .NoteSampleOffset(noteNumber + noteSampleOffset);

                var dur = note.GetTimedNoteOffEvent().GetNoteMs(tempoMap) + 120000 +
                          tone.GetInstrumentFromChannel(currentChannel).SampleOffset + tone
                              .GetInstrumentFromChannel(currentChannel)
                              .NoteSampleOffset(noteNumber + noteSampleOffset) - timeOn;

                if (dur < 1) dur = 1;

                note.Time = timeOn;
                note.Length = dur;

                if (notesDictionary[noteNumber].ContainsKey(timeOn))
                {
                    var previousNote = notesDictionary[noteNumber][timeOn];
                    if (previousNote.Length < note.Length) notesDictionary[noteNumber][timeOn] = note;
                }
                else
                {
                    note.Channel = (FourBitNumber) currentChannel;
                    notesDictionary[noteNumber].Add(timeOn, note);
                }
            }
            return Task.FromResult(notesDictionary);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunks"></param>
        /// <param name="tempoMap"></param>
        /// <param name="tone"></param>
        /// <param name="lowClamp"></param>
        /// <param name="highClamp"></param>
        /// <param name="startingChannel"></param>
        /// <param name="readTones"></param>
        /// <param name="noteSampleOffset"></param>
        /// <returns></returns>
        internal static async Task<Dictionary<int, Dictionary<long, Note>>> GetNoteDictionary(this List<TrackChunk> trackChunks, TempoMap tempoMap, InstrumentTone tone, int lowClamp = 12, int highClamp = 120, int startingChannel = 0, bool readTones = true, int noteSampleOffset = 0) =>
        await GetNoteDictionary(trackChunks, tempoMap, tone, new Dictionary<int, (int, int)>(), lowClamp, highClamp, startingChannel, readTones, noteSampleOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunk"></param>
        /// <param name="tempoMap"></param>
        /// <param name="tone"></param>
        /// <param name="mapper"></param>
        /// <param name="lowClamp"></param>
        /// <param name="highClamp"></param>
        /// <param name="startingChannel"></param>
        /// <param name="readTones"></param>
        /// <param name="noteSampleOffset"></param>
        /// <returns></returns>
        internal static async Task<Dictionary<int, Dictionary<long, Note>>> GetNoteDictionary(this TrackChunk trackChunk, TempoMap tempoMap, InstrumentTone tone, Dictionary<int, (int,int)> mapper, int lowClamp = 12, int highClamp = 120, int startingChannel = 0, bool readTones = true, int noteSampleOffset = 0) =>
            await GetNoteDictionary(new List<TrackChunk> { trackChunk }, tempoMap, tone, mapper, lowClamp, highClamp, startingChannel, readTones, noteSampleOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunk"></param>
        /// <param name="tempoMap"></param>
        /// <param name="tone"></param>
        /// <param name="lowClamp"></param>
        /// <param name="highClamp"></param>
        /// <param name="startingChannel"></param>
        /// <param name="readTones"></param>
        /// <param name="noteSampleOffset"></param>
        internal static async Task<Dictionary<int, Dictionary<long, Note>>> GetNoteDictionary(this TrackChunk trackChunk, TempoMap tempoMap, InstrumentTone tone, int lowClamp = 12, int highClamp = 120, int startingChannel = 0, bool readTones = true, int noteSampleOffset = 0) =>
            await GetNoteDictionary(new List<TrackChunk> { trackChunk }, tempoMap, tone, new Dictionary<int, (int, int)>(), lowClamp, highClamp, startingChannel, readTones, noteSampleOffset);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunk"></param>
        /// <param name="playerCount"></param>
        /// <param name="lowClamp"></param>
        /// <param name="highClamp"></param>
        /// <returns></returns>
        internal static Task<Dictionary<int, Dictionary<int, Dictionary<long, Note>>>> GetPlayerNoteDictionary(this TrackChunk trackChunk, int playerCount, int lowClamp = 12, int highClamp = 120)
        {
            var notes = trackChunk.GetNotes();
            var playerNotesDictionary = Tools.GetEmptyPlayerNotesDictionary(playerCount, lowClamp, highClamp, notes.Count);
            
            var index = 0;
            foreach (var note in notes)
            {
                note.Velocity = (SevenBitNumber) index;
                note.OffVelocity = (SevenBitNumber)index;
                index++;
                if (index > 127) index = 0;
            }

            trackChunk = TimedObjectUtilities.ToTrackChunk(notes);

            using var loadBalancer = new LoadBalancer(playerCount);
            using var timedEventsManager = trackChunk.ManageTimedEvents();

            foreach (var trackEvent in timedEventsManager.Events)
            {
                switch (trackEvent.Event.EventType)
                {
                    case MidiEventType.NoteOn:
                        {
                            var note = (NoteOnEvent)trackEvent.Event;
                            if (note.NoteNumber >= lowClamp && note.NoteNumber <= highClamp)
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
            return Task.FromResult(playerNotesDictionary);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunk"></param>
        /// <param name="playerCount"></param>
        /// <param name="lowClamp"></param>
        /// <param name="highClamp"></param>
        /// <returns></returns>
        internal static async Task<Dictionary<int, Dictionary<int, Dictionary<long, Note>>>> GetPlayerNoteDictionary(this Task<TrackChunk> trackChunk, int playerCount, int lowClamp = 12, int highClamp = 120) =>
            await GetPlayerNoteDictionary(await trackChunk, playerCount, lowClamp, highClamp);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNotesDictionary"></param>
        /// <returns></returns>
        internal static Task<List<Note>> ConcatNoteDictionaryToList(this Dictionary<int, Dictionary<long, Note>> sourceNotesDictionary) =>
            Task.FromResult(sourceNotesDictionary.SelectMany(note => note.Value).Select(note => note.Value).ToList());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNotesDictionary"></param>
        /// <returns></returns>
        internal static async Task<List<Note>> ConcatNoteDictionaryToList(this Task<Dictionary<int, Dictionary<long, Note>>> sourceNotesDictionary) =>
            await ConcatNoteDictionaryToList(await sourceNotesDictionary);
    }
}