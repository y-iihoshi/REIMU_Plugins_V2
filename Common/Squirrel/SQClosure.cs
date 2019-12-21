//-----------------------------------------------------------------------
// <copyright file="SQClosure.cs" company="None">
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

    public sealed class SQClosure : SQObject
    {
        public SQClosure()
            : base(SQObjectType.Closure)
        {
        }

        public static SQClosure Create(BinaryReader reader, bool skipType = false)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (!skipType)
            {
                var type = reader.ReadInt32();
                if (type != (int)SQObjectType.Closure)
                {
                    throw new InvalidDataException(Resources.InvalidDataExceptionWrongType);
                }
            }

            return new SQClosure();
        }
    }
}
