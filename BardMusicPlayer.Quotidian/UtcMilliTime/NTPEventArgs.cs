/*
 * Copyright(c) 2019 John Kusumi
 * Licensed under the MIT license. See https://github.com/JPKusumi/UtcMilliTime/blob/master/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Quotidian.UtcMilliTime
{
    public class NTPEventArgs : EventArgs
    {
        public string Server { get; }
        public long Latency { get; }
        public long Skew { get; }
        public NTPEventArgs(string server, long latency, long skew)
        {
            Server = server;
            Latency = latency;
            Skew = skew;
        }
    }
}
