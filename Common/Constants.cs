﻿//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text;

[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:FileHeaderFileNameDocumentationMustMatchTypeName", Justification = "Reviewed.")]

namespace ReimuPlugins.Common
{
    /// <summary>
    /// Indicates the revision of the REIMU plugin interface.
    /// </summary>
    public enum Revision
    {
        /// <summary>
        /// Revision 1.
        /// </summary>
        Rev1 = 1,

        /// <summary>
        /// Revision 2.
        /// </summary>
        Rev2
    }

    /// <summary>
    /// The error codes for the REIMU plugin interface methods.
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// The function is not implemented.
        /// </summary>
        NoFunction = -1,

        /// <summary>
        /// Finished successfully.
        /// </summary>
        AllRight,

        /// <summary>
        /// The format is not supported.
        /// </summary>
        NotSupport,

        /// <summary>
        /// An error occurred while reading a file.
        /// </summary>
        FileReadError,

        /// <summary>
        /// An error occurred while writing a file.
        /// </summary>
        FileWriteError,

        /// <summary>
        /// Could not allocate memory.
        /// </summary>
        NoMemory,

        /// <summary>
        /// An unknown error occurred.
        /// </summary>
        UnknownError,

        /// <summary>
        /// Cancelled in the edit/config dialog.
        /// </summary>
        DialogCanceled
    }

    /// <summary>
    /// The type of a text alignment.
    /// </summary>
    public enum TextAlign
    {
        /// <summary>
        /// Text is left-aligned.
        /// <remarks>The value must be equal to <c>LCVFMT_LEFT</c> of Win32 API.</remarks>
        /// </summary>
        Left = 0,

        /// <summary>
        /// Text is right-aligned.
        /// <remarks>The value must be equal to <c>LCVFMT_RIGHT</c> of Win32 API.</remarks>
        /// </summary>
        Right,

        /// <summary>
        /// Text is centered.
        /// <remarks>The value must be equal to <c>LCVFMT_CENTER</c> of Win32 API.</remarks>
        /// </summary>
        Center
    }

    /// <summary>
    /// The type of a sorting method in the REIMU's list view.
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// String sort.
        /// </summary>
        String = 0,

        /// <summary>
        /// Integer sort.
        /// </summary>
        Number,

        /// <summary>
        /// Floating point sort.
        /// </summary>
        Float,

        /// <summary>
        /// Hexadecimal sort.
        /// </summary>
        Hex
    }

    /// <summary>
    /// The type of system information displayed in the REIMU's list view.
    /// </summary>
    public enum SystemInfoType
    {
        /// <summary>
        /// Not any system information.
        /// </summary>
        String = 0,

        /// <summary>
        /// The file path.
        /// </summary>
        Path,

        /// <summary>
        /// The directory path.
        /// </summary>
        Directory,

        /// <summary>
        /// The filename except the extension.
        /// </summary>
        Title,

        /// <summary>
        /// The filename extension.
        /// </summary>
        Extension,

        /// <summary>
        /// Create time of the file.
        /// </summary>
        CreateTime,

        /// <summary>
        /// Last access time of the file.
        /// </summary>
        LastAccessTime,

        /// <summary>
        /// Last write time of the file.
        /// </summary>
        LastWriteTime,

        /// <summary>
        /// File size (in bytes).
        /// </summary>
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