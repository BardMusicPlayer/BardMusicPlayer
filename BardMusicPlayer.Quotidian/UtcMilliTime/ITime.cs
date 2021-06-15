/*
 * Copyright(c) 2019 John Kusumi
 * Licensed under the MIT license. See https://github.com/JPKusumi/UtcMilliTime/blob/master/LICENSE for full license information.
 */

using System;
using System.Threading.Tasks;

namespace BardMusicPlayer.Quotidian.UtcMilliTime
{
    public interface ITime
    {
        string DefaultServer { get; set; }
        long DeviceBootTime { get; }
        long DeviceUpTime { get; }
        long DeviceUtcNow { get; }
        bool Initialized { get; }
        long Now { get; }
        long Skew { get; }
        bool Synchronized { get; }

        event EventHandler<NTPEventArgs> NetworkTimeAcquired;

        Task SelfUpdateAsync(string ntpServerHostName = Constants.fallback_server);
    }
}