//-----------------------------------------------------------------------
// <copyright file="BitmapLock.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common;

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Permissions;

/// <summary>
/// Encapsulates lock and unlock calls for a <see cref="Bitmap"/> object by using the dispose pattern.
/// </summary>
public sealed class BitmapLock : IDisposable
{
    /// <summary>
    /// A <see cref="Bitmap"/> which is locked into system memory.
    /// </summary>
    private Bitmap bitmap;

    /// <summary>
    /// A <see cref="BitmapData"/> that specifies information about the lock operation.
    /// </summary>
    private BitmapData data;

    /// <summary>
    /// Initializes a new instance of the <see cref="BitmapLock"/> class.
    /// </summary>
    /// <param name="bitmap">A <see cref="Bitmap"/> which is locked into system memory.</param>
    /// <param name="mode">
    /// An <see cref="ImageLockMode"/> enumeration that specifies the access level (read/write) for
    /// the <see cref="Bitmap"/>.
    /// </param>
    public BitmapLock(Bitmap bitmap, ImageLockMode mode)
    {
        var permission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
        permission.Demand();

        this.bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
        this.data = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height), mode, bitmap.PixelFormat);
    }

    /// <summary>
    /// Gets the address of the first pixel data in the bitmap.
    /// This can also be thought of as the first scan line in the bitmap.
    /// </summary>
    public IntPtr Scan0 => this.data != null ? this.data.Scan0 : IntPtr.Zero;

    /// <summary>
    /// Gets the stride width (also called scan width) of the <see cref="Bitmap"/> object.
    /// </summary>
    public int Stride => this.data != null ? this.data.Stride : 0;

    /// <summary>
    /// Implements the <see cref="IDisposable.Dispose"/> method.
    /// </summary>
    public void Dispose()
    {
        if ((this.bitmap != null) && (this.data != null))
        {
            this.bitmap.UnlockBits(this.data);
            this.data = null;
            this.bitmap = null;
        }
    }
}
