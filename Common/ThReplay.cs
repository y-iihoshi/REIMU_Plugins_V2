//-----------------------------------------------------------------------
// <copyright file="ThReplay.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Text;

namespace ReimuPlugins.Common
{
    /// <summary>
    /// Indicates the Touhou replay file format.
    /// </summary>
    public class ThReplay
    {
        /// <summary>
        /// The replay data.
        /// </summary>
        protected Replay replay;

        /// <summary>
        /// The user information indicating the replay file information.
        /// </summary>
        protected UserInfo userInfo0;

        /// <summary>
        /// The user information indicating the comment.
        /// </summary>
        protected UserInfo userInfo1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThReplay"/> class.
        /// </summary>
        public ThReplay()
        {
        }

        /// <summary>
        /// Reads from the specified stream.
        /// </summary>
        /// <param name="input">An input stream.</param>
        public void Read(Stream input)
        {
            var reader = new BinaryReader(input);
            replay.ReadFrom(reader);
            userInfo0.ReadFrom(reader);
            userInfo1.ReadFrom(reader);
        }

        /// <summary>
        /// Writes to the specified stream.
        /// </summary>
        /// <param name="output">An output stream.</param>
        public void Write(Stream output)
        {
            var writer = new BinaryWriter(output);
            replay.WriteTo(writer);
            userInfo0.WriteTo(writer);
            userInfo1.WriteTo(writer);
            writer.Flush();
        }

        /// <summary>
        /// Gets the information.
        /// </summary>
        public string Info
        {
            get
            {
                return this.userInfo0.DataString;
            }
        }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
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

        /// <summary>
        /// Indicates a replay data.
        /// </summary>
        protected class Replay
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Replay"/> class.
            /// </summary>
            public Replay()
            {
            }

            /// <summary>
            /// Reads data by using the specified <see cref="BinaryReader"/> instance.
            /// </summary>
            /// <param name="reader">A <see cref="BinaryReader"/> instance.</param>
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

            /// <summary>
            /// Writes data by using the specified <see cref="BinaryWriter"/> instance.
            /// </summary>
            /// <param name="writer">A <see cref="BinaryWriter"/> instance.</param>
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

            /// <summary>
            /// Gets the signature string.
            /// </summary>
            public byte[] Signature { get; private set; }

            /// <summary>
            /// Gets an unknown data.
            /// </summary>
            public byte[] Unknown1 { get; private set; }

            /// <summary>
            /// Gets the size of the replay header.
            /// <remarks>
            /// The replay header consists of:
            /// Signature, Unknown1, HeaderSize, Unknown2, the size of Data, and Unknown3.
            /// </remarks>
            /// </summary>
            public int HeaderSize { get; private set; }

            /// <summary>
            /// Gets an unknown data.
            /// </summary>
            public byte[] Unknown2 { get; private set; }

            /// <summary>
            /// Gets an unknown data.
            /// </summary>
            public byte[] Unknown3 { get; private set; }

            /// <summary>
            /// Gets the actual replay data.
            /// </summary>
            public byte[] Data { get; private set; }
        }

        /// <summary>
        /// Indicates an user information stored in the replay file.
        /// </summary>
        protected class UserInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UserInfo"/> class.
            /// </summary>
            public UserInfo()
            {
            }

            /// <summary>
            /// Reads data by using the specified <see cref="BinaryReader"/> instance.
            /// </summary>
            /// <param name="reader">A <see cref="BinaryReader"/> instance.</param>
            public void ReadFrom(BinaryReader reader)
            {
                this.Signature = reader.ReadBytes(4);
                var size = reader.ReadInt32();
                this.Type = reader.ReadInt32();
                this.Data = reader.ReadBytes(size);
            }

            /// <summary>
            /// Writes data by using the specified <see cref="BinaryWriter"/> instance.
            /// </summary>
            /// <param name="writer">A <see cref="BinaryWriter"/> instance.</param>
            public void WriteTo(BinaryWriter writer)
            {
                writer.Write(this.Signature);
                writer.Write(this.Data.Length);
                writer.Write(this.Type);
                writer.Write(this.Data);
            }

            /// <summary>
            /// Gets the signature string.
            /// </summary>
            public byte[] Signature { get; private set; }

            /// <summary>
            /// Gets the type of the user information. (0: replay file information, 1: comment)
            /// </summary>
            public int Type { get; private set; }

            /// <summary>
            /// Gets the actual data.
            /// </summary>
            public byte[] Data { get; private set; }

            /// <summary>
            /// Gets or sets the actual data represented as a Shift_JIS string.
            /// </summary>
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
