using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FFBardMusicPlayer.BmpProcessSelect;
using static FFBardMusicPlayer.Controls.BmpPlayer;
using static Sharlayan.Core.Enums.Performance;
using Sanford.Multimedia.Midi;

using Timer = System.Timers.Timer;
using System.Timers;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpLocalPerformer : UserControl {

		private FFXIVKeybindDat hotkeys = new FFXIVKeybindDat();
		private FFXIVHotbarDat hotbar = new FFXIVHotbarDat();
		private FFXIVHook hook = new FFXIVHook();

		public EventHandler onUpdate;
		private bool openDelay;

		NoteChordSimulation<BmpPlayer.NoteEvent> chordNotes = new NoteChordSimulation<NoteEvent>();

		public int TrackNum {
			get {
				return decimal.ToInt32(TrackShift.Value);
			}
			set {
				TrackShift.Invoke(t => t.Value = value);
			}
		}

		public int OctaveNum {
			get {
				return decimal.ToInt32(OctaveShift.Value);
			}
		}

		public bool PerformerEnabled {
			get {
				return EnableCheck.Checked;
			}
		}
		private bool WantsHold {
			get {
				return Properties.Settings.Default.HoldNotes;
			}
		}


		public BmpLocalPerformer() {
			InitializeComponent();

			chordNotes.NoteEvent += delegate (object o, NoteEvent e) {
				this.Invoke(t => t.ProcessOnNote(e));
			};
		}

		public void SetMultiboxProcess(MultiboxProcess mp) {
			hook.Hook(mp.process, false);
			hotkeys.LoadKeybindDat(mp.characterId);
			hotbar.LoadHotbarDat(mp.characterId);
			CharacterName.Text = mp.characterName;
		}

		public void ProcessOnNote(NoteEvent onNote) {
			if(openDelay) {
				return;
			}
			if(Properties.Settings.Default.AutoArpeggiate) {
				if(chordNotes.OnKey(onNote)) {
					// Chord detected and queued
					// Console.WriteLine("Delay " + onNote + " by 100ms");
				}
			}

			if(!chordNotes.HasTimer(onNote)) {
				if(hotkeys.GetKeybindFromNoteByte(onNote.note) is FFXIVKeybindDat.Keybind keybind) {
					if(WantsHold) {
						hook.SendKeybindDown(keybind);
					} else {
						hook.SendAsyncKeybind(keybind);
					}
				}
			}
		}

		public void ProcessOffNote(NoteEvent offNote) {
			if(hotkeys.GetKeybindFromNoteByte(offNote.note) is FFXIVKeybindDat.Keybind keybind) {
				if(WantsHold) {
					hook.SendKeybindUp(keybind);
				}
				chordNotes.OffKey(offNote);
			}
		}

		public void Update(BmpSequencer sequencer) {
			int tn = this.TrackNum;

			Sequence seq = sequencer.Sequence;
			if(!(seq is Sequence)) {
				return;
			}

			Keyboard.UpdateFrequency(new List<int>());
			if((tn >= 0 && tn < seq.Count) && seq[tn] is Track track) {
				List<int> notes = new List<int>();
				foreach(MidiEvent ev in track.Iterator()) {
					if(ev.MidiMessage.MessageType == MessageType.Channel) {
						ChannelMessage msg = (ev.MidiMessage as ChannelMessage);
						if(msg.Command == ChannelCommand.NoteOn) {
							int note = msg.Data1;
							int vel = msg.Data2;
							if(vel > 0) {
								int po = sequencer.GetTrackPreferredOctaveShift(track);
								notes.Add(NoteHelper.ApplyOctaveShift(note, this.OctaveNum + po));
							}
						}
					}
				}
				Keyboard.UpdateFrequency(notes);
			}
		}

		public void OpenInstrument(Instrument ins = Instrument.Piano) {

			string keyMap = hotbar.GetInstrumentKeyMap(ins);
			if(!string.IsNullOrEmpty(keyMap)) {
				FFXIVKeybindDat.Keybind keybind = hotkeys[keyMap];
				if(keybind is FFXIVKeybindDat.Keybind && keybind.GetKey() != Keys.None) {
					hook.SendSyncKeybind(keybind);
					openDelay = true;

					Timer openTimer = new Timer {
						Interval = 1000
					};
					openTimer.Elapsed += delegate (object o, ElapsedEventArgs e) {
						openTimer.Stop();
						openTimer = null;

						openDelay = false;
					};
					openTimer.Start();
				}
			}
		}
		public void CloseInstrument() {
			hook.ClearLastPerformanceKeybinds();

			FFXIVKeybindDat.Keybind keybind = hotkeys["ESC"];
			if(keybind is FFXIVKeybindDat.Keybind && keybind.GetKey() != Keys.None) {
				hook.SendSyncKeybind(keybind);
			}
		}

		public void ToggleMute() {
			if(hook.Process != null) {
				BmpAudioSessions.ToggleProcessMute(hook.Process);
			}
		}

		private void TrackShift_ValueChanged(object sender, EventArgs e) {
			onUpdate?.Invoke(this, EventArgs.Empty);
		}

		private void OctaveShift_ValueChanged(object sender, EventArgs e) {
			onUpdate?.Invoke(this, EventArgs.Empty);
		}
	}
}
