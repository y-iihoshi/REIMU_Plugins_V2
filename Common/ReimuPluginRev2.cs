//-----------------------------------------------------------------------
// <copyright file="ReimuPluginRev2.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable 1591

namespace ReimuPlugins.Common
{
    using System;

    /// <summary>
    /// The base class for the classes implementing the REIMU plugin interface (Revision 2).
    /// </summary>
    /// <typeparam name="TColumnKey">
    /// The key type of <see cref="ReimuPluginRev1{TColumnKey}.ManagedColumnInfo"/>.
    /// </typeparam>
    public abstract class ReimuPluginRev2<TColumnKey> : ReimuPluginRev1<TColumnKey>, IReimuPluginRev2
        where TColumnKey : struct, IComparable, IFormattable, IConvertible
    {
        /// <inheritdoc/>
        public new Revision GetPluginRevision()
        {
            return Revision.Rev2;
        }

        /// <inheritdoc/>
        public abstract ErrorCode GetFileInfoImage1(IntPtr src, uint size, out IntPtr dst, out IntPtr info);

        /// <inheritdoc/>
        public abstract ErrorCode GetFileInfoImage2(IntPtr src, uint size, out IntPtr dst, out IntPtr info);
    }
}
