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

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		//static Mutex mutex = new Mutex(true, "{FC7C9ABB-4F98-43F6-891B-F4161097C939}");

		[STAThread]
		static void Main(string[] args) {
			/*
			if(!mutex.WaitOne(TimeSpan.Zero, true)) {
				PostMessage((IntPtr) HWND_BROADCAST, App.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
				return;
			}
			*/

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
