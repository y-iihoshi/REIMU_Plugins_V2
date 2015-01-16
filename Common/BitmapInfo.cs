//-----------------------------------------------------------------------
// <copyright file="BitmapInfo.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace ReimuPlugins.Common
{
    /// <summary>
    /// <c>BITMAPINFO</c> structure of Win32 API.
    /// <para>
    /// Defines the dimensions and color information for a Windows device-independent bitmap (DIB). 
    /// </para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BitmapInfo
    {
        /// <summary>
        /// A <c>BITMAPINFOHEADER</c> structure that contains information about the dimensions and color
        /// format of a device-independent bitmap.
        /// </summary>
        public BitmapInfoHeader header;

        /// <summary>
        /// An array of <c>RGBQUAD</c> or <c>DWORD</c> data types that define the colors in the bitmap. 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=1)]
        public RgbQuad[] colors;
    }
}
