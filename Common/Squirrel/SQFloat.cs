﻿//-----------------------------------------------------------------------
// <copyright file="SQFloat.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ReimuPlugins.Common.Squirrel;

using System;
using System.IO;
using ReimuPlugins.Common.Properties;

public sealed class SQFloat : SQObject, IEquatable<SQFloat>
{
    public SQFloat(float value = default)
        : base(SQObjectType.Float)
    {
        this.Value = value;
    }

    public new float Value
    {
        get => (float)base.Value;
        private set => base.Value = value;
    }

    public static implicit operator float(SQFloat sq)
    {
        return sq?.Value ?? default;
    }

    public static SQFloat Create(BinaryReader reader, bool skipType = false)
    {
        if (reader is null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (!skipType)
        {
            var type = reader.ReadInt32();
            if (type != (int)SQObjectType.Float)
            {
                throw new InvalidDataException(Resources.InvalidDataExceptionWrongType);
            }
        }

        return new SQFloat(reader.ReadSingle());
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as SQFloat);
    }

    public override int GetHashCode()
    {
        return this.Type.GetHashCode() ^ this.Value.GetHashCode();
    }

    public bool Equals(SQFloat other)
    {
        return other is not null && this.Type == other.Type && this.Value == other.Value;
    }

    public float ToSingle()
    {
        return this.Value;
    }
}
