using System.Globalization;

namespace TextRepositoryEditor
{
    public partial class MainForm : Form
    {
        private EditingAction editingAction;
        private readonly Navigator<TreeNode> explorerNavigator;
        private bool importDone;
        private bool isDragging;
        private bool isNavigating;
        private CustomTreeNode? nodeInClipboard;
        private bool refreshSearch;

        public MainForm()
        {
            InitializeComponent();
            LoadAppSettings();
            explorerNavigator = new Navigator<TreeNode>(UpdateControls);
            Explorer.TreeViewNodeSorter = new ExplorerNodeSorter();
            Explorer.Sorted = true;
            SearchBox.Size = new Size(250, 30);
            Timer1.Stop();
            Timer1.Interval = 500;

            NoTextSelectedLabel.Dock = DockStyle.Fill;
            LiteralValueLabel.Dock = DockStyle.Fill;

            PublishedMessageLabel.Visible = false;
        }

        #region IDE Commands

        // ClearTexts
        private void ClearTexts()
        {
            if (TextNode != null && Dialogs.RequestConfirmation("Clear all texts in node?") == DialogResult.Yes)
            {
                TextNode.ClearTexts();
                PopulateGrid();
            }
        }

        // CloseProject
        private void CloseProject()
        {
            ValidationCultureComboBox.Items.Clear();
            explorerNavigator.Clear();
            Explorer.Nodes.Clear();
            UpdateControls();
            InvalidateExplorerContextMenu();
            InvalidateSearchBox();
            UpdateFormText();
            importDone = false;
        }

        // CollapseToImportResult
        private void CollapseToImportResult(ImportResult action)
        {
            if (ProjectNode != null)
            {
                Explorer.BeginUpdate();
                ProjectNode.Collapse(false);
                foreach (var textNode in ProjectNode.GetChildNodesRecursively().OfType<TextNode>())
                {
                    if (textNode.ImportResult == action)
                        textNode.EnsureVisible();
                }
                Explorer.EndUpdate();
            }
        }

        // CopyNode
        private void CopyNode()
        {
            if (Explorer.SelectedNode is CustomTreeNode node && node.CanClone)
            {
                nodeInClipboard = node;
                editingAction = EditingAction.Copy;
            }
        }

        // CutNode
        private void CutNode()
        {
            if (Explorer.SelectedNode is CustomTreeNode node && node.CanClone)
            {
                nodeInClipboard = node;
                editingAction = EditingAction.Cut;
            }
        }

