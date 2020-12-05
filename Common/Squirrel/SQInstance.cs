//-----------------------------------------------------------------------
// <copyright file="SQInstance.cs" company="None">
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

    public sealed class SQInstance : SQObject
    {
        public SQInstance()
            : base(SQObjectType.Instance)
        {
        }

        public static SQInstance Create(BinaryReader reader, bool skipType = false)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (!skipType)
            {
                var type = reader.ReadInt32();
                if (type != (int)SQObjectType.Instance)
                {
                    throw new InvalidDataException(Resources.InvalidDataExceptionWrongType);
                }
            }

            return new SQInstance();
        }
    }
}
