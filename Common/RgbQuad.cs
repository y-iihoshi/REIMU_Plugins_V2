//-----------------------------------------------------------------------
// <copyright file="RgbQuad.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace ReimuPlugins.Common
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RgbQuad
    {
        public byte blue;
        public byte green;
        public byte red;
        public byte reserved;
    }
}
