﻿namespace HeavyModManager.Classes
{
    public enum Endianness
    {
        Little,
        Big
    }

    public class EndianBinaryReader : BinaryReader
    {
        public readonly Endianness endianness;

        public EndianBinaryReader(Stream stream, Endianness endianness) : base(stream)
        {
            this.endianness = endianness;
        }

        public override float ReadSingle()
        {
            if (endianness == Endianness.Little)
                return base.ReadSingle();
            return BitConverter.ToSingle(ReadReverse4(), 0);
        }

        public override short ReadInt16()
        {
            if (endianness == Endianness.Little)
                return base.ReadInt16();
            return BitConverter.ToInt16(ReadReverse2(), 0);
        }

        public override int ReadInt32()
        {
            if (endianness == Endianness.Little)
                return base.ReadInt32();
            return BitConverter.ToInt32(ReadReverse4(), 0);
        }

        public override ushort ReadUInt16()
        {
            if (endianness == Endianness.Little)
                return base.ReadUInt16();
            return BitConverter.ToUInt16(ReadReverse2(), 0);
        }

        public override uint ReadUInt32()
        {
            if (endianness == Endianness.Little)
                return base.ReadUInt32();
            return BitConverter.ToUInt32(ReadReverse4(), 0);
        }

        private byte[] ReadReverse2()
        {
            var b0 = base.ReadByte();
            var b1 = base.ReadByte();
            return new byte[2] { b1, b0 };
        }

        private byte[] ReadReverse4()
        {
            var b0 = base.ReadByte();
            var b1 = base.ReadByte();
            var b2 = base.ReadByte();
            var b3 = base.ReadByte();
            return new byte[4] { b3, b2, b1, b0 };
        }

        public override string ReadString()
        {
            var chars = new List<char>();
            do
                chars.Add((char)ReadByte());
            while (chars.Last() != '\0');
            chars.Remove('\0');

            return new string(chars.ToArray());
        }

        public bool EndOfStream => BaseStream.Position == BaseStream.Length;
    }

    public class EndianBinaryWriter : BinaryWriter
    {
        public Endianness endianness;

        public EndianBinaryWriter(Endianness endianness) : base(new MemoryStream())
        {
            this.endianness = endianness;
        }

        public byte[] ToArray() => ((MemoryStream)BaseStream).ToArray();

        public override void Write(float f)
        {
            if (endianness == Endianness.Little)
                base.Write(f);
            else
                WriteReverse4(BitConverter.GetBytes(f));
        }

        public override void Write(int f)
        {
            if (endianness == Endianness.Little)
                base.Write(f);
            else
                WriteReverse4(BitConverter.GetBytes(f));
        }

        public override void Write(short f)
        {
            if (endianness == Endianness.Little)
                base.Write(f);
            else
                WriteReverse2(BitConverter.GetBytes(f));
        }

        public override void Write(uint f)
        {
            if (endianness == Endianness.Little)
                base.Write(f);
            else
                WriteReverse4(BitConverter.GetBytes(f));
        }

        public override void Write(ushort f)
        {
            if (endianness == Endianness.Little)
                base.Write(f);
            else
                WriteReverse2(BitConverter.GetBytes(f));
        }

        private void WriteReverse4(byte[] bytes)
        {
            base.Write(bytes[3]);
            base.Write(bytes[2]);
            base.Write(bytes[1]);
            base.Write(bytes[0]);
        }

        private void WriteReverse2(byte[] bytes)
        {
            base.Write(bytes[1]);
            base.Write(bytes[0]);
        }

        public override void Write(string f)
        {
            foreach (byte c in System.Text.Encoding.GetEncoding(1252).GetBytes(f))
                Write(c);
        }

        public void WriteMagic(string magic)
        {
            if (magic.Length != 4)
                throw new ArgumentException("Magic word must have 4 characters");
            var chars = magic.ToCharArray();
            Write(endianness == Endianness.Little ? chars : chars.Reverse().ToArray());
        }

        public void WritePaddedString(string text, int count)
        {
            for (int i = 0; i < count; i++)
                Write((byte)(i < text.Length ? text[i] : 0));
        }
    }
}
