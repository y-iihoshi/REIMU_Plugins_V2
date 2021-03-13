//-----------------------------------------------------------------------
// <copyright file="ColumnInfo.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains information about a column in the REIMU's list view.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class ColumnInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 116)]
        private string title;
#pragma warning disable IDE0032 // Use auto property
        private TextAlign align;
        private SortType sort;
        private SystemInfoType system;
#pragma warning restore IDE0032 // Use auto property

        /// <summary>
        /// Gets or sets the text displaying as a column header.
        /// </summary>
        /// <remarks>The encoding must be the code page 932.</remarks>
        public string Title { get => this.title; set => this.title = value; }

        /// <summary>
        /// Gets or sets the alignment of the column header text.
        /// </summary>
        public TextAlign Align { get => this.align; set => this.align = value; }

        /// <summary>
        /// Gets or sets the type of the sorting method for the column.
        /// </summary>
        public SortType Sort { get => this.sort; set => this.sort = value; }

        /// <summary>
        /// Gets or sets the type of the system information displayed in the column.
        /// </summary>
        public SystemInfoType System { get => this.system; set => this.system = value; }
    }
}
