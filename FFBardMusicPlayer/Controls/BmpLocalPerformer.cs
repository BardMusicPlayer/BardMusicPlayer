using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static FFBardMusicPlayer.Forms.BmpProcessSelect;
using static FFBardMusicPlayer.Controls.BmpPlayer;
using Sanford.Multimedia.Midi;
using Timer = System.Timers.Timer;
using FFBardMusicCommon;
using FFBardMusicPlayer.FFXIV;

namespace FFBardMusicPlayer.Controls
{
    public partial class BmpLocalPerformer : UserControl
    {
        private readonly FFXIVKeybindDat hotkeys = new FFXIVKeybindDat();
        private readonly FFXIVHotbarDat hotbar = new FFXIVHotbarDat();
        private readonly FFXIVHook hook = new FFXIVHook();
        private Instrument chosenInstrument = Instrument.Piano;

        public Instrument ChosenInstrument
        {
            set
            {
                chosenInstrument    = value;
                InstrumentName.Text = $"[{value.ToString()}]";
            }
        }

        private BmpSequencer sequencer;

        public BmpSequencer Sequencer
        {
            set
            {
                if (value != null)
                {
                    Console.WriteLine($"Performer [{PerformerName}] MIDI: [{value.LoadedFilename}]");
                    if (!string.IsNullOrEmpty(value.LoadedFilename))
                    {
                        sequencer         =  new BmpSequencer(value.LoadedFilename, TrackNum);
                        sequencer.OnNote  += InternalNote;
                        sequencer.OffNote += InternalNote;

                        // set the initial octave shift here, if we have a track to play
                        if (TrackNum < sequencer.Sequence.Count)
                        {
                            OctaveShift.Value = sequencer.GetTrackPreferredOctaveShift(sequencer.Sequence[TrackNum]);
                        }
                    }

                    Update(value);
                }
            }
        }

        public EventHandler OnUpdate;
        private bool openDelay;
        public bool HostProcess = false;

        public int TrackNum
        {
            get => decimal.ToInt32(TrackShift.Value);
            set { TrackShift.Invoke(t => t.Value = value); }
        }

        // this value initially holds the track octave shift (Instrument+#)
        // but changes when the user manually edits the value using the UI
        public int OctaveNum => decimal.ToInt32(OctaveShift.Value);

        public string PerformerName => CharacterName.Text;

        public bool PerformerEnabled => EnableCheck.Checked;

        private bool WantsHold => Properties.Settings.Default.HoldNotes;

        public uint PerformanceId = 0;
        public uint ActorId = 0;
        private bool performanceUp;

        public bool PerformanceUp
        {
            get => performanceUp;
            set
            {
                performanceUp = value;

                UiEnabled = performanceUp;
            }
        }

        public bool UiEnabled
        {
            set
            {
                InstrumentName.Enabled = value;
                CharacterName.Enabled  = value;
                Keyboard.Enabled       = value;
                if (!HostProcess)
                {
                    CharacterName.BackColor = value ? Color.Transparent : Color.FromArgb(235, 120, 120);
                }

                if (!value)
                {
                    hook.ClearLastPerformanceKeybinds();
                }
            }
        }

        public BmpLocalPerformer() { InitializeComponent(); }

        public BmpLocalPerformer(MultiboxProcess mp)
        {
            InitializeComponent();

            ChosenInstrument = chosenInstrument;

            if (mp != null)
            {
                hook.Hook(mp.Process, false);
                hotkeys.LoadKeybindDat(mp.CharacterId);
                hotbar.LoadHotbarDat(mp.CharacterId);
                CharacterName.Text = mp.CharacterName;
            }

            Scroller.OnScroll += delegate(object o, int scroll) { sequencer.Seek(scroll); };
            Scroller.OnStatusClick += delegate
            {
                Scroller.Text = sequencer.Position.ToString();
            };
        }

        private void InternalNote(object o, ChannelMessageEventArgs args)
        {
            var builder = new ChannelMessageBuilder(args.Message);

            var noteEvent = new NoteEvent
            {
                Note     = builder.Data1,
                OrigNote = builder.Data1,
                TrackNum = sequencer.GetTrackNum(args.MidiTrack),
                Track    = args.MidiTrack
            };

            if (sequencer.GetTrackNum(noteEvent.Track) == TrackNum)
            {
                noteEvent.Note = NoteHelper.ApplyOctaveShift(noteEvent.Note, OctaveNum);

                var cmd = args.Message.Command;
                var vel = builder.Data2;
                if (cmd == ChannelCommand.NoteOff || cmd == ChannelCommand.NoteOn && vel == 0)
                {
                    ProcessOffNote(noteEvent);
                }

                if (cmd == ChannelCommand.NoteOn && vel > 0)
                {
                    ProcessOnNote(noteEvent);
                }
            }
        }

