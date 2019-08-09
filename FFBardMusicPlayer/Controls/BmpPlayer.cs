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
		public EventHandler OnMidiStatusEnded;

		public class NoteEvent {
			public Track track;
			public int note;
		};
		public EventHandler<NoteEvent> OnMidiNote;
		public EventHandler<NoteEvent> OffMidiNote;

		public EventHandler OnSongSkip;

		public EventHandler<PlayerStatus> OnStatusChange;
		private PlayerStatus bmpStatus;
		public enum PlayerStatus {
			PerformerSolo,
			PerformerMulti,
			Conducting,
		}

		public PlayerStatus Status {
			set {
				bmpStatus = value;

				bool soloInteract = !(bmpStatus == PlayerStatus.PerformerMulti);
				bool solo = (bmpStatus == PlayerStatus.PerformerSolo);

				//SelectorSong.Enabled = soloInteract;
				TrackTable.Enabled = soloInteract;
				Keyboard.Enabled = soloInteract;

				TrackPlay.Enabled = soloInteract;
				TrackLoop.Enabled = soloInteract;
				TrackSkip.Enabled = soloInteract;

				SelectorSpeed.Visible = solo;
				SelectorOctave.Visible = solo;

				switch(bmpStatus) {
					case PlayerStatus.Conducting: {
						PlayerGroup.Text = "Conducting";
						break;
					}
					case PlayerStatus.PerformerMulti: {
						PlayerGroup.Text = "Performer";
						break;
					}
					case PlayerStatus.PerformerSolo: {
						PlayerGroup.Text = "Performer (Solo)";
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

		private int trackNameOctaveShift;
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
		private Instrument preferredInstrument;
		public Instrument PreferredInstrument {
			get {
				return preferredInstrument;
			}
			set {
				preferredInstrument = value;
			}
		}

		private bool trackHoldPlaying;

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
						Status = PlayerStatus.PerformerMulti;
					} else if(Status == PlayerStatus.PerformerMulti) {
						Status = PlayerStatus.PerformerSolo;
					} else if(Status == PlayerStatus.PerformerSolo) {
						Status = PlayerStatus.Conducting;
					}
				}
			}
		}

		private int ApplyOctaveShift(int note) {
			bool osPermitted = (bmpStatus == PlayerStatus.PerformerSolo || bmpStatus == PlayerStatus.PerformerMulti);
			int os = octaveShift + trackNameOctaveShift;
			return NoteHelper.ApplyOctaveShift(note, osPermitted ? os : 0);
		}

		// Events
		private void OnTrackSelect(Object o, int track) {
			if(track != player.CurrentTrack) {
				player.Reload(track);
			}
		}

		private void OnPlayerMidiLoad(Object o, EventArgs e) {
			OnMidiTrackLoad?.Invoke(o, player.LoadedTrack);

			UpdateKeyboard(player.LoadedTrack);

			TotalProgressInfo.Invoke(t => t.Text = player.MaxTime);

			string lyric = (player.LyricNum > 0) ? string.Format("{0} lyric(s)", player.LyricNum) : string.Empty;
			InfoHasLyrics.Invoke(t => t.Text = lyric);

			UpdatePlayer();
		}

		private void OnPlayerMidiNote(Object o, ChannelMessageEventArgs e) {
			OnMidiNote?.Invoke(o, new NoteEvent {
				track = e.MidiTrack,
				note = ApplyOctaveShift(e.Message.Data1),
			});
		}
		private void OffPlayerMidiNote(Object o, ChannelMessageEventArgs e) {
			OffMidiNote?.Invoke(o, new NoteEvent {
				track = e.MidiTrack,
				note = ApplyOctaveShift(e.Message.Data1),
			});
		}

		private void OnMidiTempoChange(Object o, int tempo) {
			Tempo = tempo;
		}
		private void OnMidiTrackNameChange(Object o, string name) {
			TrackName = name;


			if(string.IsNullOrEmpty(name)) {
				PreferredInstrument = Instrument.Piano;
				trackNameOctaveShift = 0;
			} else {
				Regex rex = new Regex(@"^([A-Za-z]+)([-+]\d)?");
				if(rex.Match(name) is Match match) {
					string instrument = match.Groups[1].Value;
					string octaveshift = match.Groups[2].Value;

					bool foundInstrument = false;

					if(!string.IsNullOrEmpty(instrument)) {
						if(Enum.TryParse<Instrument>(instrument, out Instrument tempInst)) {
							PreferredInstrument = tempInst;
							foundInstrument = true;
						}
					}
					if(foundInstrument) {
						if(!string.IsNullOrEmpty(octaveshift)) {
							if(int.TryParse(octaveshift, out int os)) {
								if(Math.Abs(os) <= 4) {
									trackNameOctaveShift = os;
								}
							}
						}
					}
				}
			}
		}


		private void OnPlayEnded(Object o, EventArgs e) {
			if(Loop) {
				Player.Play();
			}
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

		private void MusicReload_Click(object sender, EventArgs e) {
			player.Stop();
			player.Reload();
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
				if(v > 0f) {
					v = v * (TrackProgress.Maximum - TrackProgress.Minimum);
					player.Position = ((int) v);
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
					Status = PlayerStatus.PerformerMulti;
				} else if(Status == PlayerStatus.PerformerMulti) {
					Status = PlayerStatus.PerformerSolo;
				} else if(Status == PlayerStatus.PerformerSolo) {
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
