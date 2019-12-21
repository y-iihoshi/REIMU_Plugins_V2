//-----------------------------------------------------------------------
// <copyright file="Encoding.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    /// <summary>
    /// Contains read-only instances of <see cref="System.Text.Encoding"/> class for convenience.
    /// </summary>
    public static class Encoding
    {
        /// <summary>
        /// Gets the code page 932 encoding.
        /// </summary>
        public static System.Text.Encoding CP932 { get; } = System.Text.Encoding.GetEncoding(932);

        /// <summary>
        /// Gets the UTF-8 encoding.
        /// </summary>
        public static System.Text.Encoding UTF8 { get; } = System.Text.Encoding.UTF8;

        /// <summary>
        /// Gets the UTF-8 encoding which does not provide a byte order mark.
        /// </summary>
        public static System.Text.Encoding UTF8NoBOM { get; } = new System.Text.UTF8Encoding();
    }
}
