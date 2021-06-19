using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using FFBardMusicCommon;

namespace FFBardMusicPlayer.Controls
{
    public partial class BmpPlayer : UserControl
    {
        // Player manager and manipulator
        public EventHandler<Track> OnMidiTrackLoad;
        public EventHandler<bool> OnMidiStatusChange;
        public EventHandler<int> OnMidiProgressChange;
        public EventHandler OnMidiStatusEnded;

        public class NoteEvent
        {
            public Track Track;
            public int TrackNum;
            public int Note;
            public int OrigNote;
        };

        public class ProgChangeEvent
        {
            public Track track;
            public int trackNum;
            public int voice;
        };

        public EventHandler<NoteEvent> OnMidiNote;
        public EventHandler<NoteEvent> OffMidiNote;
        public EventHandler<ProgChangeEvent> ProgChangeMidi;
        public EventHandler OnSongSkip;
        public EventHandler<PlayerStatus> OnStatusChange;
        private PlayerStatus bmpStatus;

        public enum PlayerStatus
        {
            Performing,
            Conducting
        }

        public PlayerStatus Status
        {
            set
            {
                bmpStatus = value;

                var solo = bmpStatus == PlayerStatus.Performing;

                SelectorOctave.Visible = solo;
                SelectorSpeed.Visible  = solo;

                switch (bmpStatus)
                {
                    case PlayerStatus.Performing:
                    {
                        PlayerGroup.Text = "Performing";
                        break;
                    }
                    case PlayerStatus.Conducting:
                    {
                        PlayerGroup.Text = "Conducting";
                        break;
                    }
                }

                Keyboard.Refresh();

                OnStatusChange?.Invoke(this, Status);
            }
            get => bmpStatus;
        }

        public bool Interactable
        {
            get => PlayTable.Enabled;
            set => PlayTable.Enabled = value;
        }

        public BmpKeyboard Keyboard => KeyboardCtl;

        public BmpSequencer Player { get; } = new BmpSequencer();

        private bool loop;

        public bool Loop
        {
            get => loop;
            set
            {
                loop = value;

                TrackLoop.Invoke(t => t.Checked = loop);
            }
        }

        public int Tempo { get; set; }

        private string trackname;

        public string TrackName
        {
            get => trackname;
            set
            {
                trackname = value;
                InfoTrackName.Invoke(t => t.Text = trackname);
            }
        }

        private int octaveShift;

        public int OctaveShift
        {
            get => octaveShift;
            set
            {
                octaveShift = value.Clamp(-4, 4);

                SelectorOctave.Invoke(t => t.Value = octaveShift);
                UpdateKeyboard(Player.LoadedTrack);
            }
        }

        private float speedShift = 1.0f;

        public float SpeedShift
        {
            get => speedShift;
            set
            {
                speedShift = value.Clamp(0.1f, 2.0f);

                SelectorSpeed.Invoke(t => t.Value = (decimal) (speedShift * 100f));
                Player.Speed = speedShift;
            }
        }

        public Instrument PreferredInstrument =>
            Player.LoadedTrack == null ? 0 : Player.GetTrackPreferredInstrument(Player.LoadedTrack);

        public int TotalNoteCount => Player.NotesPlayedCount.Values.Sum();

        public int CurrentNoteCount =>
            Player.LoadedTrack == null ? 0 : Player.NotesPlayedCount[Player.LoadedTrack];

        private bool trackHoldPlaying;
        private readonly Dictionary<Track, int> trackNumLut = new Dictionary<Track, int>();

        public BmpPlayer()
        {
            InitializeComponent();

            Player.OnLoad += OnPlayerMidiLoad;
            Player.OnTick += OnMidiTick;

            Player.OnTempoChange     += OnMidiTempoChange;
            Player.OnTrackNameChange += OnMidiTrackNameChange;

            Player.OnNote  += OnPlayerMidiNote;
            Player.OffNote += OffPlayerMidiNote;
            Player.ProgChange += ProgChangePlayerMidi;

            Player.PlayEnded        += OnPlayEnded;
            Player.PlayStatusChange += OnMidiPlayStatusChange;

            SelectorOctave.MouseWheel += Disable_Scroll;
            SelectorSpeed.MouseWheel  += Disable_Scroll;
            TrackProgress.MouseWheel  += Disable_Scroll;
            TrackSkip.Click           += TrackSkip_Click;

            Keyboard.MouseClick += Keyboard_Click;
        }

