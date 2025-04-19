using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Engendro
{
    public partial class ProjectSettingsForm : Form
    {
        public ProjectSettingsForm()
        {
            InitializeComponent();
        }

        // ProjectNode
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ProjectNode? ProjectNode { get; set; }

        private static readonly char[] separator = new[] { '\n' };

        private void ProjectSettingsForm_Load(object sender, EventArgs e)
        {
            if (ProjectNode != null)
            {
                Output.Text = ProjectNode.OutputFileName;
                BuildID.Value = ProjectNode.BuildID;
                EncryptionKey.Text = ProjectNode.EncryptionKey;
                UseEncryption.Checked = ProjectNode.UseEncryption;

                FileNameLabel.Text = ProjectNode.FileName;

                FoldersLabel.Text = ProjectNode.GetChildNodesRecursively().OfType<FolderNode>().Count().ToString(CultureInfo.InvariantCulture);

                System.Collections.Generic.List<ScriptNode> scriptNodes = ProjectNode.GetChildNodesRecursively().OfType<ScriptNode>().ToList();
                ScriptsLabel.Text = scriptNodes.Count.ToString(CultureInfo.InvariantCulture);

                var lineCount = 0;
                foreach (var script in scriptNodes)
                {
                    // Split the string into an array of lines, ignoring empty entries
                    var lines = script.SourceCode.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                    // Count the number of non-empty lines
                    lineCount += lines.Length;
                }

                LinesLabel.Text = lineCount.ToString(CultureInfo.InvariantCulture);
            }

            Output.Focus();
        }

        private void Accept_Click(object sender, EventArgs e)
        {
            if (ProjectNode != null)
            {
                ProjectNode.OutputFileName = Output.Text;
                ProjectNode.BuildID = (int)BuildID.Value;
                ProjectNode.EncryptionKey = EncryptionKey.Text;
                ProjectNode.UseEncryption = UseEncryption.Checked;
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            var dialog = Dialogs.CreateSetOutputDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Output.Text = dialog.FileName;
            }
        }
    }
}
