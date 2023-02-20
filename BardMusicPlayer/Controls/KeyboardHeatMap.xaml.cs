using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using BardMusicPlayer.DryWetMidi.Core.Utilities;
using BardMusicPlayer.DryWetMidi.Interaction.Notes;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Transmogrify.Song;

namespace BardMusicPlayer.Controls;

public struct NoteRectInfo
{
    public NoteRectInfo(string n, bool blk, int freq )
    {
        name      = n;
        black_key = blk;
        frequency = freq;
    }
    public string name;
    public bool black_key;
    public int frequency;
}

/// <summary>
/// Interaction logic for KeyboardHeatMap.xaml
/// </summary>
public partial class KeyboardHeatMap
{
    //note frequencies
    private int mOctave = 4; // default octave (octaves can be from 1 to 7)

    Dictionary<int, NoteRectInfo> noteInfo = new()
    {
        {  0, new NoteRectInfo("C", false, 60) },
        {  1, new NoteRectInfo("CSharp", true, 61) },
        {  2, new NoteRectInfo("D", false, 62) },
        {  3, new NoteRectInfo("DSharp", true, 63) },
        {  4, new NoteRectInfo("E", false, 64) },
        {  5, new NoteRectInfo("F", false, 65) },
        {  6, new NoteRectInfo("FSharp", true, 66) },
        {  7, new NoteRectInfo("G", false, 67) },
        {  8, new NoteRectInfo("GSharp", true, 68) },
        {  9, new NoteRectInfo("A", false, 69) },
        {  10, new NoteRectInfo("ASharp", true, 70) },
        {  11, new NoteRectInfo("H", false, 71) },

        {  12, new NoteRectInfo("COne", false, 60) },
        {  13, new NoteRectInfo("CSharpOne", true, 61) },
        {  14, new NoteRectInfo("DOne", false, 62) },
        {  15, new NoteRectInfo("DSharpOne", true, 63) },
        {  16, new NoteRectInfo("EOne", false, 64) },
        {  17, new NoteRectInfo("FOne", false, 65) },
        {  18, new NoteRectInfo("FSharpOne", true, 66) },
        {  19, new NoteRectInfo("GOne", false, 67) },
        {  20, new NoteRectInfo("GSharpOne", true, 68) },
        {  21, new NoteRectInfo("AOne", false, 69) },
        {  22, new NoteRectInfo("ASharpOne", true, 70) },
        {  23, new NoteRectInfo("HOne", false, 71) },

        {  24, new NoteRectInfo("CTwo", false, 60) },
        {  25, new NoteRectInfo("CSharpTwo", true, 61) },
        {  26, new NoteRectInfo("DTwo", false, 62) },
        {  27, new NoteRectInfo("DSharpTwo", true, 63) },
        {  28, new NoteRectInfo("ETwo", false, 64) },
        {  29, new NoteRectInfo("FTwo", false, 65) },
        {  30, new NoteRectInfo("FSharpTwo", true, 66) },
        {  31, new NoteRectInfo("GTwo", false, 67) },
        {  32, new NoteRectInfo("GSharpTwo", true, 68) },
        {  33, new NoteRectInfo("ATwo", false, 69) },
        {  34, new NoteRectInfo("ASharpTwo", true, 70) },
        {  35, new NoteRectInfo("HTwo", false, 71) },

        {  36, new NoteRectInfo("CThree", false, 72) }
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
        initUI();
    }

    public int getOctave() { return mOctave; }


    #region UI handling *************************************************************************************


