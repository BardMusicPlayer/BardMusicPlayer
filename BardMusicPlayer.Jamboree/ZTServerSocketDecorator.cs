/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Jamboree
{
    internal sealed class ZTServerSocketDecorator : IServerSocket
    {
        private readonly PartyClient.ZeroTier.Socket socket;
        private bool disposedValue;

        internal ZTServerSocketDecorator(PartyClient.ZeroTier.Socket socket)
        {
            if (!socket.IsBound)
            {
                throw new ArgumentException("Attempt to decorate a non-listening socket as IServerSocket");
            }

            this.socket = socket;
            this.disposedValue = false;
        }

        public ISocket Accept()
        {
            ISocket ret;
            try
            {
                PartyClient.ZeroTier.Socket accepted = this.socket.Accept();
                ret = new ZTSocketDecorator(accepted);
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

        public bool IsBound()
        {
            return this.socket.IsBound;
        }

        public bool IsClosed()
        {
            return this.socket.IsClosed;
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
                    }
                    catch { }
                }

                disposedValue = true;
            }
        }
    }
}
