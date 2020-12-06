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
using System.Threading;
using Sharlayan.Models.ReadResults;

namespace FFBardMusicPlayer.Controls {
	public partial class BmpLocalPerformer : UserControl {

		private FFXIVKeybindDat hotkeys = new FFXIVKeybindDat();
		private FFXIVHotbarDat hotbar = new FFXIVHotbarDat();
		private FFXIVHook hook = new FFXIVHook();

		private Instrument chosenInstrument = Instrument.Piano;
		public Instrument ChosenInstrument {
			set {
				chosenInstrument = value;
				InstrumentName.Text = string.Format("[{0}]", value.ToString());
			}
		}

		private BmpSequencer sequencer;
        public BmpSequencer Sequencer
        {
            set
            {
                if (value != null)
                {
                    Console.WriteLine(string.Format("Performer [{0}] MIDI: [{1}]", this.PerformerName, value.LoadedFilename));
                    if (!string.IsNullOrEmpty(value.LoadedFilename))
                    {
                        sequencer = new BmpSequencer(value.LoadedFilename, this.TrackNum);
                        sequencer.OnNote += InternalNote;
                        sequencer.OffNote += InternalNote;

                        // set the initial octave shift here, if we have a track to play
                        if (this.TrackNum < sequencer.Sequence.Count)
                            OctaveShift.Value = sequencer.GetTrackPreferredOctaveShift(sequencer.Sequence[this.TrackNum]);
                    }
                    this.Update(value);
                }
            }
        }

        public EventHandler onUpdate;
		private bool openDelay;
		public bool hostProcess = false;

		public int TrackNum {
			get {
				return decimal.ToInt32(TrackShift.Value);
			}
			set {
				TrackShift.Invoke(t => t.Value = value);
			}
		}

        // this value initially holds the track octave shift (Instrument+#)
        // but changes when the user manually edits the value using the UI
		public int OctaveNum {
			get {
				return decimal.ToInt32(OctaveShift.Value);
			}
		}