        // DeleteNode
        private void DeleteNode()
        {
            if (ProjectNode == null)
                return;

            if (Explorer.SelectedNode is not CustomTreeNode node)
                return;

            if (!node.AllowRemove)
                return;

            if (Dialogs.RequestConfirmation($"'{node.Text}' will be deleted permanently.", false, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            explorerNavigator.RemoveItem(node);
            foreach (var childNode in node.GetChildNodesRecursively())
            {
                explorerNavigator.RemoveItem(childNode);
            }

            node.Remove();

            var items = SearchResultsGrid.Items.OfType<ListViewItem>();
            foreach (var item in items)
            {
                if (item.Tag == node)
                    item.Remove();
            }

            if (node is LanguagePackageNode)
            {
                PopulateGrid();
                ValidationCultureComboBox.Items.Remove(node);
                ValidationCultureComboBox.SelectedIndex = 0;
            }

            ProjectNode.ValidateAll();
            InvalidateStatusBar();
        }

        // NextError
        private void NextError()
        {
            if (ProjectNode != null)
            {
                var nodes = new List<TreeNode>(ProjectNode.GetChildNodesRecursively());
                nodes.Insert(0, ProjectNode);

                var index = Explorer.SelectedNode == null ? 0 : nodes.IndexOf(Explorer.SelectedNode) + 1;
                if (index == nodes.Count)
                    index = 0;

                for (int i = index; i < nodes.Count - 1; i++)
                {
                    if (nodes[i] is CustomTreeNode customNode && customNode.HasErrors)
                    {
                        Explorer.SelectedNode = nodes[i];
                        break;
                    }
                }
            }
        }

        // NewFolder
        private void NewFolder() => NewNode(new FolderNode(string.Empty));

        // NextImportResult
        private void NextImportResult()
        {
            if (ProjectNode != null)
            {
                var nodes = new List<TreeNode>(ProjectNode.GetChildNodesRecursively());
                nodes.Insert(0, ProjectNode);

                var index = Explorer.SelectedNode == null ? 0 : nodes.IndexOf(Explorer.SelectedNode);

                for (int i = index; i < nodes.Count - 1; i++)
                {
                    if (nodes[i] is TextNode textNode && !nodes[i].IsSelected)
                    {
                        if (textNode.ImportResult == ImportResult.New || textNode.ImportResult == ImportResult.Updated || textNode.ImportResult == ImportResult.NotFound)
                        {
                            Explorer.SelectedNode = nodes[i];
                            break;
                        }
                    }
                }
            }
        }

        // NewLanguagePackage
        private void NewLanguagePackage()
        {
            if (ProjectNode is null)
                throw new InvalidOperationException();

            var form = new NewLanguagePackageForm
            {
                ProjectNode = ProjectNode
            };

            if (form.ShowDialog() == DialogResult.OK && form.SelectedCulture != null)
            {
                var node = new LanguagePackageNode(form.SelectedCulture);
                ProjectNode.Nodes[0].Nodes.Add(node);
                Explorer.SelectedNode = node;
                PopulateGrid();
            }
        }

        // NewProject
        private void NewProject()
        {
            if (ShouldClose())
            {
                CloseProject();
                LoadProject(string.Empty);
            }
        }

        // NewText
        private void NewText() => NewNode(new TextNode("New text"));

        // OpenProject
        private void OpenProject()
        {
            var dialog = Dialogs.CreateOpenProjectDialog();

            if (!string.IsNullOrWhiteSpace(AppSettings.Default.LastOpenProjectPath))
                dialog.InitialDirectory = AppSettings.Default.LastOpenProjectPath;

            if (dialog.ShowDialog() == DialogResult.OK)
                LoadProject(dialog.FileName);
        }

        // PreviousError
        private void PreviousError()
        {
            if (ProjectNode != null)
            {
                var nodes = new List<TreeNode>(ProjectNode.GetChildNodesRecursively());
                nodes.Insert(0, ProjectNode);

                var index = Explorer.SelectedNode == null ? nodes.Count - 1 : nodes.IndexOf(Explorer.SelectedNode) - 1;
                if (index == -1)
                    index = nodes.Count - 1;

                for (int i = index; i >= 0; i--)
                {
                    if (nodes[i] is CustomTreeNode customNode && customNode.HasErrors)
                    {
                        Explorer.SelectedNode = nodes[i];
                        break;
                    }
                }
            }
        }

        // PreviousImportResult
        private void PreviousImportResult()
        {
            if (ProjectNode != null)
            {
                var nodes = new List<TreeNode>(ProjectNode.GetChildNodesRecursively());
                var index = Explorer.SelectedNode == null ? -1 : nodes.IndexOf(Explorer.SelectedNode);

                for (int i = index; i >= 0; i--)
                {
                    if (nodes[i] is TextNode textNode && !nodes[i].IsSelected)
                    {
                        if (textNode.ImportResult == ImportResult.New || textNode.ImportResult == ImportResult.Updated || textNode.ImportResult == ImportResult.NotFound)
                        {
                            Explorer.SelectedNode = nodes[i];
                            break;
                        }
                    }
                }
            }
        }

        // RemoveChildNodes
        private void RemoveChildNodes()
        {
            if (Explorer.SelectedNode is FolderNode && Dialogs.RequestConfirmation("Do you want to remove all child nodes?") == DialogResult.Yes)
            {
                Explorer.BeginUpdate();
                Explorer.SelectedNode.Nodes.Clear();
                PopulateGrid();
                Explorer.EndUpdate();
            }
        }

        // SaveProject
        private void SaveProject()
        {
            if (ProjectNode is ProjectNode projectNode)
            {
                var fileName = projectNode.Path;

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    var dialog = Dialogs.CreateSaveProjectDialog();
                    if (dialog.ShowDialog() == DialogResult.OK)
                        fileName = dialog.FileName;
                }

                projectNode.Save(fileName);

                ProjectStatusLabel.Text = "Saved";
            }
        }

        #endregion

        #region Private members

        // ActivateToolCaption
        private static void ActivateToolCaption(Label obj)
        {
            obj.BackColor = SystemColors.HotTrack;
            obj.ForeColor = Color.White;
        }

        // DeactivateToolCaption
        private static void DeactivateToolCaption(Label obj)
        {
            obj.BackColor = SystemColors.Control;
            obj.ForeColor = SystemColors.ControlText;
        }

        // InvalidateExplorerContextMenu
        private void InvalidateExplorerContextMenu()
        {
            var node = Explorer.SelectedNode;

            ClearMenuItem.Available = TextNode != null;
            RemoveChildNodesMenuItem.Available = FolderNode != null;
            RemoveChildNodesMenuItem.Enabled = FolderNode != null && FolderNode.Nodes.Count > 0;
            NewLanguagePackageMenuItem.Available = node is LanguagePackageFolderNode;
            NewFolderMenuItem.Enabled = node is ProjectNode || node is FolderNode;
            NewTextMenuItem.Enabled = node is FolderNode;
            DeleteMenuItem.Enabled = node is CustomTreeNode customTreeNode && customTreeNode.AllowRemove;
            ImportMenuItem.Available = node is LanguagePackageNode;
            deleteSeparator.Available = DeleteMenuItem.Available;

            var customNode = node as CustomTreeNode;

            CutMenuItem.Enabled = customNode is not null && customNode.CanClone;
            CopyMenuItem.Enabled = CutMenuItem.Enabled;
            PasteMenuItem.Enabled = customNode != null && nodeInClipboard != null && customNode.CanBeParentOf(nodeInClipboard);

            LanguagePackageSeparator.Available = ImportMenuItem.Available || NewLanguagePackageMenuItem.Available;

            toolStripSeparator8.Available = ClearMenuItem.Available || RemoveChildNodesMenuItem.Available;
        }

        // InvalidateGridItem
        private void InvalidateGridItem(ListViewItem item)
        {
            if (item.Tag is not CultureInfo culture || TextNode is null || ProjectNode == null)
                return;

            if (item.SubItems.Count == 1)
            {
                for (int i = 0; i < 2; i++)
                    item.SubItems.Add(string.Empty);
            }

            item.SubItems[1].Text = TextNode.GetText(culture);

            int length = item.SubItems[1].Text.Length;
            item.SubItems[2].Text = length == 0 ? string.Empty : length.ToString();

            item.ToolTipText = item.SubItems[1].Text;
        }

        // InvalidateSearchBox
        private void InvalidateSearchBox()
        {
            if (SearchBox.Focused)
            {
                SearchBox.Clear();
                SearchBox.ForeColor = SystemColors.WindowText;
            }
            else
            {
                SearchBox.Text = "Search Nodes...";
                SearchBox.ForeColor = SystemColors.GrayText;
            }
        }

        // InvalidateSearchResults
        private void InvalidateSearchResults()
        {
            SearchResultsGrid.BeginUpdate();
            try
            {
                SearchResultsGrid.Items.Clear();

                if (ProjectNode == null)
                    return;

                if (SearchBox.Text.Length > 0)
                {
                    foreach (var node in Explorer.Nodes[0].GetChildNodesRecursively().OfType<CustomTreeNode>())
                    {
                        if (node.Text.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase) || node is TextNode textNode && textNode.Contains(null, SearchBox.Text, true))
                        {
                            var item = SearchResultsGrid.Items.Add(node.GetPath(true), node.ImageIndex);
                            item.Tag = node;
                        }
                    }
                }
            }
            finally
            {
                SearchResultsGrid.EndUpdate();
                SearchCaption.Text = SearchResultsGrid.Items.Count == 0 ? ":::: Search Results" : $"Search Results for '{SearchBox.Text}' - {SearchResultsGrid.Items.Count} occurrences.";
            }
        }

