/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Threading;

namespace BardMusicPlayer.Jamboree
{
    internal sealed class ZTNodeManager
    {
        private static readonly Lazy<ZTNodeManager> INSTANCE = new(() => new ZTNodeManager());

        // This is meant to be used atomically via the Interlocked class.
        private int SocketCount;

        internal ZTNodeManager()
        {
            this.SocketCount = 0;
        }

        /// <summary>
        /// Returns the singleton instance of this class.
        /// </summary>
        static ZTNodeManager Instance
        {
            get
            {
                return INSTANCE.Value;
            }
        }

        private int GetAndIncrementCount()
        {
            return Interlocked.Increment(ref this.SocketCount) - 1;
        }

        private int DecrementAndGetCount()
        {
            return Interlocked.Decrement(ref this.SocketCount);
        }
    }
}
 