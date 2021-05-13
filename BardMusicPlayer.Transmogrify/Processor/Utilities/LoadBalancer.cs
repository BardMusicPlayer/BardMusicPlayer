/*
 * Load Balancer based on example from CSharpSynthProject ( https://archive.codeplex.com/?p=csharpsynthproject & https://github.com/sinshu/CSharpSynthProject )
 * C# Digital Audio Synth 
 * Copyright Alex Veltsistas 2014  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using BardMusicPlayer.Quotidian;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Transmogrify.Processor.Utilities
{
    internal sealed class LoadBalancer : IDisposable
    {
        private readonly SortedDictionary<int, BardVoice> _freeVoices;
        private readonly Stack<BardVoice> _activeVoices;
        private readonly BardVoice[,,] _registry;
        
        internal LoadBalancer(int voiceCount)
        {
            var voicePool = new BardVoice[voiceCount];
            for (var x = 0; x < voicePool.Length; x++) voicePool[x] = new BardVoice(x);
            _freeVoices = new SortedDictionary<int, BardVoice>();
            foreach (var voice in voicePool.Reverse()) _freeVoices.Add(voice.BardNumber, voice);
            _activeVoices = new Stack<BardVoice>();
            _registry = new BardVoice[16, 128, 128];
        }

        internal (int, Note) NotifyNoteOn(long time, int channel, int note, int velocity)
        {
            BardVoice voice;
            if (_freeVoices.Count > 0)
            {
                var voiceKvp = _freeVoices.First();
                voice = voiceKvp.Value;
                _freeVoices.Remove(voiceKvp.Key);
                voice.Start(time, channel, note, velocity);
                _registry[channel, note, velocity] = voice;
                _activeVoices.Push(voice);
                return (-1, null);
            }

            voice = _activeVoices.Pop();
            var (stoppedBard, stoppedNote) = voice.Stop(time);
            _registry[voice.Channel, voice.Note, voice.Velocity] = null;

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
            _freeVoices.Add(voice.BardNumber, voice);
            return (stoppedBard, stoppedNote);
        }

        public void Dispose()
        {
            _freeVoices.Clear();
            _activeVoices.Clear();
        }

        internal class BardVoice
        {
            internal int BardNumber { get; }
            internal int Channel { get; private set; }
            internal int Note { get; private set; }
            internal int Velocity { get; private set; }
            internal long Time { get; private set; }
            internal BardVoice(int bardNumber)
            {
                BardNumber = bardNumber;
            }
            internal void Start(long startTime, int startChannel, int startNote, int startVelocity)
            {
                Channel = startChannel;
                Note = startNote;
                Velocity = startVelocity;
                Time = startTime;
            }
            internal (int,Note) Stop(long stopTime) => (BardNumber, new Note((SevenBitNumber) Note, stopTime - Time <= 0 ? 1 : stopTime - Time, Time)
            {
                Channel = (FourBitNumber) Channel,
                Velocity = (SevenBitNumber) 127
            });
        }
    }
}
