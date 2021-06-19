﻿/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */


using BardMusicPlayer.Jamboree.PartyClient.ZeroTier;
using System;

namespace BardMusicPlayer.Jamboree
{
    internal sealed class ZTSocketDecorator : ISocket
    {
        private readonly Socket socket;
        private bool disposedValue;

        internal ZTSocketDecorator(Socket socket)
        {
            if (socket.IsBound || !socket.Connected)
            {
                throw new ArgumentException("Attempt to decorate invalid socket as ISocket");
            }

            this.socket = socket;
            this.disposedValue = false;
        }

        public void Close()
        {
            try
            {
                this.socket.Close();
            }
            catch (PartyClient.ZeroTier.SocketException e)
            {
                throw new SocketException(e.Message, e);
            }
            catch (ObjectDisposedException)
            {
                // Close should be safe to call repeatedly.
            }
        }

        public bool IsClosed()
        {
            return this.socket.IsClosed;
        }

        public bool IsConnected()
        {
            return this.socket.Connected;
        }

        public int Receive(byte[] buffer, int offset, int len)
        {
            int ret;

            try
            {
                ret = this.socket.Receive(buffer, offset, len);
            }
            catch (PartyClient.ZeroTier.SocketException e)
            {
                throw new SocketException(e.Message, e);
            }
            catch (ObjectDisposedException e)
            {
                throw new SocketException(e.Message, e);
            }

            return ret;
        }

        public int Send(byte[] data, int offset, int len)
        {
            int ret;

            try
            {
                ret = this.socket.Send(data, offset, len);
            }
            catch (PartyClient.ZeroTier.SocketException e)
            {
                throw new SocketException(e.Message, e);
            }
            catch (ObjectDisposedException e)
            {
                throw new SocketException(e.Message, e);
            }

            return ret;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        this.Close();
                    } catch { }
                }

                disposedValue = true;
            }
        }

    }
}