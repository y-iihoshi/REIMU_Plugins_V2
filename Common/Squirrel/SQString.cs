//-----------------------------------------------------------------------
// <copyright file="SQString.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ReimuPlugins.Common.Squirrel
{
    using System;
    using System.IO;
    using ReimuPlugins.Common.Extensions;
    using ReimuPlugins.Common.Properties;

    public sealed class SQString : SQObject, IEquatable<SQString>
    {
        public SQString(string value = "")
            : base(SQObjectType.String)
        {
            this.Value = value;
        }

        public new string Value
        {
            get => base.Value as string;
            private set => base.Value = value;
        }

        public static implicit operator string(SQString sq)
        {
            return sq?.Value ?? default;
        }

        public static SQString Create(BinaryReader reader, bool skipType = false)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (!skipType)
            {
                var type = reader.ReadInt32();
                if (type != (int)SQObjectType.String)
                {
                    throw new InvalidDataException(Resources.InvalidDataExceptionWrongType);
                }
            }

            var size = reader.ReadInt32();
            return new SQString(size > 0 ? Encoding.CP932.GetString(reader.ReadExactBytes(size)) : string.Empty);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SQString);
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode() ^ this.Value.GetHashCode();
        }

        public bool Equals(SQString other)
        {
            return other is not null && this.Type == other.Type && this.Value == other.Value;
        }

        public new string ToString()
        {
            return this.Value;
        }
    }
}
