using System.Windows.Forms;

namespace Engendro
{
    /// <summary>
    /// Dialogs
    /// </summary>
    internal static class Dialogs
    {
        private static readonly string DialogTitle = "Engendro Editor";
        private static readonly string DefaultOutputExtension = "esl";
        private static readonly string DefaultProjectExtension = "engendro";

        // ConfirmDelete
        internal static DialogResult ConfirmDelete(string message)
        {
            return MessageBox.Show(message, DialogTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
        }

        // CreateOpenProjectDialog
        internal static OpenFileDialog CreateOpenProjectDialog()
        {
            return new() { DefaultExt = DefaultProjectExtension, Filter = ProjectFileFilter, Title = "Open Project" };
        }

        // CreateSetOutputDialog
        internal static SaveFileDialog CreateSetOutputDialog()
        {
            return new() { DefaultExt = DefaultOutputExtension, Filter = OutputFileFilter, Title = "Set Project Output" };
        }

        // CreateSaveProjectDialog
        internal static SaveFileDialog CreateSaveProjectDialog()
        {
            return new() { DefaultExt = DefaultProjectExtension, Filter = ProjectFileFilter, Title = "Save Project" };
        }

        // OutputFileFilter
        internal static string OutputFileFilter => $"Engendro Script Library (*.{DefaultOutputExtension})|*.{DefaultOutputExtension}|Any File (*.*)|*.*";

        // ProjectFileFilter
        internal static string ProjectFileFilter => $"Engendro project (*.{DefaultProjectExtension})|*.{DefaultProjectExtension}|Any File (*.*)|*.*";

        // RequestConfirmation
        internal static DialogResult RequestConfirmation(string message)
        {
            return RequestConfirmation(message, false, MessageBoxIcon.Information);
        }

        // RequestConfirmation
        internal static DialogResult RequestConfirmation(string message, bool cancelButton, MessageBoxIcon icon)
        {
            return MessageBox.Show(message, DialogTitle, cancelButton ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.YesNo, icon);
        }

        // ShowMessage
        internal static void ShowMessage(string message, MessageBoxIcon messageBoxIcon)
        {
            MessageBox.Show(message, DialogTitle, MessageBoxButtons.OK, messageBoxIcon);
        }
    }
}
