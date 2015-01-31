//-----------------------------------------------------------------------
// <copyright file="IReimuPluginRev1.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The interface of REIMU plugin (Revision 1).
    /// </summary>
    public interface IReimuPluginRev1
    {
        #region Required Methods

        /// <summary>
        /// Gets the revision of the implemented plugin.
        /// </summary>
        /// <returns>The revision.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "To comply with the REIMU plugin spec.")]
        Revision GetPluginRevision();

        /// <summary>
        /// Gets information about the implemented plugin.
        /// </summary>
        /// <param name="index">
        /// The index of the information.
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>What <paramref name="info"/> indicates</term>
        /// </listheader>
        /// <item>
        /// <description><c>0</c></description>
        /// <description>The plugin's name, version, copyright notation, and so on.</description>
        /// </item>
        /// <item>
        /// <description><c>1</c></description>
        /// <description>The string displaying as the tab title of the REIMU's list view.</description>
        /// </item>
        /// <item>
        /// <description><c>2n + 2</c> (where <c>n</c> >= 0)</description>
        /// <description>
        /// The filename extension(s) supported by the plugin. (e.g. <c>"*.rpy"</c>, <c>"*.jpg;*.png"</c>)
        /// </description>
        /// </item>
        /// <item>
        /// <description><c>2n + 3</c> (where <c>n</c> >= 0)</description>
        /// <description>
        /// The description about the file extension(s) indicated by (<c>2n + 2</c>)-th
        /// <paramref name="info"/>.
        /// </description>
        /// </item>
        /// </list>
        /// <remarks>
        /// (<c>2n + 2</c>)-th and (<c>2n + 3</c>)-th <paramref name="info"/> are used for
        /// <c>OPENFILENAME::lpstrFilter</c> member of Win32 API by the application-side.
        /// </remarks>
        /// </param>
        /// <param name="info">
        /// The address to store the null-terminated string indicating the information above.
        /// <remarks>The encoding shall be the code page 932.</remarks>
        /// </param>
        /// <param name="size">The size (in bytes) of <paramref name="info"/>.</param>
        /// <returns>
        /// If <paramref name="info"/> indicates a null pointer, the size of the string (in bytes, excepts a
        /// null terminator) which will be written into <paramref name="info"/> at the next call.
        /// Otherwise, the size of the string (in bytes, excepts a null terminator) written to
        /// <paramref name="info"/>.
        /// If an error occurred, zero shall be returned.
        /// </returns>
        int GetPluginInfo(int index, IntPtr info, uint size);

        /// <summary>
        /// Gets information about the columns of the REIMU's list view.
        /// </summary>
        /// <param name="info">
        /// The address to store the head address of the <see cref="ColumnInfo"/> array allocated by the
        /// plugin.
        /// <remarks>
        /// The <c>Title</c> field of the last element of the array shall be the string which contains only
        /// one null character.
        /// </remarks>
        /// <remarks>
        /// The plugin shall allocate the memory for the <see cref="ColumnInfo"/> array by calling
        /// the Win32 API <c>LocalAlloc()</c>. The allocated memory will be freed by the application-side by
        /// calling the Win32 API <c>LocalFree()</c>.
        /// </remarks>
        /// </param>
        /// <returns>An error code.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "To comply with the REIMU plugin spec.")]
        ErrorCode GetColumnInfo(out IntPtr info);

        /// <summary>
        /// Checks whether the specified data has the structure supported by the implemented plugin.
        /// </summary>
        /// <param name="src">
        /// <list type="bullet">
        /// <item>
        /// <term><c>IntPtr.Zero</c></term>
        /// <description>
        /// When the application-side wants to get the minimum size (in bytes) of the data, used for the
        /// check, from the head of an input file.
        /// </description>
        /// </item>
        /// <item>
        /// <term>A null-terminated file path</term>
        /// <description>When the input data is a file.</description>
        /// </item>
        /// <item>
        /// <term>A pointer to the input data</term>
        /// <description>When the input data is on a memory.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <param name="size">
        /// <list type="bullet">
        /// <item>
        /// <term>Zero</term>
        /// <description>When the input data is a file.</description>
        /// </item>
        /// <item>
        /// <term>The size of the input data</term>
        /// <description>When the input data is on a memory.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// <term>
        /// The minimum size (in bytes) of the data, used for the check, from the head of an input file
        /// </term>
        /// <description>When <paramref name="src"/> is <c>IntPtr.Zero</c>.</description>
        /// </item>
        /// <item>
        /// <term>Nonzero</term>
        /// <description>The input data has the structure supported by the plugin.</description>
        /// </item>
        /// <item>
        /// <term>Zero</term>
        /// <description>The input data doesn't have the supported structure.</description>
        /// </item>
        /// </list>
        /// </returns>
        uint IsSupported(IntPtr src, uint size);

        /// <summary>
        /// Gets the file information displaying to the REIMU's list view.
        /// </summary>
        /// <param name="src">
        /// <list type="bullet">
        /// <item>
        /// <term>A null-terminated file path</term>
        /// <description>When the input data is a file.</description>
        /// </item>
        /// <item>
        /// <term>A pointer to the input data</term>
        /// <description>When the input data is on a memory.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <param name="size">
        /// <list type="bullet">
        /// <item>
        /// <term>Zero</term>
        /// <description>When the input data is a file.</description>
        /// </item>
        /// <item>
        /// <term>The size of the input data</term>
        /// <description>When the input data is on a memory.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <param name="info">
        /// The address to store the head address of the <see cref="FileInfo"/> array allocated by the plugin.
        /// <remarks>
        /// The number of elements of the array shall be equal to that of the <see cref="ColumnInfo"/> array
        /// (except the last element -- it's a sentinel.) returned by <see cref="GetColumnInfo"/>.
        /// </remarks>
        /// </param>
        /// <returns>An error code.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "To comply with the REIMU plugin spec.")]
        ErrorCode GetFileInfoList(IntPtr src, uint size, out IntPtr info);

        #endregion

        #region Optional Methods

        /// <summary>
        /// Gets the file information displaying in the left-down area of the REIMU's main window.
        /// </summary>
        /// <param name="src">
        /// <list type="bullet">
        /// <item>
        /// <term>A null-terminated file path</term>
        /// <description>When the input data is a file.</description>
        /// </item>
        /// <item>
        /// <term>A pointer to the input data</term>
        /// <description>When the input data is on a memory.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <param name="size">
        /// <list type="bullet">
        /// <item>
        /// <term>Zero</term>
        /// <description>When the input data is a file.</description>
        /// </item>
        /// <item>
        /// <term>The size of the input data</term>
        /// <description>When the input data is on a memory.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <param name="dst">
        /// The address to store the head address of the null-terminated string indicating the file
        /// information.
        /// <remarks>
        /// The plugin shall allocate the memory for <paramref name="dst"/> by calling the Win32 API
        /// <c>LocalAlloc()</c>. The allocated memory will be freed by the application-side by calling the
        /// Win32 API <c>LocalFree()</c>.
        /// </remarks>
        /// <remarks>The encoding shall be the code page 932.</remarks>
        /// </param>
        /// <returns>An error code.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "To comply with the REIMU plugin spec.")]
        ErrorCode GetFileInfoText1(IntPtr src, uint size, out IntPtr dst);

        /// <summary>
        /// Gets the file information displaying in the right-down area of the REIMU's main window.
        /// </summary>
        /// <param name="src">
        /// <list type="bullet">
        /// <item>
        /// <term>A null-terminated file path</term>
        /// <description>When the input data is a file.</description>
        /// </item>
        /// <item>
        /// <term>A pointer to the input data</term>
        /// <description>When the input data is on a memory.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <param name="size">
        /// <list type="bullet">
        /// <item>
        /// <term>Zero</term>
        /// <description>When the input data is a file.</description>
        /// </item>
        /// <item>
        /// <term>The size of the input data</term>
        /// <description>When the input data is on a memory.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <param name="dst">
        /// The address to store the head address of the null-terminated string indicating the file
        /// information.
        /// <remarks>
        /// The plugin shall allocate the memory for <paramref name="dst"/> by calling the Win32 API
        /// <c>LocalAlloc()</c>. The allocated memory will be freed by the application-side by calling the
        /// Win32 API <c>LocalFree()</c>.
        /// </remarks>
        /// <remarks>The encoding shall be the code page 932.</remarks>
        /// </param>
        /// <returns>An error code.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "To comply with the REIMU plugin spec.")]
        ErrorCode GetFileInfoText2(IntPtr src, uint size, out IntPtr dst);

        /// <summary>
        /// Opens the dialog for editing the file selected in the REIMU's list view.
        /// </summary>
        /// <param name="parent">
        /// The handle (the value of <c>HWND</c> type of Win32 API) of the parent window of the dialog.
        /// </param>
        /// <param name="file">The null-terminated path string of the selected file.</param>
        /// <returns>
        /// <see cref="ErrorCode.AllRight"/> if "OK" button is clicked; others when cancelled.
        /// </returns>
        ErrorCode EditDialog(IntPtr parent, string file);

        /// <summary>
        /// Opens the config dialog of the implemented plugin.
        /// </summary>
        /// <param name="parent">
        /// The handle (the value of <c>HWND</c> type of Win32 API) of the parent window of the dialog.
        /// </param>
        /// <returns>
        /// <see cref="ErrorCode.AllRight"/> if "OK" button is clicked; others when cancelled.
        /// </returns>
        ErrorCode ConfigDialog(IntPtr parent);

        #endregion
    }
}
