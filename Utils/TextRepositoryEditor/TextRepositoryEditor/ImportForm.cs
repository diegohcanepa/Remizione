namespace TextRepositoryEditor
{
    public partial class ImportForm : Form
    {
        public ImportForm()
        {
            InitializeComponent();
            label1.Location = ListView1.Location;
            label1.Size = ListView1.Size;
        }

        public ImportForm(string fileName, ProjectNode projectNode, LanguagePackageNode languagePackageNode)
            : this()
        {
            Text = $"Import Preview ({languagePackageNode.Text})";

            ListView1.BeginUpdate();

            var allNodes = new Dictionary<string, TextNode>();

            foreach (var node in projectNode.GetChildNodesRecursively().OfType<TextNode>())
            {
                allNodes[node.GetPath(true)] = node;
            }

            var importDictionary = Importer.PreviewImport(fileName);

            foreach (var keyValue in importDictionary)
            {
                var key = keyValue.Key;
                if (keyValue.Key.StartsWith("IMPORTS."))
                    key = key.Replace("IMPORTS", FolderNode.ImportsNamePrefix);

                var isNew = !allNodes.TryGetValue(key, out TextNode? textNode);
                if (textNode != null && textNode.GetText(languagePackageNode.Culture) == TextNode.NormalizeText(keyValue.Value))
                    continue;

                var item = new ListViewItem(key);
                item.SubItems.Add(keyValue.Value);
                item.Group = ListView1.Groups[isNew ? 0 : 1];
                if (textNode != null)
                    item.ToolTipText = textNode.GetText(languagePackageNode.Culture);
                ListView1.Items.Add(item);
            }

            ListView1.Groups[0].Header = $"Additions ({ListView1.Groups[0].Items.Count})";
            ListView1.Groups[1].Header = $"Updates ({ListView1.Groups[1].Items.Count})";

            ListView1.EndUpdate();

            ImportButton.Enabled = ListView1.Items.Count > 0;
            ListView1.Visible = ImportButton.Enabled;
            label1.Visible = !ImportButton.Enabled;

            UpdateControls();
        }

        private void UpdateControls()
        {
            if (ListView1.SelectedItems.Count == 0)
            {
                CurrentValueTitleLabel.Visible = false;
                CurrentValueLabel.Text = string.Empty;
                toolTip1.RemoveAll();
            }
            else
            {
                CurrentValueTitleLabel.Visible = true;
                CurrentValueLabel.Text = ListView1.SelectedItems[0].ToolTipText;
                toolTip1.SetToolTip(CurrentValueLabel, CurrentValueLabel.Text);
            }

            CurrentValueTitleLabel.Visible = !string.IsNullOrEmpty(CurrentValueLabel.Text);
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }
    }
}
