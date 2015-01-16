//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

using System.Text;

namespace ReimuPlugins.Common
{
    /// <summary>
    /// Indicates the revision of the REIMU plugin interface.
    /// </summary>
    public enum Revision
    {
        Rev1 = 1,
        Rev2
    }

    /// <summary>
    /// The error codes for the REIMU plugin interface methods.
    /// </summary>
    public enum ErrorCode
    {
        NoFunction = -1,
        AllRight,
        NotSupport,
        FileReadError,
        FileWriteError,
        NoMemory,
        UnknownError,
        DialogCanceled
    }

    /// <summary>
    /// The type of a text alignment.
    /// </summary>
    public enum TextAlign
    {
        Left = 0,
        Right,
        Center
    }

    /// <summary>
    /// The type of a sorting method in the REIMU's list view.
    /// </summary>
    public enum SortType
    {
        String = 0,
        Number,
        Float,
        Hex
    }

    /// <summary>
    /// The type of system information displayed in the REIMU's list view.
    /// </summary>
    public enum SystemInfoType
    {
        String = 0,
        Path,
        Directory,
        Title,
        Extension,
        CreateTime,
        LastAccessTime,
        LastWriteTime,
        FileSize
    }

    /// <summary>
    /// Contains read-only instances of <see cref="Encoding"/> class for convenience.
    /// </summary>
    public static class Enc
    {
        /// <summary>
        /// The Shift_JIS encoding.
        /// </summary>
        public static readonly Encoding SJIS = Encoding.GetEncoding("shift_jis");

        /// <summary>
        /// The UTF-8 encoding.
        /// </summary>
        public static readonly Encoding UTF8 = Encoding.UTF8;
    }
}
