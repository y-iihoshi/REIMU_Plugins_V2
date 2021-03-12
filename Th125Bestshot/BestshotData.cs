//-----------------------------------------------------------------------
// <copyright file="BestshotData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th125Bestshot
{
    using System.Collections.Specialized;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using ReimuPlugins.Common;

    public sealed class BestshotData : BestshotDataBase
    {
        private static readonly int[] Masks;

        private BitVector32 bonusFields;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static BestshotData()
        {
            Masks = new int[32];
            Masks[0] = BitVector32.CreateMask();
            for (var i = 1; i < Masks.Length; i++)
            {
                Masks[i] = BitVector32.CreateMask(Masks[i - 1]);
            }
        }
#pragma warning restore CA1810 // Initialize reference type static fields inline

        public BestshotData()
        {
            this.Signature = string.Empty;
            this.Level = 0;
            this.Scene = 0;
            this.Width = 0;
            this.Height = 0;
            this.Width2 = 0;
            this.Height2 = 0;
            this.HalfWidth = 0;
            this.HalfHeight = 0;
            this.DateTime = 0;
            this.SlowRate = 0;
            this.bonusFields = default;
            this.ResultScore = 0;
            this.BasePoint = 0;
            this.RiskBonus = 0;
            this.BossShot = 0;
            this.NiceShot = 0;
            this.AngleBonus = 0;
            this.MacroBonus = 0;
            this.FrontSideBackShot = 0;
            this.ClearShot = 0;
            this.Angle = 0;
            this.ResultScore2 = 0;
            this.CardName = string.Empty;
            this.Bitmap = null;
        }

        public string Signature { get; private set; }

        public short Level { get; private set; }

        public short Scene { get; private set; }

        public short Width { get; private set; }

        public short Height { get; private set; }

        public short Width2 { get; private set; }

        public short Height2 { get; private set; }

        public short HalfWidth { get; private set; }

        public short HalfHeight { get; private set; }

        public uint DateTime { get; private set; }

        public float SlowRate { get; private set; }

        public bool TwoShotBit => this.bonusFields[Masks[2]];

        public bool NiceShotBit => this.bonusFields[Masks[3]];

        public bool RiskBonusBit => this.bonusFields[Masks[4]];

        public bool RedShotBit => this.bonusFields[Masks[6]];

        public bool PurpleShotBit => this.bonusFields[Masks[7]];

        public bool BlueShotBit => this.bonusFields[Masks[8]];

        public bool CyanShotBit => this.bonusFields[Masks[9]];

        public bool GreenShotBit => this.bonusFields[Masks[10]];

        public bool YellowShotBit => this.bonusFields[Masks[11]];

        public bool OrangeShotBit => this.bonusFields[Masks[12]];

        public bool ColorfulShotBit => this.bonusFields[Masks[13]];

        public bool RainbowShotBit => this.bonusFields[Masks[14]];

        public bool SoloShotBit => this.bonusFields[Masks[16]];

        public bool MacroBonusBit => this.bonusFields[Masks[22]];

        public bool FrontShotBit => this.bonusFields[Masks[24]];

        public bool BackShotBit => this.bonusFields[Masks[25]];

        public bool SideShotBit => this.bonusFields[Masks[26]];

        public bool ClearShotBit => this.bonusFields[Masks[27]];

        public bool CatBonusBit => this.bonusFields[Masks[28]];

        public int ResultScore { get; private set; }

        public int BasePoint { get; private set; }

        public int RiskBonus { get; private set; }

        public float BossShot { get; private set; }

        public float NiceShot { get; private set; }

        public float AngleBonus { get; private set; }

        public int MacroBonus { get; private set; }

        public int FrontSideBackShot { get; private set; }

        public int ClearShot { get; private set; }

        public float Angle { get; private set; }

        public int ResultScore2 { get; private set; }

        public string CardName { get; private set; }

        public Bitmap Bitmap { get; private set; }

        public override void Read(Stream input, bool withBitmap)
        {
            using var reader = new BinaryReader(input);

            this.Signature = Encoding.CP932.GetString(reader.ReadBytes(4));
            _ = reader.ReadInt16();
            this.Level = reader.ReadInt16();
            this.Scene = reader.ReadInt16();
            _ = reader.ReadInt16();
            this.Width = reader.ReadInt16();
            this.Height = reader.ReadInt16();
            _ = reader.ReadInt32();
            this.Width2 = reader.ReadInt16();
            this.Height2 = reader.ReadInt16();
            this.HalfWidth = reader.ReadInt16();
            this.HalfHeight = reader.ReadInt16();
            this.DateTime = reader.ReadUInt32();
            _ = reader.ReadInt32();
            this.SlowRate = reader.ReadSingle();
            this.bonusFields = new BitVector32(reader.ReadInt32());
            this.ResultScore = reader.ReadInt32();
            this.BasePoint = reader.ReadInt32();
            _ = reader.ReadInt32();
            _ = reader.ReadInt32();
            this.RiskBonus = reader.ReadInt32();
            this.BossShot = reader.ReadSingle();
            this.NiceShot = reader.ReadSingle();
            this.AngleBonus = reader.ReadSingle();
            this.MacroBonus = reader.ReadInt32();
            this.FrontSideBackShot = reader.ReadInt32();
            this.ClearShot = reader.ReadInt32();
            _ = reader.ReadBytes(0x30);
            this.Angle = reader.ReadSingle();
            this.ResultScore2 = reader.ReadInt32();
            _ = reader.ReadInt32();
            this.CardName = Encoding.CP932.GetString(reader.ReadBytes(0x50));

            if (withBitmap)
            {
                this.Bitmap = ReadBitmap(input, this.Width, this.Height);
            }
        }

        private static Bitmap ReadBitmap(Stream input, int width, int height)
        {
            using var extracted = new MemoryStream();
            Lzss.Extract(input, extracted);
            _ = extracted.Seek(0, SeekOrigin.Begin);

            using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            using (var locked = new BitmapLock(bitmap, ImageLockMode.WriteOnly))
            {
                var source = extracted.ToArray();
                var destination = locked.Scan0;
                Marshal.Copy(source, 0, destination, source.Length);
            }

            return bitmap.Clone() as Bitmap;
        }
    }
}
