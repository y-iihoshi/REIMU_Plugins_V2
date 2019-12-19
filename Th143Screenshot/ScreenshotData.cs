//-----------------------------------------------------------------------
// <copyright file="ScreenshotData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Th143Screenshot
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using ReimuPlugins.Common;

    public sealed class ScreenshotData
    {
        public ScreenshotData()
        {
            this.Signature = string.Empty;
            this.Day = 0;
            this.Scene = 0;
            this.Width = 0;
            this.Height = 0;
            this.DateTime = 0;
            this.SlowRate = 0;
            this.Bitmap = null;
        }

        public string Signature { get; private set; }

        public short Day { get; private set; }

        public short Scene { get; private set; }

        public short Width { get; private set; }

        public short Height { get; private set; }

        public uint DateTime { get; private set; }

        public float SlowRate { get; private set; }

        public Bitmap Bitmap { get; private set; }

        public void Read(Stream input)
        {
            this.Read(input, false);
        }

        public void Read(Stream input, bool withBitmap)
        {
            using var reader = new BinaryReader(input);

            this.Signature = Enc.CP932.GetString(reader.ReadBytes(4));
            _ = reader.ReadInt16();
            this.Day = reader.ReadInt16();
            this.Scene = reader.ReadInt16();
            _ = reader.ReadInt16();
            this.Width = reader.ReadInt16();
            this.Height = reader.ReadInt16();
            _ = reader.ReadInt32();
            this.DateTime = reader.ReadUInt32();
            this.SlowRate = reader.ReadSingle();
            _ = reader.ReadBytes(0x58);

            if (withBitmap)
            {
                this.Bitmap = ReadBitmap(input, this.Width, this.Height);
            }
        }

        public void Read(string path)
        {
            this.Read(path, false);
        }

        public void Read(string path, bool withBitmap)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            this.Read(stream, withBitmap);
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
