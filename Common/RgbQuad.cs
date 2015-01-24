//-----------------------------------------------------------------------
// <copyright file="RgbQuad.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <c>RGBQUAD</c> structure of Win32 API.
    /// <para>Describes a color consisting of relative intensities of red, green, and blue.</para>
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Used to communicate an external unmanaged app.")]
    [StructLayout(LayoutKind.Sequential)]
    public struct RgbQuad
    {
        /// <summary>
        /// The intensity of blue in the color.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public byte Blue;

        /// <summary>
        /// The intensity of green in the color.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public byte Green;

        /// <summary>
        /// The intensity of red in the color.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public byte Red;

        /// <summary>
        /// This member is reserved and must be zero.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public byte Reserved;
    }
}
