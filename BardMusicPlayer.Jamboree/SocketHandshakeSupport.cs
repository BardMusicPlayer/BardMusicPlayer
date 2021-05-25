/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Jamboree
{
    internal class SocketHandshakeSupport : ISerializedSocketAcceptor, ISerialziedSocketCreator
    {
        private const int ZT_SOCKET_VERSION = 1;
        private const int GUID_SIZE = 16;

        private readonly ISerializationAdapter adapter;
        private readonly Guid sessionId;

        internal SocketHandshakeSupport(ISerializationAdapter adapter, Guid sessionId)
        {
            this.adapter = adapter ?? throw new ArgumentNullException();
            this.sessionId = sessionId;
        }

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

        private static int ReadHandshakeVersion(ISocket socket)
        {
            byte[] buffer = new byte[sizeof(int)];
            Sockets.FullRead(socket, buffer, buffer.Length);

            return BitConverter.ToInt32(buffer, 0);
        }
    }
}
