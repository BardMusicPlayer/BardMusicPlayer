using System;
using System.Text;
using BardMusicPlayer.Config;
using BardMusicPlayer.Notate.Song;
using BardMusicPlayer.Synth;
using Melanchall.DryWetMidi.Core;

namespace BardMusicPlayer.Notate.ApiTest
{
    class Program
    {
        private static string _lastLyric = "";
        static void Main(string[] args)
        {
            BmpConfig.Initialize(AppContext.BaseDirectory + @"\Notate.ApiTest.json");

            Synthesizer.Instance.Setup();

            if (args.Length == 0)
            {
                Console.WriteLine("drag a bmp 2.0 formatted midi file onto this exe to test.");
                Console.ReadLine();
                Environment.Exit(0);
            }

            Console.WriteLine("Attempting to process midi file data with BmpNotate alpha 12..");
            var song = BmpSong.OpenMidiFile(args[0]).GetAwaiter().GetResult();

            //var song = BmpSong.OpenMidiFile(@"bongocheck.mid").GetAwaiter().GetResult();

            song.GetProcessedMidiFile().GetAwaiter().GetResult().Write(@"test.mid", true, MidiFileFormat.MultiTrack, new WritingSettings { TextEncoding = Encoding.UTF8 });;

            

            Console.WriteLine("Loading song into preview synthesizer (lyrics currently will not display if used)..");

            while (!Synthesizer.Instance.IsReady)
            {
            }

            Synthesizer.Instance.SynthTimePositionChanged += PositionChanged;
            Synthesizer.Instance.LyricTrigger += LyricTrigger;

            Synthesizer.Instance.Load(song).GetAwaiter().GetResult();

            while (!Synthesizer.Instance.IsReadyForPlayback)
            {
            }

            Console.WriteLine("Press Enter to quit.");
            Console.WriteLine(" ");

            Synthesizer.Instance.Play();

            Console.ReadLine();

            Synthesizer.Instance.SynthTimePositionChanged -= PositionChanged;
            Synthesizer.Instance.LyricTrigger -= LyricTrigger;

            Synthesizer.Instance.Stop();
            Synthesizer.Instance.ShutDown();
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
}
