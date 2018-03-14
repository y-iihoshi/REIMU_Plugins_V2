//-----------------------------------------------------------------------
// <copyright file="EditDialog.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ReimuPlugins.Common
{
    using System.Windows.Forms;

    /// <summary>
    /// Provides an edit dialog.
    /// </summary>
    public partial class EditDialog : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditDialog"/> class.
        /// </summary>
        public EditDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a content of the textbox on the dialog.
        /// </summary>
        public string Content
        {
            get { return this.textEdit.Text; }
            set { this.textEdit.Text = value; }
        }
    }
}
