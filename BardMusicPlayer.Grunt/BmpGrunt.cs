/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using BardMusicPlayer.Config;
using BardMusicPlayer.Grunt.Helper.Dalamud;
using BardMusicPlayer.Seer;

namespace BardMusicPlayer.Grunt
{
    public class BmpGrunt
    {
        private static readonly Lazy<BmpGrunt> LazyInstance = new(() => new BmpGrunt());

        /// <summary>
        /// 
        /// </summary>
        public bool Started { get; private set; }

        internal DalamudServer DalamudServer;

        private BmpGrunt()
        {
        }

        public static BmpGrunt Instance => LazyInstance.Value;

        /// <summary>
        /// Start Grunt.
        /// </summary>
        public void Start()
        {
            if (Started) return;
            if (!BmpConfig.Initialized) throw new BmpGruntException("Grunt requires Config to be initialized.");
            if (!BmpSeer.Instance.Started) throw new BmpGruntException("Grunt requires Seer to be running.");
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

        ~BmpGrunt() => Dispose();
        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}
