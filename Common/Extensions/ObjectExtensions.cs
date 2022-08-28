//-----------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common.Extensions;

using System;

/// <summary>
/// Provides some extension methods for the <see cref="object"/> type.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Converts the specified object with an integer value to an enumeration member.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <param name="value">The value convert to an enumeration member.</param>
    /// <returns>The enumeration member which value is <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// No member of <typeparamref name="TEnum"/> equals to <paramref name="value"/>.
    /// </exception>
    public static TEnum ToValidEnum<TEnum>(this object value)
        where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        var type = typeof(TEnum);
        return Enum.IsDefined(type, value)
            ? (TEnum)Enum.ToObject(type, value) : throw new ArgumentOutOfRangeException(nameof(value));
    }
}