    private Dictionary<int, double> getNoteCountForKey(BmpSong song, int tracknumber, int octaveshift)
    {
        var midiFile = song.GetProcessedMidiFile().Result;
        var trackChunks = midiFile.GetTrackChunks().ToList();
        var notedict = new Dictionary<int, int>();
        var notecount = 0;

        //If your host has an invalid track, set it to 0
        if (tracknumber - 1 <= trackChunks.Count)
            tracknumber = 0;

        if (tracknumber != 0)
        {
            foreach (var note in trackChunks[tracknumber - 1].GetNotes())
            {
                int noteNum = note.NoteNumber;
                noteNum -= 48 - 12*octaveshift;
                var count = 1;
                if (notedict.ContainsKey(noteNum))
                {
                    notedict.TryGetValue(noteNum, out count);
                    count++;
                    notedict.Remove(noteNum);
                }
                if (noteNum >= 0)
                    notedict.Add(noteNum, count);
            }
            notecount = trackChunks[tracknumber - 1].GetNotes().Count;
        }
        else
        {
            for (var iter = 0; iter != trackChunks.Count; iter++)
            {
                foreach (var note in trackChunks[iter].GetNotes())
                {
                    int noteNum = note.NoteNumber;
                    noteNum -= 48 - 12 * octaveshift;
                    var count = 1;
                    if (notedict.ContainsKey(noteNum))
                    {
                        notedict.TryGetValue(noteNum, out count);
                        count++;
                        notedict.Remove(noteNum);
                    }
                    if (noteNum >= 0)
                        notedict.Add(noteNum, count);
                }
                notecount += trackChunks[iter].GetNotes().Count;
            }
        }

        var result = new Dictionary<int, double>();
        foreach (var note in notedict)
        {
            var f = note.Value / (double)notecount * 100;
            result.Add(note.Key, (int)f);
        }
        return result;
    }

    /// <summary>
    /// Init the keyboard rects and the Grid constraints
    /// </summary>
    public void initUI(BmpSong song = null, int tracknumber = -1, int octaveshift = 0)
    {
        ResetFill();

        if (song != null)
        {
            if (tracknumber-1 >= song.TrackContainers.Count)
                return;
              
            
            /** The follow code has a memory leak and needs to be fixed. **/
            var noteCountDict = getNoteCountForKey(song, tracknumber, octaveshift);

            foreach (var n in noteCountDict)
            {
                if (n.Key >= noteInfo.Count)
                    continue;
                var wantedNode = FindName(noteInfo[n.Key].name);
                if (wantedNode is Rectangle r) r.Fill = NoteFill(noteInfo[n.Key].black_key, n.Value);
            }
            /** END - The follow code has a memory leak and needs to be fixed. **/
        }
    }

    private void ResetFill()
    {
        foreach (var n in noteInfo)
        {
            var wantedNode = FindName(n.Value.name);
            if (wantedNode is Rectangle r) r.Fill = n.Value.black_key ? Brushes.Black : Brushes.White;
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

    /// <summary>
    /// Highlight key as it is pressed
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="isBlack"></param>
    public void highlightKey(Rectangle rect, bool isBlack)
    {
    }

    /// <summary>
    /// Remove highlight from key as it is stopped being pressed
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="isBlack"></param>
    public void unHighlightKey(Rectangle rect, bool isBlack)
    {
    }


    /// <summary>
    /// Handle left button clicked event for a rectangle
    /// </summary>
    /// <param name="r"></param>
    private void evtLeftButtonDown(Rectangle r)
    {
        Console.WriteLine(@"left button down");
    }

    /// <summary>
    /// Handle left button up event for a rectangle
    /// </summary>
    /// <param name="r"></param>
    private void evtLeftButtonUp(Rectangle r)
    {
        Console.WriteLine(@"left button up");
    }

    /// <summary>
    /// Handle mouse leave event for a rectangle
    /// </summary>
    /// <param name="r"></param>
    /// <param name="e"></param>
    private void evtMouseLeave(Rectangle r, MouseEventArgs e)
    {
        Console.WriteLine(@"mouse leave");
    }

    /// <summary>
    /// Handle mouse entered event for a rectangle
    /// </summary>
    /// <param name="r"></param>
    /// <param name="e"></param>
    private void evtMouseEnter(Rectangle r, MouseEventArgs e)
    {
        Console.WriteLine(@"mouse enter");
    }

    #endregion ****************************************************************************************

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {

    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
    }
}