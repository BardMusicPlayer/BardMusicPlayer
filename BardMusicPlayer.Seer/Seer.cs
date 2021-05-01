/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using BardMusicPlayer.Seer.Utilities;

namespace BardMusicPlayer.Seer
{
    public partial class Seer : IDisposable
    {
        private static readonly Lazy<Seer> LazyInstance = new(() => new Seer());
        private bool _started;
        private readonly Dictionary<int, Game> _games;
        private Seer()
        {
            _games = new Dictionary<int, Game>();
        }

        public static Seer Instance => LazyInstance.Value;

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
            if (_started) return;
            StartEventsHandler();
            StartProcessWatcher();
            _started = true;
        }

        /// <summary>
        /// Stop Seer monitoring.
        /// </summary>
        public void Stop()
        {
            if (!_started) return;
            StopProcessWatcher();
            StopEventsHandler();
            foreach (var game in _games.Values) game?.Dispose();
            _games.Clear();
            _started = false;
        }

        ~Seer() => Dispose();
        public void Dispose()
        {
            Stop();
            MachinaManager.Instance.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
