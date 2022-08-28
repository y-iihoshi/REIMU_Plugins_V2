//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common;

/// <summary>
/// Indicates the revision of the REIMU plugin interface.
/// </summary>
public enum Revision
{
    /// <summary>
    /// An invalid revision.
    /// </summary>
    Invalid = 0,

    /// <summary>
    /// Revision 1.
    /// </summary>
    Rev1,

    /// <summary>
    /// Revision 2.
    /// </summary>
    Rev2,
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
    DialogCanceled,
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
    Center,
}

/// <summary>
/// The type of a sorting method in the REIMU's list view.
/// </summary>
public enum SortType
{
#pragma warning disable CA1720 // Identifier contains type name
    /// <summary>
    /// String sort.
    /// </summary>
    String = 0,

    /// <summary>
    /// Integer sort.
    /// </summary>
    Integer,
#pragma warning restore CA1720 // Identifier contains type name

    /// <summary>
    /// Floating point sort.
    /// </summary>
    FloatingPoint,

    /// <summary>
    /// Hexadecimal sort.
    /// </summary>
    Hexadecimal,
}

/// <summary>
/// The type of system information displayed in the REIMU's list view.
/// </summary>
public enum SystemInfoType
{
    /// <summary>
    /// Not any system information.
    /// </summary>
    None = 0,

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
    FileSize,
}
