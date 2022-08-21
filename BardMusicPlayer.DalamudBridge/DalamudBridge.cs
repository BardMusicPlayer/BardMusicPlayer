/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using BardMusicPlayer.DalamudBridge.Helper.Dalamud;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer;

namespace BardMusicPlayer.DalamudBridge
{
    public class DalamudBridge
    {
        private static readonly Lazy<DalamudBridge> LazyInstance = new(() => new DalamudBridge());

        /// <summary>
        /// 
        /// </summary>
        public bool Started { get; private set; }

        internal DalamudServer DalamudServer;

        private DalamudBridge()
        {
        }

        public static DalamudBridge Instance => LazyInstance.Value;

        /// <summary>
        /// Start Grunt.
        /// </summary>
        public void Start()
        {
            if (Started) 
                return;
            if (!BmpSeer.Instance.Started) throw new DalamudBridgeException("DalamudBridge requires Seer to be running.");
            DalamudServer = new DalamudServer();
            Started = true;
        }

        /// <summary>
        /// Stop Grunt.
        /// </summary>
        public void Stop()
        {
            if (!Started) return;
            DalamudServer?.Dispose();
            DalamudServer = null;
            Started = false;
        }

        ~DalamudBridge() => Dispose();
        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}
