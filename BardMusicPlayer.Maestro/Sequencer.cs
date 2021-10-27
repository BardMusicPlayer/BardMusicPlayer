using System;
using BardMusicPlayer.Grunt;
using BardMusicPlayer.Seer;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;

namespace BardMusicPlayer.Maestro
{
    public partial class Sequencer
    {
        private Game _game;
        private Playback _playback;

        private int _tracknumber = 0;
        ITimeSpan _startingpoint;
        public Sequencer(Game game, MidiFile container, int tracknr = -1)
        {
            _game = game;
            _playback = container.GetPlayback();
            //Start the melanchall sequencer
            PlaybackCurrentTimeWatcher.Instance.AddPlayback(_playback, TimeSpanType.Metric);
            PlaybackCurrentTimeWatcher.Instance.CurrentTimeChanged += OnTick;
            PlaybackCurrentTimeWatcher.Instance.Start();

            _playback.Speed = 1;                    //Yep that's the playback speed and we'll set it
            _playback.EventPlayed += OnNoteEvent;
            _tracknumber = tracknr;

            BmpMaestro.Instance.OnSongMaxTime?.Invoke(this, _playback.GetDuration(TimeSpanType.Metric));
        }

        public void SetPlaybackStart(double f)
        {
            TimeSpan time = TimeSpan.FromMilliseconds(f / 1000); //We have microseconds and want some milis....
            _startingpoint = new MetricTimeSpan(hours: time.Hours, minutes: time.Minutes, seconds: time.Seconds, milliseconds: time.Milliseconds);

            _playback.MoveToTime(_startingpoint);
        }

        public void OnTick(object sender, PlaybackCurrentTimeChangedEventArgs e)
        {
            BmpMaestro.Instance.OnPlaybackTimeChanged?.Invoke(this, _playback.GetCurrentTime(TimeSpanType.Metric));
        }

        public void OnNoteEvent(object sender, MidiEventPlayedEventArgs e)
        {
            switch (e.Event.EventType)
            {
                case MidiEventType.SetTempo:
                    var tempo = e.Event as SetTempoEvent;
                    return;
                case MidiEventType.NoteOn:
                    NoteOnEvent non = e.Event as NoteOnEvent;
                    if ((non.Channel == _tracknumber) || (_tracknumber == -1))
                        GameExtensions.SendNoteOn(_game, non.NoteNumber, non.Channel); //No await here, we don't wait else we'll get a messy timing
                    return;
                case MidiEventType.NoteOff:
                    NoteOffEvent noff = e.Event as NoteOffEvent;
                    if ((noff.Channel == _tracknumber) || (_tracknumber == -1))
                        GameExtensions.SendNoteOff(_game, noff.NoteNumber); //same as above
                    return;
                case MidiEventType.ProgramChange:
                    ProgramChangeEvent prog = e.Event as ProgramChangeEvent;
                    if (prog.Channel == _tracknumber) //No progchange for all tracks, else it will be messy as hell
                    {
                        Console.WriteLine(prog.ProgramNumber);
                    }
                    return;
                default:
                    return;
            }
        }

        public void ChangeTracknumer(int track)
        { _tracknumber = track; }

        public void Start()
        {
            _playback.Start();      //start from the point you stopped
        }

        public void Pause()
        {
            _playback.Stop();       //missleading, it only pauses
        }

        public void Stop()
        {
            _playback.Stop();        //Stop
            _playback.MoveToStart(); //To the beginning of da song
        }

        public void Destroy()
        {
            _playback.Dispose();
        }

    }
}