/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Maestro
{
    public partial class BmpMaestro
    {
        private static readonly Lazy<BmpMaestro> LazyInstance = new(() => new BmpMaestro());

        /// <summary>
        /// 
        /// </summary>
        public bool Started { get; private set; }

        private BmpMaestro()
        {
        }

        public static BmpMaestro Instance => LazyInstance.Value;

        /// <summary>
        /// Start Grunt.
        /// </summary>
        public void Start()
        {
            if (Started) return;
            Started = true;
        }

        /// <summary>
        /// Stop Grunt.
        /// </summary>
        public void Stop()
        {
            if (!Started) return;
            Started = false;
        }

        ~BmpMaestro() => Dispose();
        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}
