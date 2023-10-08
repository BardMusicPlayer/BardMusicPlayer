/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BardMusicPlayer.Functions;
using BardMusicPlayer.Maestro.Old;
using BardMusicPlayer.Quotidian;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Siren;
using BardMusicPlayer.Transmogrify.Song;
using BardMusicPlayer.Transmogrify.Song.Importers;
using BardMusicPlayer.Transmogrify.Song.Manipulation;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Microsoft.Win32;

//using Newtonsoft.Json;

namespace BardMusicPlayer.Controls;

public class MidiBardConverterInstrumentHelper
{
    public static Dictionary<int, string> Instruments()
    {
        return Instrument.All.ToDictionary(instr => instr.Index, instr => instr.Name);
    }
}

/// <summary>
/// Interaction logic for MidiBardConverterWindow.xaml
/// </summary>
public partial class MidiBardConverterWindow
{
    private readonly List<MidiBardImporter.MidiTrack?> _tracks;

    private string _midiName { get; set; } = "Unknown";
    private MidiFile? Midifile { get; set; }

    private bool AlignMidiToFirstNote { get; set; }

    private object? Sender { get; set; }

    public MidiBardConverterWindow()
    {
        _tracks = new List<MidiBardImporter.MidiTrack?>();
        InitializeComponent();
        AlignToFirstNoteCheckBox.IsChecked = AlignMidiToFirstNote;
    }

    public MidiBardConverterWindow(string filename)
    {
        _tracks = new List<MidiBardImporter.MidiTrack?>();
        InitializeComponent();
        ReadMidi(filename);
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        if (_tracks.Any())
            _tracks.Clear();
    }

    private void ReadMidi(string filename)
    {
        _midiName = Path.GetFileNameWithoutExtension(filename);
        ReadWithoutConfig(filename);
        // if (File.Exists(Path.ChangeExtension(filename, "json")))
        //     ReadWithConfig(filename);
        // else
        //     ReadWithoutConfig(filename);
    }

    /*
    /// <summary>
    /// Called when there is a config
    /// </summary>
    /// <param name="filename"></param>
    private void ReadWithConfig(string filename)
    {
        MemoryStream memoryStream = new MemoryStream();
        FileStream fileStream = File.Open(Path.ChangeExtension(filename, "json"), FileMode.Open);
        fileStream.CopyTo(memoryStream);
        fileStream.Close();

        var data = memoryStream.ToArray();
        MidiBardImporter.MidiFileConfig? pdatalist = JsonConvert.DeserializeObject<MidiBardImporter.MidiFileConfig>(new UTF8Encoding(true).GetString(data));
        GuitarModeSelector.SelectedIndex = pdatalist.ToneMode;

        //Read the midi
        Midifile = MidiFile.Read(filename);

        //create the dict for the cids to tracks
        Dictionary<int, int> cids = new Dictionary<int, int>();
        int idx = 0;
        int cid_count = 1;
        foreach (TrackChunk chunk in Midifile.GetTrackChunks())
        {
            if (chunk.GetNotes().Count < 1)
                continue;

            int cid = (int)pdatalist.Tracks[idx].AssignedCids[0];
            if (cids.ContainsKey(cid))
                cid = cids[cid];
            else
            {
                cids.Add(cid, cid_count);
                cid = cid_count;
                cid_count++;
            }

            MidiBardImporter.MidiTrack? midiTrack = new MidiBardImporter.MidiTrack();
            midiTrack.Index           = pdatalist.Tracks[idx].Index;
            midiTrack.TrackNumber     = cid;
            midiTrack.trackInstrument = pdatalist.Tracks[idx].Instrument - 1;
            midiTrack.Transpose       = pdatalist.Tracks[idx].Transpose / 12;
            midiTrack.ToneMode        = pdatalist.ToneMode;
            midiTrack.trackChunk      = chunk;

            _tracks.Add(midiTrack);
            idx++;
        }
        TrackList.ItemsSource = _tracks;
        TrackList.Items.Refresh();
    }
    */

    /// <summary>
    /// Called when there is no config
    /// </summary>
    /// <param name="filename"></param>
    private void ReadWithoutConfig(string filename)
    {
        Midifile = MidiFile.Read(filename);
        ReadMidiData();
    }

