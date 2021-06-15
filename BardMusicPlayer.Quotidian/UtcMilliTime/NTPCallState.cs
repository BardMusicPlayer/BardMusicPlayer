/*
 * Copyright(c) 2019 John Kusumi
 * Licensed under the MIT license. See https://github.com/JPKusumi/UtcMilliTime/blob/master/LICENSE for full license information.
 */

using System.Diagnostics;
using System.Net.Sockets;

namespace BardMusicPlayer.Quotidian.UtcMilliTime
{
    public class NTPCallState
    {
        public bool priorSyncState;
        public byte[] buffer = new byte[Constants.bytes_per_buffer];
        public short methodsCompleted;
        public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public Stopwatch latency;
        public Stopwatch timer;
        public string serverResolved;
        public NTPCallState()
        {
            latency = Stopwatch.StartNew();
            buffer[0] = 0x1B;
        }
        public void OrderlyShutdown()
        {
            if (timer != null)
            {
                if (timer.IsRunning) timer.Stop();
                timer = null;
            }
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket = null;
            if (latency != null)
            {
                if (latency.IsRunning) latency.Stop();
                latency = null;
            }
        }
    }
}
