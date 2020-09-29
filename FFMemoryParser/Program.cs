using CommandLine;
using NamedPipeWrapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFMemoryParser {
	class Program {

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();


		public class Options {
			[Option('t', "window-title", HelpText = "Hook in to the specified window title. Useful for sandboxes.")]
			public string HookWindowTitle { get; set; }

			[Option('p', "window-pid", HelpText = "Hook in to the specified window process id.")]
			public int HookPid { get; set; }

			[Option('s', "signature-file", HelpText = "Use the specified signature file.")]
			public string LoadSignatureFile { get; set; }

		}

		public static Options programOptions = new Options();

		static void Main(string[] args) {

			// Parse args
			Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(options => { programOptions = options; });


			if (GetConsoleWindow() != IntPtr.Zero) {
				Console.OutputEncoding = System.Text.Encoding.UTF8;
			}

			List<Process> processes = new List<Process>(Process.GetProcessesByName("ffxiv_dx11"));
			Process ffxivProcess = null;
			if(processes.Count > 0) {
				ffxivProcess = processes[0];
			}
			if(ffxivProcess != null) {
				Memory mem = new Memory(ffxivProcess);
				List<Signature> sigList = new List<Signature>();

				string file = programOptions.LoadSignatureFile;
				if(string.IsNullOrEmpty(file)) {
					file = "C:\\ProgramData\\FFBardMusicPlayer\\signatures.json";
				}
				using (var streamReader = new StreamReader(file)) {
					var json = streamReader.ReadToEnd();
					sigList = JsonConvert.DeserializeObject<List<Signature>>(json);
				}

				mem.SearchMemory(sigList);

				Console.WriteLine("-- Start thread");
				do {
					while (true) {
						while(!Console.KeyAvailable) {
							mem.MemoryLoop();
							Thread.Sleep(1000);
						}
					}
				} while (Console.ReadKey(true).Key != ConsoleKey.NoName);

			}
		}
	}
}
