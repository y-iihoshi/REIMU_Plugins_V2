//-----------------------------------------------------------------------
// <copyright file="BitmapInfo.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <c>BITMAPINFO</c> structure of Win32 API.
    /// <para>
    /// Defines the dimensions and color information for a Windows device-independent bitmap (DIB). 
    /// </para>
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Used to communicate an external unmanaged app.")]
    [StructLayout(LayoutKind.Sequential)]
    public struct BitmapInfo
    {
        /// <summary>
        /// A <c>BITMAPINFOHEADER</c> structure that contains information about the dimensions and color
        /// format of a device-independent bitmap.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public BitmapInfoHeader Header;

        /// <summary>
        /// An array of <c>RGBQUAD</c> or <c>DWORD</c> data types that define the colors in the bitmap. 
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public RgbQuad[] Colors;
    }
}
