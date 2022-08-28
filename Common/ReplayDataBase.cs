//-----------------------------------------------------------------------
// <copyright file="ReplayDataBase.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// Indicates the Touhou replay file format.
/// </summary>
public class ReplayDataBase
{
    /// <summary>
    /// The information as a read-only string array.
    /// </summary>
    private IEnumerable<string> info;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReplayDataBase"/> class.
    /// </summary>
    public ReplayDataBase()
    {
        this.ReplayData = new Replay();
        this.UserInfo0 = new UserInfo();
        this.UserInfo1 = new UserInfo();
    }

    /// <summary>
    /// Gets the information as a string.
    /// </summary>
    public string Info => this.UserInfo0.DataString;

    /// <summary>
    /// Gets or sets the comment.
    /// </summary>
    public string Comment
    {
        get => this.UserInfo1.DataString;
        set => this.UserInfo1.DataString = value;
    }

    /// <summary>
    /// Gets the information as an array.
    /// </summary>
    protected IEnumerable<string> InfoArray
    {
        get
        {
            this.info ??= this.Info.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            return this.info;
        }
    }

    /// <summary>
    /// Gets the replay data.
    /// </summary>
    protected Replay ReplayData { get; private set; }

    /// <summary>
    /// Gets the user information indicating the replay file information.
    /// </summary>
    protected UserInfo UserInfo0 { get; private set; }

    /// <summary>
    /// Gets the user information indicating the comment.
    /// </summary>
    protected UserInfo UserInfo1 { get; private set; }

    /// <summary>
    ///   Gets a number string from the specified file path.
    /// </summary>
    /// <param name="path">A file path to get a number string.</param>
    /// <param name="patternBasic">The regular expression pattern to judge the file is numbered.</param>
    /// <param name="patternUser">
    ///   The regular expression pattern to judge the file is user-defined.
    /// </param>
    /// <returns>
    ///   <list type="bullet">
    ///     <item>
    ///       <term><c>"No.XX"</c></term>
    ///       <description>
    ///         When the filename is matched by <paramref name="patternBasic"/>.
    ///         <c>"XX"</c> is the two-digit number string included in the filename.
    ///       </description>
    ///     </item>
    ///     <item>
    ///       <term><c>"User"</c></term>
    ///       <description>When the filename is matched by <paramref name="patternUser"/>.</description>
    ///     </item>
    ///     <item>
    ///       <term><see cref="string.Empty"/></term>
    ///       <description>
    ///         When the filename is not matched neither <paramref name="patternBasic"/> nor
    ///         <paramref name="patternUser"/>.
    ///       </description>
    ///     </item>
    ///   </list>
    /// </returns>
    /// <exception cref="ArgumentException">
    ///   <list type="bullet">
    ///     <item>
    ///       <paramref name="path"/> contains one or more of the invalid characters defined in
    ///       <see cref="Path.GetInvalidPathChars"/>.
    ///     </item>
    ///     <item>A regular expression parsing error occurred.</item>
    ///   </list>
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="path"/> or <paramref name="patternBasic"/> is <c>null</c>.
    /// </exception>
    public static string GetNumberFromPath(string path, string patternBasic, string patternUser)
    {
        var number = string.Empty;
        var filename = Path.GetFileName(path);

        var match = Regex.Match(filename, patternBasic, RegexOptions.IgnoreCase);
        if (match.Success)
        {
            number = "No." + match.Groups[1];
        }
        else
        {
            if (Regex.IsMatch(filename, patternUser, RegexOptions.IgnoreCase))
            {
                number = "User";
            }
        }

        return number;
    }

    /// <summary>
    /// Reads from the specified stream.
    /// </summary>
    /// <param name="input">An input stream.</param>
    public virtual void Read(Stream input)
    {
        using var reader = new BinaryReader(input, Encoding.UTF8NoBOM, true);
        this.ReplayData.ReadFrom(reader);
        this.UserInfo0.ReadFrom(reader);
        this.UserInfo1.ReadFrom(reader);
    }

    /// <summary>
    /// Reads from the specified file path.
    /// </summary>
    /// <param name="path">A path string of the input file.</param>
    public void Read(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        this.Read(stream);
    }

    /// <summary>
    /// Writes to the specified stream.
    /// </summary>
    /// <param name="output">An output stream.</param>
    public void Write(Stream output)
    {
        using var writer = new BinaryWriter(output, Encoding.UTF8NoBOM, true);
        this.ReplayData.WriteTo(writer);
        this.UserInfo0.WriteTo(writer);
        this.UserInfo1.WriteTo(writer);
    }

    /// <summary>
    /// Writes to the specified file path.
    /// </summary>
    /// <param name="path">A path string of the output file.</param>
    public void Write(string path)
    {
        using var stream = new FileStream(path, FileMode.Truncate, FileAccess.Write);
        this.Write(stream);
    }

