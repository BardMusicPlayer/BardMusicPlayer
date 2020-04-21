using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using System.Text.RegularExpressions;
using static Sharlayan.Core.Enums.Performance;
using FFBardMusicPlayer.Components;
using System.IO;
using FFBardMusicCommon;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpPlayer : UserControl {

		// Player manager and manipulator
		BmpSequencer player = new BmpSequencer();

		public EventHandler<Track> OnMidiTrackLoad;
		public EventHandler<string> OnMidiLyric;
		public EventHandler<bool> OnMidiStatusChange;
		public EventHandler<int> OnMidiProgressChange;
		public EventHandler OnMidiStatusEnded;

		public class NoteEvent {
			public Track track;
			public int trackNum;
			public int note;
			public int origNote;
		};
		public EventHandler<NoteEvent> OnMidiNote;
		public EventHandler<NoteEvent> OffMidiNote;

		public EventHandler OnSongSkip;

		public EventHandler<PlayerStatus> OnStatusChange;
		private PlayerStatus bmpStatus;
		public enum PlayerStatus {
			Performing,
			Conducting,
		}

		public PlayerStatus Status {
			set {
				bmpStatus = value;

				bool solo = (bmpStatus == PlayerStatus.Performing);

				SelectorOctave.Visible = solo;
				SelectorSpeed.Visible = solo;

				switch(bmpStatus) {
					case PlayerStatus.Performing: {
						PlayerGroup.Text = "Performing";
						break;
					}
					case PlayerStatus.Conducting: {
						PlayerGroup.Text = "Conducting";
						break;
					}
				}

				Keyboard.Refresh();

				OnStatusChange?.Invoke(this, Status);
			}
			get {
				return bmpStatus;
			}
		}

		public bool Interactable {
			get {
				return PlayTable.Enabled;
			}
			set {
				PlayTable.Enabled = value;
			}
		}

		public BmpKeyboard Keyboard {
			get {
				return KeyboardCtl;
			}
		}

		public BmpSequencer Player {
			get {
				return player;
			}
		}

		private bool loop;
		public bool Loop {
			get { return loop; }
			set {
				loop = value;

				TrackLoop.Invoke(t => t.Checked = loop);
			}
		}

		private int tempo;
		public int Tempo {
			get { return tempo; }
			set {
				tempo = value;
				InfoTempo.Invoke(t => t.Text = string.Format("{0} BPM", tempo));
			}
		}

		private string trackname;
		public string TrackName {
			get { return trackname; }
			set {
				trackname = value;
				InfoTrackName.Invoke(t => t.Text = trackname);
			}
		}

		private int octaveShift;
		public int OctaveShift {
			get { return octaveShift; }
			set {
				octaveShift = value.Clamp(-4, 4);

				SelectorOctave.Invoke(t => t.Value = (decimal) (octaveShift));
				UpdateKeyboard(player.LoadedTrack);
			}
		}


		private float speedShift = 1.0f;
		public float SpeedShift {
			get { return speedShift; }
			set {
				speedShift = value.Clamp(0.1f, 2.0f);

				SelectorSpeed.Invoke(t => t.Value = (decimal) (speedShift * 100f));
				Player.Speed = speedShift;
			}
		}
		public Instrument PreferredInstrument {
			get {
				if(player.LoadedTrack == null) {
					return 0;
				}
				return player.GetTrackPreferredInstrument(player.LoadedTrack);
			}
		}

		public int TotalNoteCount {
			get {
				int sum = 0;
				foreach(int s in player.notesPlayedCount.Values) {
					sum += s;
				}
				return sum;
			}
		}
		public int CurrentNoteCount {
			get {
				if(player.LoadedTrack == null) {
					return 0;
				}
				return player.notesPlayedCount[player.LoadedTrack];
			}
		}

		private bool trackHoldPlaying;
		private Dictionary<Track, int> trackNumLut = new Dictionary<Track, int>();

		public BmpPlayer() {
			InitializeComponent();


			player.OnLoad += OnPlayerMidiLoad;
			player.OnTick += OnMidiTick;

			player.OnTempoChange += OnMidiTempoChange;
			player.OnTrackNameChange += OnMidiTrackNameChange;
			player.OnLyric += delegate (object o, string s) {
				OnMidiLyric?.Invoke(o, s);
			};

			player.OnNote += OnPlayerMidiNote;
			player.OffNote += OffPlayerMidiNote;

			player.PlayEnded += OnPlayEnded;
			player.PlayStatusChange += OnMidiPlayStatusChange;

			SelectorOctave.MouseWheel += Disable_Scroll;
			SelectorSpeed.MouseWheel += Disable_Scroll;
			TrackProgress.MouseWheel += Disable_Scroll;
			TrackSkip.Click += TrackSkip_Click;

			Keyboard.MouseClick += Keyboard_Click;
		}

		private void TrackSkip_Click(object sender, EventArgs e) {
			OnSongSkip?.Invoke(sender, e);
		}

		private void Keyboard_Click(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Middle) {
				bool shiftKey = (Control.ModifierKeys & Keys.Shift) != 0;
				if(shiftKey) {
					if(Status == PlayerStatus.Conducting) {
						Status = PlayerStatus.Performing;
					} else if(Status == PlayerStatus.Performing) {
						Status = PlayerStatus.Conducting;
					}
				}
			}
		}

		private int ApplyOctaveShift(int note) {
			int os = octaveShift + player.GetTrackPreferredOctaveShift(player.LoadedTrack);
			return NoteHelper.ApplyOctaveShift(note, os);
		}

		// Events
		private void OnPlayerMidiLoad(Object o, EventArgs e) {
			OnMidiTrackLoad?.Invoke(o, player.LoadedTrack);

			UpdateKeyboard(player.LoadedTrack);

			TotalProgressInfo.Invoke(t => t.Text = player.MaxTime);

			string lyric = (player.LyricNum > 0) ? string.Format("{0} lyric(s)", player.LyricNum) : string.Empty;
			InfoHasLyrics.Invoke(t => t.Text = lyric);

			trackNumLut.Clear();
			for(int i = 0; i < player.Sequence.Count; i++) {
				trackNumLut[player.Sequence[i]] = i;
			}

			UpdatePlayer();
		}

		private int GetTrackLutNum(Track track) {
			if(track != null) {
				if(trackNumLut.ContainsKey(track)) {
					return trackNumLut[track];
				}
			}
			return 0;
		}

		private void OnPlayerMidiNote(Object o, ChannelMessageEventArgs e) {
			OnMidiNote?.Invoke(o, new NoteEvent {
				track = e.MidiTrack,
				trackNum = GetTrackLutNum(e.MidiTrack),
				note = ApplyOctaveShift(e.Message.Data1),
				origNote = e.Message.Data1,
			});
		}
		private void OffPlayerMidiNote(Object o, ChannelMessageEventArgs e) {
			OffMidiNote?.Invoke(o, new NoteEvent {
				track = e.MidiTrack,
				trackNum = GetTrackLutNum(e.MidiTrack),
				note = ApplyOctaveShift(e.Message.Data1),
				origNote = e.Message.Data1,
			});
		}

		private void OnMidiTempoChange(Object o, int tempo) {
			Tempo = tempo;
		}
		private void OnMidiTrackNameChange(Object o, string name) {
			TrackName = name;
		}


		private void OnPlayEnded(Object o, EventArgs e) {
			if(Loop)
				Player.Play();
			else
				Player.Stop();
			OnMidiStatusEnded?.Invoke(this, EventArgs.Empty);
		}

		private void OnMidiPlayStatusChange(Object o, EventArgs e) {
			UpdatePlayer();
			OnMidiStatusChange?.Invoke(o, player.IsPlaying);
		}

		private void OnMidiTick(Object o, int position) {
			if(position < TrackProgress.Maximum) {
				TrackProgress.Invoke(t => t.Value = position);

				CurrentProgressInfo.Invoke(t => t.Text = player.CurrentTime);
			}
		}

		// Generic funcs

		public void LoadFile(string filename, int track) {
			if(!string.IsNullOrEmpty(filename)) {
				player.Load(filename, track);
			}
		}

		public void UpdatePlayer() {
			TrackProgress.Invoke(t => t.Maximum = player.MaxTick);
			TrackProgress.Invoke(t => t.Value = 0);
			if(player.CurrentTick < player.MaxTick) {
				TrackProgress.Invoke(t => t.Value = player.CurrentTick);
			}

			Bitmap playPauseBmp = (player.IsPlaying ? Properties.Resources.Pause : Properties.Resources.Play);
			TrackPlay.Invoke(t => t.Image = playPauseBmp);
		}

		public void UpdateKeyboard(Track track) {
			List<int> notes = new List<int>();

			if(!(track is Track)) {
				return;
			}
			foreach(MidiEvent ev in track.Iterator()) {
				if(ev.MidiMessage.MessageType == MessageType.Channel) {
					ChannelMessage msg = (ev.MidiMessage as ChannelMessage);
					if(msg.Command == ChannelCommand.NoteOn) {
						int note = msg.Data1;
						int vel = msg.Data2;
						if(vel > 0) {
							notes.Add(ApplyOctaveShift(note));
						}
					}
				}
			}

			Keyboard.Invoke(t => t.UpdateFrequency(notes));
		}

		// UI Events

		private void Disable_Scroll(object sender, EventArgs e) {
			if(!(sender as Control).Enabled) {
				((HandledMouseEventArgs) e).Handled = true;
			}
		}

		private void TrackProgress_MouseDown(object sender, MouseEventArgs e) {
			if(!player.Loaded) {
				return;
			}
			if(e.Button == MouseButtons.Left) {
				trackHoldPlaying = player.IsPlaying;
				player.Pause();
				TrackProgress_MouseMove(sender, e);
			}
		}

		private void TrackProgress_MouseMove(object sender, MouseEventArgs e) {
			if(!player.Loaded) {
				return;
			}
			if(e.Button == MouseButtons.Left && !player.IsPlaying) {
				float v = ((float) (e.X - 6) / (float) (TrackProgress.Width - 12));
				if(v >= 0f && v <= 1f) {
					v = v * (TrackProgress.Maximum - TrackProgress.Minimum);
					player.Position = ((int) v);
					OnMidiProgressChange?.Invoke(this, (int) v);
					UpdatePlayer();
				}
			}
		}

		private void TrackProgress_MouseUp(object sender, MouseEventArgs e) {
			if(!player.Loaded) {
				return;
			}
			if(e.Button == MouseButtons.Left && !player.IsPlaying) {
				if(trackHoldPlaying) {
					player.Play();
				}
				UpdatePlayer();
				if(player.CurrentTick > player.MaxTick) {
					return;
				}
				TrackProgress.Invoke(t => t.Value = (int) player.CurrentTick);
			}
		}

		private void TrackPlay_Click(object sender, EventArgs e) {
			if(player.IsPlaying) {
				player.Pause();
			} else {
				player.Play();
			}
		}

		private void SelectorSpeed_ValueChanged(object sender, EventArgs e) {
			decimal speed = (sender as NumericUpDown).Value;
			float ss = decimal.ToSingle(speed) / 100f;
			SpeedShift = ss;
		}

		private void SelectorOctave_ValueChanged(object sender, EventArgs e) {
			decimal octave = (sender as NumericUpDown).Value;
			int os = decimal.ToInt32(octave);
			OctaveShift = os;
		}

		private void TrackLoop_CheckedChanged(object sender, EventArgs e) {
			bool l = (sender as CheckBox).Checked;
			Loop = l;
		}

		private void PlayerControl_MouseClick(object sender, MouseEventArgs e) {
			if(Control.ModifierKeys == Keys.Shift && e.Button == MouseButtons.Left) {
				if(Status == PlayerStatus.Conducting) {
					Status = PlayerStatus.Performing;
				} else if(Status == PlayerStatus.Performing) {
					Status = PlayerStatus.Conducting;
				}
			}
		}
	}

	public static class NoteHelper {
		public static int ApplyOctaveShift(int note, int octave) {
			return (note - (12 * 4)) + (12 * octave);
		}
	}

}
