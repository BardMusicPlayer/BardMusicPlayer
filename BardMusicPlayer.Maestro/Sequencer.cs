using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using BardMusicPlayer.Grunt;
using BardMusicPlayer.Maestro.Events;
using BardMusicPlayer.Quotidian.Enums;
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
            //PlaybackCurrentTimeWatcher.Instance.PollingInterval = TimeSpan.FromMilliseconds(250);  //Not sure, but seems to affect OnNoteEvent polling too
            PlaybackCurrentTimeWatcher.Instance.Start();
            
            _playback.Speed = 1;                    //Yep that's the playback speed and we'll set it
            _playback.EventPlayed += OnNoteEvent;
            _playback.Stopped     += OnPlaybackStopped;
            _tracknumber = tracknr;

            BmpMaestro.Instance.PublishEvent(new MaxPlayTimeEvent(_playback.GetDuration(TimeSpanType.Metric)));
        }

        public void SetPlaybackStart(double f)
        {
            TimeSpan time = TimeSpan.FromMilliseconds(f/1000); //We have microseconds and want some milis....
            _startingpoint = new MetricTimeSpan(hours: time.Hours, minutes: time.Minutes, seconds: time.Seconds, milliseconds: time.Milliseconds);

            _playback.MoveToTime(_startingpoint);
        }

        public void OnTick(object sender, PlaybackCurrentTimeChangedEventArgs e)
        {
            BmpMaestro.Instance.PublishEvent(new CurrentPlayPositionEvent(_playback.GetCurrentTime(TimeSpanType.Metric)));
        }

        private void OnPlaybackStopped(object sender, EventArgs e)
        {
            BmpMaestro.Instance.PublishEvent(new PlaybackStoppedEvent());
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
                       _ = GameExtensions.SendNoteOn(_game, non.NoteNumber, non.Channel); //No await here, we don't wait else we'll get a messy timing
                    return;
                case MidiEventType.NoteOff:
                    NoteOffEvent noff = e.Event as NoteOffEvent;
                    if ((noff.Channel == _tracknumber) || (_tracknumber == -1))
                        _ = GameExtensions.SendNoteOff(_game, noff.NoteNumber); //same as above
                    return;
                case MidiEventType.ProgramChange:
                    ProgramChangeEvent prog = e.Event as ProgramChangeEvent;
                    if (prog.Channel == _tracknumber) //No progchange for all tracks, else it will be messy as hell
                        _ = GameExtensions.GuitarByPrognumber(_game, prog.ProgramNumber);
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
            _playback.Stop();
            _playback.Dispose();
        }

    }
}
