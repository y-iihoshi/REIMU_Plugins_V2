//-----------------------------------------------------------------------
// <copyright file="BitmapInfoHeader.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace ReimuPlugins.Common
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BitmapInfoHeader
    {
        uint size;
        int width;
        int height;
        ushort planes;
        ushort bitCount;
        uint compression;
        uint sizeImage;
        int xPelsPerMeter;
        int yPelsPerMeter;
        uint clrUsed;
        uint clrImportant;
    }
}
