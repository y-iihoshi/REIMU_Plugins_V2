﻿//-----------------------------------------------------------------------
// <copyright file="SQNull.cs" company="None">
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

    public sealed class SQNull : SQObject, IEquatable<SQNull>
    {
        private SQNull()
            : base(SQObjectType.Null)
        {
        }

        public static SQNull Instance { get; } = new SQNull();

        public static SQNull Create(BinaryReader reader, bool skipType = false)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (!skipType)
            {
                var type = reader.ReadInt32();
                if (type != (int)SQObjectType.Null)
                {
                    throw new InvalidDataException(Resources.InvalidDataExceptionWrongType);
                }
            }

            return Instance;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SQNull);
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }

        public bool Equals(SQNull other)
        {
            return other is null ? false : this.Type == other.Type && this.Value == other.Value;
        }
    }
}
