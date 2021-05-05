using System;
using System.IO;
using BardMusicPlayer.Notate.Song;
using BardMusicPlayer.Synth;

namespace BardMusicPlayer.Notate.ApiTest
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("drag a bmp 2.0 formatted midi file onto this exe to test.");
                Console.ReadLine();
                Environment.Exit(0);
            }

            Console.WriteLine("Attempting to process midi file data with BmpNotate alpha6..");
            var song = BmpSong.OpenMidiFile(args[0]).GetAwaiter().GetResult();

         //   var song = BmpSong.OpenMidiFile(@"Chrono_Trigger_Corridors_of_Time_octet.mid").GetAwaiter().GetResult();

            File.Delete(@"test.mid");
            song.GetProcessedMidiFile().GetAwaiter().GetResult().Write(@"test.mid");;

            Synthesizer.Instance.Setup();

            Console.WriteLine("Loading song into preview synthesizer..");

            while (!Synthesizer.Instance.IsReady)
            {
            }

            Synthesizer.Instance.SynthTimePositionChanged += PositionChanged;

            Synthesizer.Instance.Load(song).GetAwaiter().GetResult();

            while (!Synthesizer.Instance.IsReadyForPlayback)
            {
            }

            Console.WriteLine("Press Enter to quit.");
            Console.WriteLine(" ");

            Synthesizer.Instance.Play();

            Console.ReadLine();

            Synthesizer.Instance.SynthTimePositionChanged -= PositionChanged;

            Synthesizer.Instance.Stop();
            Synthesizer.Instance.ShutDown();
            Environment.Exit(0);
        }

        private static void PositionChanged(string songTitle, double currenttime, double endtime, int activeVoices)
        {
            Console.Write("\r" + songTitle + ": " + TimeSpan.FromMilliseconds(currenttime).ToString(@"mm\:ss") + "/" + TimeSpan.FromMilliseconds(endtime).ToString(@"mm\:ss") + " ActiveVoices: " + activeVoices);
        }
        
    }
}
