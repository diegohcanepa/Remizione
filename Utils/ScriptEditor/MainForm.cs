using Engendro.Nodes;
using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Engendro
{
    public partial class MainForm : Form
    {
        #region Private fields

        private readonly Navigator<TreeNode> documentNavigator;
        private readonly DocumentSelectorForm documentSelectorForm;
        private readonly Navigator<TreeNode> explorerNavigator;
        private bool isDragging;
        private bool isOpeningProject;
        private bool isNavigating;
        private readonly ElementHost textEditorHost;

        #endregion

        public MainForm()
        {
            InitializeComponent();
            documentNavigator = new Navigator<TreeNode>(InvalidateDocumentContainerToolbar);
            explorerNavigator = new Navigator<TreeNode>(InvalidateExplorerToolbar);
            DocumentExplorer.TreeViewNodeSorter = new DocumentNodeSorter();
            DocumentExplorer.Sorted = true;
            ProjectName.Visible = false;
            SearchResultsPanel.Dock = DockStyle.Bottom;
            textEditorHost = new ElementHost { ContextMenuStrip = DocumentContextMenu, Dock = DockStyle.Fill };
            textEditorHost.ChildChanged += TextEditorHost_ChildChanged;
            intellisenseControl1.Visible = false;

            DocumentPanel.Controls.Add(textEditorHost);

            documentSelectorForm = new DocumentSelectorForm();

            LoadAppSettings();

            InvalidateSearchResults();
        }

        private void TextEditorHost_ChildChanged(object? sender, ChildChangedEventArgs e)
        {
            if (textEditorHost.Child != null)
            {
                textEditorHost.Child.KeyDown += Child_KeyDown;
                if (ActiveScriptNode?.SearchPanel != null && !string.IsNullOrWhiteSpace(SearchBox.Text))
                {
                    ActiveScriptNode.SearchPanel.SearchPattern = SearchBox.Text;
                }
            }
        }

        private void Child_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Document selector
            if (e.Key == System.Windows.Input.Key.Tab && (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) || System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl)))
            {
                ShowDocumentSelector();
                e.Handled = true;
            }
        }

        #region Commands

        // DoAddFolderCommand
        private void DoAddFolderCommand()
        {
            if (Explorer.SelectedNode == null)
                return;

            FolderNode folderNode = new("New Folder");
            Explorer.SelectedNode.Nodes.Add(folderNode);
            Explorer.SelectedNode = folderNode;
            explorerNavigator.Add(folderNode);
            folderNode.BeginEdit();
            CurrentProjectNode?.NotifyChange();
        }

        // DoAddScriptCommand
        private void DoAddScriptCommand()
        {
            if (Explorer.SelectedNode == null)
                return;

            ScriptNode scriptNode = new(DocumentExplorer);
            Explorer.SelectedNode.Nodes.Add(scriptNode);
            Explorer.SelectedNode = scriptNode;
            explorerNavigator.Add(scriptNode);
            scriptNode.OpenDocument(true);
            Application.DoEvents();
            textEditorHost.Child?.Focus();
        }

        // DoCloseAllDocumentsCommand
        private void DoCloseAllDocumentsCommand(bool keepSelected = false)
        {
            TreeNode[] nodes = new TreeNode[DocumentExplorer.Nodes.Count];
            DocumentExplorer.Nodes.CopyTo(nodes, 0);

            DocumentExplorer.BeginUpdate();
            foreach (var node in nodes.OfType<DocumentNode>())
            {
                if (node.IsPinned)
                {
                    continue;
                }

                if (keepSelected && node.IsSelected)
                {
                    continue;
                }

                CloseDocumentCore(node);
            }
            DocumentExplorer.EndUpdate();
        }

        // DoCloseDocumentCommand
        private void DoCloseDocumentCommand()
        {
            if (DocumentExplorer.SelectedNode is DocumentNode node)
            {
                CloseDocumentCore(node);
            }
        }

        // DoCloseProjectCommand
        private void DoCloseProjectCommand()
        {
            if (CurrentProjectNode != null)
            {
                CurrentProjectNode.Remove();
                Explorer.Nodes.Clear();
            }

            explorerNavigator.Clear();
            SearchBox.Clear();
            SearchResults.Clear();
            ProjectName.Visible = false;
            SearchMenu.Visible = false;
            splitContainer1.Visible = false;
            InvalidateMainMenu();
            InvalidateScriptBuild();
            InvalidateSearchResults();
            ProjectStatus.Text = string.Empty;
            ExportStatus.Text = string.Empty;
        }

        // DoCollapseAllCommand
        private void DoCollapseAllCommand()
        {
            Explorer.CollapseAll();
        }

        // DoCollapseAllButThisCommand
        private void DoCollapseAllButThisCommand()
        {
            if (Explorer.SelectedNode == null)
                return;

            Explorer.BeginUpdate();
            var node = Explorer.SelectedNode;
            Explorer.CollapseAll();
            Explorer.SelectedNode = node;
            node.Expand();
            Explorer.EndUpdate();
        }

        // DoCopyDocumentNameCommand
        private void DoCopyDocumentNameCommand()
        {
            if (DocumentExplorer.SelectedNode is TreeNode node)
            {
                Clipboard.SetText(node.Text);
            }
        }

        // DoCopyNodeNameCommand
        private void DoCopyNodeNameCommand()
        {
            if (Explorer.SelectedNode is TreeNode node)
            {
                Clipboard.SetText(node.Text);
            }
        }

        // DoDeleteNodeCommand
        private void DoDeleteNodeCommand()
        {
            var confirm = true;
            if (Explorer.SelectedNode is not ObjectModelNode node)
            {
                return;
            }

            ScriptNode? scriptNode = node as ScriptNode;

            if (scriptNode != null && string.IsNullOrWhiteSpace(scriptNode.SourceCode))
            {
                confirm = false;
            }

            if (confirm && Dialogs.RequestConfirmation($"'{node.Text}' will be deleted permanently.", false, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }

            if (node.DocumentNode != null)
            {
                documentNavigator.RemoveItem(node.DocumentNode);
            }

            DoCloseDocumentCommand();

            node.CloseDocument();

            explorerNavigator.RemoveItem(node);
            node.Remove();
            CurrentProjectNode?.NotifyChange();

            SearchResults.BeginUpdate();
            var items = SearchResults.Items.OfType<ListViewItem>();
            foreach (var item in items)
            {
                if (item.Tag == node)
                {
                    item.Remove();
                }
            }
            SearchResults.EndUpdate();

            InvalidateSearchResults();
        }

        // DoGoToNextTaskCommand
        private void DoGoToNextTaskCommand()
        {
            if (CurrentProjectNode == null)
            {
                return;
            }

            List<TreeNode> nodes = new(CurrentProjectNode.GetChildNodesRecursively());
            nodes.Insert(0, CurrentProjectNode);

            var index = Explorer.SelectedNode == null ? 0 : nodes.IndexOf(Explorer.SelectedNode);

            for (var i = index; i < nodes.Count - 1; i++)
            {
                if (nodes[i] is ScriptNode scriptNode && !nodes[i].IsSelected && scriptNode.HasTask)
                {
                    Explorer.SelectedNode = nodes[i];
                    break;
                }
            }
        }

        // DOGoToPreviousTaskCommand
        private void DoGoToPreviousTaskCommand()
        {
            if (CurrentProjectNode == null)
            {
                return;
            }

            List<TreeNode> nodes = new(CurrentProjectNode.GetChildNodesRecursively());
            var index = Explorer.SelectedNode == null ? -1 : nodes.IndexOf(Explorer.SelectedNode);

            for (var i = index; i >= 0; i--)
            {
                if (nodes[i] is ScriptNode scriptNode && !nodes[i].IsSelected && scriptNode.HasTask)
                {
                    Explorer.SelectedNode = nodes[i];
                    break;
                }
            }
        }

        // DoHomeCommand
        private void DoHomeCommand()
        {
            Explorer.SelectedNode = Explorer.Nodes[0];
        }

        // DoNavigateDocumentBackCommand
        private bool DoNavigateDocumentBackCommand()
        {
            var result = false;
            isNavigating = true;
            if (documentNavigator.Back())
            {
                DocumentExplorer.SelectedNode = documentNavigator.CurrentItem;
                result = true;
            }
            isNavigating = false;

            return result;
        }

        // DoNavigateDocumentForwardCommand
        private bool DoNavigateDocumentForwardCommand()
        {
            var result = false;
            isNavigating = true;
            if (documentNavigator.Forward())
            {
                DocumentExplorer.SelectedNode = documentNavigator.CurrentItem;
                result = true;
            }
            isNavigating = false;

            return result;
        }

        // DoNewProjectCommand
        private void DoNewProjectCommand()
        {
            if (ShouldClose())
            {
                DoCloseProjectCommand();
                OpenProject(string.Empty);
            }
        }

        // DoOpenAllSearchResultsDocumentsCommand
        private void DoOpenAllSearchResultsDocumentsCommand()
        {
            DocumentExplorer.BeginUpdate();
            ObjectModelNode.AutomaticallySortDocumentExplorer = false;

            var select = true;
            foreach (var item in SearchResults.Items.OfType<ListViewItem>())
            {
                if (item.Tag is ObjectModelNode objectModelNode)
                {
                    objectModelNode.OpenDocument(select);
                }

                select = false;
            }

            ObjectModelNode.AutomaticallySortDocumentExplorer = true;
            DocumentExplorer.EndUpdate();
            DocumentExplorer.SortTree();
        }

        // DoOpenProjectCommand
        private void DoOpenProjectCommand()
        {
            var dialog = Dialogs.CreateOpenProjectDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                OpenProject(dialog.FileName);
            }
        }

        // DoRenameCommand
        private void DoRenameCommand()
        {
            Explorer.SelectedNode?.BeginEdit();
        }

        // DoSaveCommand
        private void DoSaveCommand()
        {
            if (CurrentProjectNode is ProjectNode projectNode)
            {
                var fileName = projectNode.FileName;

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    var dialog = Dialogs.CreateSaveProjectDialog();
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        fileName = dialog.FileName;
                    }
                }

                projectNode.Save(fileName);
                projectNode.Export();

                ProjectStatus.Text = "Saved";
                ExportStatus.Text = "Output: ";
                if (string.IsNullOrWhiteSpace(projectNode.OutputFileName))
                {
                    ExportStatus.Text += "No output file.";
                }
                else
                {
                    ExportStatus.Text += projectNode.OutputFileName;
                }

                InvalidateScriptBuild();
            }
        }

        // DoSortNodesCommand
        private void DoSortNodesCommand()
        {
            Explorer.SortTree();
            ProjectChanged();
        }

        // DoSyncWithDocumentCommand
        private void DoSyncWithDocumentCommand()
        {
            if (DocumentExplorer.SelectedNode is DocumentNode node)
            {
                SearchBox.Clear();
                Explorer.SelectedNode = node.ObjectModelNode;
                Explorer.SelectedNode.EnsureVisible();
            }
        }

        #endregion

        // ActiveScriptNode
        private ScriptNode? ActiveScriptNode => (DocumentExplorer.SelectedNode as DocumentNode)?.ObjectModelNode as ScriptNode;

        // ActivateToolCaption
        private static void ActivateToolCaption(Label obj)
        {
            obj.BackColor = SystemColors.HotTrack;
            obj.ForeColor = Color.White;
        }

        // CloseDocumentCore
        private void CloseDocumentCore(DocumentNode node)
        {
            CurrentProjectNode?.RemoveDocument(node);
            documentNavigator.RemoveItem(node);
            node.ObjectModelNode.CloseDocument();

            InvalidateDocumentContainer();
        }

        // CreateProjectNode
        private ProjectNode CreateProjectNode()
        {
            return new(DocumentExplorer, ProjectChanged);
        }

        // CurrentProjectNode
        private ProjectNode? CurrentProjectNode => Explorer.Nodes.Count == 0 ? null : Explorer.Nodes[0] as ProjectNode;

        // DeactivateToolCaption
        private static void DeactivateToolCaption(Label obj)
        {
            obj.BackColor = SystemColors.Control;
            obj.ForeColor = SystemColors.ControlText;
        }

        // InvalidateDocumentContainerToolbar
        private void InvalidateDocumentContainerToolbar()
        {
            NavigateDocumentBack.Enabled = documentNavigator.CurrentIndex > 0;
            NavigateDocumentForward.Enabled = documentNavigator.CurrentIndex < documentNavigator.Count - 1;
        }

        // InvalidateDocumentContainer
        private void InvalidateDocumentContainer()
        {
            if (DocumentExplorer.Nodes.Count == 0)
            {
                textEditorHost.Child = null;
                documentNavigator.Clear();
            }

            InvalidateDocumentContainerToolbar();
        }

        // InvalidateExplorerContextMenu
        private void InvalidateExplorerContextMenu()
        {
            var node = Explorer.SelectedNode;

            AddFolder.Enabled = node is ExpandableNode;
            AddScript.Enabled = node is ExpandableNode;
            DeleteNode.Enabled = node is not ProjectNode;
            RenameNode.Enabled = node is ExpandableNode;
        }

        // InvalidateExplorerToolbar
        private void InvalidateExplorerToolbar()
        {
            NavigateExplorerBack.Enabled = explorerNavigator.CurrentIndex > 0;
            NavigateExplorerForward.Enabled = explorerNavigator.CurrentIndex < explorerNavigator.Count - 1;
        }

        // InvalidateMainMenu
        private void InvalidateMainMenu()
        {
            FileClose.Enabled = CurrentProjectNode != null;
            FileSave.Enabled = CurrentProjectNode != null;
            ToolsMenu.Visible = Explorer.Nodes.Count > 0;
        }

        // InvalidateScriptBuild
        private void InvalidateScriptBuild()
        {
            ScriptVersion.Text = CurrentProjectNode is ProjectNode projectNode ? $"Script Build: {projectNode.BuildID}" : string.Empty;
        }

        // InvalidateSearchBox
        private void InvalidateSearchBox()
        {
            SearchResults.BeginUpdate();
            try
            {
                SearchResults.Items.Clear();

                if (SearchBox.Text.Length == 0)
                {
                    return;
                }

                if (SearchBox.Text.Length > 0)
                {
                    foreach (var node in Explorer.Nodes[0].GetChildNodesRecursively().OfType<ScriptNode>())
                    {
                        if (node.SourceCode.Contains(SearchBox.Text, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var item = SearchResults.Items.Add(" " + node.Text, node.ImageIndex);
                            item.Tag = node;
                        }
                    }
                }

                SearchResultsCaption.Text = $":::: Search Results - {SearchResults.Items.Count} occurrences.";
            }
            finally
            {
                SearchResults.EndUpdate();
                ClearSearch.Enabled = SearchBox.Text.Length > 0;
                SearchResultsPanel.Visible = SearchBox.Text.Length > 0;
                InvalidateSearchResults();
            }
        }

        // InvalidateSearchResults
        private void InvalidateSearchResults()
        {
            SearchResultsPanel.Visible = SearchResultsPanel.Visible = SearchBox.Text.Length > 0;
            splitter1.Visible = SearchResultsPanel.Visible;
        }

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
                    {
                        windowState = FormWindowState.Normal;
                    }

                    WindowState = windowState;
                }

                splitContainer1.SplitterDistance = AppSettings.Default.ExplorerWidth;
                SearchResults.Height = AppSettings.Default.SearchResultsHeight;
            }
        }

        // OpenProject
        private void OpenProject(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            isOpeningProject = true;

            DoCloseProjectCommand();
            var projectNode = CreateProjectNode();
            Explorer.Nodes.Add(projectNode);

            if (!string.IsNullOrWhiteSpace(fileName))
                projectNode.Load(fileName);

            ProjectName.Text = projectNode.Text;
            ProjectName.Visible = true;
            SearchMenu.Visible = true;
            splitContainer1.Visible = true;
            explorerNavigator.Clear();
            documentNavigator.Clear();

            InvalidateMainMenu();
            InvalidateExplorerToolbar();
            InvalidateDocumentContainerToolbar();
            InvalidateSearchBox();
            InvalidateScriptBuild();

            ProjectStatus.Text = "Unchanged";
            ExportStatus.Text = string.Empty;

            DoSortNodesCommand();

            if (textEditorHost.Child != null)
            {
                textEditorHost.Child.Focus();
                Application.DoEvents();
            }
            else
            {
                Explorer.Focus();
            }

            isOpeningProject = false;
        }

        // ProjectChanged
        private void ProjectChanged()
        {
            if (isOpeningProject)
            {
                return;
            }

            if (SearchResults.Items.Count > 0)
            {
                SearchResults.BeginUpdate();
                foreach (var item in SearchResults.Items.OfType<ListViewItem>())
                {
                    if (item.Tag is ScriptNode scriptNode)
                    {
                        item.Text = scriptNode.Text;
                    }
                }
                SearchResults.EndUpdate();
            }

            ProjectStatus.Text = "Changed";
            InvalidateScriptBuild();
        }

        // SaveAppSettings
        private void SaveAppSettings()
        {
            AppSettings.Default.LastUsedFile = CurrentProjectNode == null ? string.Empty : CurrentProjectNode.FileName;
            AppSettings.Default.WindowLeft = Left;
            AppSettings.Default.WindowTop = Top;
            AppSettings.Default.WindowWidth = Width;
            AppSettings.Default.WindowHeight = Height;
            AppSettings.Default.WindowState = WindowState.ToString();
            AppSettings.Default.ExplorerWidth = splitContainer1.SplitterDistance;
            AppSettings.Default.SearchResultsHeight = SearchResults.Height;

            AppSettings.Default.Save();
        }

        // SearchResultsSelectedNode
        private TreeNode? SearchResultsSelectedNode => SearchResults.SelectedItems.Count == 0 ? null : SearchResults.SelectedItems[0].Tag as TreeNode;

        // ShouldClose
        private bool ShouldClose()
        {
            // Check if project has changes
            if (CurrentProjectNode == null || !CurrentProjectNode.HasChanges)
            {
                return true;
            }
            else
            {
                var result = Dialogs.RequestConfirmation("Do you want to save changes?", true, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    DoSaveCommand();
                }

                return result != DialogResult.Cancel;
            }
        }

        // ShowDocumentSelector
        private void ShowDocumentSelector()
        {
            if (CurrentProjectNode != null && CurrentProjectNode.Documents.Count > 0)
            {
                documentSelectorForm.Show(CurrentProjectNode);
                documentSelectorForm.SelectedDocument?.ObjectModelNode.OpenDocument(true);
                Application.DoEvents();

                if (textEditorHost.Child is TextEditor textEditor)
                {
                    textEditor.Focus();
                    textEditor.TextArea.Caret.BringCaretToView();
                    Application.DoEvents();
                }
            }
        }

        private void FileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FileNew_Click(object sender, EventArgs e)
        {
            DoNewProjectCommand();
        }

        private void AddFolder_Click(object sender, EventArgs e)
        {
            DoAddFolderCommand();
        }

        private void Explorer_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (isDragging)
            {
                return;
            }

            InvalidateExplorerContextMenu();

            if (e.Node is ScriptNode scriptNode && scriptNode.IsDocumentOpen)
            {
                scriptNode.OpenDocument(true);
            }

            if (!isNavigating && e.Node != null)
            {
                explorerNavigator.Add(e.Node);
            }

            Explorer.Focus();
        }

        private void AddScript_Click(object sender, EventArgs e)
        {
            DoAddScriptCommand();
        }

        private void RenameNode_Click(object sender, EventArgs e)
        {
            DoRenameCommand();
        }

        private void DeleteNode_Click(object sender, EventArgs e)
        {
            DoDeleteNodeCommand();
        }

        private void CopyNodeName_Click(object sender, EventArgs e)
        {
            DoCopyNodeNameCommand();
        }

        private void CollapseAllButThis_Click(object sender, EventArgs e)
        {
            DoCollapseAllButThisCommand();
        }

        private void SyncWithDocumentButton_Click(object sender, EventArgs e)
        {
            DoSyncWithDocumentCommand();
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            InvalidateSearchBox();
        }

        private void FileSave_Click(object sender, EventArgs e)
        {
            DoSaveCommand();
        }

        private void Explorer_AfterExpand(object sender, TreeViewEventArgs e)
        {
            (e.Node as FolderNode)?.Invalidate();
            if (!isOpeningProject)
            {
                ProjectChanged();
            }
        }

        private void Explorer_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            (e.Node as FolderNode)?.Invalidate();
            if (!isOpeningProject)
            {
                ProjectChanged();
            }
        }

        private void FileOpen_Click(object sender, EventArgs e)
        {
            DoOpenProjectCommand();
        }

        private void SearchResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SearchResults.Focused && SearchResultsSelectedNode is ScriptNode scriptNode)
            {
                if (scriptNode.IsDocumentOpen)
                {
                    scriptNode.OpenDocument(true);
                }
            }
        }

        private void Explorer_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            e.CancelEdit = e.Node is ScriptNode;
        }

        private void GoToDefinition_Click(object sender, EventArgs e)
        {
            var node = ActiveScriptNode;
            if (node == null)
            {
                return;
            }

            var word = node.GetCurrentWord();
            var index = word.IndexOf("*", StringComparison.InvariantCultureIgnoreCase);
            if (index != -1)
            {
                word = word[..index];
            }

            foreach (var scriptNode in Explorer.Nodes[0].GetChildNodesRecursively().OfType<ScriptNode>())
            {
                if (scriptNode.IsDefinition && word == scriptNode.Text)
                {
                    scriptNode.OpenDocument(true);
                    textEditorHost.Child?.Focus();
                    return;
                }
            }

            Dialogs.ShowMessage("No script found.", MessageBoxIcon.Information);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !ShouldClose();

            if (!e.Cancel)
            {
                SaveAppSettings();
            }
        }

        private void Explorer_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is ScriptNode scriptNode)
            {
                scriptNode.OpenDocument(true);
            }
        }

        private void Explorer_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            e.CancelEdit = string.IsNullOrWhiteSpace(e.Label);
            if (e.CancelEdit)
            {
                return;
            }

            if (e.Node is ProjectNode)
            {
                ProjectName.Text = e.Label;
            }

            ProjectChanged();
        }

        private void Explorer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (Explorer.SelectedNode is ScriptNode scriptNode)
                {
                    scriptNode.OpenDocument(true);
                    scriptNode.TextEditor?.Focus();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
            }
        }

        private void SearchResults_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && SearchResultsSelectedNode is ScriptNode scriptNode)
            {
                scriptNode.OpenDocument(false);
                scriptNode.TextEditor?.Focus();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void SearchResults_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (SearchResultsSelectedNode is ScriptNode scriptNode)
            {
                scriptNode.OpenDocument(true);
            }
        }

        private void FileClose_Click(object sender, EventArgs e)
        {
            DoCloseProjectCommand();
        }

        private void Explorer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && Explorer.GetNodeAt(e.Location) is TreeNode node)
            {
                Explorer.SelectedNode = node;
            }
        }

        private void Explorer_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Effect != DragDropEffects.Move)
            {
                return;
            }

            // Retrieve the client coordinates of the drop location.  
            var targetPoint = Explorer.PointToClient(new Point(e.X, e.Y));

            // Retrieve the node at the drop location.  
            if (Explorer.GetNodeAt(targetPoint) is not ExpandableNode targetNode)
            {
                return;
            }

            // Retrieve the node that was dragged.
            TreeNode? draggedNode = null;
            if (e.Data != null)
            {
                if (typeof(ScriptNode).FullName is string scriptNodeTypeName && e.Data?.GetData(scriptNodeTypeName) is TreeNode scriptNode)
                {
                    draggedNode = scriptNode;
                }

                if (draggedNode == null)
                {
                    if (typeof(FolderNode).FullName is string folderNodeTypeName && e.Data?.GetData(folderNodeTypeName) is TreeNode folderNode)
                    {
                        draggedNode = folderNode;
                    }
                }
            }

            if (draggedNode == targetNode || draggedNode == null)
            {
                return;
            }

            if (Explorer.IsParentOf(draggedNode, targetNode))
            {
                return;
            }

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
            if (Explorer.Nodes.Count == 0 || e.Item == Explorer.Nodes[0])
            {
                return;
            }
            else if (e.Item is TreeNode)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        private void ProjectSettings_Click(object sender, EventArgs e)
        {
            ProjectSettingsForm form = new() { ProjectNode = CurrentProjectNode };
            form.ShowDialog();
        }

        private void NavigateExplorerBack_Click(object sender, EventArgs e)
        {
            isNavigating = true;
            if (explorerNavigator.Back())
            {
                Explorer.SelectedNode = explorerNavigator.CurrentItem;
            }

            isNavigating = false;
        }

        private void NavigateExplorerForward_Click(object sender, EventArgs e)
        {
            isNavigating = true;
            if (explorerNavigator.Forward())
            {
                Explorer.SelectedNode = explorerNavigator.CurrentItem;
            }

            isNavigating = false;
        }

        private void NavigateDocumentBack_Click(object sender, EventArgs e)
        {
            DoNavigateDocumentBackCommand();
        }

        private void NavigateDocumentForward_Click(object sender, EventArgs e)
        {
            DoNavigateDocumentForwardCommand();
        }

        private void Home_Click(object sender, EventArgs e)
        {
            DoHomeCommand();
        }

        private void CollapseAll_Click(object sender, EventArgs e)
        {
            DoCollapseAllCommand();
        }

        private void SortNodes_Click(object sender, EventArgs e)
        {
            DoSortNodesCommand();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(AppSettings.Default.LastUsedFile))
            {
                OpenProject(AppSettings.Default.LastUsedFile);
            }
            else
            {
                DoCloseProjectCommand();
            }
        }

        private void SearchBox_Enter(object sender, EventArgs e)
        {
            ActivateToolCaption(ProjectExplorerCaption);
            SearchBox.SelectAll();
        }

        private void ClearSearch_Click(object sender, EventArgs e)
        {
            SearchBox.Clear();
        }

        private void OpenAllSearchResultsDocuments_Click(object sender, EventArgs e)
        {
            DoOpenAllSearchResultsDocumentsCommand();
        }

        private void DocumentExplorer_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!isNavigating && !isOpeningProject && DocumentExplorer.SelectedNode != null)
            {
                documentNavigator.Add(DocumentExplorer.SelectedNode);
                CurrentProjectNode?.NotifyChange();
            }

            if (DocumentExplorer.SelectedNode is DocumentNode documentNode && documentNode.ObjectModelNode is ScriptNode scriptNode)
            {
                textEditorHost.Child = scriptNode.TextEditor;
            }

            DocumentExplorer.ContextMenuStrip = DocumentExplorer.SelectedNode == null ? null : DocumentExplorerContextMenu;

            if (e.Node is DocumentNode node)
            {
                CurrentProjectNode?.AddDocument(node);
            }
        }

        private void CloseDocument_Click(object sender, EventArgs e)
        {
            DoCloseDocumentCommand();
        }

        private void DocumentContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = textEditorHost.Child == null;
        }

        private void DocumentExplorerContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = DocumentExplorer.Nodes.Count == 0;
            PinDocumentMenuItem.Checked = DocumentExplorer.SelectedNode is DocumentNode node && node.IsPinned;
        }

        private void CloseAllButThis_Click(object sender, EventArgs e)
        {
            DoCloseAllDocumentsCommand(true);
        }

        private void DocumentExplorer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && DocumentExplorer.GetNodeAt(e.Location) is TreeNode node)
            {
                DocumentExplorer.SelectedNode = node;
            }
        }

        private void CloseAll_Click(object sender, EventArgs e)
        {
            DoCloseAllDocumentsCommand();
        }

        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            // Document Selector
            if (e.KeyCode == Keys.Tab && e.Control)
            {
                ShowDocumentSelector();
                e.Handled = true;
            }
        }

        private void FindInExplorer_Click(object sender, EventArgs e)
        {
            if (ActiveScriptNode is not ScriptNode scriptNode)
            {
                return;
            }

            var word = scriptNode.GetCurrentWord();
            SearchBox.Text = word;
        }

        private void CloseCurrentDocument_Click(object sender, EventArgs e)
        {
            DoCloseDocumentCommand();
            textEditorHost.Focus();
            Application.DoEvents();
            textEditorHost.Child?.Focus();
        }

        private void Explorer_Enter(object sender, EventArgs e)
        {
            ActivateToolCaption(ProjectExplorerCaption);
            if (Explorer.SelectedNode is ScriptNode scriptNode && scriptNode.IsDocumentOpen)
            {
                scriptNode.OpenDocument(true);
            }
        }

        private void Explorer_Leave(object sender, EventArgs e)
        {
            DeactivateToolCaption(ProjectExplorerCaption);
        }

        private void SearchBox_Leave(object sender, EventArgs e)
        {
            DeactivateToolCaption(ProjectExplorerCaption);
        }

        private void SearchResults_Enter(object sender, EventArgs e)
        {
            ActivateToolCaption(SearchResultsCaption);
        }

        private void SearchResults_Leave(object sender, EventArgs e)
        {
            DeactivateToolCaption(SearchResultsCaption);
        }

        private void Explorer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (Explorer.SelectedNode is TreeNode node && node.IsEditing)
            {
                if (!ExplorerContextMenu.Visible && e.KeyCode == Keys.Delete)
                {
                    e.IsInputKey = true;
                }
            }
        }

        private void IntellisensePopup_Click(object sender, EventArgs e)
        {
            if (textEditorHost.Child is TextEditor textEditor)
            {
                if (Explorer.Nodes[0] is ProjectNode projectNode)
                {
                    intellisenseControl1.Show(textEditor, projectNode);
                }
            }
        }

        private void CopyDocumentName_Click(object sender, EventArgs e)
        {
            DoCopyDocumentNameCommand();
        }

        private void PinDocumentMenuItem_Click(object sender, EventArgs e)
        {
            if (DocumentExplorer.SelectedNode is DocumentNode documentNode)
            {
                documentNode.IsPinned = !documentNode.IsPinned;
                DocumentExplorer.SortTree();
            }
        }

        private void SearchMenu_Click(object sender, EventArgs e)
        {
            SearchBox.Focus();
        }

        private void GoToPreviousTask_Click(object sender, EventArgs e)
        {
            DoGoToPreviousTaskCommand();
        }

        private void GoToNextTask_Click(object sender, EventArgs e)
        {
            DoGoToNextTaskCommand();
        }
    }
}
