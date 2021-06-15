/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Transmogrify.Processor.Utilities
{
    internal static partial class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        internal static Task<List<Note>> FixChords(this List<Note> notes)
        {
            notes = notes.OrderBy(x=>x.Time).Reverse().ToList();
            
            for (var i = 1; i < notes.Count; i++)
            {
                var time = notes[i].Time;

                var lowestParent = notes[0].Time;

                for (var k = i - 1; k >= 0; k--)
                {
                    var lastOn = notes[k].Time;
                    if (lastOn < lowestParent) lowestParent = lastOn;
                }

                if (lowestParent > time + 50) continue;
                time = lowestParent - 50;

                if (time < 0) continue;

                notes[i].Time = time;
                notes[i].Length = 25;
            }
            return Task.FromResult(notes.OrderBy(x => x.Time).ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        internal static async Task<List<Note>> FixChords(this Task<List<Note>> notes) => await FixChords(await notes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        internal static Task<List<Note>> OffSet50Ms(this List<Note> notes)
        {
            notes = notes.OrderBy(x=>x.Time).ToList();
            
            var thisNotes = new List<Note>(notes.Count);

            long lastStartTime = 0;
            long lastStopTime = 0;
            long lastDur = 0;
            int lastNoteNum = 0;
            long last50MsTimeStamp = 0;
            bool hasNotes = false;
            int lastChannel = 0;
            int lastVelocity = 0;
            
            for (var j = 0; j < notes.Count; j++)
            {
                long startTime = notes[j].GetTimedNoteOnEvent().Time;
                long stopTime;
                long dur = notes[j].Length;
                int noteNum = notes[j].NoteNumber;
                int channel = notes[j].Channel;
                int velocity = notes[j].Velocity;
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
                        thisNotes.Add(new Note((SevenBitNumber) lastNoteNum, lastDur, lastStartTime)
                        {
                            Channel = (FourBitNumber) lastChannel,
                            Velocity = (SevenBitNumber) lastVelocity,
                            OffVelocity = (SevenBitNumber) lastVelocity
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
                thisNotes.Add(new Note((SevenBitNumber) lastNoteNum, lastDur, lastStartTime)
                {
                    Channel = (FourBitNumber) lastChannel,
                    Velocity = (SevenBitNumber) lastVelocity,
                    OffVelocity = (SevenBitNumber) lastVelocity
                });

            return Task.FromResult(thisNotes.OrderBy(x => x.Time).ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        internal static async Task<List<Note>> OffSet50Ms(this Task<List<Note>> notes) => await OffSet50Ms(await notes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        internal static Task<List<Note>> FixEndSpacing(this List<Note> notes)
        {
            notes = notes.OrderBy(x=>x.Time).ToList();
            
            var thisNotes = new List<Note>(notes.Count);
            for (var j = 0; j < notes.Count; j++)
            {
                var noteNum = notes[j].NoteNumber;
                var time = notes[j].Time;
                var dur = notes[j].Length;
                var channel = notes[j].Channel;
                var velocity = notes[j].Velocity;

                if (j + 1 < notes.Count)
                {
                    if (notes[j + 1].Time <= notes[j].Time + notes[j].Length + 25)
                    {
                        dur = notes[j + 1].Time - notes[j].Time - 25;
                        dur = dur < 25 ? 25 : dur;
                    }
                }
                thisNotes.Add(new Note(noteNum, dur, time)
                {
                    Channel = channel,
                    Velocity = velocity,
                    OffVelocity = velocity
                });
            }

            return Task.FromResult(thisNotes.OrderBy(x => x.Time).ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        internal static async Task<List<Note>> FixEndSpacing(this Task<List<Note>> notes) => await FixEndSpacing(await notes);
    }
}