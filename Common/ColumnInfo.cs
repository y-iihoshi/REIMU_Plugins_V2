//-----------------------------------------------------------------------
// <copyright file="ColumnInfo.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains information about a column in the REIMU's list view.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Used to communicate an external unmanaged app.")]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ColumnInfo
    {
        /// <summary>
        /// The text displaying as a column header.
        /// </summary>
        /// <remarks>The encoding must be the code page 932.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 116)]
        public string Title;

        /// <summary>
        /// The alignment of the column header text.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public TextAlign Align;

        /// <summary>
        /// The type of the sorting method for the column.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public SortType Sort;

        /// <summary>
        /// The type of the system information displayed in the column.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Used to communicate an external unmanaged app.")]
        public SystemInfoType System;
    }
}
