//-----------------------------------------------------------------------
// <copyright file="FileInfo.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace ReimuPlugins.Common
{
    /// <summary>
    /// Contains the file information displaying in the REIMU's list view.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
    public struct FileInfo
    {
        /// <summary>
        /// The displaying text.
        /// </summary>
        /// <remarks>The encoding must be Shift_JIS.</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=512)]
        public string Text;
    }
}
