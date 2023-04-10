/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayerApi/blob/develop/LICENSE for full license information.
 */

using System;
using System.Text;
using BardMusicPlayer.DryWetMidi.Core;
using BardMusicPlayer.DryWetMidi.Core.WritingSettings;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Transmogrify.Song;

namespace BardMusicPlayer.Siren.ApiTest;

internal class Program
{
    private static string _lastLyric = "";

    private static void Main(string[] args)
    {
        BmpPigeonhole.Initialize(AppContext.BaseDirectory + @"\Siren.ApiTest.json");

        BmpSiren.Instance.Setup(AppContext.BaseDirectory + @"\vst");

#if DEBUG
            var song = BmpSong.OpenFile(AppContext.BaseDirectory + @"..\..\..\..\Resources\the_planets.mid").GetAwaiter().GetResult();
#else
        if (args.Length == 0)
        {
            Console.WriteLine("drag a bmp 2.0 formatted midi file onto this exe to test.");
            Console.ReadLine();
            Environment.Exit(0);
        }

        Console.WriteLine("Attempting to process midi file data..");
        var song = BmpSong.OpenFile(args[0]).GetAwaiter().GetResult();
#endif
            

        song.GetProcessedMidiFile().GetAwaiter().GetResult().Write(@"test.mid", true, MidiFileFormat.MultiTrack, new WritingSettings { TextEncoding = Encoding.UTF8 });;

            

        Console.WriteLine("Loading song into preview synthesizer..");

        while (!BmpSiren.Instance.IsReady)
        {
        }

        BmpSiren.Instance.SynthTimePositionChanged += PositionChanged;
        BmpSiren.Instance.LyricTrigger             += LyricTrigger;

        BmpSiren.Instance.Load(song).GetAwaiter().GetResult();

        while (!BmpSiren.Instance.IsReadyForPlayback)
        {
        }

        Console.WriteLine("Press Enter to quit.");
        Console.WriteLine(" ");

        BmpSiren.Instance.Play();

        Console.ReadLine();

        BmpSiren.Instance.SynthTimePositionChanged -= PositionChanged;
        BmpSiren.Instance.LyricTrigger             -= LyricTrigger;

        BmpSiren.Instance.Stop();
        BmpSiren.Instance.ShutDown();
        Environment.Exit(0);
    }

    private static void LyricTrigger(int singer, string line) =>_lastLyric = "Singer: " + singer + " Lyric: " + line;

    private static void PositionChanged(string songTitle, double currenttime, double endtime, int activeVoices)
    {
        Console.SetCursorPosition(0, Console.CursorTop -3);
        Console.WriteLine(songTitle + ": ");
        Console.WriteLine(TimeSpan.FromMilliseconds(currenttime).ToString(@"mm\:ss") + "/" + TimeSpan.FromMilliseconds(endtime).ToString(@"mm\:ss") + " ActiveVoices: " + activeVoices);
        Console.WriteLine(_lastLyric);
    }
        
}