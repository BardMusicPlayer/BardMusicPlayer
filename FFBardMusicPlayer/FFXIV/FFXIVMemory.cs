using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

using Sharlayan;
using Sharlayan.Models;
using Sharlayan.Models.ReadResults;
using Sharlayan.Events;
using Sharlayan.Core;

namespace FFBardMusicPlayer {
	public class FFXIVMemory {

		public CurrentPlayerResult currentPlayer;
		public PartyResult party;
		public ActorResult actors;
		public PerformanceResult performance;
		public bool performanceReady;

		private string characterId;
		public string CharacterID {
			get {
				return characterId;
			}
			set {
				characterId = value;
				OnCharacterIdChanged?.Invoke(this, characterId);
			}
		}

		public bool ChatInputOpen {
			get {
				if(Reader.CanGetChatInput()) {
					return Reader.IsChatInputOpen();
				}
				return false;
			}
		}
		public string ChatInputString {
			get {
				if(Reader.CanGetChatInput()) {
					return Reader.GetChatInputString();
				}
				return string.Empty;
			}
		}

		int _previousArrayIndex = 0;
		int _previousOffset = 0;
		Stack<ChatLogItem> logItems = new Stack<ChatLogItem>();
		List<ChatLogItem> completeLog = new List<ChatLogItem>();

		public event EventHandler OnProcessSeek;
		public event EventHandler OnProcessLost;
		public event EventHandler<Process> OnProcessReady;

		public event EventHandler<ChatLogItem> OnChatReceived;
		public event EventHandler<CurrentPlayerResult> OnCurrentPlayerLogin;
		public event EventHandler<CurrentPlayerResult> OnCurrentPlayerLogout;
		public event EventHandler<CurrentPlayerResult> OnCurrentPlayerJobChange;
		public event EventHandler<PartyResult> OnPartyChanged;
		public event EventHandler<Dictionary<uint, ActorItemBase>> OnPcChanged;
		public event EventHandler<List<uint>> OnPerformanceChanged;
		public event EventHandler<bool> OnPerformanceReadyChanged;
		public event EventHandler<string> OnCharacterIdChanged;

		bool hasLost = true;

		Process ffxivProcess;
		Thread thread;
		bool hasScanned;
		bool isLoggedIn;

		public FFXIVMemory() {
			Reset();
		}

		private void Reset() {
			hasScanned = false;
			isLoggedIn = false;

			logItems.Clear();
			completeLog.Clear();
			_previousArrayIndex = 0;
			_previousOffset = 0;

			currentPlayer = new CurrentPlayerResult();
			party = new PartyResult();
			actors = new ActorResult();
			performance = new PerformanceResult();
		}

		public void SetProcess(Process process) {
			if(process == null || process.HasExited) {
				return;
			}
			ffxivProcess = process;
			Reset();

			string gameLanguage = "English";
			bool useLocalCache = true;
			string patchVersion = "latest";

			ProcessModel processModel = new ProcessModel {
				Process = process,
				IsWin64 = process.ProcessName.Contains("_dx11")
			};
			MemoryHandler.Instance.ExceptionEvent += MemoryHandler_ExceptionEvent;
			MemoryHandler.Instance.SignaturesFoundEvent += MemoryHandler_SignaturesFoundEvent;
			MemoryHandler.Instance.SetProcess(processModel, gameLanguage, patchVersion, useLocalCache, false);
		}

		public void UnsetProcess() {
			MemoryHandler.Instance.ExceptionEvent -= MemoryHandler_ExceptionEvent;
			MemoryHandler.Instance.SignaturesFoundEvent -= MemoryHandler_SignaturesFoundEvent;
			MemoryHandler.Instance.UnsetProcess();
			ffxivProcess = null;
		}

		public void StartThread() {
            // run the refresh through once first, so other things aren't waiting
            // for valid information on initialization
            // this is a hack around threading
            Refresh();
			if(thread == null) {
				ThreadStart memoryThread = new ThreadStart(FFXIVMemory_Main);
				thread = new Thread(memoryThread);
				thread.Start();
			}
		}

		public void StopThread() {
			if(thread != null) {
				thread.Interrupt();
				thread = null;
			}
		}

		public bool IsThreadAlive() {
			if(thread == null) {
				return false;
			}
			return thread.IsAlive;
		}

		public void FFXIVMemory_Main() {
			Console.WriteLine("Memory main loop");
			bool alive = true;
			try {
				while(alive) {
					if(!Refresh()) {
						// Restart?
					}
					Thread.Sleep(100);
				}
			} catch(ThreadInterruptedException) {
				alive = false;
			}
			Console.WriteLine("Reached end");
		}