    public void MidiFromSong(BmpSong? song)
    {
        if (song == null)
            return;
        _tracks.Clear();
        _midiName = song.Title;
        Midifile  = song.GetMelanchallMidiFile();
        ReadMidiData();
    }

    private void ReadMidiData()
    {
        //GuitarModeSelector.SelectedIndex = 3;

        var idx = 0;
        foreach (var chunk in Midifile.GetTrackChunks())
        {
            if (chunk.GetNotes().Count < 1)
                continue;

            var trackName = TrackManipulations.GetTrackName(chunk);
            var octaveShift = 0;
            var progNum = -1;

            var rex = new Regex(@"^([A-Za-z _:]+)([-+]\d)?");
            if (rex.Match(trackName) is { } match)
                if (!string.IsNullOrEmpty(match.Groups[1].Value))
                {
                    progNum = Instrument.Parse(match.Groups[1].Value).MidiProgramChangeCode;
                    if (!string.IsNullOrEmpty(match.Groups[2].Value))
                        if (int.TryParse(match.Groups[2].Value, out var os))
                            octaveShift = os;
                }

            var midiTrack = new MidiBardImporter.MidiTrack
            {
                Index           = idx + 1,
                TrackNumber     = idx + 1,
                trackInstrument = Instrument.ParseByProgramChange(progNum).Index-1,
                Transpose       = octaveShift,
                ToneMode        = 3,
                trackChunk      = chunk
            };

            _tracks.Add(midiTrack);
            idx++;
        }

        TrackList.ItemsSource = _tracks;
        TrackList.Items.Refresh();
    }

    #region Octave Up/Down

