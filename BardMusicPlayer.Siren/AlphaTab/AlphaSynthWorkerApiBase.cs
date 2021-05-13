﻿/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

using System;
using BardMusicPlayer.Siren.AlphaTab.Audio.Synth;
using BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Midi;
using BardMusicPlayer.Siren.AlphaTab.Util;

namespace BardMusicPlayer.Siren.AlphaTab
{
    internal abstract class AlphaSynthWorkerApiBase : IAlphaSynth
    {
        private readonly ISynthOutput _output;
        private LogLevel _logLevel;

        protected AlphaSynth Player;

        protected AlphaSynthWorkerApiBase(ISynthOutput output, LogLevel logLevel)
        {
            _output = output;
            _logLevel = logLevel;
        }

        public abstract void Destroy();
        protected abstract void DispatchOnUiThread(Action action);
        protected abstract void DispatchOnWorkerThread(Action action);

        protected void Initialize()
        {
            Player = new AlphaSynth(_output);
            Player.PositionChanged += OnPositionChanged;
            Player.StateChanged += OnStateChanged;
            Player.Finished += OnFinished;
            Player.SoundFontLoaded += OnSoundFontLoaded;
            Player.SoundFontLoadFailed += OnSoundFontLoadFailed;
            Player.MidiLoaded += OnMidiLoaded;
            Player.MidiLoadFailed += OnMidiLoadFailed;
            Player.ReadyForPlayback += OnReadyForPlayback;

            DispatchOnUiThread(OnReady);
        }

        public bool IsReady => Player != null && Player.IsReady;
        public bool IsReadyForPlayback => Player != null && Player.IsReadyForPlayback;

        public PlayerState State => Player == null ? PlayerState.Paused : Player.State;

        public LogLevel LogLevel
        {
            get => _logLevel;
            set
            {
                _logLevel = value;
                DispatchOnWorkerThread(() => { Player.LogLevel = value; });
            }
        }

        public float MasterVolume
        {
            get => Player.MasterVolume;
            set => DispatchOnWorkerThread(() => { Player.MasterVolume = value; });
        }

        public double PlaybackSpeed
        {
            get => Player.PlaybackSpeed;
            set => DispatchOnWorkerThread(() => { Player.PlaybackSpeed = value; });
        }

        public int TickPosition
        {
            get => Player.TickPosition;
            set => DispatchOnWorkerThread(() => { Player.TickPosition = value; });
        }

        public double TimePosition
        {
            get => Player.TimePosition;
            set => DispatchOnWorkerThread(() => { Player.TimePosition = value; });
        }

        public PlaybackRange PlaybackRange
        {
            get => Player.PlaybackRange;
            set => DispatchOnWorkerThread(() => { Player.PlaybackRange = value; });
        }

        public bool IsLooping
        {
            get => Player.IsLooping;
            set => DispatchOnWorkerThread(() => { Player.IsLooping = value; });
        }

        public bool Play()
        {
            if (State == PlayerState.Playing || !IsReadyForPlayback)
            {
                return false;
            }
            DispatchOnWorkerThread(() => { Player.Play(); });
            return true;
        }

        public void Pause()
        {
            DispatchOnWorkerThread(() => { Player.Pause(); });
        }

        public void PlayPause()
        {
            DispatchOnWorkerThread(() => { Player.PlayPause(); });
        }

        public void Stop()
        {
            DispatchOnWorkerThread(() => { Player.Stop(); });
        }

        public void LoadSoundFont(byte[] data, bool append)
        {
            DispatchOnWorkerThread(() => { Player.LoadSoundFont(data, append); });
        }

        public void LoadMidiFile(MidiFile midi)
        {
            DispatchOnWorkerThread(() => { Player.LoadMidiFile(midi); });
        }

        public void SetChannelMute(int channel, bool mute)
        {
            DispatchOnWorkerThread(() => { Player.SetChannelMute(channel, mute); });
        }

        public void ResetChannelStates()
        {
            DispatchOnWorkerThread(() => { Player.ResetChannelStates(); });
        }

        public void SetChannelSolo(int channel, bool solo)
        {
            DispatchOnWorkerThread(() => { Player.SetChannelSolo(channel, solo); });
        }

        public void SetChannelVolume(int channel, float volume)
        {
            DispatchOnWorkerThread(() => { Player.SetChannelVolume(channel, volume); });
        }

        public void SetChannelProgram(int channel, byte program)
        {
            DispatchOnWorkerThread(() => { Player.SetChannelProgram(channel, program); });
        }

        public event Action Ready;
        public event Action ReadyForPlayback;
        public event Action Finished;
        public event Action SoundFontLoaded;
        public event Action<Exception> SoundFontLoadFailed;
        public event Action MidiLoaded;
        public event Action<Exception> MidiLoadFailed;
        public event Action<PlayerStateChangedEventArgs> StateChanged;
        public event Action<PositionChangedEventArgs> PositionChanged;

        protected virtual void OnReady()
        {
            DispatchOnUiThread(() => Ready?.Invoke());
        }

        protected virtual void OnReadyForPlayback()
        {
            DispatchOnUiThread(() => ReadyForPlayback?.Invoke());
        }

        protected virtual void OnFinished()
        {
            DispatchOnUiThread(() => Finished?.Invoke());
        }

        protected virtual void OnSoundFontLoaded()
        {
            DispatchOnUiThread(() => SoundFontLoaded?.Invoke());
        }

        protected virtual void OnSoundFontLoadFailed(Exception e)
        {
            DispatchOnUiThread(() => SoundFontLoadFailed?.Invoke(e));
        }

        protected virtual void OnMidiLoaded()
        {
            DispatchOnUiThread(() => MidiLoaded?.Invoke());
        }

        protected virtual void OnMidiLoadFailed(Exception e)
        {
            DispatchOnUiThread(() => MidiLoadFailed?.Invoke(e));
        }

        protected virtual void OnStateChanged(PlayerStateChangedEventArgs obj)
        {
            DispatchOnUiThread(() => StateChanged?.Invoke(obj));
        }

        protected virtual void OnPositionChanged(PositionChangedEventArgs obj)
        {
            DispatchOnUiThread(() => PositionChanged?.Invoke(obj));
        }
    }
}
