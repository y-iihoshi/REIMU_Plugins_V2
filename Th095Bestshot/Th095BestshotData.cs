//-----------------------------------------------------------------------
// <copyright file="Th095BestshotData.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed.")]

namespace ReimuPlugins.Th095Bestshot
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ReimuPlugins.Common;

    public sealed class Th095BestshotData
    {
        public Th095BestshotData()
        {
            this.Signature = string.Empty;
            this.Level = 0;
            this.Scene = 0;
            this.Width = 0;
            this.Height = 0;
            this.Score = 0;
            this.SlowRate = 0;
            this.CardName = string.Empty;
        }

        public string Signature { get; private set; }

        public short Level { get; private set; }

        public short Scene { get; private set; }

        public short Width { get; private set; }

        public short Height { get; private set; }

        public int Score { get; private set; }

        public float SlowRate { get; private set; }

        public string CardName { get; private set; }

        public void Read(Stream input)
        {
            using (var reader = new BinaryReader(input))
            {
                this.Signature = Enc.CP932.GetString(reader.ReadBytes(4));
                reader.ReadInt16();
                this.Level = reader.ReadInt16();
                this.Scene = reader.ReadInt16();
                reader.ReadInt16();
                this.Width = reader.ReadInt16();
                this.Height = reader.ReadInt16();
                this.Score = reader.ReadInt32();
                this.SlowRate = reader.ReadSingle();
                this.CardName = Enc.CP932.GetString(reader.ReadBytes(0x50));
            }
        }

        public void Read(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                this.Read(stream);
            }
        }
    }
}
