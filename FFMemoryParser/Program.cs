using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace FFMemoryParser {
	static class Program {

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();


		public class Options {
			[Option('t', "window-title", HelpText = "Hook in to the specified window title. Useful for sandboxes.")]
			public string HookWindowTitle { get; set; }

			[Option('p', "window-pid", HelpText = "Hook in to the specified window process id.")]
			public int HookPid { get; set; }

			[Option('s', "signature-file", HelpText = "Use the specified signature file.")]
			public string LoadSignatureFile { get; set; }

			[Option('r', "polling-rate", HelpText = "Use the given polling rate (milliseconds).", Default = 500)]
			public int PollingRate { get; set; }

		}

		public static Options programOptions = new Options();

		[STAThread]
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
				foreach (Process proc in processes) {
					if(proc.Id == programOptions.HookPid) {
						ffxivProcess = proc;
						break;
					}
				}
			}
			if(ffxivProcess != null) {
				PipeMemory mem = new PipeMemory(ffxivProcess);
				List<Signature> sigList = new List<Signature>();

				string file = programOptions.LoadSignatureFile;
				if(string.IsNullOrEmpty(file)) {
					file = "signatures.json";
				}
				using (var streamReader = new StreamReader(file)) {
					sigList = JsonSerializer.DeserializeFromReader<List<Signature>>(streamReader);
				}

				mem.SearchMemory(sigList);

				int MemoryDelay = programOptions.PollingRate;
				Console.WriteLine(string.Format("-- Start thread ({0})", MemoryDelay));
				do {
					while (true) {
						while(!Console.KeyAvailable) {
							mem.MemoryLoop();
							Thread.Sleep(MemoryDelay);
						}
					}
				} while (Console.ReadKey(true).Key != ConsoleKey.NoName);

			}
		}
	}
}
