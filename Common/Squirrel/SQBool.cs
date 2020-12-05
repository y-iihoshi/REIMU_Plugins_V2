//-----------------------------------------------------------------------
// <copyright file="SQBool.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ReimuPlugins.Common.Squirrel
{
    using System;
    using System.IO;
    using ReimuPlugins.Common.Properties;

    public sealed class SQBool : SQObject, IEquatable<SQBool>
    {
        private SQBool(bool value = default)
            : base(SQObjectType.Bool)
        {
            this.Value = value;
        }

        public static SQBool True { get; } = new SQBool(true);

        public static SQBool False { get; } = new SQBool(false);

        public new bool Value
        {
            get => (bool)base.Value;
            private set => base.Value = value;
        }

        public static implicit operator bool(SQBool sq)
        {
            return sq?.Value ?? default;
        }

        public static SQBool Create(BinaryReader reader, bool skipType = false)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (!skipType)
            {
                var type = reader.ReadInt32();
                if (type != (int)SQObjectType.Bool)
                {
                    throw new InvalidDataException(Resources.InvalidDataExceptionWrongType);
                }
            }

            return reader.ReadByte() != 0x00 ? True : False;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SQBool);
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode() ^ this.Value.GetHashCode();
        }

        public bool Equals(SQBool other)
        {
            return other is not null && this.Type == other.Type && this.Value == other.Value;
        }

        public bool ToBoolean()
        {
            return this.Value;
        }
    }
}
