//-----------------------------------------------------------------------
// <copyright file="Win32Window.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Converter from a <c>HWND</c> value to a <see cref="System.Windows.Forms.IWin32Window"/> instance.
    /// </summary>
    public class Win32Window : IWin32Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Win32Window"/> class.
        /// </summary>
        /// <param name="handle">The handle (the value of <c>HWND</c> type of Win32 API).</param>
        public Win32Window(IntPtr handle)
        {
            this.Handle = handle;
        }

        /// <summary>
        /// Gets the window handle for the current instance.
        /// </summary>
        public IntPtr Handle { get; private set; }
    }
}
