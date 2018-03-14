//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System.Linq;
    using System.Text;

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

        /// <summary>
        /// Converts the character encoding.
        /// </summary>
        /// <param name="str">A string to convert.</param>
        /// <param name="src">The encoding of <paramref name="str"/>.</param>
        /// <param name="dst">The encoding of the converted string.</param>
        /// <returns>The converted string.</returns>
        public static string Convert(this string str, Encoding src, Encoding dst)
        {
            return ((src != null) && (dst != null))
                ? dst.GetString(Encoding.Convert(src, dst, src.GetBytes(str))) : str;
        }

        /// <summary>
        /// Converts the character encoding to the code page 932.
        /// <remarks>It is supposed to be used for C# string literals.</remarks>
        /// </summary>
        /// <param name="str">An UTF-8 string to convert.</param>
        /// <returns>The converted string.</returns>
        public static string ToCP932(this string str)
        {
            return str.Convert(Enc.UTF8, Enc.CP932);
        }
    }
}
