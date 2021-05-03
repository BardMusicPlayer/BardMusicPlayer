/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        /// <param name="trackChunk"></param>
        /// <returns></returns>
        internal static async Task<TrackChunk> FixClassicChords(this TrackChunk trackChunk)
        {
            var notes = trackChunk.GetNotes().ToArray().Where(c => c != null).Reverse().ToArray();

            for (var i = 1; i < notes.Length; i++)
            {
                var time = notes[i].GetTimedNoteOnEvent().Time;
                var dur = notes[i].Length;
                var lowestParent = notes[0].GetTimedNoteOnEvent().Time;

                for (var k = i - 1; k >= 0; k--)
                {
                    var lastOn = notes[k].GetTimedNoteOnEvent().Time;
                    if (lastOn < lowestParent) lowestParent = lastOn;
                }

                if (lowestParent > time + 50) continue;
                time = lowestParent - 50;

                if (time < 0) continue;

                notes[i].Time = time;
                notes[i].Length = 25;
            }

            trackChunk = new TrackChunk();

            trackChunk.AddObjects(notes);
            return trackChunk;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunk"></param>
        /// <returns></returns>
        internal static async Task<TrackChunk> FixClassicChords(this Task<TrackChunk> trackChunk) => await FixClassicChords(await trackChunk);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunk"></param>
        /// <returns></returns>
        internal static async Task<TrackChunk> OffSet50Ms(this TrackChunk trackChunk)
        {
            var processedChunk = new TrackChunk();

            var events = trackChunk.GetNotes().ToArray();

            long lastStartTime = 0;
            long lastStopTime = 0;
            long lastDur = 0;
            int lastNoteNum = 0;
            long last50MsTimeStamp = 0;
            bool hasNotes = false;
            int lastChannel = 0;
            int lastVelocity = 0;

            List<Note> thisNotes = new();
            for (var j = 0; j < events.Count(); j++)
            {
                long startTime = events[j].GetTimedNoteOnEvent().Time;
                long stopTime;
                long dur = events[j].Length;
                int noteNum = events[j].NoteNumber;
                int channel = events[j].Channel;
                int velocity = events[j].Velocity;
                hasNotes = true;
                if (j == 0)
                {
                    if (startTime % 100 != 0 && startTime % 100 != 50)
                    {
                        if (startTime % 100 < 25 || (startTime % 100 < 75 && startTime % 100 > 50)) startTime = 50 * ((startTime + 49) / 50);
                        else startTime = 50 * (startTime / 50);
                    }

                    if (dur % 100 != 0 && dur % 100 != 25 && dur % 100 != 50 && dur % 100 != 75)
                    {
                        dur = 25 * ((dur + 24) / 25);
                        if (dur < 25) dur = 25;
                    }

                    stopTime = startTime + dur;
                    last50MsTimeStamp = startTime;
                    lastStartTime = startTime;
                    lastStopTime = stopTime;
                    lastDur = dur;
                    lastNoteNum = noteNum;
                    lastChannel = channel;
                    lastVelocity = velocity;
                }
                else
                {
                    while (last50MsTimeStamp < startTime) last50MsTimeStamp += 50;

                    startTime = last50MsTimeStamp;
                    stopTime = startTime + dur;

                    if (startTime - lastStopTime < 25)
                        lastDur = startTime - lastStartTime - 25;

                    if (dur % 100 != 0 && dur % 100 != 25 && dur % 100 != 50 && dur % 100 != 75)
                    {
                        dur = 25 * ((dur + 24) / 25);
                        if (dur < 25) dur = 25;
                    }

                    if (lastDur > 0)
                        thisNotes.Add(new Note((SevenBitNumber)lastNoteNum, lastDur, lastStartTime)
                        {
                            Channel = (FourBitNumber)lastChannel,
                            Velocity = (SevenBitNumber)lastVelocity,
                            OffVelocity = (SevenBitNumber)lastVelocity
                        });

                    lastStartTime = startTime;
                    lastStopTime = stopTime;
                    lastDur = dur;
                    lastNoteNum = noteNum;
                    lastChannel = channel;
                    lastVelocity = velocity;
                }

            }

            if (hasNotes)
                thisNotes.Add(new Note((SevenBitNumber)lastNoteNum, lastDur, lastStartTime)
                {
                    Channel = (FourBitNumber)lastChannel,
                    Velocity = (SevenBitNumber)lastVelocity,
                    OffVelocity = (SevenBitNumber)lastVelocity
                });

            processedChunk.AddObjects(thisNotes);
            return processedChunk;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunk"></param>
        /// <returns></returns>
        internal static async Task<TrackChunk> OffSet50Ms(this Task<TrackChunk> trackChunk) => await OffSet50Ms(await trackChunk);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunk"></param>
        /// <returns></returns>
        internal static async Task<TrackChunk> FixEndSpacing(this TrackChunk trackChunk)
        {
            var events = trackChunk.GetNotes().ToArray();
            var tempChunk = new TrackChunk();
            var thisNotes = new List<Note>();
            for (var j = 0; j < events.Count(); j++)
            {
                var noteNum = events[j].NoteNumber;
                var time = events[j].Time;
                var dur = events[j].Length;
                var channel = events[j].Channel;
                var velocity = events[j].Velocity;

                if (j + 1 < events.Length)
                {
                    if (events[j + 1].Time <= events[j].Time + events[j].Length + 25)
                    {
                        dur = events[j + 1].Time - events[j].Time - 25;
                        dur = dur < 25 ? 1 : dur;
                    }
                }
                thisNotes.Add(new Note(noteNum, dur, time)
                {
                    Channel = channel,
                    Velocity = velocity,
                    OffVelocity = velocity
                });
            }
            tempChunk.AddObjects(thisNotes);
            return tempChunk;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trackChunk"></param>
        /// <returns></returns>
        internal static async Task<TrackChunk> FixEndSpacing(this Task<TrackChunk> trackChunk) => await FixEndSpacing(await trackChunk);
    }
}
#pragma warning restore 1998