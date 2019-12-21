//-----------------------------------------------------------------------
// <copyright file="TextWriterExtensions.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common.Extensions
{
    using System;
    using System.IO;

    /// <summary>
    /// Defines the extension methods for <see cref="TextWriter"/> class.
    /// </summary>
    public static class TextWriterExtensions
    {
        /// <summary>
        /// A conditional version of <see cref="TextWriter.WriteLine(string)"/>.
        /// </summary>
        /// <param name="writer">A <see cref="TextWriter"/> object.</param>
        /// <param name="cond"><c>true</c> if it writes out; otherwise, <c>false</c>.</param>
        /// <param name="value">
        /// The string to write. If <paramref name="value"/> is <c>null</c>, only the line termination
        /// characters are written.
        /// </param>
        public static void CondWriteLine(this TextWriter writer, bool cond, string value)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (cond)
            {
                writer.WriteLine(value);
            }
        }

        /// <summary>
        /// A conditional version of <see cref="TextWriter.WriteLine(string, object)"/>.
        /// </summary>
        /// <param name="writer">A <see cref="TextWriter"/> object.</param>
        /// <param name="cond"><c>true</c> if it writes out; otherwise, <c>false</c>.</param>
        /// <param name="format">The formatted string.</param>
        /// <param name="arg0">The object to write into the formatted string.</param>
        public static void CondWriteLine(this TextWriter writer, bool cond, string format, object arg0)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (cond)
            {
                writer.WriteLine(format, arg0);
            }
        }

        /// <summary>
        /// A conditional version of <see cref="TextWriter.WriteLine(string, object[])"/>.
        /// </summary>
        /// <param name="writer">A <see cref="TextWriter"/> object.</param>
        /// <param name="cond"><c>true</c> if it writes out; otherwise, <c>false</c>.</param>
        /// <param name="format">The formatting string.</param>
        /// <param name="arg">The object array to write into format string.</param>
        public static void CondWriteLine(this TextWriter writer, bool cond, string format, params object[] arg)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (cond)
            {
                writer.WriteLine(format, arg);
            }
        }
    }
}
