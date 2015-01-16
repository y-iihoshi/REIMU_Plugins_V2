//-----------------------------------------------------------------------
// <copyright file="ThReplay.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Text;

namespace ReimuPlugins.Common
{
    public class ThReplay
    {
        protected Replay replay;

        protected UserInfo userInfo0;

        protected UserInfo userInfo1;

        public ThReplay()
        {
        }

        public void Read(Stream input)
        {
            var reader = new BinaryReader(input);
            replay.ReadFrom(reader);
            userInfo0.ReadFrom(reader);
            userInfo1.ReadFrom(reader);
        }

        public void Write(Stream output)
        {
            var writer = new BinaryWriter(output);
            replay.WriteTo(writer);
            userInfo0.WriteTo(writer);
            userInfo1.WriteTo(writer);
            writer.Flush();
        }

        public string Info
        {
            get
            {
                return this.userInfo0.DataString;
            }
        }

        public string Comment
        {
            get
            {
                return this.userInfo1.DataString;
            }

            set
            {
                this.userInfo1.DataString = value;
            }
        }

        protected class Replay
        {
            public Replay()
            {
            }

            public void ReadFrom(BinaryReader reader)
            {
                this.Signature = reader.ReadBytes(4);
                this.Unknown1 = reader.ReadBytes(8);
                this.HeaderSize = reader.ReadInt32();
                this.Unknown2 = reader.ReadBytes(12);
                var dataSize = reader.ReadInt32();
                this.Unknown3 = reader.ReadBytes(4);
                this.Data = reader.ReadBytes(dataSize);
            }

            public void WriteTo(BinaryWriter writer)
            {
                writer.Write(this.Signature);
                writer.Write(this.Unknown1);
                writer.Write(this.HeaderSize);
                writer.Write(this.Unknown2);
                writer.Write(this.Data.Length);
                writer.Write(this.Unknown3);
                writer.Write(this.Data);
            }

            public byte[] Signature { get; private set; }

            public byte[] Unknown1 { get; private set; }

            public int HeaderSize { get; private set; }

            public byte[] Unknown2 { get; private set; }

            public byte[] Unknown3 { get; private set; }

            public byte[] Data { get; private set; }
        }

        protected class UserInfo
        {
            public UserInfo()
            {
            }

            public void ReadFrom(BinaryReader reader)
            {
                this.Signature = reader.ReadBytes(4);
                var size = reader.ReadInt32();
                this.Type = reader.ReadInt32();
                this.Data = reader.ReadBytes(size);
            }

            public void WriteTo(BinaryWriter writer)
            {
                writer.Write(this.Signature);
                writer.Write(this.Data.Length);
                writer.Write(this.Type);
                writer.Write(this.Data);
            }

            public byte[] Signature { get; private set; }

            public int Type { get; private set; }

            public byte[] Data { get; private set; }

            public string DataString
            {
                get
                {
                    return Enc.SJIS.GetString(this.Data);
                }

                set
                {
                    this.Data = Enc.SJIS.GetBytes(value);
                }
            }
        }
    }
}