		public bool Refresh() {

			if(ffxivProcess != null) {
				ffxivProcess.Refresh();
				if(ffxivProcess.HasExited) {
					OnProcessLost?.Invoke(this, EventArgs.Empty);
					ffxivProcess = null;
					hasLost = true;
					Reset();

					Console.WriteLine("Exited game");
				}
				if(IsScanning() && !hasScanned) {
					Console.WriteLine("Scanning...");
					while(IsScanning()) {
						Thread.Sleep(100);
					}
					Console.WriteLine("Finished scanning");
					OnProcessReady?.Invoke(this, ffxivProcess);
					hasScanned = true;
				}
			}
			if((ffxivProcess == null) && hasLost) {
				OnProcessSeek?.Invoke(this, EventArgs.Empty);
				hasLost = false;
				return false;
			}


			if(Reader.CanGetCharacterId()) {
				string id = Reader.GetCharacterId();
				if(!string.IsNullOrEmpty(id)) {
					if(string.IsNullOrEmpty(CharacterID) ||
						(!string.IsNullOrEmpty(CharacterID) && !CharacterID.Equals(id))) {
						CharacterID = id;
					}
				}
			}
			if(Reader.CanGetPlayerInfo()) {
				CurrentPlayerResult res = Reader.GetCurrentPlayer();
				if(res.CurrentPlayer.Job != currentPlayer.CurrentPlayer.Job) {
					if(currentPlayer.CurrentPlayer.Job == Sharlayan.Core.Enums.Actor.Job.Unknown) {
						// Logged in
						OnCurrentPlayerLogin?.Invoke(this, res);
						isLoggedIn = true;

					} else if(res.CurrentPlayer.Job == Sharlayan.Core.Enums.Actor.Job.Unknown) {
						// Logged out
						OnCurrentPlayerLogout?.Invoke(this, currentPlayer);
						isLoggedIn = false;
						Reset();

					} else {
						OnCurrentPlayerJobChange?.Invoke(this, currentPlayer);
					}
				}
				currentPlayer = res;
			}
			if(!isLoggedIn) {
				return false;
			}
			if(Reader.CanGetPartyMembers()) {
				PartyResult party2 = Reader.GetPartyMembers();
				if(party2.NewPartyMembers.Count > 0 ||
					party2.RemovedPartyMembers.Count > 0) {
					// Something changed
					party = party2;
					OnPartyChanged?.Invoke(this, party2);
				}
				int pcount = party.PartyMembers.Count;
				int pcount2 = party2.PartyMembers.Count;
				if(!(party is PartyResult) || (party is PartyResult && (pcount != pcount2))) {
					party = party2;
					OnPartyChanged?.Invoke(this, party2);
				}
			}
			if(Reader.CanGetPerformance()) {


				List<uint> changedIds = new List<uint>();
				PerformanceResult perf = Reader.GetPerformance();
				if(!perf.Performances.IsEmpty && !performance.Performances.IsEmpty) {
					foreach(KeyValuePair<uint, PerformanceItem> pp in perf.Performances) {
						if(performance.Performances.ContainsKey(pp.Key) && pp.Value.Status != performance.Performances[pp.Key].Status) {
							changedIds.Add(pp.Key);
						}
					}
				}

				if(changedIds.Count > 0) {
					List<uint> actorIds = new List<uint>();
					if(Reader.CanGetActors()) {
						foreach(ActorItem actor in Reader.GetActors().CurrentPCs.Values) {
							if(changedIds.Contains(actor.PerformanceID / 2)) {
								actorIds.Add(actor.ID);
							}
						}
					}
					if(actorIds.Count > 0) {
						OnPerformanceChanged?.Invoke(this, actorIds);
					}
				}
				
				//Update
				performance = perf;

				bool r = perf.Performances[0].IsReady();
				if(r != performanceReady) {
					performanceReady = r;
					OnPerformanceReadyChanged?.Invoke(this, performanceReady);
				}
			}

			logItems.Clear();
			if(Reader.CanGetChatLog()) {
				ChatLogResult readResult = Reader.GetChatLog(_previousArrayIndex, _previousOffset);
				_previousArrayIndex = readResult.PreviousArrayIndex;
				_previousOffset = readResult.PreviousOffset;
				foreach(ChatLogItem item in readResult.ChatLogItems) {
					logItems.Push(item);
					completeLog.Add(item);
					OnChatReceived?.Invoke(this, item);
				}
			}
			if(Reader.CanGetActors()) {
				int jobsum0 = 0;
				if(actors != null) {
					jobsum0 = actors.CurrentPCs.Sum(e => (int) e.Value.Job);
				}

				ActorResult actorRes = Reader.GetActors();
				if(actors != null) {
					if(actorRes.CurrentPCs.Count != actors.CurrentPCs.Count) {
						actors = actorRes;
						OnPcChanged?.Invoke(this, actorRes.CurrentPCs.ToDictionary(k => k.Key, k => k.Value as ActorItemBase));
					}
					int jobsum1 = actorRes.CurrentPCs.Sum(e => (int) e.Value.Job);

					if(jobsum0 != jobsum1) {
						actors = actorRes;
						OnPcChanged?.Invoke(this, actorRes.CurrentPCs.ToDictionary(k => k.Key, k => k.Value as ActorItemBase));
					}
				} else {
					actors = actorRes;
					OnPcChanged?.Invoke(this, actorRes.CurrentPCs.ToDictionary(k => k.Key, k => k.Value as ActorItemBase));
				}
			}
			return true;
		}

		public List<ActorItem> GetActorItems(List<uint> keys) {
			if(Reader.CanGetActors()) {
				ActorResult res = Reader.GetActors();
				if(res != null && res.CurrentPCs.Count > 0) {
					List<ActorItem> actors = res.CurrentPCs.Where(t => keys.Contains(t.Key)).Select(t => t.Value).ToList();
					return actors;
				}
			}
			return new List<ActorItem>();
		}

		public bool IsScanning() {
			return Scanner.Instance.IsScanning;
		}
		public bool IsAttached() {
			return ffxivProcess != null;
		}
		public bool IsReady() {
			return (IsAttached() && !IsScanning());
		}

		private static void MemoryHandler_ExceptionEvent(object sender, ExceptionEvent e) {
			if(true) {
				Console.WriteLine(e.Exception.Message);
			}
		}

		private static void MemoryHandler_SignaturesFoundEvent(object sender, SignaturesFoundEvent e) {
			foreach(KeyValuePair<string, Signature> kvp in e.Signatures) {
				Console.WriteLine(string.Format($"Signature [{kvp.Key}] Found At Address: [{((IntPtr) kvp.Value).ToString("X")}]"));
			}
		}
	}
}
