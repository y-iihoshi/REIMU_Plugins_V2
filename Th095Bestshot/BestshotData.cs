//-----------------------------------------------------------------------
// <copyright file="BestshotData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th095Bestshot
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using ReimuPlugins.Common;

    public sealed class BestshotData
    {
        public BestshotData()
        {
            this.Signature = string.Empty;
            this.Level = 0;
            this.Scene = 0;
            this.Width = 0;
            this.Height = 0;
            this.Score = 0;
            this.SlowRate = 0;
            this.CardName = string.Empty;
            this.Bitmap = null;
        }

        public string Signature { get; private set; }

        public short Level { get; private set; }

        public short Scene { get; private set; }

        public short Width { get; private set; }

        public short Height { get; private set; }

        public int Score { get; private set; }

        public float SlowRate { get; private set; }

        public string CardName { get; private set; }

        public Bitmap Bitmap { get; private set; }

        public void Read(Stream input)
        {
            this.Read(input, false);
        }

        public void Read(Stream input, bool withBitmap)
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

                if (withBitmap)
                {
                    this.Bitmap = ReadBitmap(input, this.Width, this.Height);
                }
            }
        }

        public void Read(string path)
        {
            this.Read(path, false);
        }

        public void Read(string path, bool withBitmap)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                this.Read(stream, withBitmap);
            }
        }

        private static Bitmap ReadBitmap(Stream input, int width, int height)
        {
            using (var extracted = new MemoryStream())
            {
                Lzss.Extract(input, extracted);
                extracted.Seek(0, SeekOrigin.Begin);

                using (var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb))
                {
                    using (var locked = new BitmapLock(bitmap, ImageLockMode.WriteOnly))
                    {
                        var source = extracted.ToArray();
                        var sourceStride = 3 * width;   // "3" means 24bpp.
                        var destination = locked.Scan0;
                        for (var index = 0; index < source.Length; index += sourceStride)
                        {
                            Marshal.Copy(source, index, destination, sourceStride);
                            destination = new IntPtr(destination.ToInt32() + locked.Stride);
                        }
                    }

                    return bitmap.Clone() as Bitmap;
                }
            }
        }
    }
}
