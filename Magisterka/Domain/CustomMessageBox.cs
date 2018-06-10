using System;
using System.Windows.Forms;

namespace Domain
{
    public static class CustomMessageBox
    {
        public static DialogResult Show(string text, string message, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(text, message, buttons, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }
    }
}