        // InvalidateStatusBar 
        private void InvalidateStatusBar()
        {
            if (ProjectNode == null)
            {
                ProjectStatusLabel.Text = string.Empty;
                ProjectValidationStatusLabel.Text = string.Empty;
            }
            else
            {
                ProjectStatusLabel.Text = ProjectNode.HasChanges ? "Changed" : "Unchanged";
                ProjectValidationStatusLabel.Text = "|    " + (ProjectNode.ErrorCount == 0 ? "No validation errors" : $"{ProjectNode.ErrorCount} validation error(s)");
            }
        }

        // FolderNode
        private FolderNode? FolderNode => Explorer.SelectedNode as FolderNode;

        // LoadAppSettings
        private void LoadAppSettings()
        {
            if (AppSettings.Default.WindowWidth == 0 || AppSettings.Default.WindowLeft < -1000)
            {
                StartPosition = FormStartPosition.CenterScreen;
            }
            else
            {
                StartPosition = FormStartPosition.Manual;

                Left = AppSettings.Default.WindowLeft;
                Top = AppSettings.Default.WindowTop;
                Width = AppSettings.Default.WindowWidth;
                Height = AppSettings.Default.WindowHeight;

                if (Enum.TryParse<FormWindowState>(AppSettings.Default.WindowState, out var windowState))
                {
                    if (windowState == FormWindowState.Minimized)
                        windowState = FormWindowState.Normal;

                    WindowState = windowState;
                }

                splitContainer1.SplitterDistance = AppSettings.Default.ExplorerWidth;
                SearchPanel.Height = AppSettings.Default.SearchResultsHeight;

                if (AppSettings.Default.PropertiesPanel > 0)
                    PropertiesPanel.Height = AppSettings.Default.PropertiesPanel;
            }
        }

