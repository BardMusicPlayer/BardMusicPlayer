/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Jamboree
{
    internal sealed class ZTSocketHandshakeSupport : ISerializedSocketAcceptor, ISerialziedSocketCreator
    {
        private const int ZT_SOCKET_VERSION = 1;
        private const int GUID_SIZE = 16;

        private readonly ISerializationAdapter adapter;
        private readonly Guid sessionId;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="sessionId"></param>
        internal ZTSocketHandshakeSupport(ISerializationAdapter adapter, Guid sessionId)
        {
            this.adapter = adapter ?? throw new ArgumentNullException();
            this.sessionId = sessionId;
        }

        /// <summary>
        /// Socket acceptance method.
        /// </summary>
        /// <param name="accepted"></param>
        /// <returns></returns>
        public ISerializedSocket Accept(ISocket accepted)
        {
            int version = ReadHandshakeVersion(accepted);

            ISerializedSocket ret;
            if (version == ZT_SOCKET_VERSION)
            {
                byte[] buffer = new byte[GUID_SIZE];
                Sockets.FullRead(accepted, buffer, GUID_SIZE);

                Guid sentId = new(buffer);
                if (this.sessionId.Equals(sentId))
                {
                    Sockets.FullWrite(accepted, BitConverter.GetBytes(ZT_SOCKET_VERSION));
                    ret = new BmpSerializedSocket(this.adapter, accepted);
                }
                else
                {
                    ret = null;
                }
            }
            else
            {
                Sockets.FullWrite(accepted, BitConverter.GetBytes(ZT_SOCKET_VERSION));
                ret = null;
            }

            return ret;
        }

        /// <summary>
        /// Socket creation method.
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public ISerializedSocket Create(ISocket socket)
        {
            // Write out the handshake.
            Sockets.FullWrite(socket, BitConverter.GetBytes(ZT_SOCKET_VERSION));
            Sockets.FullWrite(socket, this.sessionId.ToByteArray());

            // Read the response.
            int version = ReadHandshakeVersion(socket);
            if (version != ZT_SOCKET_VERSION)
            {
                throw new SocketException("Handshake version mismatch");
            }

            return new BmpSerializedSocket(this.adapter, socket);
        } 

        /// <summary>
        /// Internal helper method for reading the version.
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        private static int ReadHandshakeVersion(ISocket socket)
        {
            byte[] buffer = new byte[sizeof(int)];
            Sockets.FullRead(socket, buffer, buffer.Length);

            return BitConverter.ToInt32(buffer, 0);
        }
    }
}
