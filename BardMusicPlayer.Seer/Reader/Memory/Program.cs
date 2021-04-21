using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BardMusicPlayer.Seer.Reader.Memory {
	class Program {
		

		static void Main(string[] args) {



			List<Process> processes = new List<Process>(Process.GetProcessesByName("ffxiv_dx11"));
			Process ffxivProcess = null;
			if(processes.Count > 0) {
				ffxivProcess = processes[0];
			}
			if(ffxivProcess != null) {
				Memory mem = new Memory(ffxivProcess);
				List<Signature> sigList = new List<Signature>();

                string file = "";
				if(string.IsNullOrEmpty(file)) {
					file = "C:\\ProgramData\\FFBardMusicPlayer\\signatures.json";
				}
				using (var streamReader = new StreamReader(file)) {
					var json = streamReader.ReadToEnd();
					//sigList = JsonConvert.DeserializeObject<List<Signature>>(json);
				}

				mem.SearchMemory(sigList);

				//int MemoryDelay = programOptions.PollingRate;
				Console.WriteLine("-- Start thread");
				do {
					while (true) {
						while(!Console.KeyAvailable) {
							mem.MemoryLoop();
							//Thread.Sleep(MemoryDelay);
						}
					}
				} while (Console.ReadKey(true).Key != ConsoleKey.NoName);

			}
		}
	}
}
