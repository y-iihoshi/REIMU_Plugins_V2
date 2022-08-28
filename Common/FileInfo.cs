//-----------------------------------------------------------------------
// <copyright file="FileInfo.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common;

using System.Runtime.InteropServices;

/// <summary>
/// Contains the file information displaying in the REIMU's list view.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public class FileInfo
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
    private string text;

    /// <summary>
    /// Gets or sets the displaying text.
    /// </summary>
    /// <remarks>The encoding must be the code page 932.</remarks>
    public string Text { get => this.text; set => this.text = value; }
}