        // LoadProject
        private void LoadProject(string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName) && !File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            CloseProject();
            var projectNode = new ProjectNode(ProjectChanged);
            Explorer.Nodes.Add(projectNode);

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                Cursor = Cursors.WaitCursor;
                ProjectStatusLoading.Visible = true;
                Refresh();
                projectNode.Load(fileName);
                Explorer.SelectedNode = projectNode;
                ProjectStatusLoading.Visible = false;
                Cursor = Cursors.Default;
            }
            else
            {
                Explorer.SelectedNode = projectNode;
                projectNode.Nodes.Add(new LanguagePackageFolderNode());
                projectNode.Nodes.Add(new FolderNode(FolderNode.ImportsNamePrefix));
                projectNode.ExpandAll();
            }

            Explorer.Focus();
            PopulateGrid();
            PopulateValidationCultureComboBox();
            UpdateControls();
            UpdateFormText();
            AppSettings.Default.LastOpenProjectPath = Path.GetDirectoryName(fileName);
        }

        // NewNode
        private void NewNode(CustomTreeNode node)
        {
            if (Explorer.SelectedNode is not TreeNode parent)
                return;

            parent.Nodes.Add(node);
            node.GenerateUniqueName();
            Explorer.SelectedNode = node;
            explorerNavigator.Add(node);
            ProjectNode?.SortNodes();
            node.BeginEdit();
        }

        // PasteNode
        private void PasteNode()
        {
            if (nodeInClipboard is null || editingAction == EditingAction.None)
                return;

            if (Explorer.SelectedNode is not CustomTreeNode targetNode || !targetNode.CanBeParentOf(nodeInClipboard))
                return;

            if (editingAction == EditingAction.Copy)
            {
                var clonedNode = nodeInClipboard.CloneNode();
                targetNode.Nodes.Add(clonedNode);
                clonedNode.GenerateUniqueName(nodeInClipboard.Text);
                Explorer.SelectedNode = clonedNode;
            }
            else
            {
                nodeInClipboard.Remove();
                targetNode.Nodes.Add(nodeInClipboard);
                editingAction = EditingAction.None;
                Explorer.SelectedNode = nodeInClipboard;
                nodeInClipboard = null;
            }

            InvalidateExplorerContextMenu();
        }

        // ProjectChanged
        private void ProjectChanged()
        {
            InvalidateStatusBar();
        }

        // ProjectNode
        private ProjectNode? ProjectNode => Explorer.Nodes.Count == 0 ? null : Explorer.Nodes[0] as ProjectNode;

        // UpdateControls
        private void UpdateControls()
        {
            SuspendLayout();
            ValidationCultureComboBox.Enabled = ProjectNode != null;
            SearchBox.Enabled = ProjectNode != null;
            SaveMenuItem.Enabled = ProjectNode != null;
            SaveButton.Enabled = ProjectNode != null;
            splitContainer1.Visible = ProjectNode != null;
            NoTextSelectedLabel.Visible = TextNode == null;
            LiteralValueLabel.Visible = TextNode != null && TextNode.IsLiteral;
            NavigateBackwardButton.Enabled = explorerNavigator.CanNavigateBackward;
            NavigateForwardButton.Enabled = explorerNavigator.CanNavigateForward;
            NextErrorButton.Enabled = ProjectNode != null && ProjectNode.ErrorCount > 0;
            NextImportResultButton.Enabled = ProjectNode != null && importDone;
            PreviousErrorButton.Enabled = ProjectNode != null && ProjectNode.ErrorCount > 0;
            PreviousImportResultButton.Enabled = ProjectNode != null && importDone;
            PublishMenuItem.Enabled = ProjectNode != null;
            PublishButton.Enabled = PublishMenuItem.Enabled;
            CloseMenuItem.Enabled = ProjectNode! != null;
            SortButton.Enabled = ProjectNode! != null;
            InvalidateStatusBar();
            ResumeLayout();
        }

        // UpdateFormText
        private void UpdateFormText()
        {
            Text = "Text Repository Editor (Engendro 4.0)";
            if (ProjectNode?.Path is string path)
                Text += " - " + path;
        }

        // SaveAppSettings
        private void SaveAppSettings()
        {
            AppSettings.Default.WindowLeft = Left;
            AppSettings.Default.WindowTop = Top;
            AppSettings.Default.WindowWidth = Width;
            AppSettings.Default.WindowHeight = Height;
            AppSettings.Default.WindowState = WindowState.ToString();
            AppSettings.Default.ExplorerWidth = splitContainer1.SplitterDistance;
            AppSettings.Default.SearchResultsHeight = SearchPanel.Height;
            AppSettings.Default.PropertiesPanel = PropertiesPanel.Height;

            AppSettings.Default.Save();
        }

        // ShouldClose
        private bool ShouldClose()
        {
            // Check if project has changes
            if (ProjectNode == null || !ProjectNode.HasChanges)
            {
                return true;
            }
            else
            {
                DialogResult result = Dialogs.RequestConfirmation("Do you want to save changes?", true, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                    SaveProject();

                return result != DialogResult.Cancel;
            }
        }

        // TextNode
        private TextNode? TextNode => Explorer.SelectedNode as TextNode;

        // PopulateGridValues
        private void PopulateGridValues()
        {
            if (ProjectNode is null)
                return;

            CultureGrid.BeginUpdate();
            try
            {
                foreach (ListViewItem item in CultureGrid.Items)
                {
                    if (item.Tag is CultureInfo culture)
                        item.SubItems[1].Text = TextNode is null ? string.Empty : TextNode.GetText(culture);
                }
            }
            finally
            {
                CultureGrid.EndUpdate();
            }
        }

        // PopulateGrid
        private void PopulateGrid()
        {
            CultureGrid.BeginUpdate();
            try
            {
                CultureGrid.Items.Clear();

                // No project or culture
                if (ProjectNode is null || ProjectNode.GetLanguagePackageNodes().Count == 0)
                    return;

                var nodes = from languagePackageNode in ProjectNode.GetLanguagePackageNodes()
                            orderby languagePackageNode.Culture.DisplayName
                            select languagePackageNode;

                foreach (LanguagePackageNode node in nodes)
                {
                    var item = new ListViewItem(node.Culture.DisplayName) { Tag = node.Culture };
                    item.SubItems.Add(string.Empty);
                    item.SubItems.Add(string.Empty);
                    CultureGrid.Items.Add(item);
                }

                CultureGrid.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);

                if (CultureGrid.Items.Count > 0)
                    CultureGrid.SelectedIndices.Add(0);
            }
            finally
            {
                CultureGrid.EndUpdate();
            }
        }

        // PopulateValidationCultureComboBox
        private void PopulateValidationCultureComboBox()
        {
            ValidationCultureComboBox.Items.Clear();

            if (ProjectNode == null)
                return;

            LanguagePackageNode? selectedItem = null;
            foreach (var package in ProjectNode.GetLanguagePackageNodes())
            {
                ValidationCultureComboBox.Items.Add(package);
                if (ProjectNode.ValidationScope == package.Culture.LCID)
                    selectedItem = package;
            }

            ValidationCultureComboBox.Items.Insert(0, "<Validate All>");
            if (selectedItem == null)
                ValidationCultureComboBox.SelectedIndex = 0;
            else
                ValidationCultureComboBox.SelectedItem = selectedItem;
        }

        #endregion

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void NewMenuItem_Click(object sender, EventArgs e)
        {
            NewProject();
        }

        private void NewLanguagePackageMenuItem_Click(object sender, EventArgs e)
        {
            NewLanguagePackage();
        }

        private void Explorer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && Explorer.GetNodeAt(e.Location) is TreeNode node)
                Explorer.SelectedNode = node;
        }

        private void Explorer_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            e.CancelEdit = e.Node is CustomTreeNode node && !node.AllowLabelEdit;

            if (!e.CancelEdit)
                DeleteMenuItem.ShortcutKeys = Keys.None;
        }

        private void Explorer_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (ProjectNode == null)
                return;

            if (e.Node is CustomTreeNode customTreeNode && customTreeNode.AllowLabelEdit && !string.IsNullOrWhiteSpace(e.Label))
            {
                foreach (var node in customTreeNode.Parent.Nodes.OfType<CustomTreeNode>())
                {
                    if (node == customTreeNode)
                        continue;

                    if (string.Compare(node.Text, e.Label, true) == 0)
                    {
                        if (customTreeNode is FolderNode && node is TextNode ||
                            customTreeNode is TextNode && node is FolderNode)
                            continue;

                        e.CancelEdit = true;
                        Dialogs.ShowMessage("Duplicated name", MessageBoxIcon.Error);
                        return;
                    }
                }

                customTreeNode.Text = e.Label;
            }

            DeleteMenuItem.ShortcutKeys = Keys.Delete;

            ProjectChanged();
        }

        private void Explorer_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (isDragging)
                return;

            (Explorer.SelectedNode as CustomTreeNode)?.Invalidate();
            Explorer.LabelEdit = e.Node is CustomTreeNode node && node.AllowLabelEdit;
            PropertyGrid1.SelectedObject = (e.Node as CustomTreeNode)?.PropertyGridNodeWrapper;
            CultureGrid.Visible = TextNode != null && !TextNode.IsLiteral;
            InvalidateExplorerContextMenu();
            PopulateGridValues();

            if (!isNavigating && e.Node != null)
                explorerNavigator.Add(e.Node);

            UpdateControls();

            Explorer.Focus();
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            SaveProject();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !ShouldClose();

            if (!e.Cancel)
                SaveAppSettings();
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenProject();
        }

        private void NewFolderMenuItem_Click(object sender, EventArgs e)
        {
            NewFolder();
        }

        private void Explorer_AfterExpand(object sender, TreeViewEventArgs e)
        {
            (e.Node as CustomTreeNode)?.Invalidate();
        }

        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            DeleteNode();
        }

        private void Explorer_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            (e.Node as FolderNode)?.Invalidate();
        }

        private void NewTextMenuItem_Click(object sender, EventArgs e)
        {
            NewText();
        }

        private void CultureGrid_DoubleClick(object sender, EventArgs e)
        {
            if (TextNode == null || CultureGrid.SelectedIndices.Count != 1)
                return;

            if (CultureGrid.SelectedItems[0].Tag is CultureInfo culture)
            {
                var form = new TextEditorForm();
                form.Prepare(culture, TextNode);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    InvalidateGridItem(CultureGrid.SelectedItems[0]);
                    InvalidateStatusBar();
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CloseProject();
        }

        private void Explorer_DoubleClick(object sender, EventArgs e)
        {
            if (TextNode != null)
                CultureGrid_DoubleClick(sender, e);
        }

        private void Explorer_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Effect != DragDropEffects.Move || e.Data == null)
                return;

            // Retrieve the client coordinates of the drop location.  
            var targetPoint = Explorer.PointToClient(new Point(e.X, e.Y));

            // Retrieve the node at the drop location.  
            if (Explorer.GetNodeAt(targetPoint) is not CustomTreeNode targetNode)
                return;

            // Retrieve the node that was dragged.
            CustomTreeNode? draggedNode = null;

            if (typeof(TextNode).FullName is string textNodeTypeFullName)
                draggedNode = e.Data.GetData(textNodeTypeFullName) as CustomTreeNode;

            if (draggedNode == null)
            {
                if (typeof(FolderNode).FullName is string folderNodeTypeFullName)
                    draggedNode = e.Data.GetData(folderNodeTypeFullName) as CustomTreeNode;
            }

            if (draggedNode == null || draggedNode == targetNode)
                return;

            if (!targetNode.CanBeParentOf(draggedNode))
                return;

            draggedNode.Remove();
            targetNode.Nodes.Add(draggedNode);

            Explorer.SelectedNode = draggedNode;
        }

        private void Explorer_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void Explorer_DragOver(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse position.  
            var targetPoint = Explorer.PointToClient(new Point(e.X, e.Y));

            // Select the node at the mouse position.  
            isDragging = true;
            Explorer.SelectedNode = Explorer.GetNodeAt(targetPoint);
            isDragging = false;
        }

        private void Explorer_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (Explorer.Nodes.Count == 0)
                return;

            else if (e.Item is CustomTreeNode node && !node.AllowDrag)
                return;

            else if (e.Item is TreeNode)
                DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void NavigateBackwardButton_Click(object sender, EventArgs e)
        {
            isNavigating = true;
            if (explorerNavigator.Back())
                Explorer.SelectedNode = explorerNavigator.CurrentItem;
            isNavigating = false;
        }

        private void NavigateForwardButton_Click(object sender, EventArgs e)
        {
            isNavigating = true;
            if (explorerNavigator.Forward())
                Explorer.SelectedNode = explorerNavigator.CurrentItem;
            isNavigating = false;
        }

        private void PublishMenuItem_Click(object sender, EventArgs e)
        {
            if (ProjectNode != null)
            {
                if (!Directory.Exists(ProjectNode.OutputFolder))
                {
                    Dialogs.ShowMessage($"The output folder {ProjectNode.OutputFolder} does not exist.", MessageBoxIcon.Warning);
                    return;
                }

                if (Publisher.Publish(ProjectNode))
                {
                    PublishedMessageLabel.Visible = true;
                    PublishedMessageTimer.Stop();
                    PublishedMessageTimer.Start();
                    PropertyGrid1.Refresh();
                }
            }
        }

        private void ImportMenuItem_Click(object sender, EventArgs e)
        {
            if (ProjectNode != null && Explorer.SelectedNode is LanguagePackageNode languagePackageNode)
            {
                var dialog = Dialogs.CreateOpenXmlDialog();

                if (!string.IsNullOrWhiteSpace(AppSettings.Default.LastImportedPath))
                    dialog.InitialDirectory = AppSettings.Default.LastImportedPath;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    AppSettings.Default.LastImportedPath = Path.GetDirectoryName(dialog.FileName);
                    var form = new ImportForm(dialog.FileName, ProjectNode, languagePackageNode);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        Explorer.BeginUpdate();
                        Importer.Import(dialog.FileName, languagePackageNode);
                        Explorer.Sort();
                        Explorer.EndUpdate();
                        importDone = true;
                        languagePackageNode.LastImportedFile = dialog.FileName;
                        languagePackageNode.LastImported = DateTime.Now;
                        PropertyGrid1.Refresh();
                    }
                }
            }
        }

        private void ClearMenuItem_Click(object sender, EventArgs e)
        {
            ClearTexts();
        }

        private void CutMenuItem_Click(object sender, EventArgs e)
        {
            CutNode();
        }

        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            CopyNode();
        }

        private void PasteMenuItem_Click(object sender, EventArgs e)
        {
            PasteNode();
        }

        private void SearchBox_Enter(object sender, EventArgs e)
        {
            InvalidateSearchBox();
            SearchResultsGrid_Enter(sender, e);
        }

        private void SearchBox_Leave(object sender, EventArgs e)
        {
            InvalidateSearchBox();
            SearchResultsGrid_Leave(sender, e);
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            refreshSearch = true;
            Timer1.Stop();
            Timer1.Start();
        }

        private void NextErrorButton_Click(object sender, EventArgs e)
        {
            NextError();
        }

        private void PreviousErrorButton_Click(object sender, EventArgs e)
        {
            PreviousError();
        }

        private void RemoveChildNodesMenuItem_Click(object sender, EventArgs e)
        {
            RemoveChildNodes();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (refreshSearch && SearchBox.Focused && SearchBox.Text.Length > 0)
            {
                refreshSearch = false;
                Timer1.Enabled = false;
                InvalidateSearchResults();
            }
        }

        private void SearchResultsGrid_DoubleClick(object sender, EventArgs e)
        {
            if (SearchResultsGrid.SelectedItems.Count > 0)
                Explorer.SelectedNode = SearchResultsGrid.SelectedItems[0].Tag as TreeNode;
        }

        private void Explorer_Enter(object sender, EventArgs e)
        {
            ActivateToolCaption(ProjectExplorerCaption);
        }

        private void Explorer_Leave(object sender, EventArgs e)
        {
            DeactivateToolCaption(ProjectExplorerCaption);
        }

        private void PropertyGrid1_Enter(object sender, EventArgs e)
        {
            ActivateToolCaption(PropertiesCaption);
        }

        private void PropertyGrid1_Leave(object sender, EventArgs e)
        {
            DeactivateToolCaption(PropertiesCaption);
        }

        private void SearchResultsGrid_Enter(object sender, EventArgs e)
        {
            ActivateToolCaption(SearchCaption);
        }

        private void SearchResultsGrid_Leave(object sender, EventArgs e)
        {
            DeactivateToolCaption(SearchCaption);
        }

        private void Explorer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2 && Explorer.SelectedNode is TreeNode node && !node.IsEditing)
                node.BeginEdit();
        }

        private void ClearSearchResultsButton_Click(object sender, EventArgs e)
        {
            SearchResultsGrid.Items.Clear();
            InvalidateSearchResults();
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            if (ShouldClose())
                CloseProject();
        }

        private void CopyNodePathMenuItem_Click(object sender, EventArgs e)
        {
            if (Explorer.SelectedNode is CustomTreeNode node)
                Clipboard.SetText(node.GetPath(true));
        }

        private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateControls();
        }

        private void SortButton_Click(object sender, EventArgs e)
        {
            ProjectNode?.SortNodes();
        }

        private void PublishedMessageTimer_Tick(object sender, EventArgs e)
        {
            PublishedMessageLabel.Visible = false;
        }

        private void ValidationCultureComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProjectNode != null)
            {
                if (ValidationCultureComboBox.SelectedItem is LanguagePackageNode languagePackageNode)
                    ProjectNode.ValidationScope = languagePackageNode.Culture.LCID;
                else
                    ProjectNode.ValidationScope = 0;

                foreach (var node in ProjectNode.GetLanguagePackageNodes())
                {
                    node.AllowValidation = ValidationCultureComboBox.SelectedIndex == 0 || ValidationCultureComboBox.SelectedItem == node;
                }

                ProjectNode.ValidateAll();

                InvalidateStatusBar();
            }
        }

        private void CopyTextMenuItem_Click(object sender, EventArgs e)
        {
            if (CultureGrid.SelectedItems.Count > 0)
                Clipboard.SetText(CultureGrid.SelectedItems[0].SubItems[1].Text);
        }

        private void ContextMenuCultureGrid_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CopyTextMenuItem.Enabled = CultureGrid.SelectedItems.Count > 0;
        }

        private void CollapseChildrenMenuItem_Click(object sender, EventArgs e)
        {
            if (Explorer.SelectedNode is TreeNode node)
                node.Collapse(false);
        }

        private void PreviousImportedNodeButton_Click(object sender, EventArgs e)
        {
            PreviousImportResult();
        }

        private void NextImportedNodeButton_Click(object sender, EventArgs e)
        {
            NextImportResult();
        }

        private void ImportResultFilterUnchangedMenuItem_Click(object sender, EventArgs e)
        {
            CollapseToImportResult(ImportResult.Unchanged);
        }

        private void ImportResultFilterNewMenuItem_Click(object sender, EventArgs e)
        {
            CollapseToImportResult(ImportResult.New);
        }

        private void ImportResultFilterNotFoundMenuItem_Click(object sender, EventArgs e)
        {
            CollapseToImportResult(ImportResult.NotFound);
        }

        private void ImportResultFilterUpdatedMenuItem_Click(object sender, EventArgs e)
        {
            CollapseToImportResult(ImportResult.Updated);
        }
    }
}
