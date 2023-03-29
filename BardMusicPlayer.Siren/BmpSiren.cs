/*
 * Copyright(c) 2023 MoogleTroupe, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Quotidian;
using BardMusicPlayer.Siren.AlphaTab;
using BardMusicPlayer.Siren.AlphaTab.Audio.Synth;
using BardMusicPlayer.Siren.AlphaTab.Util;
using BardMusicPlayer.Transmogrify.Song;
using NAudio.CoreAudioApi;

namespace BardMusicPlayer.Siren;

public class BmpSiren
{
    public string CurrentSongTitle { get; private set; } = "";
    public BmpSong CurrentSong { get; private set; }

    internal IAlphaSynth _player;
    private Dictionary<int, Dictionary<long, string>> _lyrics;
    private double _lyricIndex;
    private MMDevice _mdev;
    internal bool _vstDownloaded = false;
    internal string _vstLocation = "";

    private static readonly System.Lazy<BmpSiren> LazyInstance = new(() => new BmpSiren());
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
    /// <param name="vstLocation">Storage location for VST files</param>
    /// <param name="defaultVolume"></param>
    /// <param name="bufferCount"></param>
    /// <param name="latency"></param>
    public void Setup(MMDevice device, string vstLocation, float defaultVolume = 0.4f, byte bufferCount = 2, byte latency = 100)
    {
        if (Environment.GetEnvironmentVariable("WINEPREFIX") != null) return; // Temporary Mac/Linux disable.
        ShutDown();
        _mdev        = device;
        _vstLocation = vstLocation + @"\";
        VSTLoader.UpdateAndLoadVST();
        _player = new ManagedThreadAlphaSynthWorkerApi(new NAudioSynthOutput(device, bufferCount, latency), LogLevel.None, BeginInvoke);
        _player.PositionChanged += NotifyTimePosition;
        _player.MasterVolume    =  defaultVolume;
    }

    /// <summary>
    /// Sets the volume
    /// </summary>
    public int GetVolume()
    {
        if (Environment.GetEnvironmentVariable("WINEPREFIX") != null) return 0; // Temporary Mac/Linux disable.
        return (int)(_mdev.AudioSessionManager.AudioSessionControl.SimpleAudioVolume.Volume * 30);
    }

    /// <summary>
    /// Sets the volume
    /// </summary>
    /// <param name="x">Used to get the maximum on scale position.</param>
    /// <param name="max">Reduces the maximum volume by a fraction out of 100. i.e. max: 20 = 1/5th volume</param>
    public void SetVolume(float x, float max)
    {
        if (Environment.GetEnvironmentVariable("WINEPREFIX") != null) return; // Temporary Mac/Linux disable.
        _mdev.AudioSessionManager.AudioSessionControl.SimpleAudioVolume.Volume = x / 30 * (max / 100);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsReady => _vstDownloaded && _player is { IsReady: true };

    /// <summary>
    /// 
    /// </summary>
    public bool IsReadyForPlayback => _vstDownloaded && IsReady && _player.IsReadyForPlayback;

    private readonly TaskQueue _taskQueue = new();
    internal void BeginInvoke(Action action) => _taskQueue.Enqueue(() => Task.Run(action));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataPath">Storage location for VST files</param>
    /// <param name="defaultVolume"></param>
    /// <param name="bufferCount"></param>
    /// <param name="latency"></param>
    public void Setup(string vstLocation, float defaultVolume = 0.4f, byte bufferCount = 2, byte latency = 100)
    {
        if (Environment.GetEnvironmentVariable("WINEPREFIX") != null) return; // Temporary Mac/Linux disable.
        var mmAudio = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        Setup(mmAudio, vstLocation, defaultVolume, bufferCount, latency);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ShutDown()
    {
        if (Environment.GetEnvironmentVariable("WINEPREFIX") != null) return; // Temporary Mac/Linux disable.
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
        if (Environment.GetEnvironmentVariable("WINEPREFIX") != null) return this; // Temporary Mac/Linux disable.
        if (!IsReady) throw new BmpException("Siren not initialized.");
        if (_player.State == PlayerState.Playing) _player.Stop();
        (var midiFile, _lyrics) = await song.GetSynthMidi();
        _lyricIndex             = 0;
        _player.LoadMidiFile(midiFile);
        CurrentSongTitle = song.Title;
        CurrentSong      = song;
        return this;
    }

    /// <summary>
    /// Starts the playback if possible
    /// </summary>
    /// <returns>This BmpSiren</returns>
    public BmpSiren Play()
    {
        if (Environment.GetEnvironmentVariable("WINEPREFIX") != null) return this; // Temporary Mac/Linux disable.
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
        if (Environment.GetEnvironmentVariable("WINEPREFIX") != null) return this; // Temporary Mac/Linux disable.
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
        if (Environment.GetEnvironmentVariable("WINEPREFIX") != null) return this; // Temporary Mac/Linux disable.
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
        if (Environment.GetEnvironmentVariable("WINEPREFIX") != null) return this; // Temporary Mac/Linux disable.
        if (!IsReadyForPlayback) return this; // throw new BmpException("Siren not loaded with a song.");
        if (time < 0) time = 0;
        //if (time > _player.PlaybackRange.EndTick) return Stop();
        _player.TickPosition = time;
        _lyricIndex          = time;
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