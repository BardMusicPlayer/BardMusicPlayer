using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using FFBardMusicCommon;
using FFMemoryParser;
using NamedPipeWrapper;

namespace FFBardMusicPlayer {
	public class FFXIVMemory {
		private Process memoryProcess = null;
		private NamedPipeClient<PipeData> dataPipe = null;

		// Loaded signatures

		// Cache of all data structures

		// TODO
		// Memory_OnCurrentPlayerJobChange
		// Memory_OnLocalOrchestraUpdate ? check LocalOrchestraUpdate in old BmpMain
		// + make BmpProcessSelect instance this and quickly get relevant sig


		public event EventHandler<ChatLogItem> OnChatReceived;
		public event EventHandler<string> OnCharacterId;

		public event EventHandler<SigPerfData> OnPerformanceChanged;

		public event EventHandler<ActorData> OnLocalPlayerLogin;
		public event EventHandler<ActorData> OnLocalPlayerLogout;

		public string World = string.Empty;
		public string CharacterID = string.Empty;
		public bool ChatInputOpen = false;

		private SigPerfData performanceData;
		public bool LocalPerformanceUp {
			get {
				if(performanceData != null) {
					return performanceData.IsUp();
				}
				return false;
			}
		}

		public bool LocalPerformanceBardJob {
			get {
				if(localPlayer != null) {
					return (localPlayer.jobid == 0x17);
				}
				return false;
			}
		}

		private SigActorsData actorData;
		public ActorData localPlayer;

		public FFXIVMemory() {
		}

		public void Start(Process ffxivProcess) {
			Process proc = new Process();
			proc.StartInfo.FileName = "FFMemoryParser.exe";
			proc.StartInfo.Arguments = string.Format("-p {0}", ffxivProcess.Id);
			proc.StartInfo.UseShellExecute = false;
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

		public Object ByteArrayToObject(byte[] bytes) {
			MemoryStream memStream = new MemoryStream();
			BinaryFormatter binForm = new BinaryFormatter();
			memStream.Write(bytes, 0, bytes.Length);
			memStream.Seek(0, SeekOrigin.Begin);
			return (Object) binForm.Deserialize(memStream);
		}

		public void PipeMessage(NamedPipeConnection<PipeData, PipeData> conn, PipeData message) {
			Type type = Type.GetType(message.id + ",FFMemoryParser");
			if(type == null) { return; }

			Object obj = message.data.ToObject();
			if(obj != null) {
				if (type.Name == "SignatureList") {
					Memory.SignatureList data = (obj as Memory.SignatureList);
					if (data != null) {
						foreach (Signature item in data) {
							Console.WriteLine(string.Format("Signature {0} Not found!", item.Key));
						}
					}
				}

				if (type.Name == "SigWorldData") {
					SigWorldData data = (obj as SigWorldData);
					if (data != null) {
						World = data.world;
					}
				}
				if (type.Name == "SigCharIdData") {
					SigCharIdData data = (obj as SigCharIdData);
					if (data != null) {
						CharacterID = data.id;
						OnCharacterId?.Invoke(this, CharacterID);
					}
				}
				// ...
				if (type.Name == "SigPerfData") {
					SigPerfData data = (obj as SigPerfData);
					if (data != null) {
						performanceData = data;

						OnPerformanceChanged?.Invoke(this, performanceData);
					}
				}
				if (type.Name == "SigActorsData") {
					SigActorsData data = (obj as SigActorsData);
					if (data != null) {
						actorData = data;

						ActorData local = null;
						if (actorData.currentActors.Count > 0) {
							local = actorData.currentActors.First().Value;
						}
						if(localPlayer == null || local.name != localPlayer.name) {
							localPlayer = local;
							if(localPlayer != null) {
								OnLocalPlayerLogin?.Invoke(this, localPlayer);
							} else {
								OnLocalPlayerLogout?.Invoke(this, localPlayer);
							}
						}
					}
				}
				if (type.Name == "SigChatLogData") {
					SigChatLogData data = (obj as SigChatLogData);
					if (data != null) {
						foreach (ChatLogItem item in data.chatMessages) {
							OnChatReceived?.Invoke(this, item);
						}
					}
				}
				if (type.Name == "SigChatInputData") {
					SigChatInputData data = (obj as SigChatInputData);
					if (data != null) {
						ChatInputOpen = data.open;
						if (data.open) {
							Console.WriteLine(data.text);
						}
					}
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
