//-----------------------------------------------------------------------
// <copyright file="FileInfo.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains the file information displaying in the REIMU's list view.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Reviewed.")]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct FileInfo
    {
        /// <summary>
        /// The displaying text.
        /// </summary>
        /// <remarks>The encoding must be the code page 932.</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public string Text;
    }
}
