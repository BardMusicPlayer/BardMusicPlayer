/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using BardMusicPlayer.Grunt.Helper.Dalamud;

namespace BardMusicPlayer.Grunt
{
    public class Grunt
    {
        private static readonly Lazy<Grunt> LazyInstance = new(() => new Grunt());
        private bool _started;

        private DalamudServer _dalamudServer;

        private Grunt()
        {
        }

        public static Grunt Instance => LazyInstance.Value;

        /// <summary>
        /// Start Grunt.
        /// </summary>
        public void Start()
        {
            if (_started) return;
            _dalamudServer = new DalamudServer();
            _started = true;
        }

        /// <summary>
        /// Stop Grunt.
        /// </summary>
        public void Stop()
        {
            if (!_started) return;
            _dalamudServer?.Dispose();
            _dalamudServer = null;
            _started = false;
        }

        ~Grunt() => Dispose();
        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}
