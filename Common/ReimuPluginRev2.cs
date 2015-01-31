//-----------------------------------------------------------------------
// <copyright file="ReimuPluginRev2.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The base class for the classes implementing the REIMU plugin interface (Revision 2).
    /// </summary>
    /// <typeparam name="TColumnKey">The key type of <see cref="ManagedColumnInfo"/>.</typeparam>
    public abstract class ReimuPluginRev2<TColumnKey> : ReimuPluginRev1<TColumnKey>, IReimuPluginRev2
        where TColumnKey : struct, IComparable, IFormattable, IConvertible
    {
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "See IReimuPluginRev2.")]
        public new Revision GetPluginRevision()
        {
            return Revision.Rev2;
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "See IReimuPluginRev2.")]
        public abstract ErrorCode GetFileInfoImage1(IntPtr src, uint size, out IntPtr dst, out IntPtr info);

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "See IReimuPluginRev2.")]
        public abstract ErrorCode GetFileInfoImage2(IntPtr src, uint size, out IntPtr dst, out IntPtr info);
    }
}
