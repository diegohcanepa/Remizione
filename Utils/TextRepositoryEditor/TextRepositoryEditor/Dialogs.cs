namespace TextRepositoryEditor
{
    /// <summary>
    /// Dialogs
    /// </summary>
    internal static class Dialogs
    {
        // ConfirmDelete
        internal static DialogResult ConfirmDelete(string message)
        {
            return MessageBox.Show(message, DialogTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
        }

        // CreateOpenProjectDialog
        internal static OpenFileDialog CreateOpenProjectDialog() => new() { DefaultExt = ProjectExtension, Filter = ProjectFileFilter, Title = "Open Project" };

        // CreateOpenXmlDialog
        internal static OpenFileDialog CreateOpenXmlDialog() => new() { DefaultExt = XmlExtension, Filter = XmlFileFilter, Title = "Open Import File" };

        // CreateSetOutputDialog
        internal static SaveFileDialog CreateSetOutputDialog() => new() { DefaultExt = DefaultOutputExtension, Filter = OutputFileFilter, Title = "Set Project Output" };

        // CreateSaveProjectDialog
        internal static SaveFileDialog CreateSaveProjectDialog() => new() { DefaultExt = ProjectExtension, Filter = ProjectFileFilter, Title = "Save Project" };

        // DefaultOutputExtension
        internal const string DefaultOutputExtension = "lpkg";

        // DialogTitle
        internal const string DialogTitle = "Text Repository Editor";

        // OutputFileFilter
        internal static string OutputFileFilter => $"Engendro Script Library (*.{DefaultOutputExtension})|*.{DefaultOutputExtension}|Any File (*.*)|*.*";

        // ProjectExtension
        internal const string ProjectExtension = "textrepository";

        // ProjectFileFilter
        internal static string ProjectFileFilter => $"Engendro project (*.{ProjectExtension})|*.{ProjectExtension}|Any File (*.*)|*.*";

        // RequestConfirmation
        internal static DialogResult RequestConfirmation(string message) => RequestConfirmation(message, false, MessageBoxIcon.Information);

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

        // XmlExtension
        internal const string XmlExtension = "xml";

        // XmlFileFilter
        internal static string XmlFileFilter => $"Xml file (*.{XmlExtension})|*.{XmlExtension}|Any File (*.*)|*.*";
    }
}
