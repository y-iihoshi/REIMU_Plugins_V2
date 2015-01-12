using System.Runtime.InteropServices;

namespace ReimuPlugins.Common
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BitmapInfo
    {
        public BitmapInfoHeader header;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=1)]
        public RgbQuad[] colors;
    }
}
