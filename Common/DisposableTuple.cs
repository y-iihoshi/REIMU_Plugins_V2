//-----------------------------------------------------------------------
// <copyright file="DisposableTuple.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common;

using System;

/// <summary>
/// Provides static methods for creating disposable tuple objects.
/// </summary>
public static class DisposableTuple
{
    /// <summary>
    /// Creates a new disposable 2-tuple, or pair.
    /// </summary>
    /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
    /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
    /// <param name="item1">The value of the first component of the tuple.</param>
    /// <param name="item2">The value of the second component of the tuple.</param>
    /// <returns>
    /// A disposable 2-tuple whose value is (<paramref name="item1"/>, <paramref name="item2"/>).
    /// </returns>
    /// <exception cref="OutOfMemoryException">Failed to create a new instance.</exception>
    public static DisposableTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
    {
        return new DisposableTuple<T1, T2>(item1, item2);
    }
}

/// <summary>
/// Represents a disposable 2-tuple, or pair.
/// </summary>
/// <typeparam name="T1">The type of the tuple's first component.</typeparam>
/// <typeparam name="T2">The type of the tuple's second component.</typeparam>
#pragma warning disable SA1402 // File may only contain a single type
public class DisposableTuple<T1, T2> : Tuple<T1, T2>, IDisposable
#pragma warning restore SA1402 // File may only contain a single type
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableTuple{T1,T2}"/> class.
    /// </summary>
    /// <param name="item1">The value of the tuple's first component.</param>
    /// <param name="item2">The value of the tuple's second component.</param>
    public DisposableTuple(T1 item1, T2 item2)
        : base(item1, item2)
    {
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="DisposableTuple{T1,T2}"/> class.
    /// </summary>
    ~DisposableTuple()
    {
        this.Dispose(false);
    }

    /// <summary>
    /// Implements the <see cref="IDisposable.Dispose"/> method.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the resources of the current instance.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> if calls from the <see cref="Dispose()"/> method; <c>false</c> for the destructor.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            using (var item = this.Item1 as IDisposable)
            {
            }

            using (var item = this.Item2 as IDisposable)
            {
            }
        }
    }
}
