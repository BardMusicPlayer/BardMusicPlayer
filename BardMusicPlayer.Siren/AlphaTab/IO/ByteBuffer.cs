/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

#region

using System;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.IO
{
    internal sealed class ByteBuffer : IWriteable, IReadable
    {
        private byte[] _buffer;
        private int _capacity;

        private ByteBuffer()
        {
        }

        public int Length { get; private set; }

        public int Position { get; set; }

        public void Reset()
        {
            Position = 0;
        }

        public void Skip(int offset)
        {
            Position += offset;
        }

        public int ReadByte()
        {
            var n = Length - Position;
            if (n <= 0) return -1;

            return _buffer[Position++];
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            var n = Length - Position;
            if (n > count) n = count;

            switch (n)
            {
                case <= 0:
                    return 0;
                case <= 8:
                {
                    var byteCount = n;
                    while (--byteCount >= 0) buffer[offset + byteCount] = _buffer[Position + byteCount];

                    break;
                }
                default:
                    Platform.BlockCopy(_buffer, Position, buffer, offset, n);
                    break;
            }

            Position += n;

            return n;
        }

        public byte[] ReadAll()
        {
            return ToArray();
        }

        public void WriteByte(byte value)
        {
            var buffer = new byte[1];
            buffer[0] = value;
            Write(buffer, 0, 1);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            var i = Position + count;

            if (i > Length)
            {
                if (i > _capacity) EnsureCapacity(i);

                Length = i;
            }

            if (count <= 8 && buffer != _buffer)
            {
                var byteCount = count;
                while (--byteCount >= 0) _buffer[Position + byteCount] = buffer[offset + byteCount];
            }
            else
            {
                Platform.BlockCopy(buffer, offset, _buffer, Position, Math.Min(count, buffer.Length - offset));
            }

            Position = i;
        }

        public byte[] GetBuffer()
        {
            return _buffer;
        }


        public static ByteBuffer Empty()
        {
            return WithCapactiy(0);
        }

        public static ByteBuffer WithCapactiy(int capacity)
        {
            var buffer = new ByteBuffer
            {
                _buffer = new byte[capacity],
                _capacity = capacity
            };
            return buffer;
        }

        public static ByteBuffer FromBuffer(byte[] data)
        {
            var buffer = new ByteBuffer
            {
                _buffer = data
            };
            buffer._capacity = buffer.Length = data.Length;
            return buffer;
        }

        private void SetCapacity(int value)
        {
            if (value == _capacity) return;

            if (value > 0)
            {
                var newBuffer = new byte[value];
                if (Length > 0) Platform.BlockCopy(_buffer, 0, newBuffer, 0, Length);

                _buffer = newBuffer;
            }
            else
            {
                _buffer = null;
            }

            _capacity = value;
        }

        private void EnsureCapacity(int value)
        {
            if (value <= _capacity) return;

            var newCapacity = value;
            if (newCapacity < 256) newCapacity = 256;

            if (newCapacity < _capacity * 2) newCapacity = _capacity * 2;

            SetCapacity(newCapacity);
        }

        public byte[] ToArray()
        {
            var copy = new byte[Length];
            Platform.BlockCopy(_buffer, 0, copy, 0, Length);
            return copy;
        }
    }
}