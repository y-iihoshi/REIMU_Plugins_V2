//-----------------------------------------------------------------------
// <copyright file="IReimuPluginRev2.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The interface of REIMU plugin (Revision 2).
    /// </summary>
    public interface IReimuPluginRev2 : IReimuPluginRev1
    {
        #region Optional Methods

        /// <summary>
        /// Gets the image displaying in the left-down area of the REIMU's main window.
        /// <remarks>This method or <see cref="GetFileInfoText1"/> shall exist exclusively.</remarks>
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
        /// The address to store the head address of the allocated bitmap data.
        /// <remarks>
        /// The plugin shall allocate the memory for <paramref name="dst"/> by calling the Win32 API
        /// <c>LocalAlloc()</c>. The allocated memory will be freed by the application-side by calling the
        /// Win32 API <c>LocalFree()</c>.
        /// </remarks>
        /// </param>
        /// <param name="info">
        /// The address to store the head address of the allocated <see cref="BitmapInfo"/> instance.
        /// <remarks>
        /// The plugin shall allocate the memory for <paramref name="info"/> by calling the Win32 API
        /// <c>LocalAlloc()</c>. The allocated memory will be freed by the application-side by calling the
        /// Win32 API <c>LocalFree()</c>.
        /// </remarks>
        /// </param>
        /// <returns>An error code.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Reviewed.")]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Reviewed.")]
        ErrorCode GetFileInfoImage1(IntPtr src, uint size, out IntPtr dst, out IntPtr info);

        /// <summary>
        /// Gets the image displaying in the right-down area of the REIMU's main window.
        /// <remarks>This method or <see cref="GetFileInfoText2"/> shall exist exclusively.</remarks>
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
        /// The address to store the head address of the allocated bitmap data.
        /// <remarks>
        /// The plugin shall allocate the memory for <paramref name="dst"/> by calling the Win32 API
        /// <c>LocalAlloc()</c>. The allocated memory will be freed by the application-side by calling the
        /// Win32 API <c>LocalFree()</c>.
        /// </remarks>
        /// </param>
        /// <param name="info">
        /// The address to store the head address of the allocated <see cref="BitmapInfo"/> instance.
        /// <remarks>
        /// The plugin shall allocate the memory for <paramref name="info"/> by calling the Win32 API
        /// <c>LocalAlloc()</c>. The allocated memory will be freed by the application-side by calling the
        /// Win32 API <c>LocalFree()</c>.
        /// </remarks>
        /// </param>
        /// <returns>An error code.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Reviewed.")]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Reviewed.")]
        ErrorCode GetFileInfoImage2(IntPtr src, uint size, out IntPtr dst, out IntPtr info);

        #endregion
    }
}
