//-----------------------------------------------------------------------
// <copyright file="BitmapInfoHeader.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// <c>BITMAPINFOHEADER</c> structure of Win32 API.
    /// <para>Contains information about the dimensions and color format of a DIB.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BitmapInfoHeader
    {
        /// <summary>
        /// The number of bytes required by the structure.
        /// </summary>
        public uint Size;

        /// <summary>
        /// The width of the bitmap, in pixels.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the bitmap, in pixels.
        /// </summary>
        public int Height;

        /// <summary>
        /// The number of planes for the target device.
        /// </summary>
        public ushort Planes;

        /// <summary>
        /// The number of bits-per-pixel.
        /// </summary>
        public ushort BitCount;

        /// <summary>
        /// The type of compression for a compressed bottom-up bitmap (top-down DIBs cannot be compressed).
        /// </summary>
        public uint Compression;

        /// <summary>
        /// The size, in bytes, of the image.
        /// </summary>
        public uint SizeImage;

        /// <summary>
        /// The horizontal resolution, in pixels-per-meter, of the target device for the bitmap.
        /// </summary>
        public int XPelsPerMeter;

        /// <summary>
        /// The vertical resolution, in pixels-per-meter, of the target device for the bitmap.
        /// </summary>
        public int YPelsPerMeter;

        /// <summary>
        /// The number of color indexes in the color table that are actually used by the bitmap.
        /// </summary>
        public uint ClrUsed;

        /// <summary>
        /// The number of color indexes that are required for displaying the bitmap.
        /// </summary>
        public uint ClrImportant;
    }
}
