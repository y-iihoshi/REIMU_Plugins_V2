//-----------------------------------------------------------------------
// <copyright file="EditDialog.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
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
