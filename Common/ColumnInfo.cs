//-----------------------------------------------------------------------
// <copyright file="ColumnInfo.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains information about a column in the REIMU's list view.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ColumnInfo
    {
        /// <summary>
        /// The text displaying as a column header.
        /// </summary>
        /// <remarks>The encoding must be the code page 932.</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 116)]
        public string Title;

        /// <summary>
        /// The alignment of the column header text.
        /// </summary>
        public TextAlign Align;

        /// <summary>
        /// The type of the sorting method for the column.
        /// </summary>
        public SortType Sort;

        /// <summary>
        /// The type of the system information displayed in the column.
        /// </summary>
        public SystemInfoType System;
    }
}
