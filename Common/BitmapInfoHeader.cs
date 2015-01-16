//-----------------------------------------------------------------------
// <copyright file="BitmapInfoHeader.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace ReimuPlugins.Common
{
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
        uint size;

        /// <summary>
        /// The width of the bitmap, in pixels.
        /// </summary>
        int width;

        /// <summary>
        /// The height of the bitmap, in pixels.
        /// </summary>
        int height;

        /// <summary>
        /// The number of planes for the target device.
        /// </summary>
        ushort planes;

        /// <summary>
        /// The number of bits-per-pixel.
        /// </summary>
        ushort bitCount;

        /// <summary>
        /// The type of compression for a compressed bottom-up bitmap (top-down DIBs cannot be compressed).
        /// </summary>
        uint compression;

        /// <summary>
        /// The size, in bytes, of the image.
        /// </summary>
        uint sizeImage;

        /// <summary>
        /// The horizontal resolution, in pixels-per-meter, of the target device for the bitmap.
        /// </summary>
        int xPelsPerMeter;

        /// <summary>
        /// The vertical resolution, in pixels-per-meter, of the target device for the bitmap.
        /// </summary>
        int yPelsPerMeter;

        /// <summary>
        /// The number of color indexes in the color table that are actually used by the bitmap.
        /// </summary>
        uint clrUsed;

        /// <summary>
        /// The number of color indexes that are required for displaying the bitmap.
        /// </summary>
        uint clrImportant;
    }
}
