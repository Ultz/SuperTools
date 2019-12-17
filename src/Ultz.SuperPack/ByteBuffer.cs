// Original Author: Jb Evain
// License: MIT
// Link: https://github.com/jbevain/mono.reflection

using System;
using System.Runtime.Serialization;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace Ultz.SuperPack
{
    internal class ByteBuffer
    {
        internal byte[] Buffer;
        internal int Position;

        public ByteBuffer(byte[] buffer)
        {
            Buffer = buffer;
        }

        public byte ReadByte()
        {
            CheckCanRead(1);
            return Buffer[Position++];
        }

        public byte[] ReadBytes(int length)
        {
            CheckCanRead(length);
            var value = new byte [length];
            System.Buffer.BlockCopy(Buffer, Position, value, 0, length);
            Position += length;
            return value;
        }
        public uint ReadCompressedUInt32 ()
        {
            byte first = ReadByte ();
            if ((first & 0x80) == 0)
                return first;

            if ((first & 0x40) == 0)
                return ((uint) (first & ~0x80) << 8)
                       | ReadByte ();

            return ((uint) (first & ~0xc0) << 24)
                   | (uint) ReadByte () << 16
                   | (uint) ReadByte () << 8
                   | ReadByte ();
        }

        public int ReadCompressedInt32 ()
        {
            var b = Buffer[Position];
            var u = (int) ReadCompressedUInt32 ();
            var v = u >> 1;
            if ((u & 1) == 0)
                return v;

            switch (b & 0xc0)
            {
                case 0:
                case 0x40:
                    return v - 0x40;
                case 0x80:
                    return v - 0x2000;
                default:
                    return v - 0x10000000;
            }
        }

        

        public short ReadInt16()
        {
            CheckCanRead(2);
            var value = (short) (Buffer[Position]
                                 | (Buffer[Position + 1] << 8));
            Position += 2;
            return value;
        }

        public int ReadInt32()
        {
            CheckCanRead(4);
            var value = Buffer[Position]
                        | (Buffer[Position + 1] << 8)
                        | (Buffer[Position + 2] << 16)
                        | (Buffer[Position + 3] << 24);
            Position += 4;
            return value;
        }

        public long ReadInt64()
        {
            CheckCanRead(8);
            var low = (uint) (Buffer[Position]
                              | (Buffer[Position + 1] << 8)
                              | (Buffer[Position + 2] << 16)
                              | (Buffer[Position + 3] << 24));

            var high = (uint) (Buffer[Position + 4]
                               | (Buffer[Position + 5] << 8)
                               | (Buffer[Position + 6] << 16)
                               | (Buffer[Position + 7] << 24));

            var value = ((long) high << 32) | low;
            Position += 8;
            return value;
        }

        public float ReadSingle()
        {
            if (!BitConverter.IsLittleEndian)
            {
                var bytes = ReadBytes(4);
                Array.Reverse(bytes);
                return BitConverter.ToSingle(bytes, 0);
            }

            CheckCanRead(4);
            var value = BitConverter.ToSingle(Buffer, Position);
            Position += 4;
            return value;
        }

        public double ReadDouble()
        {
            if (!BitConverter.IsLittleEndian)
            {
                var bytes = ReadBytes(8);
                Array.Reverse(bytes);
                return BitConverter.ToDouble(bytes, 0);
            }

            CheckCanRead(8);
            var value = BitConverter.ToDouble(Buffer, Position);
            Position += 8;
            return value;
        }

        private void CheckCanRead(int count)
        {
            if (Position + count > Buffer.Length)
                throw new ArgumentOutOfRangeException();
        }
    }
}