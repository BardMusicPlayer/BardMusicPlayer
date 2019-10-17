using CommandLine;
using FFBardMusicPlayer.Forms;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace FFBardMusicPlayer {

	class Program {

		public static string urlBase = "http://bmp.sqnya.se/";
		public static string appBase = Application.CommonAppDataPath;

        public static string overrideWindowTitle="BMPdefaultWindowCheck";

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

        //static Mutex mutex = new Mutex(true, "{FC7C9ABB-4F98-43F6-891B-F4161097C939}");

        public class Options
        {
            [Option('t', "override-window-title", Required = false, HelpText = "Override the ffxiv window process title to look for. Useful for custom sandboxed game instances.")]
            public string OverrideWindowTitle { get; set; }
        }

        [STAThread]
        static void Main(string[] args)
        {
            		/*
			if(!mutex.WaitOne(TimeSpan.Zero, true)) {
				PostMessage((IntPtr) HWND_BROADCAST, App.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
				return;
			}
			*/
            
            		Parser.Default.ParseArguments<Options>(args)
           		        .WithParsed<Options>(options =>
            		       {
            		           if (!string.IsNullOrEmpty(options.OverrideWindowTitle)) overrideWindowTitle = options.OverrideWindowTitle;
             		      });

          		  Application.EnableVisualStyles();

			CultureInfo nonInvariantCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = nonInvariantCulture;
			Application.CurrentInputLanguage = InputLanguage.FromCulture(new CultureInfo("en"));

			if(GetConsoleWindow() != IntPtr.Zero) {
				Console.OutputEncoding = System.Text.Encoding.UTF8;
			}

			Sharlayan.Reader.JsonPath = appBase;

			BmpMain app = new BmpMain();
			Application.Run(app);
		}
	}
}
