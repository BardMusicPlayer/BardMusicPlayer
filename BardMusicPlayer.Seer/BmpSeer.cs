/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using BardMusicPlayer.Config;
using BardMusicPlayer.Seer.Utilities;

namespace BardMusicPlayer.Seer
{
    public partial class BmpSeer : IDisposable
    {
        private static readonly Lazy<BmpSeer> LazyInstance = new(() => new BmpSeer());

        /// <summary>
        /// 
        /// </summary>
        public bool Started { get; private set; }

        private readonly Dictionary<int, Game> _games;
        private BmpSeer()
        {
            _games = new Dictionary<int, Game>();
        }

        public static BmpSeer Instance => LazyInstance.Value;

        /// <summary>
        /// Current active games
        /// </summary>
        public IReadOnlyDictionary<int, Game> Games => _games;

        /// <summary>
        /// Configure the firewall for Machina
        /// </summary>
        /// <param name="appName">This application name.</param>
        public void SetupFirewall(string appName) => MachinaManager.Instance.SetupFirewall(appName);

        /// <summary>
        /// Start Seer monitoring.
        /// </summary>
        public void Start()
        {
            if (Started) return;
            if (!BmpConfig.Initialized) throw new BmpSeerException("Seer requires Config to be initialized.");
            StartEventsHandler();
            StartProcessWatcher();
            Started = true;
        }

        /// <summary>
        /// Stop Seer monitoring.
        /// </summary>
        public void Stop()
        {
            if (!Started) return;
            StopProcessWatcher();
            StopEventsHandler();
            foreach (var game in _games.Values) game?.Dispose();
            _games.Clear();
            Started = false;
        }

        ~BmpSeer() => Dispose();
        public void Dispose()
        {
            Stop();
            MachinaManager.Instance.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
