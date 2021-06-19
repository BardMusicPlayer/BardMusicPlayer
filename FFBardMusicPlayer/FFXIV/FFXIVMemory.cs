using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Sharlayan;
using Sharlayan.Core;
using Sharlayan.Events;
using Sharlayan.Models;
using Sharlayan.Models.ReadResults;

namespace FFBardMusicPlayer.FFXIV
{
    public class FFXIVMemory
    {
        public CurrentPlayerResult CurrentPlayer;
        public PartyResult Party;
        public ActorResult Actors;
        public PerformanceResult Performance;
        public bool PerformanceReady;
        private string characterId;

        public string CharacterId
        {
            get => characterId;
            set
            {
                characterId = value;
                OnCharacterIdChanged?.Invoke(this, characterId);
            }
        }

        public bool ChatInputOpen => Reader.CanGetChatInput() && Reader.IsChatInputOpen();

        public string ChatInputString => Reader.CanGetChatInput() ? Reader.GetChatInputString() : string.Empty;

        private int previousArrayIndex;
        private int previousOffset;
        private readonly Stack<ChatLogItem> logItems = new Stack<ChatLogItem>();
        private readonly List<ChatLogItem> completeLog = new List<ChatLogItem>();

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

        private bool hasLost = true;
        private Process ffxivProcess;
        private Thread thread;
        private bool hasScanned;
        private bool isLoggedIn;

        public FFXIVMemory() { Reset(); }

        private void Reset()
        {
            hasScanned = false;
            isLoggedIn = false;

            logItems.Clear();
            completeLog.Clear();
            previousArrayIndex = 0;
            previousOffset     = 0;

            CurrentPlayer = new CurrentPlayerResult();
            Party         = new PartyResult();
            Actors        = new ActorResult();
            Performance   = new PerformanceResult();
        }

        public void SetProcess(Process process)
        {
            if (process == null || process.HasExited)
            {
                return;
            }

            ffxivProcess = process;
            Reset();

            var gameLanguage = "English";
            var useLocalCache = true;
            var patchVersion = "latest";

            var processModel = new ProcessModel
            {
                Process = process,
                IsWin64 = process.ProcessName.Contains("_dx11")
            };
            MemoryHandler.Instance.ExceptionEvent       += MemoryHandler_ExceptionEvent;
            MemoryHandler.Instance.SignaturesFoundEvent += MemoryHandler_SignaturesFoundEvent;
            MemoryHandler.Instance.SetProcess(processModel, gameLanguage, patchVersion, useLocalCache);
        }

        public void UnsetProcess()
        {
            MemoryHandler.Instance.ExceptionEvent       -= MemoryHandler_ExceptionEvent;
            MemoryHandler.Instance.SignaturesFoundEvent -= MemoryHandler_SignaturesFoundEvent;
            MemoryHandler.Instance.UnsetProcess();
            ffxivProcess = null;
        }

        public void StartThread()
        {
            // run the refresh through once first, so other things aren't waiting
            // for valid information on initialization
            // this is a hack around threading
            Refresh();
            if (thread == null)
            {
                var memoryThread = new ThreadStart(FFXIVMemory_Main);
                thread = new Thread(memoryThread);
                thread.Start();
            }
        }

        public void StopThread()
        {
            if (thread != null)
            {
                thread.Interrupt();
                thread = null;
            }
        }

        public bool IsThreadAlive() => thread != null && thread.IsAlive;

        public void FFXIVMemory_Main()
        {
            Console.WriteLine("Memory main loop");
            
            try
            {
                while (true)
                {
                    if (!Refresh())
                    {
                        // Restart?
                    }

                    Thread.Sleep(100);
                }
            }
            catch (ThreadInterruptedException)
            {
                
            }

            Console.WriteLine("Reached end");
        }

