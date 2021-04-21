/*
 * MogLib/Notate/LoadBalancer.cs
 *
 * Copyright (C) 2021  MoogleTroupe
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System.Collections.Generic;
using System.Linq;
using BardMusicPlayer.Common;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Notate
{
    internal class LoadBalancer
    {
        private readonly Stack<BardVoice> _freeVoices;
        private readonly Stack<BardVoice> _activeVoices;
        private readonly BardVoice[,,] _registry;
        
        internal LoadBalancer(int voiceCount)
        {
            var voicePool = new BardVoice[voiceCount];
            for (var x = 0; x < voicePool.Length; x++) voicePool[x] = new BardVoice(x);
            _freeVoices = new Stack<BardVoice>(voicePool.Reverse());
            _activeVoices = new Stack<BardVoice>();
            _registry = new BardVoice[16, 128, 128];
        }

        internal (int, Note) NotifyNoteOn(long time, int channel, int note, int velocity)
        {
            BardVoice voice;
            if (_freeVoices.Count > 0)
            {
                voice = _freeVoices.Pop();
                voice.Start(time, channel, note, velocity);
                _registry[channel, note, velocity] = voice;
                _activeVoices.Push(voice);
                return (-1, null);
            }

            voice = _activeVoices.Pop();
            var (stoppedBard, stoppedNote) = voice.Stop(time);
            _registry[voice.channel, voice.note, voice.velocity] = null;

            voice.Start(time, channel, note, velocity);
            _registry[channel, note, velocity] = voice;
            _activeVoices.Push(voice);

            return (stoppedBard, stoppedNote);
        }

        internal (int, Note) NotifyNoteOff(long time, int channel, int note, int velocity)
        {
            if (_registry[channel, note, velocity] == null) return (-1, null);
            var voice = _registry[channel, note, velocity];
            _registry[channel, note, velocity] = null;
            var (stoppedBard, stoppedNote) = voice.Stop(time);
            _activeVoices.Remove(voice);
            _freeVoices.Push(voice);
            return (stoppedBard, stoppedNote);
        }

        internal class BardVoice
        {
            internal int bardNumber { get; }
            internal int channel { get; private set; }
            internal int note { get; private set; }
            internal int velocity { get; private set; }
            internal long time { get; private set; }
            internal BardVoice(int bardNumber)
            {
                this.bardNumber = bardNumber;
            }
            internal void Start(long startTime, int startChannel, int startNote, int startVelocity)
            {
                channel = startChannel;
                note = startNote;
                velocity = startVelocity;
                time = startTime;
            }
            internal (int,Note) Stop(long stopTime) => (bardNumber, new Note((SevenBitNumber) note, stopTime - time <= 0 ? 1 : stopTime - time, time));
        }
    }
}
