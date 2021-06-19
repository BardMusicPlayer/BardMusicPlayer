/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

/// <summary>
/// This class exists to provide convenience methods for guaranteed full writes and reads.
/// </summary>
namespace BardMusicPlayer.Jamboree
{
    internal static class Sockets
    {
        /// <summary>
        /// Helper method that adheres to the BSD socket API standard.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        /// <exception cref="SocketException"></exception>
        internal static void FullWrite(ISocket socket, byte[] data)
        {
            int written = 0;
            int tmp;
            while (written < data.Length)
            {
                tmp = socket.Send(data, written, data.Length - written);
                if (tmp < 0)
                {
                    throw new SocketException("EOF hit during socket write");
                }

                written += tmp;
            }
        }

        /// <summary>
        /// Helper method that adheres to the BSD socket API standard. Note that the state of the buffer is undefined if an exception is thrown.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer"></param>
        /// <param name="dataLength"></param>
        /// <exception cref="SocketException"></exception>
        internal static void FullRead(ISocket socket, byte[] buffer, int dataLength)
        {
            int read = 0;
            int tmp;
            while (read < dataLength)
            {
                tmp = socket.Receive(buffer, read, dataLength - read);
                if (tmp < 0)
                {
                    throw new SocketException("EOF hit during socket read");
                }

                read += tmp;
            }
        }
    }
}