		public string PerformerName {
			get {
				return CharacterName.Text;
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

		public uint performanceId = 0;
		public uint actorId = 0;

		private bool performanceUp = false;
		public bool PerformanceUp {
			get { return performanceUp; }
			set {
				performanceUp = value;

				UiEnabled = performanceUp;
			}
		}
		
		public bool UiEnabled {
			set {
				this.InstrumentName.Enabled = value;
				this.CharacterName.Enabled = value;
				this.Keyboard.Enabled = value;
				if(!hostProcess) {
					this.CharacterName.BackColor = value ? Color.Transparent : Color.FromArgb(235, 120, 120);
				}
				if(!value) {
					hook.ClearLastPerformanceKeybinds();
				}
			}
		}


		public BmpLocalPerformer() {
			InitializeComponent();
		}

		public BmpLocalPerformer(MultiboxProcess mp) {
			InitializeComponent();

			this.ChosenInstrument = this.chosenInstrument;

			if(mp != null) {
				hook.Hook(mp.process, false);
				hotkeys.LoadKeybindDat(mp.characterId);
				hotbar.LoadHotbarDat(mp.characterId);
				CharacterName.Text = mp.characterName;
			}

			Scroller.OnScroll += delegate (object o, int scroll) {
				this.sequencer.Seek(scroll);
			};
			Scroller.OnStatusClick += delegate (object o, EventArgs a) {
				Scroller.Text = this.sequencer.Position.ToString();
			};
		}

		private void InternalNote(Object o, ChannelMessageEventArgs args) {
			ChannelMessageBuilder builder = new ChannelMessageBuilder(args.Message);

			NoteEvent noteEvent = new NoteEvent {
				note = builder.Data1,
				origNote = builder.Data1,
				trackNum = sequencer.GetTrackNum(args.MidiTrack),
				track = args.MidiTrack,
			};

			if(sequencer.GetTrackNum(noteEvent.track) == this.TrackNum) {
				noteEvent.note = NoteHelper.ApplyOctaveShift(noteEvent.note, this.OctaveNum);

				ChannelCommand cmd = args.Message.Command;
				int vel = builder.Data2;
				if((cmd == ChannelCommand.NoteOff) || (cmd == ChannelCommand.NoteOn && vel == 0)) {
					this.ProcessOffNote(noteEvent);
				}
				if((cmd == ChannelCommand.NoteOn) && vel > 0) {
					this.ProcessOnNote(noteEvent);
				}
			}
		}

		public void ProcessOnNote(NoteEvent note) {
			if(!this.PerformanceUp || !this.PerformerEnabled) {
				return;
			}

			if(openDelay) {
				return;
			}

			if (hotkeys.GetKeybindFromNoteByte(note.note) is FFXIVKeybindDat.Keybind keybind)
			{
				if (WantsHold)
				{
					hook.SendKeybindDown(keybind);
				}
				else
				{
					hook.SendAsyncKeybind(keybind);
				}
			}
		}

		public void ProcessOffNote(NoteEvent note) {
			if(!this.PerformanceUp || !this.PerformerEnabled) {
				return;
			}

			if(hotkeys.GetKeybindFromNoteByte(note.note) is FFXIVKeybindDat.Keybind keybind) {
				if(WantsHold) {
					hook.SendKeybindUp(keybind);
				}
			}
		}
		public void SetProgress(int progress) {
			if(sequencer is BmpSequencer) {
				sequencer.Position = progress;
				Scroller.Text = sequencer.Position.ToString();
			}
		}

		public void Play(bool play) {
			if(sequencer is BmpSequencer) {
				Scroller.Text = sequencer.Position.ToString();
				if(play) {
					sequencer.Play();
				} else {
					sequencer.Pause();
				}
			}
		}

		public void Stop() {
			if(sequencer is BmpSequencer) {
				sequencer.Stop();
			}
		}

		public void Update(BmpSequencer bmpSeq) {
			int tn = this.TrackNum;

			if(!(bmpSeq is BmpSequencer)) {
				return;
			}

			Sequence seq = bmpSeq.Sequence;
			if(!(seq is Sequence)) {
				return;
			}

			Keyboard.UpdateFrequency(new List<int>());
			if((tn >= 0 && tn < seq.Count) && seq[tn] is Track track) {
                // OctaveNum now holds the track octave and the selected octave together
				Console.WriteLine(String.Format("Track #{0}/{1} setOctave: {2} prefOctave: {3}", tn, bmpSeq.MaxTrack, OctaveNum, bmpSeq.GetTrackPreferredOctaveShift(track)));
				List<int> notes = new List<int>();
				foreach(MidiEvent ev in track.Iterator()) {
					if(ev.MidiMessage.MessageType == MessageType.Channel) {
						ChannelMessage msg = (ev.MidiMessage as ChannelMessage);
						if(msg.Command == ChannelCommand.NoteOn) {
							int note = msg.Data1;
							int vel = msg.Data2;
							if(vel > 0) {
								notes.Add(NoteHelper.ApplyOctaveShift(note, this.OctaveNum));
							}
						}
					}
				}
                Keyboard.UpdateFrequency(notes);
				ChosenInstrument = bmpSeq.GetTrackPreferredInstrument(track);
			}

			if(hostProcess) {
				this.BackColor = Color.FromArgb(235, 235, 120);
			} else {
				this.BackColor = Color.Transparent;
			}
		}

		public void OpenInstrument() {
			// Exert the effort to check memory i guess
			if(hostProcess) {
				if(Sharlayan.MemoryHandler.Instance.IsAttached) {
					if(Sharlayan.Reader.CanGetPerformance()) {
						if(Sharlayan.Reader.GetPerformance().IsUp()) {
							return;
						}
					}
				}
			}

			if(performanceUp) {
				return;
			}

            // don't open instrument if we don't have anything loaded
            if (sequencer == null || sequencer.Sequence == null)
                return;

            // don't open instrument if we're not on a valid track
            if (TrackNum == 0 || TrackNum >= sequencer.Sequence.Count)
                return;

			string keyMap = hotbar.GetInstrumentKeyMap(chosenInstrument);
			if(!string.IsNullOrEmpty(keyMap)) {
				FFXIVKeybindDat.Keybind keybind = hotkeys[keyMap];
				if(keybind is FFXIVKeybindDat.Keybind && keybind.GetKey() != Keys.None) {
					hook.SendTimedSyncKeybind(keybind);
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

					performanceUp = true;
				}
			}
		}

		public void CloseInstrument() {
			if(hostProcess) {
				if(Sharlayan.MemoryHandler.Instance.IsAttached) {
					if(Sharlayan.Reader.CanGetPerformance()) {
						if(!Sharlayan.Reader.GetPerformance().IsUp()) {
							return;
						}
					}
				}
			}

			if(!performanceUp) {
				return;
			}

			hook.ClearLastPerformanceKeybinds();

			FFXIVKeybindDat.Keybind keybind = hotkeys["ESC"];
			Console.WriteLine(keybind.ToString());
			if(keybind is FFXIVKeybindDat.Keybind && keybind.GetKey() != Keys.None) {
				hook.SendSyncKeybind(keybind);
				performanceUp = false;
			}
		}

		public void ToggleMute() {
			if(hostProcess) {
				return;
			}
			if(hook.Process != null) {
				BmpAudioSessions.ToggleProcessMute(hook.Process);
			}
		}

		public void EnsembleCheck() {
			// 0x24CB8DCA // Extended piano
			// 0x4536849B // Metronome
			// 0xB5D3F991 // Ready check begin
			hook.FocusWindow();
			/* // Dummied out, dunno if i should click it for the user
			Thread.Sleep(100);
			if(hook.SendUiMouseClick(addon, 0x4536849B, 130, 140)) {
				//this.EnsembleAccept();
			}
			*/
		}

		public void EnsembleAccept() {
			FFXIVKeybindDat.Keybind keybind = hotkeys["OK"];
			if(keybind is FFXIVKeybindDat.Keybind && keybind.GetKey() != Keys.None) {
				hook.SendSyncKeybind(keybind);
				hook.SendSyncKeybind(keybind);
			}
		}
		public void NoteKey(string noteKey) {
			if(hotkeys.GetKeybindFromNoteKey(noteKey) is FFXIVKeybindDat.Keybind keybind) {
				hook.SendAsyncKeybind(keybind);
			}
		}

		private void TrackShift_ValueChanged(object sender, EventArgs e) {
            if (sequencer != null)
            {
                // here, since we've changed tracks, we need to reset the OctaveShift
                // value back to the track octave (or zero, if one is not set)
                var seq = sequencer.Sequence;
                int newTn = decimal.ToInt32((sender as NumericUpDown).Value);
                int newOs = ((newTn >= 0 && newTn < seq.Count) ? sequencer.GetTrackPreferredOctaveShift(seq[newTn]) : 0);
                this.OctaveShift.Value = newOs;

                this.Invoke(t => t.Update(sequencer));
            }
		}

		private void OctaveShift_ValueChanged(object sender, EventArgs e) {
            if (sequencer != null)
            {
                var seq = sequencer.Sequence;
                int os  = decimal.ToInt32((sender as NumericUpDown).Value);
                this.OctaveShift.Value = os;

                this.Invoke(t => t.Update(sequencer));
            }
		}
	}
}
