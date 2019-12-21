//-----------------------------------------------------------------------
// <copyright file="IBestshotData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System.IO;

    /// <summary>
    /// The interface of classes indicating a Touhou bestshot file format.
    /// </summary>
    public interface IBestshotData
    {
        /// <summary>
        /// Reads a bestshot from the specified stream.
        /// </summary>
        /// <param name="input">Stream to be read.</param>
        void Read(Stream input);

        /// <summary>
        /// Reads a bestshot from the specified stream.
        /// </summary>
        /// <param name="input">Stream to be read.</param>
        /// <param name="withBitmap"><c>true</c> if it also reads a bitmap.</param>
        void Read(Stream input, bool withBitmap);

        /// <summary>
        /// Reads a bestshot from the specified file path.
        /// </summary>
        /// <param name="path">Path of the file to be read.</param>
        void Read(string path);

        /// <summary>
        /// Reads a bestshot from the specified file path.
        /// </summary>
        /// <param name="path">Path of the file to be read.</param>
        /// <param name="withBitmap"><c>true</c> if it also reads a bitmap.</param>
        void Read(string path, bool withBitmap);
    }
}