    private void OctaveControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is OctaveNumericUpDown ctl) ctl.OnValueChanged += OnOctaveValueChanged;
        _bnb = false;
    }

    private static void OnOctaveValueChanged(object sender, int s)
    {
        if ((sender as OctaveNumericUpDown)?.DataContext is MidiBardImporter.MidiTrack track) track.Transpose =  s;
        if (sender is OctaveNumericUpDown ctl) ctl.OnValueChanged                                             -= OnOctaveValueChanged;
    }
    #endregion

    #region Drag&Drop
    private bool _bnb;
    /// <summary>
    /// Drag & Drop Start
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TrackListItem_MouseMove(object sender, MouseEventArgs e)
    {
        if (_bnb)
        {
            e.Handled = true;
            return;
        }
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            if (sender is ListViewItem celltext && !_bnb)
            {
                DragDrop.DoDragDrop(TrackList, celltext, DragDropEffects.Move);
                e.Handled = true;
            }
            _bnb = false;
        }
    }

    /// <summary>
    /// Called when there is a drop
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TrackListItem_Drop(object sender, DragEventArgs e)
    {
        var draggedObject = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
        var targetObject = (ListViewItem)sender;

        var drag = draggedObject?.Content as MidiBardImporter.MidiTrack;
        var drop = targetObject.Content as MidiBardImporter.MidiTrack;

        if (drag == drop)
            return;

        var newTracks = new SortedDictionary<int, MidiBardImporter.MidiTrack?>();
        var index = 0;
        foreach (var p in _tracks.Where(p => p != drag))
        {
            if (p == drop)
            {
                if (drag != null && drop != null && drop.Index < drag.Index)
                {
                    newTracks.Add(index, drag); index++;
                    newTracks.Add(index, drop); index++;
                }
                else if (drag != null && drop != null && drop.Index > drag.Index)
                {
                    newTracks.Add(index, drop); index++;
                    newTracks.Add(index, drag); index++;
                }
            }
            else
            {
                newTracks.Add(index, p);
                index++;
            }
        }
            
        index = 0;
        foreach (var p in newTracks)
        {
            if (p.Value != null) p.Value.Index = index;
            index++;
        }

        _tracks.Clear();
        foreach (var oT in newTracks)
            _tracks.Add(oT.Value);

        TrackList.ItemsSource = _tracks;
        TrackList.Items.Refresh();
        newTracks.Clear();
    }

    /// <summary>
    /// Helper for Drag & Drop
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BardNumBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        _bnb = true;
    }

    private void Instrument_Selector_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        _bnb = false;
    }

    private void Instrument_Selector_DropDownClosed(object sender, EventArgs e)
    {
        _bnb = false;
    }
    #endregion

    #region Sidemenu

    private void Sequencer_Click(object sender, RoutedEventArgs e)
    {
        if (Midifile == null)
            return;
        if (!_tracks.Any())
            return;

        var myStream = new MemoryStream();
        if (AlignMidiToFirstNote)
        {
            RealignMidiFile(MidiBardImporter.Convert(Midifile, CloneTracks())).Write(myStream, settings: new WritingSettings
                { TextEncoding = Encoding.UTF8 });
        }
        else
        {
            MidiBardImporter.Convert(Midifile, CloneTracks()).Write(myStream, settings: new WritingSettings
                { TextEncoding = Encoding.UTF8 });
        }
        myStream.Rewind();
        var song = BmpSong.ImportMidiFromByte(myStream.ToArray(), _midiName);
        BmpMaestro.Instance.SetSong(song.Result);
        PlaybackFunctions.LoadSongFromPlaylist(song.Result);
        myStream.Close();
        myStream.Dispose();
    }

    /// <summary>
    /// Send song to Siren
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Siren_Click(object sender, RoutedEventArgs e)
    {
        if (Midifile == null)
            return;
        if (!_tracks.Any())
            return;

        var myStream = new MemoryStream();
        if (AlignMidiToFirstNote)
        {
            RealignMidiFile(MidiBardImporter.Convert(Midifile, CloneTracks())).Write(myStream, settings: new WritingSettings
                { TextEncoding = Encoding.UTF8 });
        }
        else
        {
            MidiBardImporter.Convert(Midifile, CloneTracks()).Write(myStream, settings: new WritingSettings
                { TextEncoding = Encoding.UTF8 });
        }
        myStream.Rewind();

        var song = BmpSong.ImportMidiFromByte(myStream.ToArray(), _midiName);
        _ = BmpSiren.Instance.Load(song.Result);
        myStream.Close();
        myStream.Dispose();
    }

    /// <summary>
    /// MidiExport
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Export_Click(object sender, RoutedEventArgs e)
    {
        if (Midifile == null)
            return;
        if (!_tracks.Any())
            return;

        var saveFileDialog = new SaveFileDialog
        {
            Filter           = "MIDI file (*.mid)|*.mid",
            FilterIndex      = 2,
            RestoreDirectory = true,
            OverwritePrompt  = true
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            Stream myStream;
            if ((myStream = saveFileDialog.OpenFile()) != null)
            {
                if (AlignMidiToFirstNote)
                {
                    RealignMidiFile(MidiBardImporter.Convert(Midifile, CloneTracks())).Write(myStream, settings: new WritingSettings
                        { TextEncoding = Encoding.UTF8 });
                }
                else
                {
                    MidiBardImporter.Convert(Midifile, CloneTracks()).Write(myStream, settings: new WritingSettings
                        { TextEncoding = Encoding.UTF8 });
                }
                myStream.Close();
                myStream.Dispose();
            }
        }
    }

    /// <summary>
    /// Set the GuitarMode
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    // private void GuitarModeSelector_Selected(object sender, RoutedEventArgs e)
    // {
    //     var mode = GuitarModeSelector.SelectedIndex;
    //     Parallel.ForEach(_tracks, track =>
    //     {
    //         track.ToneMode = mode;
    //     });
    //     TrackList.Items.Refresh();
    // }

    private void AlignToFirstNote_CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        AlignMidiToFirstNote = (bool)AlignToFirstNoteCheckBox.IsChecked!;
    }


    /// <summary>
    /// Realign the the notes and Events in a <see cref="MidiFile"/> to the beginning
    /// </summary>
    /// <param name="midi"></param>
    /// <returns><see cref="MidiFile"/></returns>
    private static MidiFile RealignMidiFile(MidiFile midi)
    {
        //realign the events
        var x = midi.GetTrackChunks().GetNotes().First().GetTimedNoteOnEvent().Time;
        Parallel.ForEach(midi.GetTrackChunks(), chunk =>
        {
            chunk = RealignTrackEvents(chunk, x).Result;
        });
        return midi;
    }

    /// <summary>
    /// Realigns the track events in <see cref="TrackChunk"/>
    /// </summary>
    /// <param name="originalChunk"></param>
    /// <param name="delta"></param>
    /// <returns><see cref="Task{TResult}"/> is <see cref="TrackChunk"/></returns>
    private static Task<TrackChunk> RealignTrackEvents(TrackChunk originalChunk, long delta)
    {
        using (var manager = originalChunk.ManageTimedEvents())
        {
            foreach (var _event in manager.Objects)
            {
                var newStart = _event.Time - delta;
                _event.Time = newStart <= -1 ? 0 : newStart;
            }
        }
        return Task.FromResult(originalChunk);
    }
    #endregion

    #region Context Menu
    private void TrackListItem_PreviewMouseRightButtonDown(object? sender, MouseButtonEventArgs e)
    {
        Sender    = sender;
        e.Handled = true;
    }

    private void TrackListItem_DrumMap_Click(object sender, RoutedEventArgs e)
    {
        if (Sender is ListViewItem item)
        {
            var t = item.Content as MidiBardImporter.MidiTrack;
            DrumMapping(t?.trackChunk);

            var result = MessageBox.Show("Delete old drum-track?\r\n", "Warning!", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
                return;

            _tracks.Remove(t);
            RenumberTracks();
        }
    }

    private void TrackListItem_Delete_Click(object sender, RoutedEventArgs e)
    {
        if (Sender is ListViewItem item)
        {
            var result = MessageBox.Show("Delete this track?\r\n", "Warning!", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
                return;

            var t = item.Content as MidiBardImporter.MidiTrack;
            _tracks.Remove(t);
            RenumberTracks();
        }
        Sender = null;
    }
    #endregion

    /// <summary>
    /// Renumber tracks
    /// </summary>
    private void RenumberTracks()
    {
        var index = 0;
        foreach (var p in _tracks)
        {
            if (p != null) p.Index = index;
            index++;
        }
        TrackList.Items.Refresh();
    }

    /// <summary>
    /// Clone the tracks
    /// </summary>
    /// <returns></returns>
    private List<MidiBardImporter.MidiTrack> CloneTracks()
    {
        var tracks = new List<MidiBardImporter.MidiTrack>();
        foreach (var a in _tracks)
        {
            var ntrack = new MidiBardImporter.MidiTrack();
            if (a != null)
            {
                ntrack.Index           = a.Index;
                ntrack.TrackNumber     = a.TrackNumber;
                ntrack.trackInstrument = a.trackInstrument;
                ntrack.Transpose       = a.Transpose;
                ntrack.ToneMode        = a.ToneMode;
                ntrack.trackChunk      = (TrackChunk)a.trackChunk.Clone();
            }

            tracks.Add(ntrack);
        }
        return tracks;
    }
    
    /// <summary>
    /// Split drums in <see cref="TrackChunk"/> into new <see cref="TrackChunk"/>
    /// </summary>
    /// <param name="track"></param>
    private void DrumMapping(TrackChunk? track)
    {
        if (track.GetNotes().First().Channel != 9)
        {
            var result = MessageBox.Show("Looks like this isn't a drum-track\r\nContinue the mapping?", "Warning!", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
                return;
        }
        var openFileDialog = new OpenFileDialog
        {
            Filter      = "Drum map | *.json",
            Multiselect = true
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        var drumTracks = TrackManipulations.DrumMapping(track, openFileDialog.FileName);
        if (drumTracks.Count < 1)
            return;
        if (drumTracks.First().Value == null)
        {
            MessageBox.Show(drumTracks.First().Key, "Error!", MessageBoxButton.OK);
            return;
        }

        var lastTrack = _tracks.Last();
        var idx = 1;
        foreach (var nt in drumTracks)
        {
            var ntrack = new MidiBardImporter.MidiTrack();
            if (lastTrack != null)
            {
                ntrack.Index       = lastTrack.Index + idx;
                ntrack.TrackNumber = lastTrack.TrackNumber + idx;
            }

            ntrack.trackInstrument = Instrument.Parse(nt.Key).Index-1;
            ntrack.Transpose       = 0;
            ntrack.ToneMode        = 0;
            ntrack.trackChunk      = nt.Value;
            _tracks.Add(ntrack);
            idx++;
        }
        RenumberTracks();
    }
}