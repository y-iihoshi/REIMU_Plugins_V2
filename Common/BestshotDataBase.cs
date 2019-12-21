//-----------------------------------------------------------------------
// <copyright file="BestshotDataBase.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System;
    using System.IO;

    /// <summary>
    /// The base class for the classes implementing <see cref="IBestshotData"/>.
    /// </summary>
    public class BestshotDataBase : IBestshotData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BestshotDataBase"/> class.
        /// </summary>
        protected BestshotDataBase()
        {
        }

        /// <inheritdoc/>
        public void Read(Stream input)
        {
            this.Read(input, false);
        }

        /// <inheritdoc/>
        public virtual void Read(Stream input, bool withBitmap)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Read(string path)
        {
            this.Read(path, false);
        }

        /// <inheritdoc/>
        public void Read(string path, bool withBitmap)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            this.Read(stream, withBitmap);
        }
    }
}
