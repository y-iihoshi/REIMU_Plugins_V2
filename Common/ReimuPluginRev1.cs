//-----------------------------------------------------------------------
// <copyright file="ReimuPluginRev1.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// The base class for the classes implementing the REIMU plugin interface (Revision 1).
    /// </summary>
    /// <typeparam name="TColumnIndex">The key type of <see cref="ManagedColumnInfo"/>.</typeparam>
    public abstract class ReimuPluginRev1<TColumnIndex> : IReimuPluginRev1
        where TColumnIndex : struct, IComparable, IFormattable, IConvertible
    {
        /// <summary>
        /// Gets the information about the plugin implemented by the derived class.
        /// See <see cref="IReimuPluginRev1.GetPluginInfo"/> for details.
        /// </summary>
        protected abstract ReadOnlyCollection<string> ManagedPluginInfo { get; }

        /// <summary>
        /// Gets the information about the columns of the REIMU's list view provided by the derived class.
        /// See <see cref="IReimuPluginRev1.GetColumnInfo"/> for details.
        /// </summary>
        /// <remarks>
        /// Actually, I want to use <c>ReadOnlyDictionary</c>, but it is not available for .NET 4.0...
        /// </remarks>
        protected abstract IDictionary<TColumnIndex, ColumnInfo> ManagedColumnInfo { get; }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "See IReimuPluginRev1.")]
        public Revision GetPluginRevision()
        {
            return Revision.Rev1;
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "See IReimuPluginRev1.")]
        public int GetPluginInfo(int index, IntPtr info, uint size)
        {
            try
            {
                var byteCount = Enc.CP932.GetByteCount(this.ManagedPluginInfo[index]);
                if (info == IntPtr.Zero)
                {
                    return byteCount - 1;   // except a null terminator
                }
                else
                {
                    if (size >= byteCount)
                    {
                        Marshal.Copy(Enc.CP932.GetBytes(this.ManagedPluginInfo[index]), 0, info, byteCount);
                        return byteCount - 1;   // except a null terminator
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            catch (ArgumentNullException)
            {
            }
            catch (EncoderFallbackException)
            {
            }

            return 0;
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "See IReimuPluginRev1.")]
        public ErrorCode GetColumnInfo(out IntPtr info)
        {
            var errorCode = ErrorCode.UnknownError;

            info = IntPtr.Zero;

            try
            {
                var size = Marshal.SizeOf(typeof(ColumnInfo));

                info = Marshal.AllocHGlobal(size * this.ManagedColumnInfo.Count);

                var address = info.ToInt64();
                foreach (var index in Utils.GetEnumerator<TColumnIndex>())
                {
                    var pointer = new IntPtr(address);
                    Marshal.StructureToPtr(this.ManagedColumnInfo[index], pointer, false);
                    address += size;
                }

                errorCode = ErrorCode.AllRight;
            }
            catch (OutOfMemoryException)
            {
                errorCode = ErrorCode.NoMemory;
            }
            catch (ArgumentException)
            {
            }
            catch (OverflowException)
            {
            }

            if (errorCode != ErrorCode.AllRight)
            {
                Marshal.FreeHGlobal(info);
                info = IntPtr.Zero;
            }

            return errorCode;
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "See IReimuPluginRev1.")]
        public abstract uint IsSupported(IntPtr src, uint size);

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "See IReimuPluginRev1.")]
        public abstract ErrorCode GetFileInfoList(IntPtr src, uint size, out IntPtr info);

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "See IReimuPluginRev1.")]
        public abstract ErrorCode GetFileInfoText1(IntPtr src, uint size, out IntPtr dst);

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "See IReimuPluginRev1.")]
        public abstract ErrorCode GetFileInfoText2(IntPtr src, uint size, out IntPtr dst);

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "See IReimuPluginRev1.")]
        public abstract ErrorCode EditDialog(IntPtr parent, string file);

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "See IReimuPluginRev1.")]
        public abstract ErrorCode ConfigDialog(IntPtr parent);
    }
}
