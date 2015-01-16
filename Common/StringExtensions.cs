//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;
using System.Text;

namespace ReimuPlugins.Common
{
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
            return dst.GetString(Encoding.Convert(src, dst, src.GetBytes(str)));
        }

        /// <summary>
        /// Converts the character encoding to Shift_JIS.
        /// <remarks>It is supposed to be used for C# string literals.</remarks>
        /// </summary>
        /// <param name="str">An UTF-8 string to convert.</param>
        /// <returns>The converted string.</returns>
        public static string ToSJIS(this string str)
        {
            return str.Convert(Enc.UTF8, Enc.SJIS);
        }
    }
}
