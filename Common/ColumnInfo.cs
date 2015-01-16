//-----------------------------------------------------------------------
// <copyright file="ColumnInfo.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace ReimuPlugins.Common
{
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
    public struct ColumnInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=116)]
        public string Title;

        public TextAlign Align;

        public SortType Sort;

        public SystemInfoType System;
    }
}
