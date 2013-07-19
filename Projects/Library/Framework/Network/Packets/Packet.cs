/*
 * Copyright (C) 2012-2013 Arctium Emulation <http://arctium.org>
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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Framework.Constans;

namespace Framework.Network.Packets
{
    public class Packet
    {
        public PacketHeader Header { get; set; }
        public byte[] DataBuffer { get; set; }

        object stream;

        // Needs to be global since the bit reader is working with the
        // previous & next byte
        int shiftedBits = 0;
        byte preByte = 0;

        public Packet(byte[] data)
        {
            if (data.Length >= 4)
            {
                var offset = 0;

                stream = new BinaryReader(new MemoryStream(data));

                Header = new PacketHeader();

                Header.Size = Read<uint>(24);
                Header.Message = Read<ushort>(11);

                if (Header.Message == (ushort)ClientMessage.MultiPacket)
                {
                    Header.Size = Read<uint>(24);
                    Header.Message = Read<ushort>();

                    offset = 5;
                }

                DataBuffer = new byte[Header.Size];
                Buffer.BlockCopy(data, offset, DataBuffer, 0, (int)Header.Size);
            }
        }

        public Packet(ServerMessage message, bool old = false)
        {
            stream = new BinaryWriter(new MemoryStream());

            Header = new PacketHeader();
            Header.Message = (ushort)message;

            if (old)
            {
                WriteDefault<ushort>(4);
                WriteDefault<ushort>(BitConverter.ToUInt16(BitConverter.GetBytes((ushort)message).Reverse().ToArray(), 0));
            }
            else
            {
                Write<uint>(4, true, 24);
                Write<ushort>((ushort)message, true, 11);
            }
        }

        public byte[] GetDataToSend()
        {
            var writer = stream as BinaryWriter;

            if (writer == null)
                throw new InvalidOperationException("");

            Header.Size = (ushort)writer.BaseStream.Length;

            writer.BaseStream.Seek(0, SeekOrigin.Begin);
            writer.Write((ushort)Header.Size);

            var data = new byte[Header.Size];
            writer.Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < Header.Size; i++)
                data[i] = (byte)writer.BaseStream.ReadByte();

            return DataBuffer = data;
        }

        public T Read<T>(byte bits = 0, bool addBits = true)
        {
            var reader = stream as BinaryReader;

            if (reader == null)
                throw new InvalidOperationException("");

            if (!addBits)
                return reader.Read<T>();

            var ret = default(T);

            if (bits != 0)
            {
                ret = AddBits<T>(bits);

                return (T)Convert.ChangeType(ret, typeof(T));
            }

            switch (typeof(T).Name)
            {
                case "SByte":
                case "Byte":
                    ret = AddBits<T>(0x08);
                    break;
                case "Int16":
                case "UInt16":
                    ret = AddBits<T>(0x10);
                    break;
                case "Int32":
                case "UInt32":
                case "Single":
                    ret = AddBits<T>(0x20);
                    break;
                case "Int64":
                case "UInt64":
                    ret = AddBits<T>(0x40);
                    break;
                default:
                    break;
            }

            return (T)Convert.ChangeType(ret, typeof(T));
        }

        T AddBits<T>(int bits)
        {
            var reader = stream as BinaryReader;

            if (reader == null)
                throw new InvalidOperationException("");

            ulong unpackedValue = 0;
            var packedByte = Read<byte>(8, false);
            var bitsToRead = 0;
            var readBitCT = 0;
            var count = 0;

            while (readBitCT < bits)
            {
                bitsToRead = 8 - shiftedBits;

                if (bitsToRead > bits - readBitCT)
                    bitsToRead = bits - readBitCT;

                var valuePart = (uint)((((1 << bitsToRead) - 1) & (packedByte >> (byte)shiftedBits)) << count);

                readBitCT = bitsToRead + count;
                count += bitsToRead;

                unpackedValue |= valuePart;

                shiftedBits = (bitsToRead + shiftedBits) & 7;

                if (shiftedBits == 0)
                    packedByte = Read<byte>(8, false);
            }

            reader.BaseStream.Position -= 1;

            return (T)Convert.ChangeType(unpackedValue, typeof(T));
        }

        public byte[] Read(int count)
        {
            var reader = stream as BinaryReader;

            if (reader == null)
                throw new InvalidOperationException("");

            return reader.ReadBytes(count);
        }

        public string ReadString()
        {
            var s = "";
            var length = Read<byte>() >> 1;

            for (int i = 0; i < length; i++)
                s += Convert.ToChar(Read<ushort>());

            return s;
        }

        public void WriteDefault<T>(T value)
        {
            var writer = stream as BinaryWriter;

            if (writer == null)
                throw new InvalidOperationException("");

            if (typeof(T) != typeof(string))
                shiftedBits = 0;

            switch (value.GetType().Name)
            {
                case "SByte":
                    writer.Write(Convert.ToSByte(value));
                    break;
                case "Byte":
                    writer.Write(Convert.ToByte(value));
                    break;
                case "Int16":
                    writer.Write(Convert.ToInt16(value));
                    break;
                case "UInt16":
                    writer.Write(Convert.ToUInt16(value));
                    break;
                case "Int32":
                    writer.Write(Convert.ToInt32(value));
                    break;
                case "UInt32":
                    writer.Write(Convert.ToUInt32(value));
                    break;
                case "Int64":
                    writer.Write(Convert.ToInt64(value));
                    break;
                case "UInt64":
                    writer.Write(Convert.ToUInt64(value));
                    break;
                case "Single":
                    writer.Write(Convert.ToSingle(value));
                    break;
                case "Byte[]":
                    writer.Write(value as byte[]);
                    break;
                case "String":
                    WriteString(value as string);
                    break;
                default:
                    break;
            }
        }

        public void Write<T>(T value, bool bla = true, int bits = 0)
        {
            var writer = stream as BinaryWriter;

            if (writer == null)
                throw new InvalidOperationException("");

            if (bits != 0)
            {
                var data = Convert.ToUInt32(value);

                WriteBits<T>(data, bits);

                return;
            }

            switch (typeof(T).Name)
            {
                case "SByte":
                case "Byte":
                    ulong val = Convert.ToByte(value);

                    WriteBits<byte>(val, 0x08, false);
                    break;
                case "Int16":
                case "UInt16":
                    val = Convert.ToUInt16(value);

                    WriteBits<ushort>(val, 0x10);
                    break;
                case "Int32":
                case "UInt32":
                    val = Convert.ToUInt32(value);

                    WriteBits<uint>(val, 0x20);
                    break;
                case "Single":
                    WriteDefault<T>(value);
                    break;
                case "Int64":
                case "UInt64":
                    val = Convert.ToUInt64(value);

                    WriteBits<ulong>(val, 0x40, false);
                    break;
                case "String":
                    WriteString(value as string);
                    break;
                default:
                    break;
            }
        }

        void WriteBits<T>(ulong value, int bits, bool cutLastByte = true)
        {
            var writer = stream as BinaryWriter;

            if (writer == null)
                throw new InvalidOperationException("");

            var unpackedValue = value;
            var packedByte = (byte)0;

            if (shiftedBits != 0)
                bits -= unpackedValue == 0 && cutLastByte ? 8 : 0;

            while (bits != 0)
            {
                var bitsToWrite = 8 - shiftedBits;

                if (bitsToWrite > bits)
                    bitsToWrite = bits;

                if (shiftedBits != 0)
                    packedByte |= (byte)((unpackedValue & (ulong)((1 << bitsToWrite) - 1)) << shiftedBits);
                else
                    packedByte = (byte)(unpackedValue & (ulong)((1 << bitsToWrite) - 1));

                packedByte |= preByte;
                preByte = 0;

                writer.Write(packedByte);

                unpackedValue >>= bitsToWrite;
                bits -= bitsToWrite;

                shiftedBits = (bitsToWrite + shiftedBits) & 7;

                if (shiftedBits != 0 && writer.BaseStream.Length > 0)
                    writer.BaseStream.Position -= 1;
            }

            preByte = packedByte;
        }

        void WriteString(string value)
        {
            var s = Encoding.UTF8.GetBytes(value);

            Write<byte>((byte)(value.Length << 1));

            for (int i = 0; i < s.Length; i++)
                Write<ushort>(Convert.ToChar(s[i]));
        }

        public void WriteUnicode(string value, byte mp = 2)
        {
            var length = (byte)(value.Length << 2);
            var s = UTF8Encoding.Unicode.GetBytes(value);

            WriteDefault<byte>(length);

            for (int i = 0; i < s.Length; i++)
            {
                var sB = Convert.ToByte(Convert.ToChar(s[i]));

                WriteDefault<byte>((byte)(sB << (mp - 1)));
            }
        }
    }
}
