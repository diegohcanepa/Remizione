namespace Engendro
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            MainMenu = new System.Windows.Forms.MenuStrip();
            FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            FileNew = new System.Windows.Forms.ToolStripMenuItem();
            FileOpen = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            FileClose = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            FileSave = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            FileExit = new System.Windows.Forms.ToolStripMenuItem();
            ToolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            ProjectSettings = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            SearchMenu = new System.Windows.Forms.ToolStripMenuItem();
            ProjectName = new System.Windows.Forms.ToolStripTextBox();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            Explorer = new System.Windows.Forms.TreeView();
            ExplorerContextMenu = new System.Windows.Forms.ContextMenuStrip(components);
            RenameNode = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            CollapseAllButThis = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            CopyNodeName = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            AddNode = new System.Windows.Forms.ToolStripMenuItem();
            AddFolder = new System.Windows.Forms.ToolStripMenuItem();
            AddScript = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            DeleteNode = new System.Windows.Forms.ToolStripMenuItem();
            ExplorerTreeImageList = new System.Windows.Forms.ImageList(components);
            ExplorerTreeStateImageList = new System.Windows.Forms.ImageList(components);
            panel2 = new System.Windows.Forms.Panel();
            panel3 = new System.Windows.Forms.Panel();
            SearchBox = new System.Windows.Forms.TextBox();
            ClearSearch = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            ExplorerToolStrip = new System.Windows.Forms.ToolStrip();
            Home = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            NavigateExplorerBack = new System.Windows.Forms.ToolStripButton();
            NavigateExplorerForward = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            GoToPreviousTask = new System.Windows.Forms.ToolStripButton();
            GoToNextTask = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            SortNodes = new System.Windows.Forms.ToolStripButton();
            CollapseAll = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            SyncWithDocumentButton = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            ProjectExplorerCaption = new System.Windows.Forms.Label();
            DocumentPanel = new System.Windows.Forms.Panel();
            intellisenseControl1 = new IntellisenseControl();
            splitter2 = new System.Windows.Forms.Splitter();
            DocumentExplorerPanel = new System.Windows.Forms.Panel();
            DocumentExplorer = new System.Windows.Forms.TreeView();
            DocumentExplorerStateImageList = new System.Windows.Forms.ImageList(components);
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            NavigateDocumentForward = new System.Windows.Forms.ToolStripButton();
            NavigateDocumentBack = new System.Windows.Forms.ToolStripButton();
            DocumentExplorerContextMenu = new System.Windows.Forms.ContextMenuStrip(components);
            CloseDocument = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            CloseAll = new System.Windows.Forms.ToolStripMenuItem();
            CloseAllButThis = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            PinDocumentMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            CopyDocumentName = new System.Windows.Forms.ToolStripMenuItem();
            SearchResultsPanel = new System.Windows.Forms.Panel();
            SearchResults = new System.Windows.Forms.ListView();
            SearchResultsContextMenu = new System.Windows.Forms.ContextMenuStrip(components);
            OpenAllSearchResultsDocuments = new System.Windows.Forms.ToolStripMenuItem();
            panel4 = new System.Windows.Forms.Panel();
            SearchResultsCaption = new System.Windows.Forms.Label();
            DocumentContextMenu = new System.Windows.Forms.ContextMenuStrip(components);
            GoToDefinition = new System.Windows.Forms.ToolStripMenuItem();
            ShowIntellisensePopup = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            FindInExplorer = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            CloseCurrentDocument = new System.Windows.Forms.ToolStripMenuItem();
            StatusBar = new System.Windows.Forms.StatusStrip();
            ProjectStatus = new System.Windows.Forms.ToolStripStatusLabel();
            ScriptVersion = new System.Windows.Forms.ToolStripStatusLabel();
            ExportStatus = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            splitter1 = new System.Windows.Forms.Splitter();
            MainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ExplorerContextMenu.SuspendLayout();
            panel3.SuspendLayout();
            ExplorerToolStrip.SuspendLayout();
            DocumentPanel.SuspendLayout();
            DocumentExplorerPanel.SuspendLayout();
            toolStrip1.SuspendLayout();
            DocumentExplorerContextMenu.SuspendLayout();
            SearchResultsPanel.SuspendLayout();
            SearchResultsContextMenu.SuspendLayout();
            DocumentContextMenu.SuspendLayout();
            StatusBar.SuspendLayout();
            SuspendLayout();
            // 
            // MainMenu
            // 
            MainMenu.AutoSize = false;
            MainMenu.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { FileMenu, ToolsMenu, toolStripMenuItem1, SearchMenu, ProjectName });
            MainMenu.Location = new System.Drawing.Point(5, 5);
            MainMenu.Name = "MainMenu";
            MainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            MainMenu.Size = new System.Drawing.Size(1022, 34);
            MainMenu.TabIndex = 2;
            MainMenu.Text = "menuStrip1";
            // 
            // FileMenu
            // 
            FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { FileNew, FileOpen, toolStripSeparator1, FileClose, toolStripSeparator2, FileSave, toolStripSeparator6, FileExit });
            FileMenu.Name = "FileMenu";
            FileMenu.Size = new System.Drawing.Size(39, 30);
            FileMenu.Text = "&File";
            // 
            // FileNew
            // 
            FileNew.Image = (System.Drawing.Image)resources.GetObject("FileNew.Image");
            FileNew.Name = "FileNew";
            FileNew.Size = new System.Drawing.Size(208, 22);
            FileNew.Text = "New Project";
            FileNew.Click += FileNew_Click;
            // 
            // FileOpen
            // 
            FileOpen.Image = (System.Drawing.Image)resources.GetObject("FileOpen.Image");
            FileOpen.Name = "FileOpen";
            FileOpen.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;
            FileOpen.Size = new System.Drawing.Size(208, 22);
            FileOpen.Text = "Open Project...";
            FileOpen.Click += FileOpen_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(205, 6);
            // 
            // FileClose
            // 
            FileClose.Image = (System.Drawing.Image)resources.GetObject("FileClose.Image");
            FileClose.Name = "FileClose";
            FileClose.Size = new System.Drawing.Size(208, 22);
            FileClose.Text = "Close Project";
            FileClose.Click += FileClose_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(205, 6);
            // 
            // FileSave
            // 
            FileSave.Image = (System.Drawing.Image)resources.GetObject("FileSave.Image");
            FileSave.Name = "FileSave";
            FileSave.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S;
            FileSave.Size = new System.Drawing.Size(208, 22);
            FileSave.Text = "&Save";
            FileSave.Click += FileSave_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new System.Drawing.Size(205, 6);
            // 
            // FileExit
            // 
            FileExit.Name = "FileExit";
            FileExit.Size = new System.Drawing.Size(208, 22);
            FileExit.Text = "Exit";
            FileExit.Click += FileExit_Click;
            // 
            // ToolsMenu
            // 
            ToolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { ProjectSettings });
            ToolsMenu.Name = "ToolsMenu";
            ToolsMenu.Size = new System.Drawing.Size(51, 30);
            ToolsMenu.Text = "&Tools";
            // 
            // ProjectSettings
            // 
            ProjectSettings.Image = (System.Drawing.Image)resources.GetObject("ProjectSettings.Image");
            ProjectSettings.Name = "ProjectSettings";
            ProjectSettings.Size = new System.Drawing.Size(175, 22);
            ProjectSettings.Text = "Project Settings...";
            ProjectSettings.Click += ProjectSettings_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.AutoSize = false;
            toolStripMenuItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            toolStripMenuItem1.Enabled = false;
            toolStripMenuItem1.Image = Properties.Resources.LineSeparator;
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(18, 21);
            // 
            // SearchMenu
            // 
            SearchMenu.Image = Properties.Resources.Search;
            SearchMenu.Name = "SearchMenu";
            SearchMenu.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T;
            SearchMenu.Size = new System.Drawing.Size(75, 30);
            SearchMenu.Text = "Search";
            SearchMenu.Click += SearchMenu_Click;
            // 
            // ProjectName
            // 
            ProjectName.BackColor = System.Drawing.Color.White;
            ProjectName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            ProjectName.Enabled = false;
            ProjectName.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            ProjectName.Margin = new System.Windows.Forms.Padding(8, 0, 1, 0);
            ProjectName.Name = "ProjectName";
            ProjectName.ReadOnly = true;
            ProjectName.ShortcutsEnabled = false;
            ProjectName.Size = new System.Drawing.Size(100, 30);
            ProjectName.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(5, 39);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(Explorer);
            splitContainer1.Panel1.Controls.Add(panel2);
            splitContainer1.Panel1.Controls.Add(panel3);
            splitContainer1.Panel1.Controls.Add(panel1);
            splitContainer1.Panel1.Controls.Add(ExplorerToolStrip);
            splitContainer1.Panel1.Controls.Add(ProjectExplorerCaption);
            splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(10, 6, 0, 6);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(DocumentPanel);
            splitContainer1.Panel2.Controls.Add(splitter2);
            splitContainer1.Panel2.Controls.Add(DocumentExplorerPanel);
            splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(0, 6, 0, 6);
            splitContainer1.Size = new System.Drawing.Size(1022, 438);
            splitContainer1.SplitterDistance = 297;
            splitContainer1.SplitterWidth = 8;
            splitContainer1.TabIndex = 4;
            splitContainer1.TabStop = false;
            splitContainer1.Text = "splitContainer1";
            // 
            // Explorer
            // 
            Explorer.AllowDrop = true;
            Explorer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            Explorer.ContextMenuStrip = ExplorerContextMenu;
            Explorer.Dock = System.Windows.Forms.DockStyle.Fill;
            Explorer.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            Explorer.FullRowSelect = true;
            Explorer.HideSelection = false;
            Explorer.ImageIndex = 0;
            Explorer.ImageList = ExplorerTreeImageList;
            Explorer.Indent = 21;
            Explorer.ItemHeight = 24;
            Explorer.LabelEdit = true;
            Explorer.Location = new System.Drawing.Point(10, 88);
            Explorer.Name = "Explorer";
            Explorer.SelectedImageIndex = 0;
            Explorer.ShowLines = false;
            Explorer.Size = new System.Drawing.Size(287, 344);
            Explorer.StateImageList = ExplorerTreeStateImageList;
            Explorer.TabIndex = 0;
            Explorer.BeforeLabelEdit += Explorer_BeforeLabelEdit;
            Explorer.AfterLabelEdit += Explorer_AfterLabelEdit;
            Explorer.AfterCollapse += Explorer_AfterCollapse;
            Explorer.AfterExpand += Explorer_AfterExpand;
            Explorer.ItemDrag += Explorer_ItemDrag;
            Explorer.AfterSelect += Explorer_AfterSelect;
            Explorer.NodeMouseDoubleClick += Explorer_NodeMouseDoubleClick;
            Explorer.DragDrop += Explorer_DragDrop;
            Explorer.DragEnter += Explorer_DragEnter;
            Explorer.DragOver += Explorer_DragOver;
            Explorer.Enter += Explorer_Enter;
            Explorer.KeyDown += Explorer_KeyDown;
            Explorer.Leave += Explorer_Leave;
            Explorer.MouseDown += Explorer_MouseDown;
            Explorer.PreviewKeyDown += Explorer_PreviewKeyDown;
            // 
            // ExplorerContextMenu
            // 
            ExplorerContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { RenameNode, toolStripSeparator3, CollapseAllButThis, toolStripSeparator4, CopyNodeName, toolStripSeparator5, AddNode, toolStripSeparator7, DeleteNode });
            ExplorerContextMenu.Name = "ExplorerContextMenu";
            ExplorerContextMenu.Size = new System.Drawing.Size(183, 138);
            // 
            // RenameNode
            // 
            RenameNode.Image = (System.Drawing.Image)resources.GetObject("RenameNode.Image");
            RenameNode.Name = "RenameNode";
            RenameNode.ShortcutKeys = System.Windows.Forms.Keys.F2;
            RenameNode.Size = new System.Drawing.Size(182, 22);
            RenameNode.Text = "Rename";
            RenameNode.Click += RenameNode_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(179, 6);
            // 
            // CollapseAllButThis
            // 
            CollapseAllButThis.Name = "CollapseAllButThis";
            CollapseAllButThis.Size = new System.Drawing.Size(182, 22);
            CollapseAllButThis.Text = "Collapse All But This";
            CollapseAllButThis.Click += CollapseAllButThis_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(179, 6);
            // 
            // CopyNodeName
            // 
            CopyNodeName.Image = Properties.Resources.CopyItem;
            CopyNodeName.Name = "CopyNodeName";
            CopyNodeName.Size = new System.Drawing.Size(182, 22);
            CopyNodeName.Text = "Copy Node Name";
            CopyNodeName.Click += CopyNodeName_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new System.Drawing.Size(179, 6);
            // 
            // AddNode
            // 
            AddNode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { AddFolder, AddScript });
            AddNode.Name = "AddNode";
            AddNode.Size = new System.Drawing.Size(182, 22);
            AddNode.Text = "Add";
            // 
            // AddFolder
            // 
            AddFolder.Image = (System.Drawing.Image)resources.GetObject("AddFolder.Image");
            AddFolder.Name = "AddFolder";
            AddFolder.Size = new System.Drawing.Size(107, 22);
            AddFolder.Text = "Folder";
            AddFolder.Click += AddFolder_Click;
            // 
            // AddScript
            // 
            AddScript.Image = (System.Drawing.Image)resources.GetObject("AddScript.Image");
            AddScript.Name = "AddScript";
            AddScript.Size = new System.Drawing.Size(107, 22);
            AddScript.Text = "Script";
            AddScript.Click += AddScript_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new System.Drawing.Size(179, 6);
            // 
            // DeleteNode
            // 
            DeleteNode.Image = (System.Drawing.Image)resources.GetObject("DeleteNode.Image");
            DeleteNode.Name = "DeleteNode";
            DeleteNode.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            DeleteNode.Size = new System.Drawing.Size(182, 22);
            DeleteNode.Text = "Delete";
            DeleteNode.Click += DeleteNode_Click;
            // 
            // ExplorerTreeImageList
            // 
            ExplorerTreeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            ExplorerTreeImageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("ExplorerTreeImageList.ImageStream");
            ExplorerTreeImageList.TransparentColor = System.Drawing.Color.Transparent;
            ExplorerTreeImageList.Images.SetKeyName(0, "ProjectFilterFile_16x.png");
            ExplorerTreeImageList.Images.SetKeyName(1, "FolderClosed_16x.png");
            ExplorerTreeImageList.Images.SetKeyName(2, "FolderOpened_16x.png");
            ExplorerTreeImageList.Images.SetKeyName(3, "Question_16x.png");
            ExplorerTreeImageList.Images.SetKeyName(4, "InitializationPhase.png");
            ExplorerTreeImageList.Images.SetKeyName(5, "PostInitializationPhase.png");
            ExplorerTreeImageList.Images.SetKeyName(6, "NewType.png");
            ExplorerTreeImageList.Images.SetKeyName(7, "Room.png");
            ExplorerTreeImageList.Images.SetKeyName(8, "Home.png");
            ExplorerTreeImageList.Images.SetKeyName(9, "Thing.png");
            ExplorerTreeImageList.Images.SetKeyName(10, "TransientThing.png");
            ExplorerTreeImageList.Images.SetKeyName(11, "StateIndicator_16x.png");
            ExplorerTreeImageList.Images.SetKeyName(12, "Unload.png");
            ExplorerTreeImageList.Images.SetKeyName(13, "EnterRoom.png");
            ExplorerTreeImageList.Images.SetKeyName(14, "Outcome.png");
            ExplorerTreeImageList.Images.SetKeyName(15, "Script_16x.png");
            ExplorerTreeImageList.Images.SetKeyName(16, "EnterRoom.png");
            ExplorerTreeImageList.Images.SetKeyName(17, "NewAttributeRelationship.png");
            ExplorerTreeImageList.Images.SetKeyName(18, "Variable.png");
            ExplorerTreeImageList.Images.SetKeyName(19, "TransientVariable.png");
            // 
            // ExplorerTreeStateImageList
            // 
            ExplorerTreeStateImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            ExplorerTreeStateImageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("ExplorerTreeStateImageList.ImageStream");
            ExplorerTreeStateImageList.TransparentColor = System.Drawing.Color.Transparent;
            ExplorerTreeStateImageList.Images.SetKeyName(0, "CommentFilled_16x.png");
            // 
            // panel2
            // 
            panel2.Dock = System.Windows.Forms.DockStyle.Top;
            panel2.Location = new System.Drawing.Point(10, 83);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(287, 5);
            panel2.TabIndex = 4;
            // 
            // panel3
            // 
            panel3.Controls.Add(SearchBox);
            panel3.Controls.Add(ClearSearch);
            panel3.Dock = System.Windows.Forms.DockStyle.Top;
            panel3.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            panel3.Location = new System.Drawing.Point(10, 59);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(287, 24);
            panel3.TabIndex = 5;
            // 
            // SearchBox
            // 
            SearchBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            SearchBox.Dock = System.Windows.Forms.DockStyle.Fill;
            SearchBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            SearchBox.HideSelection = false;
            SearchBox.Location = new System.Drawing.Point(0, 0);
            SearchBox.Name = "SearchBox";
            SearchBox.PlaceholderText = "Search (Ctrl+T)";
            SearchBox.Size = new System.Drawing.Size(260, 22);
            SearchBox.TabIndex = 1;
            SearchBox.WordWrap = false;
            SearchBox.TextChanged += SearchBox_TextChanged;
            SearchBox.Enter += SearchBox_Enter;
            SearchBox.Leave += SearchBox_Leave;
            // 
            // ClearSearch
            // 
            ClearSearch.Dock = System.Windows.Forms.DockStyle.Right;
            ClearSearch.Image = Properties.Resources.ClearWindowContent;
            ClearSearch.Location = new System.Drawing.Point(260, 0);
            ClearSearch.Name = "ClearSearch";
            ClearSearch.Size = new System.Drawing.Size(27, 24);
            ClearSearch.TabIndex = 2;
            ClearSearch.TabStop = false;
            ClearSearch.UseVisualStyleBackColor = true;
            ClearSearch.Click += ClearSearch_Click;
            // 
            // panel1
            // 
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(10, 56);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(287, 3);
            panel1.TabIndex = 3;
            // 
            // ExplorerToolStrip
            // 
            ExplorerToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            ExplorerToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { Home, toolStripSeparator8, NavigateExplorerBack, NavigateExplorerForward, toolStripSeparator11, GoToPreviousTask, GoToNextTask, toolStripSeparator14, SortNodes, CollapseAll, toolStripSeparator9, SyncWithDocumentButton, toolStripSeparator10 });
            ExplorerToolStrip.Location = new System.Drawing.Point(10, 31);
            ExplorerToolStrip.Name = "ExplorerToolStrip";
            ExplorerToolStrip.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            ExplorerToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            ExplorerToolStrip.Size = new System.Drawing.Size(287, 25);
            ExplorerToolStrip.TabIndex = 2;
            ExplorerToolStrip.Text = "toolStrip1";
            // 
            // Home
            // 
            Home.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            Home.Image = (System.Drawing.Image)resources.GetObject("Home.Image");
            Home.ImageTransparentColor = System.Drawing.Color.Magenta;
            Home.Name = "Home";
            Home.Size = new System.Drawing.Size(23, 22);
            Home.ToolTipText = "Home";
            Home.Click += Home_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new System.Drawing.Size(6, 25);
            // 
            // NavigateExplorerBack
            // 
            NavigateExplorerBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            NavigateExplorerBack.Image = Properties.Resources.Backwards;
            NavigateExplorerBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            NavigateExplorerBack.Name = "NavigateExplorerBack";
            NavigateExplorerBack.Size = new System.Drawing.Size(23, 22);
            NavigateExplorerBack.Text = "toolStripButton2";
            NavigateExplorerBack.ToolTipText = "Back";
            NavigateExplorerBack.Click += NavigateExplorerBack_Click;
            // 
            // NavigateExplorerForward
            // 
            NavigateExplorerForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            NavigateExplorerForward.Image = Properties.Resources.Forwards;
            NavigateExplorerForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            NavigateExplorerForward.Name = "NavigateExplorerForward";
            NavigateExplorerForward.Size = new System.Drawing.Size(23, 22);
            NavigateExplorerForward.Text = "toolStripButton1";
            NavigateExplorerForward.ToolTipText = "Forward";
            NavigateExplorerForward.Click += NavigateExplorerForward_Click;
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            toolStripSeparator11.Size = new System.Drawing.Size(6, 25);
            // 
            // GoToPreviousTask
            // 
            GoToPreviousTask.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            GoToPreviousTask.Image = (System.Drawing.Image)resources.GetObject("GoToPreviousTask.Image");
            GoToPreviousTask.ImageTransparentColor = System.Drawing.Color.Magenta;
            GoToPreviousTask.Name = "GoToPreviousTask";
            GoToPreviousTask.Size = new System.Drawing.Size(23, 22);
            GoToPreviousTask.Text = "toolStripButton1";
            GoToPreviousTask.ToolTipText = "Previous Task";
            GoToPreviousTask.Click += GoToPreviousTask_Click;
            // 
            // GoToNextTask
            // 
            GoToNextTask.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            GoToNextTask.Image = (System.Drawing.Image)resources.GetObject("GoToNextTask.Image");
            GoToNextTask.ImageTransparentColor = System.Drawing.Color.Magenta;
            GoToNextTask.Name = "GoToNextTask";
            GoToNextTask.Size = new System.Drawing.Size(23, 22);
            GoToNextTask.Text = "toolStripButton2";
            GoToNextTask.ToolTipText = "Next Task";
            GoToNextTask.Click += GoToNextTask_Click;
            // 
            // toolStripSeparator14
            // 
            toolStripSeparator14.Name = "toolStripSeparator14";
            toolStripSeparator14.Size = new System.Drawing.Size(6, 25);
            // 
            // SortNodes
            // 
            SortNodes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            SortNodes.Image = (System.Drawing.Image)resources.GetObject("SortNodes.Image");
            SortNodes.ImageTransparentColor = System.Drawing.Color.Magenta;
            SortNodes.Name = "SortNodes";
            SortNodes.Size = new System.Drawing.Size(23, 22);
            SortNodes.Text = "toolStripButton1";
            SortNodes.ToolTipText = "Sort";
            SortNodes.Click += SortNodes_Click;
            // 
            // CollapseAll
            // 
            CollapseAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            CollapseAll.Image = (System.Drawing.Image)resources.GetObject("CollapseAll.Image");
            CollapseAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            CollapseAll.Name = "CollapseAll";
            CollapseAll.Size = new System.Drawing.Size(23, 22);
            CollapseAll.ToolTipText = "Collapse All";
            CollapseAll.Click += CollapseAll_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            // 
            // SyncWithDocumentButton
            // 
            SyncWithDocumentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            SyncWithDocumentButton.Image = (System.Drawing.Image)resources.GetObject("SyncWithDocumentButton.Image");
            SyncWithDocumentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            SyncWithDocumentButton.Name = "SyncWithDocumentButton";
            SyncWithDocumentButton.Size = new System.Drawing.Size(23, 22);
            SyncWithDocumentButton.ToolTipText = "Sync With Active Document";
            SyncWithDocumentButton.Click += SyncWithDocumentButton_Click;
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new System.Drawing.Size(6, 25);
            // 
            // ProjectExplorerCaption
            // 
            ProjectExplorerCaption.Dock = System.Windows.Forms.DockStyle.Top;
            ProjectExplorerCaption.Font = new System.Drawing.Font("Segoe UI", 9F);
            ProjectExplorerCaption.Location = new System.Drawing.Point(10, 6);
            ProjectExplorerCaption.Name = "ProjectExplorerCaption";
            ProjectExplorerCaption.Size = new System.Drawing.Size(287, 25);
            ProjectExplorerCaption.TabIndex = 6;
            ProjectExplorerCaption.Text = ":::: Project Explorer";
            ProjectExplorerCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DocumentPanel
            // 
            DocumentPanel.BackColor = System.Drawing.SystemColors.Window;
            DocumentPanel.Controls.Add(intellisenseControl1);
            DocumentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            DocumentPanel.Location = new System.Drawing.Point(0, 6);
            DocumentPanel.Name = "DocumentPanel";
            DocumentPanel.Padding = new System.Windows.Forms.Padding(15, 8, 0, 0);
            DocumentPanel.Size = new System.Drawing.Size(446, 426);
            DocumentPanel.TabIndex = 5;
            // 
            // intellisenseControl1
            // 
            intellisenseControl1.Font = new System.Drawing.Font("Segoe UI", 12F);
            intellisenseControl1.Location = new System.Drawing.Point(57, 50);
            intellisenseControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            intellisenseControl1.Name = "intellisenseControl1";
            intellisenseControl1.Padding = new System.Windows.Forms.Padding(3);
            intellisenseControl1.Size = new System.Drawing.Size(459, 278);
            intellisenseControl1.TabIndex = 0;
            // 
            // splitter2
            // 
            splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            splitter2.Location = new System.Drawing.Point(446, 6);
            splitter2.Name = "splitter2";
            splitter2.Size = new System.Drawing.Size(8, 426);
            splitter2.TabIndex = 4;
            splitter2.TabStop = false;
            // 
            // DocumentExplorerPanel
            // 
            DocumentExplorerPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            DocumentExplorerPanel.Controls.Add(DocumentExplorer);
            DocumentExplorerPanel.Controls.Add(toolStrip1);
            DocumentExplorerPanel.Dock = System.Windows.Forms.DockStyle.Right;
            DocumentExplorerPanel.Location = new System.Drawing.Point(454, 6);
            DocumentExplorerPanel.Margin = new System.Windows.Forms.Padding(6);
            DocumentExplorerPanel.Name = "DocumentExplorerPanel";
            DocumentExplorerPanel.Padding = new System.Windows.Forms.Padding(6, 3, 6, 3);
            DocumentExplorerPanel.Size = new System.Drawing.Size(263, 426);
            DocumentExplorerPanel.TabIndex = 3;
            // 
            // DocumentExplorer
            // 
            DocumentExplorer.BackColor = System.Drawing.SystemColors.ControlLight;
            DocumentExplorer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            DocumentExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            DocumentExplorer.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            DocumentExplorer.FullRowSelect = true;
            DocumentExplorer.HideSelection = false;
            DocumentExplorer.ImageIndex = 0;
            DocumentExplorer.ImageList = ExplorerTreeImageList;
            DocumentExplorer.Indent = 23;
            DocumentExplorer.ItemHeight = 24;
            DocumentExplorer.Location = new System.Drawing.Point(6, 28);
            DocumentExplorer.Margin = new System.Windows.Forms.Padding(0);
            DocumentExplorer.Name = "DocumentExplorer";
            DocumentExplorer.SelectedImageIndex = 0;
            DocumentExplorer.ShowLines = false;
            DocumentExplorer.ShowPlusMinus = false;
            DocumentExplorer.ShowRootLines = false;
            DocumentExplorer.Size = new System.Drawing.Size(251, 395);
            DocumentExplorer.StateImageList = DocumentExplorerStateImageList;
            DocumentExplorer.TabIndex = 9;
            DocumentExplorer.AfterSelect += DocumentExplorer_AfterSelect;
            DocumentExplorer.MouseDown += DocumentExplorer_MouseDown;
            // 
            // DocumentExplorerStateImageList
            // 
            DocumentExplorerStateImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            DocumentExplorerStateImageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("DocumentExplorerStateImageList.ImageStream");
            DocumentExplorerStateImageList.TransparentColor = System.Drawing.Color.Transparent;
            DocumentExplorerStateImageList.Images.SetKeyName(0, "Pin.png");
            // 
            // toolStrip1
            // 
            toolStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            toolStrip1.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripLabel1, NavigateDocumentForward, NavigateDocumentBack });
            toolStrip1.Location = new System.Drawing.Point(6, 3);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new System.Windows.Forms.Padding(7, 0, 5, 0);
            toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            toolStrip1.Size = new System.Drawing.Size(251, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Font = new System.Drawing.Font("Segoe UI", 9F);
            toolStripLabel1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new System.Drawing.Size(119, 22);
            toolStripLabel1.Text = ":::: Active Documents";
            // 
            // NavigateDocumentForward
            // 
            NavigateDocumentForward.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            NavigateDocumentForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            NavigateDocumentForward.Image = Properties.Resources.Forwards;
            NavigateDocumentForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            NavigateDocumentForward.Name = "NavigateDocumentForward";
            NavigateDocumentForward.Size = new System.Drawing.Size(23, 22);
            NavigateDocumentForward.Text = "Forward";
            NavigateDocumentForward.ToolTipText = "Forward";
            NavigateDocumentForward.Click += NavigateDocumentForward_Click;
            // 
            // NavigateDocumentBack
            // 
            NavigateDocumentBack.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            NavigateDocumentBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            NavigateDocumentBack.Image = Properties.Resources.Backwards;
            NavigateDocumentBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            NavigateDocumentBack.Name = "NavigateDocumentBack";
            NavigateDocumentBack.Size = new System.Drawing.Size(23, 22);
            NavigateDocumentBack.Text = "Back";
            NavigateDocumentBack.ToolTipText = "Back";
            NavigateDocumentBack.Click += NavigateDocumentBack_Click;
            // 
            // DocumentExplorerContextMenu
            // 
            DocumentExplorerContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { CloseDocument, toolStripSeparator12, CloseAll, CloseAllButThis, toolStripSeparator16, PinDocumentMenuItem, toolStripSeparator15, CopyDocumentName });
            DocumentExplorerContextMenu.Name = "DocumentExplorerContextMenu";
            DocumentExplorerContextMenu.Size = new System.Drawing.Size(197, 132);
            DocumentExplorerContextMenu.Opening += DocumentExplorerContextMenu_Opening;
            // 
            // CloseDocument
            // 
            CloseDocument.Name = "CloseDocument";
            CloseDocument.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4;
            CloseDocument.Size = new System.Drawing.Size(196, 22);
            CloseDocument.Text = "Close";
            CloseDocument.Click += CloseDocument_Click;
            // 
            // toolStripSeparator12
            // 
            toolStripSeparator12.Name = "toolStripSeparator12";
            toolStripSeparator12.Size = new System.Drawing.Size(193, 6);
            // 
            // CloseAll
            // 
            CloseAll.Image = Properties.Resources.CloseAll;
            CloseAll.Name = "CloseAll";
            CloseAll.Size = new System.Drawing.Size(196, 22);
            CloseAll.Text = "Close All";
            CloseAll.Click += CloseAll_Click;
            // 
            // CloseAllButThis
            // 
            CloseAllButThis.Name = "CloseAllButThis";
            CloseAllButThis.Size = new System.Drawing.Size(196, 22);
            CloseAllButThis.Text = "Close All But this";
            CloseAllButThis.Click += CloseAllButThis_Click;
            // 
            // toolStripSeparator16
            // 
            toolStripSeparator16.Name = "toolStripSeparator16";
            toolStripSeparator16.Size = new System.Drawing.Size(193, 6);
            // 
            // PinDocumentMenuItem
            // 
            PinDocumentMenuItem.Image = Properties.Resources.Pin;
            PinDocumentMenuItem.Name = "PinDocumentMenuItem";
            PinDocumentMenuItem.Size = new System.Drawing.Size(196, 22);
            PinDocumentMenuItem.Text = "Pin Document";
            PinDocumentMenuItem.Click += PinDocumentMenuItem_Click;
            // 
            // toolStripSeparator15
            // 
            toolStripSeparator15.Name = "toolStripSeparator15";
            toolStripSeparator15.Size = new System.Drawing.Size(193, 6);
            // 
            // CopyDocumentName
            // 
            CopyDocumentName.Image = Properties.Resources.CopyItem1;
            CopyDocumentName.Name = "CopyDocumentName";
            CopyDocumentName.Size = new System.Drawing.Size(196, 22);
            CopyDocumentName.Text = "Copy Document Name";
            CopyDocumentName.Click += CopyDocumentName_Click;
            // 
            // SearchResultsPanel
            // 
            SearchResultsPanel.Controls.Add(SearchResults);
            SearchResultsPanel.Controls.Add(panel4);
            SearchResultsPanel.Controls.Add(SearchResultsCaption);
            SearchResultsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            SearchResultsPanel.Location = new System.Drawing.Point(5, 486);
            SearchResultsPanel.MinimumSize = new System.Drawing.Size(0, 50);
            SearchResultsPanel.Name = "SearchResultsPanel";
            SearchResultsPanel.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            SearchResultsPanel.Size = new System.Drawing.Size(1022, 179);
            SearchResultsPanel.TabIndex = 7;
            // 
            // SearchResults
            // 
            SearchResults.BorderStyle = System.Windows.Forms.BorderStyle.None;
            SearchResults.ContextMenuStrip = SearchResultsContextMenu;
            SearchResults.Dock = System.Windows.Forms.DockStyle.Fill;
            SearchResults.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            SearchResults.FullRowSelect = true;
            SearchResults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            SearchResults.LabelWrap = false;
            SearchResults.Location = new System.Drawing.Point(5, 36);
            SearchResults.MultiSelect = false;
            SearchResults.Name = "SearchResults";
            SearchResults.ShowGroups = false;
            SearchResults.Size = new System.Drawing.Size(1012, 137);
            SearchResults.SmallImageList = ExplorerTreeImageList;
            SearchResults.TabIndex = 6;
            SearchResults.UseCompatibleStateImageBehavior = false;
            SearchResults.View = System.Windows.Forms.View.List;
            SearchResults.SelectedIndexChanged += SearchResults_SelectedIndexChanged;
            SearchResults.Enter += SearchResults_Enter;
            SearchResults.KeyDown += SearchResults_KeyDown;
            SearchResults.Leave += SearchResults_Leave;
            SearchResults.MouseDoubleClick += SearchResults_MouseDoubleClick;
            // 
            // SearchResultsContextMenu
            // 
            SearchResultsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { OpenAllSearchResultsDocuments });
            SearchResultsContextMenu.Name = "SearchResultsContextMenu";
            SearchResultsContextMenu.Size = new System.Drawing.Size(121, 26);
            // 
            // OpenAllSearchResultsDocuments
            // 
            OpenAllSearchResultsDocuments.Image = (System.Drawing.Image)resources.GetObject("OpenAllSearchResultsDocuments.Image");
            OpenAllSearchResultsDocuments.Name = "OpenAllSearchResultsDocuments";
            OpenAllSearchResultsDocuments.Size = new System.Drawing.Size(120, 22);
            OpenAllSearchResultsDocuments.Text = "Open All";
            OpenAllSearchResultsDocuments.Click += OpenAllSearchResultsDocuments_Click;
            // 
            // panel4
            // 
            panel4.BackColor = System.Drawing.SystemColors.Window;
            panel4.Dock = System.Windows.Forms.DockStyle.Top;
            panel4.Location = new System.Drawing.Point(5, 31);
            panel4.Name = "panel4";
            panel4.Size = new System.Drawing.Size(1012, 5);
            panel4.TabIndex = 8;
            // 
            // SearchResultsCaption
            // 
            SearchResultsCaption.BackColor = System.Drawing.SystemColors.Control;
            SearchResultsCaption.Dock = System.Windows.Forms.DockStyle.Top;
            SearchResultsCaption.Font = new System.Drawing.Font("Segoe UI", 9F);
            SearchResultsCaption.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            SearchResultsCaption.Location = new System.Drawing.Point(5, 6);
            SearchResultsCaption.Name = "SearchResultsCaption";
            SearchResultsCaption.Size = new System.Drawing.Size(1012, 25);
            SearchResultsCaption.TabIndex = 7;
            SearchResultsCaption.Text = ":::: Search Results";
            SearchResultsCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DocumentContextMenu
            // 
            DocumentContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { GoToDefinition, ShowIntellisensePopup, toolStripSeparator13, FindInExplorer, toolStripSeparator17, CloseCurrentDocument });
            DocumentContextMenu.Name = "DocumentContextMenu";
            DocumentContextMenu.Size = new System.Drawing.Size(219, 104);
            DocumentContextMenu.Opening += DocumentContextMenu_Opening;
            // 
            // GoToDefinition
            // 
            GoToDefinition.Image = (System.Drawing.Image)resources.GetObject("GoToDefinition.Image");
            GoToDefinition.Name = "GoToDefinition";
            GoToDefinition.ShortcutKeys = System.Windows.Forms.Keys.F12;
            GoToDefinition.Size = new System.Drawing.Size(218, 22);
            GoToDefinition.Text = "Go To Definition";
            GoToDefinition.Click += GoToDefinition_Click;
            // 
            // ShowIntellisensePopup
            // 
            ShowIntellisensePopup.Image = (System.Drawing.Image)resources.GetObject("ShowIntellisensePopup.Image");
            ShowIntellisensePopup.Name = "ShowIntellisensePopup";
            ShowIntellisensePopup.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Space;
            ShowIntellisensePopup.Size = new System.Drawing.Size(218, 22);
            ShowIntellisensePopup.Text = "Intellisense";
            ShowIntellisensePopup.Click += IntellisensePopup_Click;
            // 
            // toolStripSeparator13
            // 
            toolStripSeparator13.Name = "toolStripSeparator13";
            toolStripSeparator13.Size = new System.Drawing.Size(215, 6);
            // 
            // FindInExplorer
            // 
            FindInExplorer.Image = (System.Drawing.Image)resources.GetObject("FindInExplorer.Image");
            FindInExplorer.Name = "FindInExplorer";
            FindInExplorer.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F;
            FindInExplorer.Size = new System.Drawing.Size(218, 22);
            FindInExplorer.Text = "Find in Explorer";
            FindInExplorer.Click += FindInExplorer_Click;
            // 
            // toolStripSeparator17
            // 
            toolStripSeparator17.Name = "toolStripSeparator17";
            toolStripSeparator17.Size = new System.Drawing.Size(215, 6);
            // 
            // CloseCurrentDocument
            // 
            CloseCurrentDocument.Name = "CloseCurrentDocument";
            CloseCurrentDocument.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4;
            CloseCurrentDocument.Size = new System.Drawing.Size(218, 22);
            CloseCurrentDocument.Text = "Close";
            CloseCurrentDocument.Click += CloseCurrentDocument_Click;
            // 
            // StatusBar
            // 
            StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { ProjectStatus, ScriptVersion, ExportStatus });
            StatusBar.Location = new System.Drawing.Point(5, 665);
            StatusBar.Name = "StatusBar";
            StatusBar.Size = new System.Drawing.Size(1022, 24);
            StatusBar.TabIndex = 5;
            StatusBar.Text = "statusStrip1";
            // 
            // ProjectStatus
            // 
            ProjectStatus.AutoSize = false;
            ProjectStatus.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            ProjectStatus.Name = "ProjectStatus";
            ProjectStatus.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            ProjectStatus.Size = new System.Drawing.Size(88, 19);
            ProjectStatus.Text = "ProjectStatus";
            // 
            // ScriptVersion
            // 
            ScriptVersion.AutoSize = false;
            ScriptVersion.Name = "ScriptVersion";
            ScriptVersion.Size = new System.Drawing.Size(120, 19);
            ScriptVersion.Text = "Script Build";
            // 
            // ExportStatus
            // 
            ExportStatus.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            ExportStatus.Name = "ExportStatus";
            ExportStatus.Size = new System.Drawing.Size(85, 19);
            ExportStatus.Text = "Output Folder";
            // 
            // toolStripTextBox1
            // 
            toolStripTextBox1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            toolStripTextBox1.BackColor = System.Drawing.SystemColors.ControlLight;
            toolStripTextBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            toolStripTextBox1.ForeColor = System.Drawing.SystemColors.WindowFrame;
            toolStripTextBox1.Margin = new System.Windows.Forms.Padding(1, 0, 10, 0);
            toolStripTextBox1.Name = "toolStripTextBox1";
            toolStripTextBox1.ReadOnly = true;
            toolStripTextBox1.Size = new System.Drawing.Size(100, 23);
            toolStripTextBox1.Text = "VladCircus";
            toolStripTextBox1.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // splitter1
            // 
            splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            splitter1.Location = new System.Drawing.Point(5, 477);
            splitter1.Name = "splitter1";
            splitter1.Size = new System.Drawing.Size(1022, 9);
            splitter1.TabIndex = 8;
            splitter1.TabStop = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1032, 694);
            Controls.Add(splitContainer1);
            Controls.Add(splitter1);
            Controls.Add(SearchResultsPanel);
            Controls.Add(MainMenu);
            Controls.Add(StatusBar);
            Font = new System.Drawing.Font("Segoe UI", 9.75F);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MainMenuStrip = MainMenu;
            MinimumSize = new System.Drawing.Size(480, 270);
            Name = "MainForm";
            Padding = new System.Windows.Forms.Padding(5);
            Text = "Script Editor (Engendro 4.0)";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            KeyDown += MainForm_KeyDown;
            MainMenu.ResumeLayout(false);
            MainMenu.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ExplorerContextMenu.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ExplorerToolStrip.ResumeLayout(false);
            ExplorerToolStrip.PerformLayout();
            DocumentPanel.ResumeLayout(false);
            DocumentExplorerPanel.ResumeLayout(false);
            DocumentExplorerPanel.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            DocumentExplorerContextMenu.ResumeLayout(false);
            SearchResultsPanel.ResumeLayout(false);
            SearchResultsContextMenu.ResumeLayout(false);
            DocumentContextMenu.ResumeLayout(false);
            StatusBar.ResumeLayout(false);
            StatusBar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem FileMenu;
        private System.Windows.Forms.ToolStripMenuItem FileNew;
        private System.Windows.Forms.ToolStripMenuItem FileOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem FileClose;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem FileSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem FileExit;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ImageList ExplorerTreeImageList;
        private System.Windows.Forms.TextBox SearchBox;
        private System.Windows.Forms.TreeView Explorer;
        private System.Windows.Forms.ToolStrip ExplorerToolStrip;
        private System.Windows.Forms.ToolStripButton NavigateExplorerBack;
        private System.Windows.Forms.ToolStripButton NavigateExplorerForward;
        private System.Windows.Forms.ToolStripButton SyncWithDocumentButton;
        private System.Windows.Forms.ContextMenuStrip ExplorerContextMenu;
        private System.Windows.Forms.ContextMenuStrip DocumentContextMenu;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem RenameNode;
        private System.Windows.Forms.ToolStripMenuItem CollapseAllButThis;
        private System.Windows.Forms.ToolStripMenuItem CopyNodeName;
        private System.Windows.Forms.ToolStripMenuItem DeleteNode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem AddNode;
        private System.Windows.Forms.ToolStripMenuItem AddFolder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem AddScript;
        private System.Windows.Forms.StatusStrip StatusBar;
        private System.Windows.Forms.Panel SearchResultsPanel;
        private System.Windows.Forms.ListView SearchResults;
        private System.Windows.Forms.Label SearchResultsCaption;
        private System.Windows.Forms.ToolStripMenuItem GoToDefinition;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripMenuItem ToolsMenu;
        private System.Windows.Forms.ToolStripMenuItem ProjectSettings;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton NavigateDocumentBack;
        private System.Windows.Forms.ToolStripButton NavigateDocumentForward;
        private System.Windows.Forms.ToolStripStatusLabel ProjectStatus;
        private System.Windows.Forms.ToolStripStatusLabel ExportStatus;
        private System.Windows.Forms.ToolStripButton Home;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripButton CollapseAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripButton SortNodes;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button ClearSearch;
        private System.Windows.Forms.ContextMenuStrip SearchResultsContextMenu;
        private System.Windows.Forms.ToolStripMenuItem OpenAllSearchResultsDocuments;
        private System.Windows.Forms.ToolStripStatusLabel ScriptVersion;
        private System.Windows.Forms.ImageList ExplorerTreeStateImageList;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Panel DocumentExplorerPanel;
        private System.Windows.Forms.TreeView DocumentExplorer;
        private System.Windows.Forms.Panel DocumentPanel;
        private System.Windows.Forms.ContextMenuStrip DocumentExplorerContextMenu;
        private System.Windows.Forms.ToolStripMenuItem CloseDocument;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem CloseAll;
        private System.Windows.Forms.ToolStripMenuItem CloseAllButThis;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem FindInExplorer;
        private System.Windows.Forms.ToolStripMenuItem CloseCurrentDocument;
        private System.Windows.Forms.Label ProjectExplorerCaption;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ToolStripMenuItem ShowIntellisensePopup;
        private IntellisenseControl intellisenseControl1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripMenuItem CopyDocumentName;
        private System.Windows.Forms.ImageList DocumentExplorerStateImageList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem PinDocumentMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem SearchMenu;
        private System.Windows.Forms.ToolStripButton GoToPreviousTask;
        private System.Windows.Forms.ToolStripButton GoToNextTask;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripTextBox ProjectName;
    }
}

