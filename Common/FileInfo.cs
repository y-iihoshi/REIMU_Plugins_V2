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
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Used to communicate an external unmanaged app.")]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct FileInfo
    {
        /// <summary>
        /// The displaying text.
        /// </summary>
        /// <remarks>The encoding must be the code page 932.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public string Text;
    }
}
