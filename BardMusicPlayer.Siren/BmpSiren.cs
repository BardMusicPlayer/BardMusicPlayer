/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BardMusicPlayer.Quotidian;
using BardMusicPlayer.Siren.AlphaTab;
using BardMusicPlayer.Siren.AlphaTab.Audio.Synth;
using BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Midi;
using BardMusicPlayer.Siren.Properties;
using BardMusicPlayer.Transmogrify.Song;
using NAudio.CoreAudioApi;

namespace BardMusicPlayer.Siren
{
    public class BmpSiren
    {
        public string CurrentSongTitle { get; private set; } = "";
        private IAlphaSynth _player;
        private Dictionary<int, Dictionary<long, string>> _lyrics;
        private double _lyricIndex;

        private static readonly Lazy<BmpSiren> LazyInstance = new(() => new BmpSiren());
        public static BmpSiren Instance => LazyInstance.Value;

        internal BmpSiren()
        {
        }

        ~BmpSiren()
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
        public void Setup(float defaultVolume = 0.8f, byte bufferCount = 2, byte latency = 100)
        {
            var mmAudio = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            Setup(mmAudio, defaultVolume, bufferCount, latency);
        }

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
        /// <returns>This BmpSiren</returns>
        public async Task<BmpSiren> Load(BmpSong song)
        {
            if (!IsReady) throw new BmpException("Siren not initialized.");
            if (_player.State == PlayerState.Playing) _player.Stop();
            MidiFile midiFile;
            (midiFile, _lyrics) = await song.GetSynthMidi();
            _lyricIndex = 0;
            _player.LoadMidiFile(midiFile);
            CurrentSongTitle = song.Title;
            return this;
        }

        /// <summary>
        /// Starts the playback if possible
        /// </summary>
        /// <returns>This BmpSiren</returns>
        public BmpSiren Play()
        {
            if (!IsReadyForPlayback) throw new BmpException("Siren not loaded with a song.");
            _player.Play();
            return this;
        }

        /// <summary>
        /// Pauses the playback if was running
        /// </summary>
        /// <returns>This BmpSiren</returns>
        public BmpSiren Pause()
        {
            if (!IsReadyForPlayback) throw new BmpException("Siren not loaded with a song.");
            _player.Pause();
            return this;
        }
        
        /// <summary>
        /// Stops the playback
        /// </summary>
        /// <returns>This BmpSiren</returns>
        public BmpSiren Stop()
        {
            if (!IsReadyForPlayback) throw new BmpException("Siren not loaded with a song.");
            _player.Stop();
            _lyricIndex = 0;
            return this;
        }

        /// <summary>
        /// Sets the current position of this song in milliseconds
        /// </summary>
        /// <returns>This BmpSiren</returns>
        public BmpSiren SetPosition(int time)
        {
            if (!IsReadyForPlayback) throw new BmpException("Siren not loaded with a song.");
            if (time < 0) time = 0;
            if (time > _player.PlaybackRange.EndTick) return Stop();
            _player.TickPosition = time;
            _lyricIndex = time;
            return this;
        }

        /// <summary>
        /// Event fired when there is a lyric line.
        /// </summary>
        /// <param name="singer"></param>
        /// <param name="line"></param>
        public delegate void Lyric(int singer, string line);

        public event Lyric LyricTrigger;

        /// <summary>
        /// Event fired when the position of a synthesized song changes.
        /// </summary>
        /// <param name="songTitle">The title of the current song.</param>
        /// <param name="currentTime">The current time of this song in milliseconds</param>
        /// <param name="endTime">The total length of this song in milliseconds</param>
        /// <param name="activeVoices">Active voice count.</param>
        public delegate void SynthTimePosition(string songTitle, double currentTime, double endTime, int activeVoices);

        public event SynthTimePosition SynthTimePositionChanged;

        internal void NotifyTimePosition(PositionChangedEventArgs obj)
        {
            SynthTimePositionChanged?.Invoke(CurrentSongTitle, obj.CurrentTime, obj.EndTime, obj.ActiveVoices);
            for (var singer = 0; singer < _lyrics.Count; singer++)
            {
                var line = _lyrics[singer].FirstOrDefault(x => x.Key > _lyricIndex && x.Key < obj.CurrentTime).Value;
                if (!string.IsNullOrWhiteSpace(line)) LyricTrigger?.Invoke(singer, line);
            }
            _lyricIndex = obj.CurrentTime;
        }
    }
}
