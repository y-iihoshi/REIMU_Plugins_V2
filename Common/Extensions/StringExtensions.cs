//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common.Extensions;

using System.Linq;

/// <summary>
/// Defines the extension methods for <c>String</c> class.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts a string to a null-terminated string.
    /// </summary>
    /// <param name="str">A string to convert.</param>
    /// <returns>The converted null-terminated string.</returns>
    public static string ToCStr(this string str)
    {
        return str.Contains('\0') ? str : str + '\0';
    }
}
