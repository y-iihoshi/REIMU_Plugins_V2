using System.Runtime.InteropServices;

namespace ReimuPlugins.Common
{
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
    public class FileInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=512)]
        public string Text;
    }
}
