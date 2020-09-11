using System;
using System.Diagnostics;
using System.Threading;
using FFMemoryParser;
using NamedPipeWrapper;

namespace FFBardMusicPlayer {
	public class FFXIVMemory {
		private Process memoryProcess = null;
		private NamedPipeClient<PipeData> dataPipe = null;

		// Loaded signatures

		// Cache of all data structures


		private bool chatInputOpen = false;
		public bool ChatInputOpen {
			get; set;
		}

		private SigActorsData actorData;
		public ActorData localPlayer {
			get {
				if(actorData.currentActors.Count > 0) {
					return actorData.currentActors[0];
				}
				return null;
			}
		}

		public FFXIVMemory() {
		}

		public void Start(Process ffxivProcess) {
			Process proc = new Process();
			proc.StartInfo.FileName = "FFMemoryParser.exe";
			proc.StartInfo.Arguments = string.Format("-p {0}", ffxivProcess.Id);
			proc.StartInfo.UseShellExecute = true;
			proc.StartInfo.Verb = "runas";
			proc.Start();

			memoryProcess = proc;

			Thread.Sleep(2000);

			string pipename = string.Format("BmpPipe{0}", ffxivProcess.Id);
			dataPipe = new NamedPipeClient<PipeData>(pipename);
			dataPipe.ServerMessage += PipeMessage;
			dataPipe.Start();
			Console.WriteLine(string.Format("Pipe name: {0}", pipename));
		}

		public void PipeMessage(NamedPipeConnection<PipeData, PipeData> conn, PipeData message) {
			Console.WriteLine("Received " + message.id);
			if(message.id == "WORLD") {
				SigWorldData data = (SigWorldData)message.data.ToObject();
				if(data != null) {
					Console.WriteLine(string.Format("CLIENT WORLD: {0}", data.world));
				}
			}
		}

		public void Stop() {
			if(IsAttached()) {
				memoryProcess.Kill();
				memoryProcess = null;
			}
		}

		public bool IsAttached() {
			return ((memoryProcess != null) && !memoryProcess.HasExited);
		}
	}
}
