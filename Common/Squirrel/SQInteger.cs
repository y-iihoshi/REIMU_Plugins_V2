﻿//-----------------------------------------------------------------------
// <copyright file="SQInteger.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

namespace ReimuPlugins.Common.Squirrel
{
    using System;
    using System.IO;
    using ReimuPlugins.Common.Properties;

    public sealed class SQInteger : SQObject, IEquatable<SQInteger>
    {
        public SQInteger(int value = default)
            : base(SQObjectType.Integer)
        {
            this.Value = value;
        }

        public new int Value
        {
            get => (int)base.Value;
            private set => base.Value = value;
        }

        public static implicit operator int(SQInteger sq)
        {
            return sq?.Value ?? default;
        }

        public static SQInteger Create(BinaryReader reader, bool skipType = false)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (!skipType)
            {
                var type = reader.ReadInt32();
                if (type != (int)SQObjectType.Integer)
                {
                    throw new InvalidDataException(Resources.InvalidDataExceptionWrongType);
                }
            }

            return new SQInteger(reader.ReadInt32());
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SQInteger);
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode() ^ this.Value.GetHashCode();
        }

        public bool Equals(SQInteger other)
        {
            return !(other is null) && this.Type == other.Type && this.Value == other.Value;
        }

        public int ToInt32()
        {
            return this.Value;
        }
    }
}
