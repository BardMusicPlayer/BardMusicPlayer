/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections;
using System.Globalization;
using System.Threading.Tasks;
using BardMusicPlayer.Common;
using BardMusicPlayer.Notate.Song;
using BardMusicPlayer.Synth.AlphaTab;
using BardMusicPlayer.Synth.AlphaTab.Audio.Synth;
using BardMusicPlayer.Synth.Properties;
using NAudio.CoreAudioApi;

namespace BardMusicPlayer.Synth
{
    public class Synthesizer
    {
        public string CurrentSongTitle { get; private set; } = "";
        private IAlphaSynth _player;

        private static readonly Lazy<Synthesizer> LazyInstance = new(() => new Synthesizer());
        public static Synthesizer Instance => LazyInstance.Value;

        internal Synthesizer()
        {
        }

        ~Synthesizer()
        {
            ShutDown();
        }

        /// <summary>
        /// Gets a collection of available MMDevice objects
        /// </summary>
        public MMDeviceCollection AudioDevices => new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="defaultVolume"></param>
        /// <param name="bufferCount"></param>
        /// <param name="latency"></param>
        public void Setup(MMDevice device, float defaultVolume = 0.8f, byte bufferCount = 3, byte latency = 100)
        {
            ShutDown();
            _player = new ManagedThreadAlphaSynthWorkerApi(new NAudioSynthOutput(device, bufferCount, latency), AlphaTab.Util.LogLevel.None, BeginInvoke);
            foreach (var resource in Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true))
                _player.LoadSoundFont((byte[])((DictionaryEntry)resource).Value, true);
            _player.PositionChanged += NotifyTimePosition;
            _player.MasterVolume = defaultVolume;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReady => _player != null && _player.IsReady;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadyForPlayback => IsReady && _player.IsReadyForPlayback;

        private readonly TaskQueue _taskQueue = new();
        internal void BeginInvoke(Action action) => _taskQueue.Enqueue(() => Task.Run(action));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultVolume"></param>
        /// <param name="bufferCount"></param>
        /// <param name="latency"></param>
        public void Setup(float defaultVolume = 0.8f, byte bufferCount = 2, byte latency = 100) => Setup(
            new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia), defaultVolume, bufferCount, latency);

        /// <summary>
        /// 
        /// </summary>
        public void ShutDown()
        {
            if (_player == null) return;
            _player.Stop();
            _player.PositionChanged -= NotifyTimePosition;
            _player.Destroy();
        }
        
        /// <summary>
        /// Loads a BmpSong into the synthesizer
        /// </summary>
        /// <param name="song"></param> 
        /// <returns>This Synthesizer</returns>
        public async Task<Synthesizer> Load(BmpSong song)
        {
            if (!IsReady) throw new BmpException("Synthesizer not initialized.");
            if (_player.State == PlayerState.Playing) _player.Stop();
            _player.LoadMidiFile(await song.GetSynthMidi());
            CurrentSongTitle = song.Title;
            return this;
        }

        /// <summary>
        /// Starts the playback if possible
        /// </summary>
        /// <returns>This Synthesizer</returns>
        public Synthesizer Play()
        {
            if (!IsReadyForPlayback) throw new BmpException("Synthesizer not loaded with a song.");
            _player.Play();
            return this;
        }

        /// <summary>
        /// Pauses the playback if was running
        /// </summary>
        /// <returns>This Synthesizer</returns>
        public Synthesizer Pause()
        {
            if (!IsReadyForPlayback) throw new BmpException("Synthesizer not loaded with a song.");
            _player.Pause();
            return this;
        }
        
        /// <summary>
        /// Stops the playback
        /// </summary>
        /// <returns>This Synthesizer</returns>
        public Synthesizer Stop()
        {
            if (!IsReadyForPlayback) throw new BmpException("Synthesizer not loaded with a song.");
            _player.Stop();
            return this;
        }

        /// <summary>
        /// Sets the current position of this song in milliseconds
        /// </summary>
        /// <returns>This Synthesizer</returns>
        public Synthesizer SetPosition(int time)
        {
            if (!IsReadyForPlayback) throw new BmpException("Synthesizer not loaded with a song.");
            if (time < 0) time = 0;
            if (time > _player.PlaybackRange.EndTick) return Stop();
            _player.TickPosition = time;
            return this;
        }

        /// <summary>
        /// Event fired when the position of a synthesized song changes.
        /// </summary>
        /// <param name="songTitle">The title of the current song.</param>
        /// <param name="currentTime">The current time of this song in milliseconds</param>
        /// <param name="endTime">The total length of this song in milliseconds</param>
        /// <param name="activeVoices">Active voice count.</param>
        public delegate void SynthTimePosition(string songTitle, double currentTime, double endTime, int activeVoices);

        public event SynthTimePosition SynthTimePositionChanged;

        internal void NotifyTimePosition(PositionChangedEventArgs obj) => SynthTimePositionChanged?.Invoke(CurrentSongTitle,obj.CurrentTime, obj.EndTime, obj.ActiveVoices);
    }
}
