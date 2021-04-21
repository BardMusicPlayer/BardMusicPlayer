using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BardMusicPlayer.Common.Enums;
using BardMusicPlayer.Common.Structs;

namespace BardMusicPlayer.Seer
{
    public sealed class Storage : IDisposable
    {
        public int Pid { get; }
        public GameRegion GameRegion { get; }
        public string GamePath { get; }
        public string ConfigPath { get; }
        public int ActorId { get; internal set; } = 0;
        public string PlayerName { get; internal set; } = "Unknown";
        public string WorldName { get; internal set; } = "Unknown";

        internal ConcurrentDictionary<int, int> PianoKeys { get; } = new();
        public Dictionary<int, int> GetPianoKeys => PianoKeys.ToDictionary(entry => entry.Key, entry => entry.Value);

        internal ConcurrentDictionary<Instrument, int> InstrumentKeys { get; } = new();
        public Dictionary<Instrument, int> GetInstrumentKeys => InstrumentKeys.ToDictionary(entry => entry.Key, entry => entry.Value);

        internal ConcurrentDictionary<GameMenuKey, int> GameMenuKeys { get; } = new();
        public Dictionary<Instrument, int> GetGameMenuKeys => InstrumentKeys.ToDictionary(entry => entry.Key, entry => entry.Value);


        internal Storage(int pid)
        {
            Pid = pid;
            GameRegion = GetGameRegion(pid);
            GamePath = GetGamePath(pid);
            ConfigPath = GetConfigPath(pid);
        }

        ~Storage() => Dispose();
        public void Dispose()
        {
        }

        private static GameRegion GetGameRegion(int pid)
        {
            // Todo
            return GameRegion.Global;
        }

        private static string GetGamePath(int pid)
        {
            // Todo
            return "";
        }

        private static string GetConfigPath(int pid)
        {
            // Todo
            return Environment.SpecialFolder.MyDocuments + @"My Games\Final Fantasy XIV";
        }
    }
}
