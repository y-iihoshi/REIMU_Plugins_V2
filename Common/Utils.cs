//-----------------------------------------------------------------------
// <copyright file="Utils.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides static methods for convenience.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Gets the <c>IEnumerable{T}</c> instance to enumerate values of the <typeparamref name="TEnum"/>
        /// type.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type.</typeparam>
        /// <returns>
        /// The <c>IEnumerable{T}</c> instance to enumerate values of the <typeparamref name="TEnum"/> type.
        /// </returns>
        [CLSCompliant(false)]
        public static IEnumerable<TEnum> GetEnumerator<TEnum>()
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }
    }
}
