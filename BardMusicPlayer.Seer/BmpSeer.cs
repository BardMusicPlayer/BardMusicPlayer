/*
 * Copyright(c) 2021 MoogleTroupe, trotlinebeercan
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using WindowsFirewallHelper;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer.Events;
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

        private readonly ConcurrentDictionary<int, Game> _games;

        private BmpSeer() { _games = new ConcurrentDictionary<int, Game>(); }

        public static BmpSeer Instance => LazyInstance.Value;

        /// <summary>
        /// Current active games
        /// </summary>
        public IReadOnlyDictionary<int, Game> Games => new ReadOnlyDictionary<int, Game>(_games);

        /// <summary>
        /// Configure the firewall for Machina
        /// </summary>
        /// <param name="appName">This application name.</param>
        /// <returns>true if successful</returns>
        public bool SetupFirewall(string appName)
        {
            try
            {
                if (!FirewallManager.IsServiceRunning) return true;

                if (FirewallManager.Instance.Rules.Any(x => x.Name != null && x.Name.Equals(appName)))
                {
                    FirewallManager.Instance.Rules.Remove(
                        FirewallManager.Instance.Rules.First(x => x.Name.Equals(appName)));
                }

                var rule = FirewallManager.Instance.CreateApplicationRule(
                    @appName,
                    FirewallAction.Allow,
                    Assembly.GetEntryAssembly()?.Location
                );

                FirewallManager.Instance.Rules.Add(rule);
                return true;
            }
            catch (Exception ex)
            {
                PublishEvent(new SeerExceptionEvent(ex));
                return false;
            }
        }

        /// <summary>
        /// Unconfigure the firewall for Machina
        /// </summary>
        /// <param name="appName">This application name.</param>
        /// <returns>true if successful</returns>
        public bool DestroyFirewall(string appName)
        {
            try
            {
                if (!FirewallManager.IsServiceRunning) return true;

                if (FirewallManager.Instance.Rules.Any(x => x.Name != null && x.Name.Equals(appName)))
                {
                    FirewallManager.Instance.Rules.Remove(
                        FirewallManager.Instance.Rules.First(x => x.Name.Equals(appName)));
                }

                return true;
            }
            catch (Exception ex)
            {
                PublishEvent(new SeerExceptionEvent(ex));
                return false;
            }
        }

        /// <summary>
        /// Start Seer monitoring.
        /// </summary>
        public void Start()
        {
            if (Started) return;

            if (!BmpPigeonhole.Initialized) throw new BmpSeerException("Seer requires Pigeonhole to be initialized.");

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

            foreach (var game in _games.Values)
            {
                game?.Dispose();
            }

            _games.Clear();

            Started = false;
        }

        ~BmpSeer() { Dispose(); }

        public void Dispose()
        {
            Stop();
            MachinaManager.Instance.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}