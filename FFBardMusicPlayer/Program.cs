using CommandLine;
using FFBardMusicPlayer.Forms;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace FFBardMusicPlayer {

	class Program {

		public static string urlBase = "http://bmp.sqnya.se/";

#if DEBUG
        public static string appBase = Application.StartupPath;
#else
        public static string appBase = Application.CommonAppDataPath;
#endif

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();
		
        public class Options {
			[Option('t', "window-title", HelpText = "Hook in to the specified window title. Useful for sandboxes.")]
			public string HookWindowTitle { get; set; }

			[Option('p', "window-pid", HelpText = "Hook in to the specified window process id.")]
			public int HookPid { get; set; }

			[Option('m', "midi-file", HelpText = "Load the specified midi file at startup.")]
			public string LoadMidiFile { get; set; }

			[Option('i', "midi-input", HelpText = "Use the specified midi input device name as input.")]
			public string MidiInput { get; set; }

			[Option('m', "disable-memory", Default = false, HelpText = "Disable Sharlayan memory polling.")]
			public bool DisableMemory { get; set; }

			[Option('u', "disable-update", Default = false, HelpText = "Disable version update check.")]
			public bool DisableUpdate { get; set; }

            [Option('d', "dat-prefix", HelpText = "Set a custom prefix, such as \"isboxer-\" (example matches isboxer-HOTBAR.DAT) for any parsed player .dat files")]
            public string DatPrefix { get; set; }
		}

		public static Options programOptions = new Options();

        [STAThread]
        static void Main(string[] args)
        {
			// Parse args
			Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(options => { programOptions = options; });

			Application.EnableVisualStyles();

			CultureInfo nonInvariantCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = nonInvariantCulture;
			Application.CurrentInputLanguage = InputLanguage.FromCulture(new CultureInfo("en"));

			if(GetConsoleWindow() != IntPtr.Zero) {
				Console.OutputEncoding = System.Text.Encoding.UTF8;
			}

			Sharlayan.Reader.JsonPath = appBase;

			BmpPluginHelper helper = BmpPluginHelper.LoadPlugins();
			string filename = "songs/test.mml";
			if (helper.LoadFile(filename, out Sequencer song, out string error))
			{
				Console.WriteLine(string.Format("Loaded MML {0} [{1}]", song, error));
			} else
			{
				Console.WriteLine(string.Format("Couldn't load [{0}]", filename));
			}
			//Console.ReadKey();

			BmpMain app = new BmpMain();
			Application.Run(app);
		}
	}
}
