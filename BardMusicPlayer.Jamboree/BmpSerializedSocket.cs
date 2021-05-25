/*
 * Copyright(c) 2021 isaki
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Jamboree
{
    internal sealed class BmpSerializedSocket : ISerializedSocket
    {
        // One MiB.
        private const int BUFFER_SIZE = 1048576;
        private const int INT_N_BYTES = sizeof(int);

        private readonly ISerializationAdapter serializer;
        private readonly ISocket socket;
        private readonly byte[] buffer;
        private volatile bool active;
        private bool disposedValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="socket"></param>
        internal BmpSerializedSocket(ISerializationAdapter serializer, ISocket socket)
        {
            this.serializer = serializer ?? throw new ArgumentNullException();
            this.socket = socket ?? throw new ArgumentNullException();
            this.buffer = new byte[BUFFER_SIZE];
            this.active = true;
        }

        /// <inheritdoc/>
        public void Close()
        {
            if (this.active)
            {
                try
                {
                    this.socket.Close();
                }
                finally
                {
                    // .NET 6.0 has this in System.Array
                    Fill<byte>(this.buffer, 0);
                    this.active = false;
                }
            }
        }

        /// <inheritdoc/>
        public bool IsClosed()
        {
            return this.socket.IsClosed();
        }

        /// <inheritdoc/>
        public bool IsConnected()
        {
            return this.socket.IsConnected();
        }

        /// <inheritdoc/>
        public T Read<T>()
        {
            if (!this.active)
            {
                throw new SocketException("Attempt to read from inactive serialized socket");
            }

            this.SocketRead(INT_N_BYTES);
            int dataLength = BitConverter.ToInt32(this.buffer, 0);

            if (dataLength > BUFFER_SIZE || dataLength < 0)
            {
                throw new SocketException("Read buffer violation in socket stream; invalid data length");
            }

            this.SocketRead(dataLength);
            return this.serializer.Decode<T>(this.buffer, 0, dataLength);
        }

        /// <inheritdoc/>
        public void Write(object obj)
        {
            if (!this.active)
            {
                throw new SocketException("Attempt to write to inactive serialized socket");
            }

            byte[] data = this.serializer.Encode(obj);

            if (data.Length > BUFFER_SIZE)
            {
                throw new ArgumentOutOfRangeException("Encoded object is too large to send");
            }

            byte[] length = BitConverter.GetBytes(data.Length);
            this.SocketWrite(length);
            this.SocketWrite(data);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// VS2019 Generated method for Dispose pattern.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        this.Close();
                    }
                    catch (Exception)
                    {
                        // Nothing we can really do here.
                    }
                    finally
                    {
                        this.socket.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Helper method that adheres to the BSD socket API standard.
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="SocketException"></exception>
        private void SocketWrite(byte[] data)
        {
            Sockets.FullWrite(this.socket, data);
        }

        /// <summary>
        /// Helper method that adheres to the BSD socket API standard.
        /// </summary>
        /// <param name="dataLength"></param>
        /// <exception cref="SocketException"></exception>
        private void SocketRead(int dataLength)
        {
            Sockets.FullRead(this.socket, this.buffer, dataLength);
        }

        /// <summary>
        /// Fill an array with the specified value.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        private static void Fill<T>(T[] array, T value)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                array[i] = value;
            }
        }
    }
}