        private void TrackSkip_Click(object sender, EventArgs e) { OnSongSkip?.Invoke(sender, e); }

        private void Keyboard_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                var shiftKey = (ModifierKeys & Keys.Shift) != 0;
                if (shiftKey)
                {
                    switch (Status)
                    {
                        case PlayerStatus.Conducting:
                            Status = PlayerStatus.Performing;
                            break;
                        case PlayerStatus.Performing:
                            Status = PlayerStatus.Conducting;
                            break;
                    }
                }
            }
        }

        private int ApplyOctaveShift(int note)
        {
            // octaveShift now holds the track octave and the selected octave together
            var os = octaveShift;
            return NoteHelper.ApplyOctaveShift(note, os);
        }

        // Events
        private void OnPlayerMidiLoad(object o, EventArgs e)
        {
            OnMidiTrackLoad?.Invoke(o, Player.LoadedTrack);

            // set the initial octave shift here
            // this will also update the keyboard
            OctaveShift = Player.GetTrackPreferredOctaveShift(Player.LoadedTrack);

            TotalProgressInfo.Invoke(t => t.Text = Player.MaxTime);

            trackNumLut.Clear();
            for (var i = 0; i < Player.Sequence.Count; i++)
            {
                trackNumLut[Player.Sequence[i]] = i;
            }

            UpdatePlayer();
        }

        private int GetTrackLutNum(Track track)
        {
            if (track != null)
            {
                return trackNumLut.ContainsKey(track) ? trackNumLut[track] : 0;
            }

            return 0;
        }

        private void OnPlayerMidiNote(object o, ChannelMessageEventArgs e)
        {
            OnMidiNote?.Invoke(o, new NoteEvent
            {
                Track    = e.MidiTrack,
                TrackNum = GetTrackLutNum(e.MidiTrack),
                Note     = ApplyOctaveShift(e.Message.Data1),
                OrigNote = e.Message.Data1
            });
        }

        private void OffPlayerMidiNote(object o, ChannelMessageEventArgs e)
        {
            OffMidiNote?.Invoke(o, new NoteEvent
            {
                Track    = e.MidiTrack,
                TrackNum = GetTrackLutNum(e.MidiTrack),
                Note     = ApplyOctaveShift(e.Message.Data1),
                OrigNote = e.Message.Data1
            });
        }

        private void ProgChangePlayerMidi(Object o, ChannelMessageEventArgs e)
        {
            ProgChangeMidi?.Invoke(o, new ProgChangeEvent
            {
                track = e.MidiTrack,
                trackNum = GetTrackLutNum(e.MidiTrack),
                voice = e.Message.Data1,
            });
        }

        private void OnMidiTempoChange(object o, int tempo) { Tempo = tempo; }

        private void OnMidiTrackNameChange(object o, string name) { TrackName = name; }

        private void OnPlayEnded(object o, EventArgs e)
        {
            Player.Stop();
            OnMidiStatusEnded?.Invoke(this, EventArgs.Empty);
        }

        private void OnMidiPlayStatusChange(object o, EventArgs e)
        {
            UpdatePlayer();
            OnMidiStatusChange?.Invoke(o, Player.IsPlaying);
        }

        private void OnMidiTick(object o, int position)
        {
            if (position < TrackProgress.Maximum)
            {
                TrackProgress.Invoke(t => t.Value = position);

                CurrentProgressInfo.Invoke(t => t.Text = Player.CurrentTime);
            }
        }

        // Generic funcs

        public void LoadFile(string filename, int track)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                Player.Load(filename, track);
            }
        }

        public void UpdatePlayer()
        {
            TrackProgress.Invoke(t => t.Maximum = Player.MaxTick);
            TrackProgress.Invoke(t => t.Value   = 0);
            if (Player.CurrentTick < Player.MaxTick)
            {
                TrackProgress.Invoke(t => t.Value = Player.CurrentTick);
            }

            var playPauseBmp = Player.IsPlaying ? Properties.Resources.Pause : Properties.Resources.Play;
            TrackPlay.Invoke(t => t.Image = playPauseBmp);
        }

        public void UpdateKeyboard(Track track)
        {
            var notes = new List<int>();

            if (track == null)
                return;
            

            foreach (var ev in track.Iterator()
                .Where(f => f.MidiMessage.MessageType == MessageType.Channel))
            {
                if (ev.MidiMessage is ChannelMessage msg 
                    && msg.Command == ChannelCommand.NoteOn)
                {
                    var note = msg.Data1;
                    var vel = msg.Data2;
                    if (vel > 0)
                    {
                        notes.Add(ApplyOctaveShift(note));
                    }
                }
            }

            Keyboard.Invoke(t => t.UpdateFrequency(notes));
        }

        // UI Events

        private void Disable_Scroll(object sender, EventArgs e)
        {
            if (!((Control) sender).Enabled)
            {
                ((HandledMouseEventArgs) e).Handled = true;
            }
        }

        private void TrackProgress_MouseDown(object sender, MouseEventArgs e)
        {
            if (!Player.Loaded)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                trackHoldPlaying = Player.IsPlaying;
                Player.Pause();
                TrackProgress_MouseMove(sender, e);
            }
        }

        private void TrackProgress_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Player.Loaded)
            {
                return;
            }

            if (e.Button == MouseButtons.Left && !Player.IsPlaying)
            {
                var v = (float) (e.X - 6) / (TrackProgress.Width - 12);
                if (v >= 0f && v <= 1f)
                {
                    v               *= (TrackProgress.Maximum - TrackProgress.Minimum);
                    Player.Position =  (int) v;
                    OnMidiProgressChange?.Invoke(this, (int) v);
                    UpdatePlayer();
                }
            }
        }

        private void TrackProgress_MouseUp(object sender, MouseEventArgs e)
        {
            if (!Player.Loaded)
            {
                return;
            }

            if (e.Button == MouseButtons.Left && !Player.IsPlaying)
            {
                if (trackHoldPlaying)
                {
                    Player.Play();
                }

                UpdatePlayer();
                if (Player.CurrentTick > Player.MaxTick)
                    return;
                
                TrackProgress.Invoke(t => t.Value = Player.CurrentTick);
            }
        }

        private void TrackPlay_Click(object sender, EventArgs e)
        {
            if (Player.IsPlaying)
            {
                Player.Pause();
            }
            else
            {
                Player.Play();
            }
        }

        private void SelectorSpeed_ValueChanged(object sender, EventArgs e)
        {
            var speed = ((NumericUpDown) sender).Value;
            var ss = decimal.ToSingle(speed) / 100f;
            SpeedShift = ss;
        }

        private void SelectorOctave_ValueChanged(object sender, EventArgs e)
        {
            var octave = ((NumericUpDown) sender).Value;
            var os = decimal.ToInt32(octave);
            OctaveShift = os;
        }

        private void TrackLoop_CheckedChanged(object sender, EventArgs e)
        {
            var l = ((CheckBox) sender).Checked;
            Loop = l;
        }

        private void PlayerControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Shift && e.Button == MouseButtons.Left)
            {
                switch (Status)
                {
                    case PlayerStatus.Conducting:
                        Status = PlayerStatus.Performing;
                        break;
                    case PlayerStatus.Performing:
                        Status = PlayerStatus.Conducting;
                        break;
                }
            }
        }
    }

    public static class NoteHelper
    {
        public static int ApplyOctaveShift(int note, int octave) => note - 12 * 4 + 12 * octave;
    }
}