//-----------------------------------------------------------------------
// <copyright file="RgbQuad.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace ReimuPlugins.Common
{
    /// <summary>
    /// <c>RGBQUAD</c> structure of Win32 API.
    /// <para>Describes a color consisting of relative intensities of red, green, and blue.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RgbQuad
    {
        /// <summary>
        /// The intensity of blue in the color.
        /// </summary>
        public byte blue;

        /// <summary>
        /// The intensity of green in the color.
        /// </summary>
        public byte green;

        /// <summary>
        /// The intensity of red in the color.
        /// </summary>
        public byte red;

        /// <summary>
        /// This member is reserved and must be zero.
        /// </summary>
        public byte reserved;
    }
}