        public bool Refresh()
        {
            if (ffxivProcess != null)
            {
                ffxivProcess.Refresh();
                if (ffxivProcess.HasExited)
                {
                    OnProcessLost?.Invoke(this, EventArgs.Empty);
                    ffxivProcess = null;
                    hasLost      = true;
                    Reset();

                    Console.WriteLine("Exited game");
                }

                if (IsScanning() && !hasScanned)
                {
                    Console.WriteLine("Scanning...");
                    while (IsScanning())
                    {
                        Thread.Sleep(100);
                    }

                    Console.WriteLine("Finished scanning");
                    OnProcessReady?.Invoke(this, ffxivProcess);
                    hasScanned = true;
                }
            }

            if (ffxivProcess == null && hasLost)
            {
                OnProcessSeek?.Invoke(this, EventArgs.Empty);
                hasLost = false;
                return false;
            }

            if (Reader.CanGetCharacterId())
            {
                var id = Reader.GetCharacterId();
                if (!string.IsNullOrEmpty(id))
                {
                    if (string.IsNullOrEmpty(CharacterId) ||
                        !string.IsNullOrEmpty(CharacterId) && !CharacterId.Equals(id))
                    {
                        CharacterId = id;
                    }
                }
            }

            if (Reader.CanGetPlayerInfo())
            {
                var res = Reader.GetCurrentPlayer();
                if (res.CurrentPlayer.Job != CurrentPlayer.CurrentPlayer.Job)
                {
                    if (CurrentPlayer.CurrentPlayer.Job == Sharlayan.Core.Enums.Actor.Job.Unknown)
                    {
                        // Logged in
                        OnCurrentPlayerLogin?.Invoke(this, res);
                        isLoggedIn = true;
                    }
                    else if (res.CurrentPlayer.Job == Sharlayan.Core.Enums.Actor.Job.Unknown)
                    {
                        // Logged out
                        OnCurrentPlayerLogout?.Invoke(this, CurrentPlayer);
                        isLoggedIn = false;
                        Reset();
                    }
                    else
                    {
                        OnCurrentPlayerJobChange?.Invoke(this, CurrentPlayer);
                    }
                }

                CurrentPlayer = res;
            }

            if (!isLoggedIn)
            {
                return false;
            }

            if (Reader.CanGetPartyMembers())
            {
                var party2 = Reader.GetPartyMembers();
                if (party2.NewPartyMembers.Count > 0 ||
                    party2.RemovedPartyMembers.Count > 0)
                {
                    // Something changed
                    Party = party2;
                    OnPartyChanged?.Invoke(this, party2);
                }

                var pcount = Party.PartyMembers.Count;
                var pcount2 = party2.PartyMembers.Count;
                if (pcount != pcount2)
                {
                    Party = party2;
                    OnPartyChanged?.Invoke(this, party2);
                }
            }

            if (Reader.CanGetPerformance())
            {
                var changedIds = new List<uint>();
                var perf = Reader.GetPerformance();
                if (!perf.Performances.IsEmpty && !Performance.Performances.IsEmpty)
                {
                    foreach (var pp in perf.Performances)
                    {
                        if (Performance.Performances.ContainsKey(pp.Key) &&
                            pp.Value.Status != Performance.Performances[pp.Key].Status)
                        {
                            changedIds.Add(pp.Key);
                        }
                    }
                }

                if (changedIds.Count > 0)
                {
                    var actorIds = new List<uint>();
                    if (Reader.CanGetActors())
                    {
                        foreach (var actor in Reader.GetActors().CurrentPCs.Values)
                        {
                            if (changedIds.Contains(actor.PerformanceID / 2))
                            {
                                actorIds.Add(actor.ID);
                            }
                        }
                    }

                    if (actorIds.Count > 0)
                    {
                        OnPerformanceChanged?.Invoke(this, actorIds);
                    }
                }

                //Update
                Performance = perf;

                var r = perf.Performances[0].IsReady();
                if (r != PerformanceReady)
                {
                    PerformanceReady = r;
                    OnPerformanceReadyChanged?.Invoke(this, PerformanceReady);
                }
            }

            logItems.Clear();
            if (Reader.CanGetChatLog())
            {
                var readResult = Reader.GetChatLog(previousArrayIndex, previousOffset);
                previousArrayIndex = readResult.PreviousArrayIndex;
                previousOffset     = readResult.PreviousOffset;
                foreach (var item in readResult.ChatLogItems)
                {
                    logItems.Push(item);
                    completeLog.Add(item);
                    OnChatReceived?.Invoke(this, item);
                }
            }

            if (Reader.CanGetActors())
            {
                var jobsum0 = 0;
                if (Actors != null)
                {
                    jobsum0 = Actors.CurrentPCs.Sum(e => (int) e.Value.Job);
                }

                var actorRes = Reader.GetActors();
                if (Actors != null)
                {
                    if (actorRes.CurrentPCs.Count != Actors.CurrentPCs.Count)
                    {
                        Actors = actorRes;
                        OnPcChanged?.Invoke(this,
                            actorRes.CurrentPCs.ToDictionary(k => k.Key, k => k.Value as ActorItemBase));
                    }

                    var jobsum1 = actorRes.CurrentPCs.Sum(e => (int) e.Value.Job);

                    if (jobsum0 != jobsum1)
                    {
                        Actors = actorRes;
                        OnPcChanged?.Invoke(this,
                            actorRes.CurrentPCs.ToDictionary(k => k.Key, k => k.Value as ActorItemBase));
                    }
                }
                else
                {
                    Actors = actorRes;
                    OnPcChanged?.Invoke(this,
                        actorRes.CurrentPCs.ToDictionary(k => k.Key, k => k.Value as ActorItemBase));
                }
            }

            return true;
        }

        public List<ActorItem> GetActorItems(List<uint> keys)
        {
            if (Reader.CanGetActors())
            {
                var res = Reader.GetActors();
                if (res != null && res.CurrentPCs.Count > 0)
                {
                    var actors = res.CurrentPCs.Where(t => keys.Contains(t.Key)).Select(t => t.Value).ToList();
                    return actors;
                }
            }

            return new List<ActorItem>();
        }

        public bool IsScanning() => Scanner.Instance.IsScanning;

        public bool IsAttached() => ffxivProcess != null;

        public bool IsReady() => IsAttached() && !IsScanning();

        private static void MemoryHandler_ExceptionEvent(object sender, ExceptionEvent e)
        {
            if (true)
            {
                Console.WriteLine(e.Exception.Message);
            }
        }

        private static void MemoryHandler_SignaturesFoundEvent(object sender, SignaturesFoundEvent e)
        {
            foreach (var kvp in e.Signatures)
            {
                Console.WriteLine(
                    string.Format($"Signature [{kvp.Key}] Found At Address: [{((IntPtr) kvp.Value).ToString("X")}]"));
            }
        }
    }
}