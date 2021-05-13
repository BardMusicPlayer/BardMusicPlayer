﻿/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Siren.AlphaTab.Model;

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Generator
{
    /// <summary>
    /// A handler is responsible for writing midi events to a custom structure
    /// </summary>
    internal interface IMidiFileHandler
    {
        /// <summary>
        /// Adds a time signature to the generated midi file
        /// </summary>
        /// <param name="tick">The midi ticks when this event should be happening. </param>
        /// <param name="timeSignatureNumerator">The time signature numerator</param>
        /// <param name="timeSignatureDenominator">The time signature denominator</param>
        void AddTimeSignature(int tick, int timeSignatureNumerator, int timeSignatureDenominator);

        /// <summary>
        /// Adds a rest to the generated midi file. 
        /// </summary>
        /// <param name="track">The midi track on which the rest should be "played".</param>
        /// <param name="tick">The midi ticks when the rest is "playing". </param>
        /// <param name="channel">The midi channel on which the rest should be "played".</param>
        void AddRest(int track, int tick, int channel);

        /// <summary>
        /// Adds a note to the generated midi file
        /// </summary>
        /// <param name="track">The midi track on which the note should be played.</param>
        /// <param name="start">The midi ticks when the note should start playing. </param>
        /// <param name="length">The duration the note in midi ticks. </param>
        /// <param name="key">The key of the note to play</param>
        /// <param name="dynamicValue">The dynamic which should be applied to the note. </param>
        /// <param name="channel">The midi channel on which the note should be played.</param>
        void AddNote(int track, int start, int length, byte key, DynamicValue dynamicValue, byte channel);

        /// <summary>
        /// Adds a control change to the generated midi file. 
        /// </summary>
        /// <param name="track">The midi track on which the controller should change.</param>
        /// <param name="tick">The midi ticks when the controller should change.</param>
        /// <param name="channel">The midi channel on which the controller should change.</param>
        /// <param name="controller">The midi controller that should change.</param>
        /// <param name="value">The value to which the midi controller should change</param>
        void AddControlChange(int track, int tick, byte channel, byte controller, byte value);

        /// <summary>
        /// Add a program change to the generated midi file
        /// </summary>
        /// <param name="track">The midi track on which the program should change.</param>
        /// <param name="tick">The midi ticks when the program should change.</param>
        /// <param name="channel">The midi channel on which the program should change.</param>
        /// <param name="program">The new program for the selected track and channel.</param>
        void AddProgramChange(int track, int tick, byte channel, byte program);

        /// <summary>
        /// Add a tempo change to the generated midi file. 
        /// </summary>
        /// <param name="tick">The midi ticks when the tempo should change change.</param>
        /// <param name="tempo">The tempo as BPM</param>
        void AddTempo(int tick, int tempo);

        /// <summary>
        /// Add a bend to the generated midi file. 
        /// </summary>
        /// <param name="track">The midi track on which the bend should change.</param>
        /// <param name="tick">The midi ticks when the bend should change.</param>
        /// <param name="channel">The midi channel on which the bend should change.</param>
        /// <param name="value">The new bend for the selected track and channel.</param>
        void AddBend(int track, int tick, byte channel, int value);

        /// <summary>
        /// Indicates that the track is finished on the given ticks.
        /// </summary>
        /// <param name="track">The track that was finished. </param>
        /// <param name="tick">The end tick for this track.</param>
        void FinishTrack(int track, int tick);
    }
}
