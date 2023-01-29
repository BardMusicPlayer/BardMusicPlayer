/*
 * Copyright (C) 2012-2020 CypherCore <http://github.com/CypherCore>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace BardMusicPlayer.Jamboree.PartyNetworking.Server_Client;

public class NetworkPacket : IDisposable
{
    public NetworkPacket()
    {
        writeStream = new BinaryWriter(new MemoryStream());
    }

    public NetworkPacket(NetworkOpcodes.OpcodeEnum opcode)
    {
        writeStream = new BinaryWriter(new MemoryStream());
        writeStream.Write((byte)opcode);
    }

    public NetworkPacket(byte[] data)
    {
        readStream = new BinaryReader(new MemoryStream(data));
        Opcode     = (NetworkOpcodes.OpcodeEnum)readStream.ReadByte();
    }

    public NetworkOpcodes.OpcodeEnum Opcode { get; } = NetworkOpcodes.OpcodeEnum.NULL_OPCODE;

    public void Dispose()
    {
        writeStream?.Dispose();

        readStream?.Dispose();
    }

    #region Read Methods
    public NetworkOpcodes.OpcodeEnum ReadOpcode()
    {
        return (NetworkOpcodes.OpcodeEnum)readStream.ReadByte();
    }

    public sbyte ReadInt8()
    {
        ResetBitPos();
        return readStream.ReadSByte();
    }

    public short ReadInt16()
    {
        ResetBitPos();
        return readStream.ReadInt16();
    }

    public int ReadInt32()
    {
        ResetBitPos();
        return readStream.ReadInt32();
    }

    public long ReadInt64()
    {
        ResetBitPos();
        return readStream.ReadInt64();
    }

    public byte ReadUInt8()
    {
        ResetBitPos();
        return readStream.ReadByte();
    }

    public ushort ReadUInt16()
    {
        ResetBitPos();
        return readStream.ReadUInt16();
    }

    public uint ReadUInt32()
    {
        ResetBitPos();
        return readStream.ReadUInt32();
    }

    public ulong ReadUInt64()
    {
        ResetBitPos();
        return readStream.ReadUInt64();
    }

    public float ReadFloat()
    {
        ResetBitPos();
        return readStream.ReadSingle();
    }

    public double ReadDouble()
    {
        ResetBitPos();
        return readStream.ReadDouble();
    }

    public string ReadCString()
    {
        ResetBitPos();
        StringBuilder tmpString = new();
        var tmpChar = readStream.ReadChar();
        var tmpEndChar = Convert.ToChar(Encoding.UTF8.GetString(new byte[] { 0 }));

        while (tmpChar != tmpEndChar)
        {
            tmpString.Append(tmpChar);
            tmpChar = readStream.ReadChar();
        }

        return tmpString.ToString();
    }

    public string ReadString(uint length)
    {
        if (length == 0)
            return "";

        ResetBitPos();
        return Encoding.UTF8.GetString(ReadBytes(length));
    }

    public bool ReadBool()
    {
        ResetBitPos();
        return readStream.ReadBoolean();
    }

    public byte[] ReadBytes(uint count)
    {
        ResetBitPos();
        return readStream.ReadBytes((int)count);
    }

    public void Skip(int count)
    {
        ResetBitPos();
        readStream.BaseStream.Position += count;
    }

    public Vector3 ReadVector3()
    {
        return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
    }

    //BitPacking
    public byte ReadBit()
    {
        if (_bitPosition == 8)
        {
            BitValue     = ReadUInt8();
            _bitPosition = 0;
        }

        int returnValue = BitValue;
        BitValue = (byte)(2 * returnValue);
        ++_bitPosition;

        return (byte)(returnValue >> 7);
    }

    public bool HasBit()
    {
        if (_bitPosition == 8)
        {
            BitValue     = ReadUInt8();
            _bitPosition = 0;
        }

        int returnValue = BitValue;
        BitValue = (byte)(2 * returnValue);
        ++_bitPosition;

        return Convert.ToBoolean(returnValue >> 7);
    }

    public T ReadBits<T>(int bitCount)
    {
        var value = 0;

        for (var i = bitCount - 1; i >= 0; --i)
            if (HasBit())
                value |= 1 << i;

        return (T)Convert.ChangeType(value, typeof(T));
    }
    #endregion

    #region Write Methods
    public void WriteOpcode(NetworkOpcodes.OpcodeEnum opcode)
    {
        writeStream.Write((byte)opcode);
    }

    public void WriteInt8(sbyte data)
    {
        FlushBits();
        writeStream.Write(data);
    }

    public void WriteInt16(short data)
    {
        FlushBits();
        writeStream.Write(data);
    }

    public void WriteInt32(int data)
    {
        FlushBits();
        writeStream.Write(data);
    }

    public void WriteInt64(long data)
    {
        FlushBits();
        writeStream.Write(data);
    }

    public void WriteUInt8(byte data)
    {
        FlushBits();
        writeStream.Write(data);
    }

    public void WriteUInt16(ushort data)
    {
        FlushBits();
        writeStream.Write(data);
    }

    public void WriteUInt32(uint data)
    {
        FlushBits();
        writeStream.Write(data);
    }

    public void WriteUInt64(ulong data)
    {
        FlushBits();
        writeStream.Write(data);
    }

    public void WriteFloat(float data)
    {
        FlushBits();
        writeStream.Write(data);
    }

    public void WriteDouble(double data)
    {
        FlushBits();
        writeStream.Write(data);
    }

    /// <summary>
    /// Writes a string to the packet with a null terminated (0)
    /// </summary>
    /// <param name="str"></param>
    public void WriteCString(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            WriteUInt8(0);
            return;
        }

        WriteString(str);
        WriteUInt8(0);
    }

    public void WriteString(string str)
    {
        if (str.Length < 1)
            return;

        var sBytes = Encoding.UTF8.GetBytes(str);
        WriteBytes(sBytes);
    }

    public void WriteBytes(byte[] data)
    {
        FlushBits();
        writeStream.Write(data, 0, data.Length);
    }

    public void WriteBytes(byte[] data, uint count)
    {
        FlushBits();
        writeStream.Write(data, 0, (int)count);
    }

    public void WriteBytes(NetworkPacket buffer)
    {
        WriteBytes(buffer.GetData());
    }

    public void WriteVector4(Vector4 pos)
    {
        WriteFloat(pos.X);
        WriteFloat(pos.Y);
        WriteFloat(pos.Z);
        WriteFloat(pos.W);
    }

    public void WriteVector3(Vector3 pos)
    {
        WriteFloat(pos.X);
        WriteFloat(pos.Y);
        WriteFloat(pos.Z);
    }

    public void WriteVector2(Vector2 pos)
    {
        WriteFloat(pos.X);
        WriteFloat(pos.Y);
    }

    public void WritePackXYZ(Vector3 pos)
    {
        uint packed = 0;
        packed |= (uint)(pos.X / 0.25f) & 0x7FF;
        packed |= ((uint)(pos.Y / 0.25f) & 0x7FF) << 11;
        packed |= ((uint)(pos.Z / 0.25f) & 0x3FF) << 22;
        WriteUInt32(packed);
    }

    public bool WriteBit(bool bit)
    {
        --_bitPosition;

        if (bit)
            BitValue |= (byte)(1 << _bitPosition);

        if (_bitPosition == 0)
        {
            writeStream.Write(BitValue);

            _bitPosition = 8;
            BitValue     = 0;
        }
        return bit;
    }

    public void WriteBits(object bit, int count)
    {
        for (var i = count - 1; i >= 0; --i)
            WriteBit(((Convert.ToUInt32(bit) >> i) & 1) != 0);
    }
    #endregion

    public bool HasUnfinishedBitPack()
    {
        return _bitPosition != 8;
    }

    public void FlushBits()
    {
        if (_bitPosition == 8)
            return;

        writeStream.Write(BitValue);
        BitValue     = 0;
        _bitPosition = 8;
    }

    public void ResetBitPos()
    {
        if (_bitPosition > 7)
            return;

        _bitPosition = 8;
        BitValue     = 0;
    }

    public byte[] GetData()
    {
        var stream = GetCurrentStream();

        var data = new byte[stream.Length];

        var pos = stream.Position;
        stream.Seek(0, SeekOrigin.Begin);
        for (var i = 0; i < data.Length; i++)
            data[i] = (byte)stream.ReadByte();

        stream.Seek(pos, SeekOrigin.Begin);
        return data;
    }

    public uint GetSize()
    {
        return (uint)GetCurrentStream().Length;
    }

    public Stream GetCurrentStream()
    {
        return writeStream != null ? writeStream.BaseStream : readStream.BaseStream;
    }

    public void Clear()
    {
        _bitPosition = 8;
        BitValue     = 0;
        writeStream  = new BinaryWriter(new MemoryStream());
    }

    private byte _bitPosition = 8;
    private byte BitValue;
    private BinaryWriter writeStream;
    private BinaryReader readStream;
}