        public void ProcessOnNote(NoteEvent note)
        {
            if (!PerformanceUp || !PerformerEnabled)
            {
                return;
            }

            if (openDelay)
            {
                return;
            }

            if (hotkeys.GetKeybindFromNoteByte(note.Note) is FFXIVKeybindDat.Keybind keybind)
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

        public void ProcessOffNote(NoteEvent note)
        {
            if (!PerformanceUp || !PerformerEnabled)
            {
                return;
            }

            if (hotkeys.GetKeybindFromNoteByte(note.Note) is FFXIVKeybindDat.Keybind keybind)
            {
                if (WantsHold)
                {
                    hook.SendKeybindUp(keybind);
                }
            }
        }

        public void SetProgress(int progress)
        {
            if (sequencer != null)
            {
                sequencer.Position = progress;
                Scroller.Text      = sequencer.Position.ToString();
            }
        }

        public void Play(bool play)
        {
            if (sequencer != null)
            {
                Scroller.Text = sequencer.Position.ToString();
                if (play)
                {
                    sequencer.Play();
                }
                else
                {
                    sequencer.Pause();
                }
            }
        }

        public void Stop()
        {
            sequencer?.Stop();
        }

        public void Update(BmpSequencer bmpSeq)
        {
            var tn = TrackNum;

            var seq = bmpSeq?.Sequence;
            if (seq == null)
                return;

            Keyboard.UpdateFrequency(new List<int>());
            if (tn >= 0 && tn < seq.Count && seq[tn] is Track track)
            {
                // OctaveNum now holds the track octave and the selected octave together
                Console.WriteLine(
                    $"Track #{tn}/{bmpSeq.MaxTrack} setOctave: {OctaveNum} prefOctave: {bmpSeq.GetTrackPreferredOctaveShift(track)}");
                var notes = new List<int>();
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
                            notes.Add(NoteHelper.ApplyOctaveShift(note, OctaveNum));
                        }
                    }
                }

                Keyboard.UpdateFrequency(notes);
                ChosenInstrument = bmpSeq.GetTrackPreferredInstrument(track);
            }

            BackColor = HostProcess 
                ? Color.FromArgb(235, 235, 120) 
                : Color.Transparent;
        }

        public void OpenInstrument()
        {
            // Exert the effort to check memory i guess
            if (HostProcess)
            {
                if (Sharlayan.MemoryHandler.Instance.IsAttached
                    && Sharlayan.Reader.CanGetPerformance()
                    && Sharlayan.Reader.GetPerformance().IsUp())
                {
                    return;
                }
            }

            if (performanceUp)
                return;
            
            // don't open instrument if we don't have anything loaded
            if (sequencer?.Sequence == null)
                return;
            
            // don't open instrument if we're not on a valid track
            if (TrackNum == 0 || TrackNum >= sequencer.Sequence.Count)
                return;

            var keyMap = hotbar.GetInstrumentKeyMap(chosenInstrument);
            if (!string.IsNullOrEmpty(keyMap))
            {
                var keybind = hotkeys[keyMap];
                if (keybind != null && keybind.GetKey() != Keys.None)
                {
                    hook.SendTimedSyncKeybind(keybind);
                    openDelay = true;

                    var openTimer = new Timer
                    {
                        Interval = 1000
                    };
                    openTimer.Elapsed += delegate
                    {
                        openTimer.Stop();
                        openTimer = null;

                        openDelay = false;
                    };
                    openTimer.Start();

                    performanceUp = true;
                }
            }
        }

        public void CloseInstrument()
        {
            if (HostProcess)
            {
                if (Sharlayan.MemoryHandler.Instance.IsAttached
                    && Sharlayan.Reader.CanGetPerformance()
                    && !Sharlayan.Reader.GetPerformance().IsUp())
                {
                    return;
                }
            }

            if (!performanceUp)
            {
                return;
            }

            hook.ClearLastPerformanceKeybinds();

            var keybind = hotkeys["ESC"];
            Console.WriteLine(keybind.ToString());
            if (keybind.GetKey() != Keys.None)
            {
                hook.SendSyncKeybind(keybind);
                performanceUp = false;
            }
        }

        public void ToggleMute()
        {
            if (HostProcess)
                return;

            if (hook.Process != null)
            {
                BmpAudioSessions.ToggleProcessMute(hook.Process);
            }
        }

        public void EnsembleCheck()
        {
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

        public void EnsembleAccept()
        {
            var keybind = hotkeys["OK"];
            if (keybind.GetKey() != Keys.None)
            {
                hook.SendSyncKeybind(keybind);
                hook.SendSyncKeybind(keybind);
            }
        }

        public void NoteKey(string noteKey)
        {
            if (hotkeys.GetKeybindFromNoteKey(noteKey) is FFXIVKeybindDat.Keybind keybind)
            {
                hook.SendAsyncKeybind(keybind);
            }
        }

        private void TrackShift_ValueChanged(object sender, EventArgs e)
        {
            if (sequencer != null)
            {
                // here, since we've changed tracks, we need to reset the OctaveShift
                // value back to the track octave (or zero, if one is not set)
                var seq = sequencer.Sequence;
                var newTn = decimal.ToInt32((sender as NumericUpDown).Value);
                var newOs = newTn >= 0 && newTn < seq.Count ? sequencer.GetTrackPreferredOctaveShift(seq[newTn]) : 0;
                OctaveShift.Value = newOs;

                this.Invoke(t => t.Update(sequencer));
            }
        }

        private void OctaveShift_ValueChanged(object sender, EventArgs e)
        {
            if (sequencer != null)
            {
                var seq = sequencer.Sequence;
                var os = decimal.ToInt32(((NumericUpDown) sender).Value);
                OctaveShift.Value = os;

                this.Invoke(t => t.Update(sequencer));
            }
        }
    }
}