﻿//-----------------------------------------------------------------------
// <copyright file="Win32Window.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common;

using System;
using System.Windows.Forms;

/// <summary>
/// Converter from a <c>HWND</c> value to a <see cref="IWin32Window"/> instance.
/// </summary>
public class Win32Window : IWin32Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Win32Window"/> class.
    /// </summary>
    /// <param name="handle">The handle (the value of <c>HWND</c> type of Win32 API).</param>
    public Win32Window(IntPtr handle)
    {
        this.Handle = handle;
    }

    /// <summary>
    /// Gets the window handle for the current instance.
    /// </summary>
    public IntPtr Handle { get; private set; }
}
