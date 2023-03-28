using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Transmogrify.Song;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace BardMusicPlayer.Controls;

public struct NoteRectInfo
{
    public NoteRectInfo(string n, bool blk )
    {
        Name     = n;
        BlackKey = blk;
    }
    public readonly string Name;
    public readonly bool BlackKey;
}

/// <summary>
/// Interaction logic for KeyboardHeatMap.xaml
/// </summary>
public partial class KeyboardHeatMap
{
    //note frequencies
    private const int MOctave = 4; // default octave (octaves can be from 1 to 7)

    private readonly Dictionary<int, NoteRectInfo> _noteInfo = new()
    {
        {  0, new NoteRectInfo("C", false) },
        {  1, new NoteRectInfo("CSharp", true) },
        {  2, new NoteRectInfo("D", false) },
        {  3, new NoteRectInfo("DSharp", true) },
        {  4, new NoteRectInfo("E", false) },
        {  5, new NoteRectInfo("F", false) },
        {  6, new NoteRectInfo("FSharp", true) },
        {  7, new NoteRectInfo("G", false) },
        {  8, new NoteRectInfo("GSharp", true) },
        {  9, new NoteRectInfo("A", false) },
        {  10, new NoteRectInfo("ASharp", true) },
        {  11, new NoteRectInfo("H", false) },

        {  12, new NoteRectInfo("COne", false) },
        {  13, new NoteRectInfo("CSharpOne", true) },
        {  14, new NoteRectInfo("DOne", false) },
        {  15, new NoteRectInfo("DSharpOne", true) },
        {  16, new NoteRectInfo("EOne", false) },
        {  17, new NoteRectInfo("FOne", false) },
        {  18, new NoteRectInfo("FSharpOne", true) },
        {  19, new NoteRectInfo("GOne", false) },
        {  20, new NoteRectInfo("GSharpOne", true) },
        {  21, new NoteRectInfo("AOne", false) },
        {  22, new NoteRectInfo("ASharpOne", true) },
        {  23, new NoteRectInfo("HOne", false) },

        {  24, new NoteRectInfo("CTwo", false) },
        {  25, new NoteRectInfo("CSharpTwo", true) },
        {  26, new NoteRectInfo("DTwo", false) },
        {  27, new NoteRectInfo("DSharpTwo", true) },
        {  28, new NoteRectInfo("ETwo", false) },
        {  29, new NoteRectInfo("FTwo", false) },
        {  30, new NoteRectInfo("FSharpTwo", true) },
        {  31, new NoteRectInfo("GTwo", false) },
        {  32, new NoteRectInfo("GSharpTwo", true) },
        {  33, new NoteRectInfo("ATwo", false) },
        {  34, new NoteRectInfo("ASharpTwo", true) },
        {  35, new NoteRectInfo("HTwo", false) },

        {  36, new NoteRectInfo("CThree", false) }
    };

    public KeyboardHeatMap()
    {
        InitializeComponent();

    }

    /// <summary>
    /// Inits the Ui
    /// </summary>
    public void InitUi()
    {
        InitiateUi();
    }

    public static int GetOctave() { return MOctave; }


    #region UI handling


    private static Dictionary<int, double> GetNoteCountForKey(BmpSong? song, int trackNumber, int octaveShift)
    {
        var midiFile = song?.GetProcessedMidiFile().Result;
        var trackChunks = midiFile.GetTrackChunks().ToList();
        var noteDictionary = new Dictionary<int, int>();
        var noteCount = 0;

        //If your host has an invalid track, set it to 0
        if (trackNumber - 1 <= trackChunks.Count)
            trackNumber = 0;

        if (trackNumber != 0)
        {
            foreach (var note in trackChunks[trackNumber - 1].GetNotes())
            {
                int noteNum = note.NoteNumber;
                noteNum -= 48 - 12*octaveShift;
                var count = 1;
                if (noteDictionary.ContainsKey(noteNum))
                {
                    noteDictionary.TryGetValue(noteNum, out count);
                    count++;
                    noteDictionary.Remove(noteNum);
                }
                if (noteNum >= 0)
                    noteDictionary.Add(noteNum, count);
            }
            noteCount = trackChunks[trackNumber - 1].GetNotes().Count;
        }
        else
        {
            for (var iter = 0; iter != trackChunks.Count; iter++)
            {
                foreach (var note in trackChunks[iter].GetNotes())
                {
                    int noteNum = note.NoteNumber;
                    noteNum -= 48 - 12 * octaveShift;
                    var count = 1;
                    if (noteDictionary.ContainsKey(noteNum))
                    {
                        noteDictionary.TryGetValue(noteNum, out count);
                        count++;
                        noteDictionary.Remove(noteNum);
                    }
                    if (noteNum >= 0)
                        noteDictionary.Add(noteNum, count);
                }
                noteCount += trackChunks[iter].GetNotes().Count;
            }
        }

        var result = new Dictionary<int, double>();
        foreach (var note in noteDictionary)
        {
            var f = note.Value / (double)noteCount * 100;
            result.Add(note.Key, (int)f);
        }
        return result;
    }

    /// <summary>
    /// Init the keyboard rects and the Grid constraints
    /// </summary>
    public void InitiateUi(BmpSong? song = null, int trackNumber = -1, int octaveShift = 0)
    {
        ResetFill();

        if (song != null)
        {
            if (trackNumber-1 >= song.TrackContainers.Count)
                return;

            var noteCountDict = GetNoteCountForKey(song, trackNumber, octaveShift);

            foreach (var n in noteCountDict)
            {
                if (n.Key >= _noteInfo.Count)
                    continue;
                var wantedNode = FindName(_noteInfo[n.Key].Name);
                if (wantedNode is Rectangle r) r.Fill = NoteFill(_noteInfo[n.Key].BlackKey, n.Value);
            }
        }
    }

    private void ResetFill()
    {
        foreach (var n in _noteInfo)
        {
            var wantedNode = FindName(n.Value.Name);
            if (wantedNode is Rectangle r) r.Fill = n.Value.BlackKey ? Brushes.Black : Brushes.White;
        }
    }

    private static LinearGradientBrush NoteFill(bool blk, double count)
    {
        count /= 50;
        if (count < 0.02)
            count = 0.02;
        var brush = new LinearGradientBrush
        {
            StartPoint = blk ? new Point(1, 0) : new Point(1, 1),
            EndPoint   = blk ? new Point(1, 1) :new Point(1, 0)
        };
        
        
        if (BmpPigeonhole.Instance.DarkStyle)
        {
            if (!blk)
            {
                brush.GradientStops.Add(new GradientStop(Colors.Firebrick, 0));
                brush.GradientStops.Add(new GradientStop(Colors.Firebrick, count));
                brush.GradientStops.Add(new GradientStop(Colors.White, count));
            }
            else
            {
                brush.GradientStops.Add(new GradientStop(Colors.Gold, 0));
                brush.GradientStops.Add(new GradientStop(Colors.Gold, count));
                brush.GradientStops.Add(new GradientStop(Colors.Black, count));
            }
        }
        else
        {
            if (!blk)
            {
                brush.GradientStops.Add(new GradientStop(Colors.Red, 0));
                brush.GradientStops.Add(new GradientStop(Colors.Red, count));
                brush.GradientStops.Add(new GradientStop(Colors.White, count));
            }
            else
            {
                brush.GradientStops.Add(new GradientStop(Colors.Yellow, 0));
                brush.GradientStops.Add(new GradientStop(Colors.Yellow, count));
                brush.GradientStops.Add(new GradientStop(Colors.Black, count));
            }
        }

        return brush;
    }
    #endregion
}