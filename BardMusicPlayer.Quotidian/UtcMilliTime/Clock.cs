/*
 * Copyright(c) 2019 John Kusumi
 * Licensed under the MIT license. See https://github.com/JPKusumi/UtcMilliTime/blob/master/LICENSE for full license information.
 */

using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace BardMusicPlayer.Quotidian.UtcMilliTime
{
    public sealed class Clock : ITime
    {
        private static readonly Lazy<Clock> instance = new(() => new Clock());
        public static Clock Time => instance.Value;
        [System.Runtime.InteropServices.DllImport("kernel32")]
        static extern ulong GetTickCount64();
        private static bool successfully_synced;
        private static bool Indicated => !successfully_synced && NetworkInterface.GetIsNetworkAvailable();
        private static long device_uptime => (long)GetTickCount64();
        private static long device_boot_time;
        private static NTPCallState ntpCall;
        public bool Initialized => device_boot_time != 0;
        public bool Synchronized => successfully_synced;
        public long DeviceBootTime => device_boot_time;
        public long DeviceUpTime => device_uptime;
        public long DeviceUtcNow => GetDeviceTime();
        public long Now => device_boot_time + device_uptime;
        public long Skew { get; private set; }
        public string DefaultServer { get; set; } = Constants.fallback_server;
        public event EventHandler<NTPEventArgs> NetworkTimeAcquired;
        private Clock()
        {
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
            if (Indicated) SelfUpdateAsync().SafeFireAndForget(false); else Initialize();
        }
        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (!Indicated) return;
            SelfUpdateAsync().SafeFireAndForget(false);
            NetworkChange.NetworkAvailabilityChanged -= NetworkChange_NetworkAvailabilityChanged;
        }
        private void Initialize()
        {
            device_boot_time = GetDeviceTime() - device_uptime;
            successfully_synced = false;
            Skew = 0;
        }
        private static long GetDeviceTime() => DateTime.UtcNow.Ticks / Constants.dotnet_ticks_per_millisecond - Constants.dotnet_to_unix_milliseconds;
        public async Task SelfUpdateAsync(string ntpServerHostName = Constants.fallback_server)
        {
            try
            {
                if (ntpCall != null) return;
                ntpCall = new NTPCallState
                {
                    priorSyncState = successfully_synced
                };
                Initialize();
                if (!Initialized || !Indicated)
                {
                    ntpCall.OrderlyShutdown();
                    ntpCall = null;
                    return;
                }
                if (ntpServerHostName == Constants.fallback_server && !string.IsNullOrEmpty(DefaultServer)) ntpServerHostName = DefaultServer;
                ntpCall.serverResolved = ntpServerHostName;
                var addresses = await Dns.GetHostAddressesAsync(ntpServerHostName);
                var ipEndPoint = new IPEndPoint(addresses[0], Constants.udp_port_number);
                ntpCall.socket.BeginConnect(ipEndPoint, new AsyncCallback(PartB), null);
                ntpCall.methodsCompleted += 1;
            }
            catch (Exception)
            {
                ntpCall.OrderlyShutdown();
                ntpCall = null;
            }
        }
        private static void PartB(IAsyncResult ar)
        {
            try
            {
                ntpCall.socket.EndConnect(ar);
                ntpCall.socket.ReceiveTimeout = Constants.three_seconds;
                ntpCall.timer = Stopwatch.StartNew();
                ntpCall.socket.BeginSend(ntpCall.buffer, 0, Constants.bytes_per_buffer, 0, new AsyncCallback(PartC), null);
                ntpCall.methodsCompleted += 1;
            }
            catch (Exception)
            {
                ntpCall?.OrderlyShutdown();
                ntpCall = null;
            }
        }
        private static void PartC(IAsyncResult ar)
        {
            try
            {
                ntpCall.socket.EndSend(ar);
                ntpCall.socket.BeginReceive(ntpCall.buffer, 0, Constants.bytes_per_buffer, 0, new AsyncCallback(PartD), null);
                ntpCall.methodsCompleted += 1;
            }
            catch (Exception)
            {
                ntpCall?.OrderlyShutdown();
                ntpCall = null;
            }
        }
        private static void PartD(IAsyncResult ar)
        {
            try
            {
                ntpCall.socket.EndReceive(ar);
                ntpCall.timer.Stop();
                long halfRoundTrip = ntpCall.timer.ElapsedMilliseconds / 2;
                const byte serverReplyTime = 40;
                ulong intPart = BitConverter.ToUInt32(ntpCall.buffer, serverReplyTime);
                ulong fractPart = BitConverter.ToUInt32(ntpCall.buffer, serverReplyTime + 4);
                intPart = SwapEndianness(intPart);
                fractPart = SwapEndianness(fractPart);
                var milliseconds = intPart * 1000 + fractPart * 1000 / 0x100000000L;
                long timeNow = (long)milliseconds - Constants.ntp_to_unix_milliseconds + halfRoundTrip;
                if (timeNow <= 0) return;
                device_boot_time = timeNow - device_uptime;
                instance.Value.Skew = timeNow - GetDeviceTime();
                ntpCall.methodsCompleted += 1;
                successfully_synced = ntpCall.methodsCompleted == 4;
                ntpCall.latency.Stop();
                if (successfully_synced && !ntpCall.priorSyncState && instance.Value.NetworkTimeAcquired != null)
                {
                    NTPEventArgs args = new NTPEventArgs(ntpCall.serverResolved, ntpCall.latency.ElapsedMilliseconds, instance.Value.Skew);
                    instance.Value.NetworkTimeAcquired.Invoke(new object(), args);
                }
            }
            catch (Exception) { } // blank intentionally; documentation says "fail silently. Check the Time.Synchronized boolean property for the outcome."
            finally
            {
                ntpCall?.OrderlyShutdown();
                ntpCall = null;
            }
        }
        private static uint SwapEndianness(ulong x) => (uint)(((x & 0x000000ff) << 24) +
            ((x & 0x0000ff00) << 8) +
            ((x & 0x00ff0000) >> 8) +
            ((x & 0xff000000) >> 24));
    }
}
