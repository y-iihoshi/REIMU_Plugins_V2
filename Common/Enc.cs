//-----------------------------------------------------------------------
// <copyright file="Enc.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System.Text;

    /// <summary>
    /// Contains read-only instances of <see cref="Encoding"/> class for convenience.
    /// </summary>
    public static class Enc
    {
        /// <summary>
        /// Initializes static members of the <see cref="Enc"/> class.
        /// </summary>
        static Enc()
        {
            CP932 = Encoding.GetEncoding(932);
            UTF8 = Encoding.UTF8;
        }

        /// <summary>
        /// Gets the code page 932 encoding.
        /// </summary>
        public static Encoding CP932 { get; private set; }

        /// <summary>
        /// Gets the UTF-8 encoding.
        /// </summary>
        public static Encoding UTF8 { get; private set; }
    }
}
