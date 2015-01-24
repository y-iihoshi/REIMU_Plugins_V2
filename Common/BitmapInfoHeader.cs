//-----------------------------------------------------------------------
// <copyright file="BitmapInfoHeader.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <c>BITMAPINFOHEADER</c> structure of Win32 API.
    /// <para>Contains information about the dimensions and color format of a DIB.</para>
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Reviewed.")]
    [StructLayout(LayoutKind.Sequential)]
    public struct BitmapInfoHeader
    {
        /// <summary>
        /// The number of bytes required by the structure.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public uint Size;

        /// <summary>
        /// The width of the bitmap, in pixels.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public int Width;

        /// <summary>
        /// The height of the bitmap, in pixels.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public int Height;

        /// <summary>
        /// The number of planes for the target device.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public ushort Planes;

        /// <summary>
        /// The number of bits-per-pixel.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public ushort BitCount;

        /// <summary>
        /// The type of compression for a compressed bottom-up bitmap (top-down DIBs cannot be compressed).
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public uint Compression;

        /// <summary>
        /// The size, in bytes, of the image.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public uint SizeImage;

        /// <summary>
        /// The horizontal resolution, in pixels-per-meter, of the target device for the bitmap.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public int XPelsPerMeter;

        /// <summary>
        /// The vertical resolution, in pixels-per-meter, of the target device for the bitmap.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public int YPelsPerMeter;

        /// <summary>
        /// The number of color indexes in the color table that are actually used by the bitmap.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public uint ClrUsed;

        /// <summary>
        /// The number of color indexes that are required for displaying the bitmap.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public uint ClrImportant;
    }
}