    /// <summary>
    /// Indicates a replay data.
    /// </summary>
    protected class Replay
    {
        /// <summary>
        /// The signature string.
        /// </summary>
        private byte[] signature;

        /// <summary>
        /// An unknown data placed after the signature string.
        /// </summary>
        private byte[] unknown1;

        /// <summary>
        /// An unknown data.
        /// </summary>
        private byte[] unknown2;

        /// <summary>
        /// An unknown data.
        /// </summary>
        private byte[] unknown3;

        /// <summary>
        /// The actual replay data.
        /// </summary>
        private byte[] data;

        /// <summary>
        /// Initializes a new instance of the <see cref="Replay"/> class.
        /// </summary>
        public Replay()
        {
        }

        /// <summary>
        /// Gets the signature string.
        /// </summary>
        public IEnumerable<byte> Signature => this.signature;

        /// <summary>
        /// Gets an unknown data.
        /// </summary>
        public IEnumerable<byte> Unknown1 => this.unknown1;

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
        public IEnumerable<byte> Unknown2 => this.unknown2;

        /// <summary>
        /// Gets an unknown data.
        /// </summary>
        public IEnumerable<byte> Unknown3 => this.unknown3;

        /// <summary>
        /// Gets the actual replay data.
        /// </summary>
        public IEnumerable<byte> Data => this.data;

        /// <summary>
        /// Reads data by using the specified <see cref="BinaryReader"/> instance.
        /// </summary>
        /// <param name="reader">A <see cref="BinaryReader"/> instance.</param>
        public void ReadFrom(BinaryReader reader)
        {
            if (reader != null)
            {
                this.signature = reader.ReadBytes(4);
                this.unknown1 = reader.ReadBytes(8);
                this.HeaderSize = reader.ReadInt32();
                this.unknown2 = reader.ReadBytes(12);
                var dataSize = reader.ReadInt32();
                this.unknown3 = reader.ReadBytes(4);
                this.data = reader.ReadBytes(dataSize);
            }
        }

        /// <summary>
        /// Writes data by using the specified <see cref="BinaryWriter"/> instance.
        /// </summary>
        /// <param name="writer">A <see cref="BinaryWriter"/> instance.</param>
        public void WriteTo(BinaryWriter writer)
        {
            if (writer != null)
            {
                writer.Write(this.signature);
                writer.Write(this.unknown1);
                writer.Write(this.HeaderSize);
                writer.Write(this.unknown2);
                writer.Write(this.data.Length);
                writer.Write(this.unknown3);
                writer.Write(this.data);
            }
        }
    }

    /// <summary>
    /// Indicates an user information stored in the replay file.
    /// </summary>
    protected class UserInfo
    {
        /// <summary>
        /// The signature string.
        /// </summary>
        private byte[] signature;

        /// <summary>
        /// The actual data.
        /// </summary>
        private byte[] data;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfo"/> class.
        /// </summary>
        public UserInfo()
        {
        }

        /// <summary>
        /// Gets the signature string.
        /// </summary>
        public IEnumerable<byte> Signature => this.signature;

        /// <summary>
        /// Gets the type of the user information. (0: replay file information, 1: comment.)
        /// </summary>
        public int InfoType { get; private set; }

        /// <summary>
        /// Gets the actual data.
        /// </summary>
        public IEnumerable<byte> Data => this.data;

        /// <summary>
        /// Gets or sets the actual data represented as a code page 932 string.
        /// </summary>
        public string DataString
        {
            get => Encoding.CP932.GetString(this.data);
            set => this.data = Encoding.CP932.GetBytes(value);
        }

        /// <summary>
        /// Reads data by using the specified <see cref="BinaryReader"/> instance.
        /// </summary>
        /// <param name="reader">A <see cref="BinaryReader"/> instance.</param>
        public void ReadFrom(BinaryReader reader)
        {
            if (reader != null)
            {
                this.signature = reader.ReadBytes(4);
                var size = reader.ReadInt32();
                this.InfoType = reader.ReadInt32();
                this.data = reader.ReadBytes(size - this.signature.Length - (sizeof(int) * 2));
            }
        }

        /// <summary>
        /// Writes data by using the specified <see cref="BinaryWriter"/> instance.
        /// </summary>
        /// <param name="writer">A <see cref="BinaryWriter"/> instance.</param>
        public void WriteTo(BinaryWriter writer)
        {
            if (writer != null)
            {
                writer.Write(this.signature);
                writer.Write(this.signature.Length + (sizeof(int) * 2) + this.data.Length);
                writer.Write(this.InfoType);
                writer.Write(this.data);
            }
        }
    }
}
