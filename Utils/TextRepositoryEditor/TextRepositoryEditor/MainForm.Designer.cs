namespace TextRepositoryEditor
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
            ListViewGroup listViewGroup1 = new ListViewGroup("ListViewGroup", HorizontalAlignment.Left);
            Explorer = new TreeView();
            contextMenuExplorer = new ContextMenuStrip(components);
            NewLanguagePackageMenuItem = new ToolStripMenuItem();
            ImportMenuItem = new ToolStripMenuItem();
            LanguagePackageSeparator = new ToolStripSeparator();
            CopyNodePathMenuItem = new ToolStripMenuItem();
            toolStripSeparator6 = new ToolStripSeparator();
            NewFolderMenuItem = new ToolStripMenuItem();
            NewTextMenuItem = new ToolStripMenuItem();
            deleteSeparator = new ToolStripSeparator();
            CutMenuItem = new ToolStripMenuItem();
            CopyMenuItem = new ToolStripMenuItem();
            PasteMenuItem = new ToolStripMenuItem();
            toolStripSeparator11 = new ToolStripSeparator();
            CollapseChildrenMenuItem = new ToolStripMenuItem();
            toolStripSeparator7 = new ToolStripSeparator();
            RemoveChildNodesMenuItem = new ToolStripMenuItem();
            ClearMenuItem = new ToolStripMenuItem();
            ShowImportResultMenuItem = new ToolStripMenuItem();
            ImportResultFilterUnchangedMenuItem = new ToolStripMenuItem();
            ImportResultFilterNewMenuItem = new ToolStripMenuItem();
            ImportResultFilterNotFoundMenuItem = new ToolStripMenuItem();
            ImportResultFilterUpdatedMenuItem = new ToolStripMenuItem();
            toolStripSeparator8 = new ToolStripSeparator();
            DeleteMenuItem = new ToolStripMenuItem();
            ImageListExplorer = new ImageList(components);
            StateImageListExplorer = new ImageList(components);
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            NewMenuItem = new ToolStripMenuItem();
            OpenMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            PublishMenuItem = new ToolStripMenuItem();
            SaveMenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            CloseMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            ExitMenuItem = new ToolStripMenuItem();
            CultureGrid = new ListView();
            cultureColumnHeader = new ColumnHeader(3);
            textColumnHeader = new ColumnHeader();
            lengthColumnHeader = new ColumnHeader();
            contextMenuCultureGrid = new ContextMenuStrip(components);
            CopyTextMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            ProjectStatusLoading = new ToolStripStatusLabel();
            ProjectStatusLabel = new ToolStripStatusLabel();
            ProjectValidationStatusLabel = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            ExplorerPanel = new Panel();
            ProjectExplorerCaption = new Label();
            splitter2 = new Splitter();
            PropertiesPanel = new Panel();
            PropertyGrid1 = new PropertyGrid();
            PropertiesCaption = new Label();
            GridPanel = new Panel();
            PublishedMessageLabel = new Label();
            LiteralValueLabel = new Label();
            splitter3 = new Splitter();
            NoTextSelectedLabel = new Label();
            splitter1 = new Splitter();
            SearchPanel = new Panel();
            SearchResultsGrid = new ListView();
            columnHeader1 = new ColumnHeader();
            toolStrip3 = new ToolStrip();
            toolStripLabel1 = new ToolStripLabel();
            SearchBox = new ToolStripTextBox();
            ClearSearchResultsButton = new ToolStripButton();
            SearchCaption = new Label();
            toolStrip1 = new ToolStrip();
            NavigateBackwardButton = new ToolStripButton();
            NavigateForwardButton = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            ButtonOpenMenuItem = new ToolStripButton();
            SaveButton = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            PreviousErrorButton = new ToolStripButton();
            NextErrorButton = new ToolStripButton();
            PublishButton = new ToolStripButton();
            toolStripSeparator9 = new ToolStripSeparator();
            SortButton = new ToolStripButton();
            toolStripSeparator10 = new ToolStripSeparator();
            PreviousImportResultButton = new ToolStripButton();
            NextImportResultButton = new ToolStripButton();
            toolStripSeparator12 = new ToolStripSeparator();
            ValidationCultureComboBox = new ToolStripComboBox();
            Timer1 = new System.Windows.Forms.Timer(components);
            panel1 = new Panel();
            PublishedMessageTimer = new System.Windows.Forms.Timer(components);
            contextMenuExplorer.SuspendLayout();
            menuStrip1.SuspendLayout();
            contextMenuCultureGrid.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ExplorerPanel.SuspendLayout();
            PropertiesPanel.SuspendLayout();
            GridPanel.SuspendLayout();
            SearchPanel.SuspendLayout();
            toolStrip3.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // Explorer
            // 
            Explorer.AllowDrop = true;
            Explorer.ContextMenuStrip = contextMenuExplorer;
            Explorer.Dock = DockStyle.Fill;
            Explorer.Font = new Font("Segoe UI", 11.25F);
            Explorer.FullRowSelect = true;
            Explorer.HideSelection = false;
            Explorer.ImageIndex = 0;
            Explorer.ImageList = ImageListExplorer;
            Explorer.Indent = 21;
            Explorer.ItemHeight = 24;
            Explorer.LabelEdit = true;
            Explorer.Location = new Point(0, 25);
            Explorer.Margin = new Padding(3, 4, 3, 4);
            Explorer.Name = "Explorer";
            Explorer.SelectedImageIndex = 0;
            Explorer.ShowLines = false;
            Explorer.ShowNodeToolTips = true;
            Explorer.Size = new Size(398, 237);
            Explorer.StateImageList = StateImageListExplorer;
            Explorer.TabIndex = 0;
            Explorer.BeforeLabelEdit += Explorer_BeforeLabelEdit;
            Explorer.AfterLabelEdit += Explorer_AfterLabelEdit;
            Explorer.AfterCollapse += Explorer_AfterCollapse;
            Explorer.AfterExpand += Explorer_AfterExpand;
            Explorer.ItemDrag += Explorer_ItemDrag;
            Explorer.AfterSelect += Explorer_AfterSelect;
            Explorer.DragDrop += Explorer_DragDrop;
            Explorer.DragEnter += Explorer_DragEnter;
            Explorer.DragOver += Explorer_DragOver;
            Explorer.DoubleClick += Explorer_DoubleClick;
            Explorer.Enter += Explorer_Enter;
            Explorer.KeyDown += Explorer_KeyDown;
            Explorer.Leave += Explorer_Leave;
            Explorer.MouseDown += Explorer_MouseDown;
            // 
            // contextMenuExplorer
            // 
            contextMenuExplorer.Items.AddRange(new ToolStripItem[] { NewLanguagePackageMenuItem, ImportMenuItem, LanguagePackageSeparator, CopyNodePathMenuItem, toolStripSeparator6, NewFolderMenuItem, NewTextMenuItem, deleteSeparator, CutMenuItem, CopyMenuItem, PasteMenuItem, toolStripSeparator11, CollapseChildrenMenuItem, toolStripSeparator7, RemoveChildNodesMenuItem, ClearMenuItem, ShowImportResultMenuItem, toolStripSeparator8, DeleteMenuItem });
            contextMenuExplorer.Name = "ContextMenuProject";
            contextMenuExplorer.Size = new Size(210, 326);
            // 
            // NewLanguagePackageMenuItem
            // 
            NewLanguagePackageMenuItem.Image = Properties.Resources.WebFile;
            NewLanguagePackageMenuItem.Name = "NewLanguagePackageMenuItem";
            NewLanguagePackageMenuItem.Size = new Size(209, 22);
            NewLanguagePackageMenuItem.Text = "New Language Package...";
            NewLanguagePackageMenuItem.Click += NewLanguagePackageMenuItem_Click;
            // 
            // ImportMenuItem
            // 
            ImportMenuItem.Image = Properties.Resources.ImportCatalogPart;
            ImportMenuItem.Name = "ImportMenuItem";
            ImportMenuItem.Size = new Size(209, 22);
            ImportMenuItem.Text = "Import...";
            ImportMenuItem.Click += ImportMenuItem_Click;
            // 
            // LanguagePackageSeparator
            // 
            LanguagePackageSeparator.Name = "LanguagePackageSeparator";
            LanguagePackageSeparator.Size = new Size(206, 6);
            // 
            // CopyNodePathMenuItem
            // 
            CopyNodePathMenuItem.Name = "CopyNodePathMenuItem";
            CopyNodePathMenuItem.Size = new Size(209, 22);
            CopyNodePathMenuItem.Text = "Copy as path";
            CopyNodePathMenuItem.Click += CopyNodePathMenuItem_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(206, 6);
            // 
            // NewFolderMenuItem
            // 
            NewFolderMenuItem.Image = Properties.Resources.FolderClosed;
            NewFolderMenuItem.Name = "NewFolderMenuItem";
            NewFolderMenuItem.Size = new Size(209, 22);
            NewFolderMenuItem.Text = "New Folder";
            NewFolderMenuItem.Click += NewFolderMenuItem_Click;
            // 
            // NewTextMenuItem
            // 
            NewTextMenuItem.Image = Properties.Resources.TextArea;
            NewTextMenuItem.Name = "NewTextMenuItem";
            NewTextMenuItem.Size = new Size(209, 22);
            NewTextMenuItem.Text = "New Text";
            NewTextMenuItem.Click += NewTextMenuItem_Click;
            // 
            // deleteSeparator
            // 
            deleteSeparator.Name = "deleteSeparator";
            deleteSeparator.Size = new Size(206, 6);
            // 
            // CutMenuItem
            // 
            CutMenuItem.Image = Properties.Resources.Cut;
            CutMenuItem.Name = "CutMenuItem";
            CutMenuItem.ShortcutKeys = Keys.Control | Keys.X;
            CutMenuItem.Size = new Size(209, 22);
            CutMenuItem.Text = "Cut";
            CutMenuItem.Click += CutMenuItem_Click;
            // 
            // CopyMenuItem
            // 
            CopyMenuItem.Image = Properties.Resources.Copy;
            CopyMenuItem.Name = "CopyMenuItem";
            CopyMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            CopyMenuItem.Size = new Size(209, 22);
            CopyMenuItem.Text = "Copy";
            CopyMenuItem.Click += CopyMenuItem_Click;
            // 
            // PasteMenuItem
            // 
            PasteMenuItem.Image = Properties.Resources.Paste;
            PasteMenuItem.Name = "PasteMenuItem";
            PasteMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            PasteMenuItem.Size = new Size(209, 22);
            PasteMenuItem.Text = "Paste";
            PasteMenuItem.Click += PasteMenuItem_Click;
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            toolStripSeparator11.Size = new Size(206, 6);
            // 
            // CollapseChildrenMenuItem
            // 
            CollapseChildrenMenuItem.Image = Properties.Resources.CollapseAll;
            CollapseChildrenMenuItem.Name = "CollapseChildrenMenuItem";
            CollapseChildrenMenuItem.Size = new Size(209, 22);
            CollapseChildrenMenuItem.Text = "Collapse Children";
            CollapseChildrenMenuItem.Click += CollapseChildrenMenuItem_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(206, 6);
            // 
            // RemoveChildNodesMenuItem
            // 
            RemoveChildNodesMenuItem.Image = Properties.Resources.ClearCollection;
            RemoveChildNodesMenuItem.Name = "RemoveChildNodesMenuItem";
            RemoveChildNodesMenuItem.Size = new Size(209, 22);
            RemoveChildNodesMenuItem.Text = "Remove child nodes";
            RemoveChildNodesMenuItem.Click += RemoveChildNodesMenuItem_Click;
            // 
            // ClearMenuItem
            // 
            ClearMenuItem.Image = Properties.Resources.ClearWindowContent;
            ClearMenuItem.Name = "ClearMenuItem";
            ClearMenuItem.Size = new Size(209, 22);
            ClearMenuItem.Text = "Clear Texts";
            ClearMenuItem.Click += ClearMenuItem_Click;
            // 
            // ShowImportResultMenuItem
            // 
            ShowImportResultMenuItem.DropDownItems.AddRange(new ToolStripItem[] { ImportResultFilterUnchangedMenuItem, ImportResultFilterNewMenuItem, ImportResultFilterNotFoundMenuItem, ImportResultFilterUpdatedMenuItem });
            ShowImportResultMenuItem.Name = "ShowImportResultMenuItem";
            ShowImportResultMenuItem.Size = new Size(209, 22);
            ShowImportResultMenuItem.Text = "Show Import Result";
            // 
            // ImportResultFilterUnchangedMenuItem
            // 
            ImportResultFilterUnchangedMenuItem.Name = "ImportResultFilterUnchangedMenuItem";
            ImportResultFilterUnchangedMenuItem.Size = new Size(135, 22);
            ImportResultFilterUnchangedMenuItem.Text = "Unchanged";
            ImportResultFilterUnchangedMenuItem.Click += ImportResultFilterUnchangedMenuItem_Click;
            // 
            // ImportResultFilterNewMenuItem
            // 
            ImportResultFilterNewMenuItem.Name = "ImportResultFilterNewMenuItem";
            ImportResultFilterNewMenuItem.Size = new Size(135, 22);
            ImportResultFilterNewMenuItem.Text = "New";
            ImportResultFilterNewMenuItem.Click += ImportResultFilterNewMenuItem_Click;
            // 
            // ImportResultFilterNotFoundMenuItem
            // 
            ImportResultFilterNotFoundMenuItem.Name = "ImportResultFilterNotFoundMenuItem";
            ImportResultFilterNotFoundMenuItem.Size = new Size(135, 22);
            ImportResultFilterNotFoundMenuItem.Text = "Not Found";
            ImportResultFilterNotFoundMenuItem.Click += ImportResultFilterNotFoundMenuItem_Click;
            // 
            // ImportResultFilterUpdatedMenuItem
            // 
            ImportResultFilterUpdatedMenuItem.Name = "ImportResultFilterUpdatedMenuItem";
            ImportResultFilterUpdatedMenuItem.Size = new Size(135, 22);
            ImportResultFilterUpdatedMenuItem.Text = "Updated";
            ImportResultFilterUpdatedMenuItem.Click += ImportResultFilterUpdatedMenuItem_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(206, 6);
            // 
            // DeleteMenuItem
            // 
            DeleteMenuItem.Image = (Image)resources.GetObject("DeleteMenuItem.Image");
            DeleteMenuItem.Name = "DeleteMenuItem";
            DeleteMenuItem.ShortcutKeys = Keys.Delete;
            DeleteMenuItem.Size = new Size(209, 22);
            DeleteMenuItem.Text = "Delete";
            DeleteMenuItem.Click += DeleteMenuItem_Click;
            // 
            // ImageListExplorer
            // 
            ImageListExplorer.ColorDepth = ColorDepth.Depth32Bit;
            ImageListExplorer.ImageStream = (ImageListStreamer)resources.GetObject("ImageListExplorer.ImageStream");
            ImageListExplorer.TransparentColor = Color.Transparent;
            ImageListExplorer.Images.SetKeyName(0, "Web.png");
            ImageListExplorer.Images.SetKeyName(1, "WebReferenceFolder.png");
            ImageListExplorer.Images.SetKeyName(2, "WebSubfolder.png");
            ImageListExplorer.Images.SetKeyName(3, "WebFile.png");
            ImageListExplorer.Images.SetKeyName(4, "FolderClosed.png");
            ImageListExplorer.Images.SetKeyName(5, "FolderOpened.png");
            ImageListExplorer.Images.SetKeyName(6, "XSLTTemplate.png");
            ImageListExplorer.Images.SetKeyName(7, "DocumentError.png");
            ImageListExplorer.Images.SetKeyName(8, "TextBlock.png");
            ImageListExplorer.Images.SetKeyName(9, "FolderClosedPurple.png");
            ImageListExplorer.Images.SetKeyName(10, "FolderOpenedNoColor.png");
            // 
            // StateImageListExplorer
            // 
            StateImageListExplorer.ColorDepth = ColorDepth.Depth32Bit;
            StateImageListExplorer.ImageStream = (ImageListStreamer)resources.GetObject("StateImageListExplorer.ImageStream");
            StateImageListExplorer.TransparentColor = Color.Transparent;
            StateImageListExplorer.Images.SetKeyName(0, "CalloutOval.png");
            StateImageListExplorer.Images.SetKeyName(1, "ValidateWarning.png");
            // 
            // menuStrip1
            // 
            menuStrip1.AutoSize = false;
            menuStrip1.Font = new Font("Segoe UI", 9.75F);
            menuStrip1.GripMargin = new Padding(0);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 3);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 3, 0, 3);
            menuStrip1.RenderMode = ToolStripRenderMode.System;
            menuStrip1.Size = new Size(1064, 28);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { NewMenuItem, OpenMenuItem, toolStripSeparator2, PublishMenuItem, SaveMenuItem, toolStripSeparator5, CloseMenuItem, toolStripSeparator1, ExitMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(39, 22);
            fileToolStripMenuItem.Text = "&File";
            // 
            // NewMenuItem
            // 
            NewMenuItem.Image = Properties.Resources.NewDocument;
            NewMenuItem.Name = "NewMenuItem";
            NewMenuItem.Size = new Size(164, 22);
            NewMenuItem.Text = "&New";
            NewMenuItem.Click += NewMenuItem_Click;
            // 
            // OpenMenuItem
            // 
            OpenMenuItem.Image = (Image)resources.GetObject("OpenMenuItem.Image");
            OpenMenuItem.Name = "OpenMenuItem";
            OpenMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            OpenMenuItem.Size = new Size(164, 22);
            OpenMenuItem.Text = "&Open...";
            OpenMenuItem.Click += OpenMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(161, 6);
            // 
            // PublishMenuItem
            // 
            PublishMenuItem.Image = Properties.Resources.PublishAllWebsites;
            PublishMenuItem.Name = "PublishMenuItem";
            PublishMenuItem.ShortcutKeys = Keys.Control | Keys.P;
            PublishMenuItem.Size = new Size(164, 22);
            PublishMenuItem.Text = "&Publish";
            PublishMenuItem.Click += PublishMenuItem_Click;
            // 
            // SaveMenuItem
            // 
            SaveMenuItem.Image = Properties.Resources.Save;
            SaveMenuItem.Name = "SaveMenuItem";
            SaveMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            SaveMenuItem.Size = new Size(164, 22);
            SaveMenuItem.Text = "&Save";
            SaveMenuItem.Click += SaveMenuItem_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(161, 6);
            // 
            // CloseMenuItem
            // 
            CloseMenuItem.Name = "CloseMenuItem";
            CloseMenuItem.Size = new Size(164, 22);
            CloseMenuItem.Text = "&Close";
            CloseMenuItem.Click += CloseMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(161, 6);
            // 
            // ExitMenuItem
            // 
            ExitMenuItem.Name = "ExitMenuItem";
            ExitMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            ExitMenuItem.Size = new Size(164, 22);
            ExitMenuItem.Text = "E&xit";
            ExitMenuItem.Click += ExitMenuItem_Click;
            // 
            // CultureGrid
            // 
            CultureGrid.Columns.AddRange(new ColumnHeader[] { cultureColumnHeader, textColumnHeader, lengthColumnHeader });
            CultureGrid.ContextMenuStrip = contextMenuCultureGrid;
            CultureGrid.Dock = DockStyle.Fill;
            CultureGrid.FullRowSelect = true;
            CultureGrid.GridLines = true;
            listViewGroup1.Header = "ListViewGroup";
            listViewGroup1.Name = "listViewGroup1";
            CultureGrid.Groups.AddRange(new ListViewGroup[] { listViewGroup1 });
            CultureGrid.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            CultureGrid.Location = new Point(0, 0);
            CultureGrid.Margin = new Padding(3, 4, 3, 4);
            CultureGrid.MultiSelect = false;
            CultureGrid.Name = "CultureGrid";
            CultureGrid.ShowGroups = false;
            CultureGrid.Size = new Size(651, 457);
            CultureGrid.SmallImageList = ImageListExplorer;
            CultureGrid.TabIndex = 3;
            CultureGrid.UseCompatibleStateImageBehavior = false;
            CultureGrid.View = View.Details;
            CultureGrid.DoubleClick += CultureGrid_DoubleClick;
            // 
            // cultureColumnHeader
            // 
            cultureColumnHeader.Text = "Culture";
            cultureColumnHeader.Width = 232;
            // 
            // textColumnHeader
            // 
            textColumnHeader.Text = "Text";
            textColumnHeader.Width = 400;
            // 
            // lengthColumnHeader
            // 
            lengthColumnHeader.Text = "Length";
            lengthColumnHeader.TextAlign = HorizontalAlignment.Right;
            lengthColumnHeader.Width = 80;
            // 
            // contextMenuCultureGrid
            // 
            contextMenuCultureGrid.Items.AddRange(new ToolStripItem[] { CopyTextMenuItem });
            contextMenuCultureGrid.Name = "contextMenuCultureGrid";
            contextMenuCultureGrid.Size = new Size(169, 26);
            contextMenuCultureGrid.Opening += ContextMenuCultureGrid_Opening;
            // 
            // CopyTextMenuItem
            // 
            CopyTextMenuItem.Image = Properties.Resources.Copy;
            CopyTextMenuItem.Name = "CopyTextMenuItem";
            CopyTextMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            CopyTextMenuItem.Size = new Size(168, 22);
            CopyTextMenuItem.Text = "Copy Text";
            CopyTextMenuItem.Click += CopyTextMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.AutoSize = false;
            statusStrip1.Items.AddRange(new ToolStripItem[] { ProjectStatusLoading, ProjectStatusLabel, ProjectValidationStatusLabel });
            statusStrip1.Location = new Point(0, 738);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(1064, 25);
            statusStrip1.TabIndex = 4;
            statusStrip1.Text = "statusStrip1";
            // 
            // ProjectStatusLoading
            // 
            ProjectStatusLoading.Name = "ProjectStatusLoading";
            ProjectStatusLoading.Padding = new Padding(15, 0, 0, 0);
            ProjectStatusLoading.Size = new Size(114, 20);
            ProjectStatusLoading.Text = "Loading project...";
            ProjectStatusLoading.Visible = false;
            // 
            // ProjectStatusLabel
            // 
            ProjectStatusLabel.Name = "ProjectStatusLabel";
            ProjectStatusLabel.Padding = new Padding(15, 0, 0, 0);
            ProjectStatusLabel.Size = new Size(133, 20);
            ProjectStatusLabel.Text = "toolStripStatusLabel1";
            // 
            // ProjectValidationStatusLabel
            // 
            ProjectValidationStatusLabel.Name = "ProjectValidationStatusLabel";
            ProjectValidationStatusLabel.Size = new Size(118, 20);
            ProjectValidationStatusLabel.Text = "toolStripStatusLabel1";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 66);
            splitContainer1.Margin = new Padding(3, 4, 3, 4);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(ExplorerPanel);
            splitContainer1.Panel1.Controls.Add(splitter2);
            splitContainer1.Panel1.Controls.Add(PropertiesPanel);
            splitContainer1.Panel1.Padding = new Padding(5, 0, 0, 0);
            splitContainer1.Panel1MinSize = 400;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.BackColor = SystemColors.Window;
            splitContainer1.Panel2.Controls.Add(GridPanel);
            splitContainer1.Panel2.Controls.Add(splitter1);
            splitContainer1.Panel2.Controls.Add(SearchPanel);
            splitContainer1.Panel2.Padding = new Padding(0, 0, 5, 0);
            splitContainer1.Size = new Size(1064, 667);
            splitContainer1.SplitterDistance = 403;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 6;
            // 
            // ExplorerPanel
            // 
            ExplorerPanel.Controls.Add(Explorer);
            ExplorerPanel.Controls.Add(ProjectExplorerCaption);
            ExplorerPanel.Dock = DockStyle.Fill;
            ExplorerPanel.Location = new Point(5, 0);
            ExplorerPanel.Name = "ExplorerPanel";
            ExplorerPanel.Size = new Size(398, 262);
            ExplorerPanel.TabIndex = 4;
            // 
            // ProjectExplorerCaption
            // 
            ProjectExplorerCaption.BackColor = SystemColors.Control;
            ProjectExplorerCaption.Dock = DockStyle.Top;
            ProjectExplorerCaption.Font = new Font("Segoe UI", 9F);
            ProjectExplorerCaption.Location = new Point(0, 0);
            ProjectExplorerCaption.Name = "ProjectExplorerCaption";
            ProjectExplorerCaption.Size = new Size(398, 25);
            ProjectExplorerCaption.TabIndex = 2;
            ProjectExplorerCaption.Text = ":::: Project Explorer";
            ProjectExplorerCaption.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // splitter2
            // 
            splitter2.Dock = DockStyle.Bottom;
            splitter2.Location = new Point(5, 262);
            splitter2.Name = "splitter2";
            splitter2.Size = new Size(398, 7);
            splitter2.TabIndex = 2;
            splitter2.TabStop = false;
            // 
            // PropertiesPanel
            // 
            PropertiesPanel.Controls.Add(PropertyGrid1);
            PropertiesPanel.Controls.Add(PropertiesCaption);
            PropertiesPanel.Dock = DockStyle.Bottom;
            PropertiesPanel.Location = new Point(5, 269);
            PropertiesPanel.Name = "PropertiesPanel";
            PropertiesPanel.Size = new Size(398, 398);
            PropertiesPanel.TabIndex = 3;
            // 
            // PropertyGrid1
            // 
            PropertyGrid1.Dock = DockStyle.Fill;
            PropertyGrid1.Font = new Font("Segoe UI", 9.75F);
            PropertyGrid1.Location = new Point(0, 25);
            PropertyGrid1.Name = "PropertyGrid1";
            PropertyGrid1.Size = new Size(398, 373);
            PropertyGrid1.TabIndex = 1;
            PropertyGrid1.ToolbarVisible = false;
            PropertyGrid1.PropertyValueChanged += PropertyGrid1_PropertyValueChanged;
            PropertyGrid1.Enter += PropertyGrid1_Enter;
            PropertyGrid1.Leave += PropertyGrid1_Leave;
            // 
            // PropertiesCaption
            // 
            PropertiesCaption.BackColor = SystemColors.Control;
            PropertiesCaption.Dock = DockStyle.Top;
            PropertiesCaption.Font = new Font("Segoe UI", 9F);
            PropertiesCaption.Location = new Point(0, 0);
            PropertiesCaption.Name = "PropertiesCaption";
            PropertiesCaption.Size = new Size(398, 25);
            PropertiesCaption.TabIndex = 1;
            PropertiesCaption.Text = ":::: Properties";
            PropertiesCaption.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // GridPanel
            // 
            GridPanel.Controls.Add(PublishedMessageLabel);
            GridPanel.Controls.Add(LiteralValueLabel);
            GridPanel.Controls.Add(CultureGrid);
            GridPanel.Controls.Add(splitter3);
            GridPanel.Controls.Add(NoTextSelectedLabel);
            GridPanel.Dock = DockStyle.Fill;
            GridPanel.Location = new Point(0, 0);
            GridPanel.Name = "GridPanel";
            GridPanel.Size = new Size(651, 460);
            GridPanel.TabIndex = 8;
            // 
            // PublishedMessageLabel
            // 
            PublishedMessageLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            PublishedMessageLabel.BackColor = Color.SkyBlue;
            PublishedMessageLabel.FlatStyle = FlatStyle.Popup;
            PublishedMessageLabel.Font = new Font("Segoe UI", 9.75F);
            PublishedMessageLabel.Location = new Point(382, 0);
            PublishedMessageLabel.Name = "PublishedMessageLabel";
            PublishedMessageLabel.Size = new Size(274, 37);
            PublishedMessageLabel.TabIndex = 12;
            PublishedMessageLabel.Text = "Publish operation successfully completed.";
            PublishedMessageLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LiteralValueLabel
            // 
            LiteralValueLabel.BackColor = SystemColors.Window;
            LiteralValueLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            LiteralValueLabel.Location = new Point(177, 205);
            LiteralValueLabel.Name = "LiteralValueLabel";
            LiteralValueLabel.Size = new Size(297, 53);
            LiteralValueLabel.TabIndex = 8;
            LiteralValueLabel.Text = "This text is set as literal. Check the \"Literal Text\" property value.";
            LiteralValueLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // splitter3
            // 
            splitter3.Dock = DockStyle.Bottom;
            splitter3.Location = new Point(0, 457);
            splitter3.Name = "splitter3";
            splitter3.Size = new Size(651, 3);
            splitter3.TabIndex = 4;
            splitter3.TabStop = false;
            // 
            // NoTextSelectedLabel
            // 
            NoTextSelectedLabel.BackColor = SystemColors.Window;
            NoTextSelectedLabel.ForeColor = SystemColors.ControlDark;
            NoTextSelectedLabel.Location = new Point(29, 25);
            NoTextSelectedLabel.Name = "NoTextSelectedLabel";
            NoTextSelectedLabel.Size = new Size(297, 53);
            NoTextSelectedLabel.TabIndex = 7;
            NoTextSelectedLabel.Text = "Select a text node from the explorer.";
            NoTextSelectedLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // splitter1
            // 
            splitter1.BackColor = SystemColors.Control;
            splitter1.Dock = DockStyle.Bottom;
            splitter1.Location = new Point(0, 460);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(651, 7);
            splitter1.TabIndex = 9;
            splitter1.TabStop = false;
            // 
            // SearchPanel
            // 
            SearchPanel.Controls.Add(SearchResultsGrid);
            SearchPanel.Controls.Add(toolStrip3);
            SearchPanel.Controls.Add(SearchCaption);
            SearchPanel.Dock = DockStyle.Bottom;
            SearchPanel.Location = new Point(0, 467);
            SearchPanel.Name = "SearchPanel";
            SearchPanel.Size = new Size(651, 200);
            SearchPanel.TabIndex = 5;
            // 
            // SearchResultsGrid
            // 
            SearchResultsGrid.BorderStyle = BorderStyle.None;
            SearchResultsGrid.Columns.AddRange(new ColumnHeader[] { columnHeader1 });
            SearchResultsGrid.Dock = DockStyle.Fill;
            SearchResultsGrid.Font = new Font("Segoe UI", 11.25F);
            SearchResultsGrid.FullRowSelect = true;
            SearchResultsGrid.HeaderStyle = ColumnHeaderStyle.None;
            SearchResultsGrid.LabelWrap = false;
            SearchResultsGrid.Location = new Point(0, 58);
            SearchResultsGrid.MultiSelect = false;
            SearchResultsGrid.Name = "SearchResultsGrid";
            SearchResultsGrid.ShowGroups = false;
            SearchResultsGrid.Size = new Size(651, 142);
            SearchResultsGrid.SmallImageList = ImageListExplorer;
            SearchResultsGrid.Sorting = SortOrder.Ascending;
            SearchResultsGrid.TabIndex = 7;
            SearchResultsGrid.UseCompatibleStateImageBehavior = false;
            SearchResultsGrid.View = View.Details;
            SearchResultsGrid.DoubleClick += SearchResultsGrid_DoubleClick;
            SearchResultsGrid.Enter += SearchResultsGrid_Enter;
            SearchResultsGrid.Leave += SearchResultsGrid_Leave;
            // 
            // columnHeader1
            // 
            columnHeader1.Width = 600;
            // 
            // toolStrip3
            // 
            toolStrip3.AutoSize = false;
            toolStrip3.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip3.Items.AddRange(new ToolStripItem[] { toolStripLabel1, SearchBox, ClearSearchResultsButton });
            toolStrip3.Location = new Point(0, 25);
            toolStrip3.Name = "toolStrip3";
            toolStrip3.Padding = new Padding(8, 0, 8, 0);
            toolStrip3.Size = new Size(651, 33);
            toolStrip3.TabIndex = 8;
            toolStrip3.Text = "toolStrip3";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripLabel1.Image = Properties.Resources.Search;
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(16, 30);
            toolStripLabel1.Text = "toolStripLabel1";
            // 
            // SearchBox
            // 
            SearchBox.AutoSize = false;
            SearchBox.Name = "SearchBox";
            SearchBox.Padding = new Padding(10, 0, 0, 0);
            SearchBox.Size = new Size(30, 25);
            SearchBox.Enter += SearchBox_Enter;
            SearchBox.Leave += SearchBox_Leave;
            SearchBox.TextChanged += SearchBox_TextChanged;
            // 
            // ClearSearchResultsButton
            // 
            ClearSearchResultsButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ClearSearchResultsButton.Image = (Image)resources.GetObject("ClearSearchResultsButton.Image");
            ClearSearchResultsButton.ImageTransparentColor = Color.Magenta;
            ClearSearchResultsButton.Name = "ClearSearchResultsButton";
            ClearSearchResultsButton.Size = new Size(23, 30);
            ClearSearchResultsButton.Text = "toolStripButton1";
            ClearSearchResultsButton.ToolTipText = "Clear Resutls";
            ClearSearchResultsButton.Click += ClearSearchResultsButton_Click;
            // 
            // SearchCaption
            // 
            SearchCaption.BackColor = SystemColors.Control;
            SearchCaption.Dock = DockStyle.Top;
            SearchCaption.Font = new Font("Segoe UI", 9F);
            SearchCaption.ImageAlign = ContentAlignment.MiddleRight;
            SearchCaption.Location = new Point(0, 0);
            SearchCaption.Name = "SearchCaption";
            SearchCaption.Size = new Size(651, 25);
            SearchCaption.TabIndex = 0;
            SearchCaption.Text = ":::: Search";
            SearchCaption.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStrip1
            // 
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { NavigateBackwardButton, NavigateForwardButton, toolStripSeparator4, ButtonOpenMenuItem, SaveButton, toolStripSeparator3, PreviousErrorButton, NextErrorButton, PublishButton, toolStripSeparator9, SortButton, toolStripSeparator10, PreviousImportResultButton, NextImportResultButton, toolStripSeparator12, ValidationCultureComboBox });
            toolStrip1.Location = new Point(0, 31);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new Padding(15, 5, 5, 5);
            toolStrip1.Size = new Size(1064, 35);
            toolStrip1.TabIndex = 10;
            toolStrip1.Text = "toolStrip1";
            // 
            // NavigateBackwardButton
            // 
            NavigateBackwardButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            NavigateBackwardButton.Image = (Image)resources.GetObject("NavigateBackwardButton.Image");
            NavigateBackwardButton.ImageTransparentColor = Color.Magenta;
            NavigateBackwardButton.Name = "NavigateBackwardButton";
            NavigateBackwardButton.Overflow = ToolStripItemOverflow.Never;
            NavigateBackwardButton.Size = new Size(23, 22);
            NavigateBackwardButton.Text = "toolStripButton3";
            NavigateBackwardButton.ToolTipText = "Navigate Backward";
            NavigateBackwardButton.Click += NavigateBackwardButton_Click;
            // 
            // NavigateForwardButton
            // 
            NavigateForwardButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            NavigateForwardButton.Image = (Image)resources.GetObject("NavigateForwardButton.Image");
            NavigateForwardButton.ImageTransparentColor = Color.Magenta;
            NavigateForwardButton.Name = "NavigateForwardButton";
            NavigateForwardButton.Overflow = ToolStripItemOverflow.Never;
            NavigateForwardButton.Size = new Size(23, 22);
            NavigateForwardButton.Text = "toolStripButton4";
            NavigateForwardButton.ToolTipText = "Navigate Forward";
            NavigateForwardButton.Click += NavigateForwardButton_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 25);
            // 
            // ButtonOpenMenuItem
            // 
            ButtonOpenMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ButtonOpenMenuItem.Image = (Image)resources.GetObject("ButtonOpenMenuItem.Image");
            ButtonOpenMenuItem.ImageTransparentColor = Color.Magenta;
            ButtonOpenMenuItem.Name = "ButtonOpenMenuItem";
            ButtonOpenMenuItem.Size = new Size(23, 22);
            ButtonOpenMenuItem.Text = "Open";
            ButtonOpenMenuItem.ToolTipText = "Open Project (Ctrl+O)";
            ButtonOpenMenuItem.Click += OpenMenuItem_Click;
            // 
            // SaveButton
            // 
            SaveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            SaveButton.Image = Properties.Resources.Save;
            SaveButton.ImageTransparentColor = Color.Magenta;
            SaveButton.Name = "SaveButton";
            SaveButton.Padding = new Padding(4, 0, 0, 0);
            SaveButton.Size = new Size(24, 22);
            SaveButton.Text = "Save";
            SaveButton.ToolTipText = "Save Project (Ctrl+S)";
            SaveButton.Click += SaveMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
            // 
            // PreviousErrorButton
            // 
            PreviousErrorButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PreviousErrorButton.Image = (Image)resources.GetObject("PreviousErrorButton.Image");
            PreviousErrorButton.ImageTransparentColor = Color.Magenta;
            PreviousErrorButton.Name = "PreviousErrorButton";
            PreviousErrorButton.Size = new Size(23, 22);
            PreviousErrorButton.Text = "toolStripButton2";
            PreviousErrorButton.ToolTipText = "Previous Error";
            PreviousErrorButton.Click += PreviousErrorButton_Click;
            // 
            // NextErrorButton
            // 
            NextErrorButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            NextErrorButton.Image = (Image)resources.GetObject("NextErrorButton.Image");
            NextErrorButton.ImageTransparentColor = Color.Magenta;
            NextErrorButton.Name = "NextErrorButton";
            NextErrorButton.Size = new Size(23, 22);
            NextErrorButton.Text = "toolStripButton1";
            NextErrorButton.ToolTipText = "Next Error";
            NextErrorButton.Click += NextErrorButton_Click;
            // 
            // PublishButton
            // 
            PublishButton.Alignment = ToolStripItemAlignment.Right;
            PublishButton.Image = Properties.Resources.PublishAllWebsites;
            PublishButton.ImageTransparentColor = Color.Magenta;
            PublishButton.Name = "PublishButton";
            PublishButton.Padding = new Padding(0, 0, 5, 0);
            PublishButton.Size = new Size(71, 22);
            PublishButton.Text = "Publish";
            PublishButton.ToolTipText = "Publish Language Packages (Ctrl+P)";
            PublishButton.Click += PublishMenuItem_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(6, 25);
            // 
            // SortButton
            // 
            SortButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            SortButton.Image = Properties.Resources.SortDescending;
            SortButton.ImageTransparentColor = Color.Magenta;
            SortButton.Name = "SortButton";
            SortButton.Size = new Size(23, 22);
            SortButton.Text = "Sort";
            SortButton.ToolTipText = "Sort";
            SortButton.Click += SortButton_Click;
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new Size(6, 25);
            // 
            // PreviousImportResultButton
            // 
            PreviousImportResultButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PreviousImportResultButton.Image = (Image)resources.GetObject("PreviousImportResultButton.Image");
            PreviousImportResultButton.ImageTransparentColor = Color.Magenta;
            PreviousImportResultButton.Name = "PreviousImportResultButton";
            PreviousImportResultButton.Size = new Size(23, 22);
            PreviousImportResultButton.Text = "toolStripButton1";
            PreviousImportResultButton.ToolTipText = "Previous Import Result";
            PreviousImportResultButton.Click += PreviousImportedNodeButton_Click;
            // 
            // NextImportResultButton
            // 
            NextImportResultButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            NextImportResultButton.Image = (Image)resources.GetObject("NextImportResultButton.Image");
            NextImportResultButton.ImageTransparentColor = Color.Magenta;
            NextImportResultButton.Name = "NextImportResultButton";
            NextImportResultButton.Size = new Size(23, 22);
            NextImportResultButton.Text = "toolStripButton2";
            NextImportResultButton.ToolTipText = "Next Import Result";
            NextImportResultButton.Click += NextImportedNodeButton_Click;
            // 
            // toolStripSeparator12
            // 
            toolStripSeparator12.Name = "toolStripSeparator12";
            toolStripSeparator12.Size = new Size(6, 25);
            // 
            // ValidationCultureComboBox
            // 
            ValidationCultureComboBox.AutoSize = false;
            ValidationCultureComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            ValidationCultureComboBox.Font = new Font("Segoe UI", 9.75F);
            ValidationCultureComboBox.Name = "ValidationCultureComboBox";
            ValidationCultureComboBox.Size = new Size(240, 25);
            ValidationCultureComboBox.Sorted = true;
            ValidationCultureComboBox.ToolTipText = "Active culture for validation";
            ValidationCultureComboBox.SelectedIndexChanged += ValidationCultureComboBox_SelectedIndexChanged;
            // 
            // Timer1
            // 
            Timer1.Interval = 5000;
            Timer1.Tick += Timer1_Tick;
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 733);
            panel1.Name = "panel1";
            panel1.Size = new Size(1064, 5);
            panel1.TabIndex = 11;
            // 
            // PublishedMessageTimer
            // 
            PublishedMessageTimer.Interval = 3000;
            PublishedMessageTimer.Tick += PublishedMessageTimer_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1064, 763);
            Controls.Add(splitContainer1);
            Controls.Add(panel1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            Controls.Add(statusStrip1);
            Font = new Font("Segoe UI", 11.25F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new Padding(3, 4, 3, 4);
            Name = "MainForm";
            Padding = new Padding(0, 3, 0, 0);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Text Repository Editor (Engendro 4.0)";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            contextMenuExplorer.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            contextMenuCultureGrid.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ExplorerPanel.ResumeLayout(false);
            PropertiesPanel.ResumeLayout(false);
            GridPanel.ResumeLayout(false);
            SearchPanel.ResumeLayout(false);
            toolStrip3.ResumeLayout(false);
            toolStrip3.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TreeView Explorer;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem OpenMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem ExitMenuItem;
        private ListView CultureGrid;
        private StatusStrip statusStrip1;
        private ToolStripMenuItem NewMenuItem;
        private ToolStripStatusLabel ProjectStatusLabel;
        private ContextMenuStrip contextMenuExplorer;
        private ImageList ImageListExplorer;
        private SplitContainer splitContainer1;
        private ToolStripMenuItem NewLanguagePackageMenuItem;
        private ToolStripMenuItem NewFolderMenuItem;
        private ToolStripMenuItem NewTextMenuItem;
        private ToolStripMenuItem DeleteMenuItem;
        private ToolStripSeparator deleteSeparator;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem SaveMenuItem;
        private ColumnHeader cultureColumnHeader;
        private ColumnHeader textColumnHeader;
        private ColumnHeader lengthColumnHeader;
        private Label NoTextSelectedLabel;
        private Panel GridPanel;
        private Splitter splitter2;
        private PropertyGrid PropertyGrid1;
        private ToolStrip toolStrip1;
        private ToolStripButton ButtonOpenMenuItem;
        private ToolStripButton SaveButton;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem PublishMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem ImportMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem ClearMenuItem;
        private ToolStripMenuItem CutMenuItem;
        private ToolStripMenuItem CopyMenuItem;
        private ToolStripMenuItem PasteMenuItem;
        private ImageList StateImageListExplorer;
        private ToolStripSeparator toolStripSeparator8;
        private Splitter splitter3;
        private Panel SearchPanel;
        private Label SearchCaption;
        private ListView SearchResultsGrid;
        private Panel PropertiesPanel;
        private Label PropertiesCaption;
        private Splitter splitter1;
        private Panel ExplorerPanel;
        private Label ProjectExplorerCaption;
        private ToolStripMenuItem RemoveChildNodesMenuItem;
        private System.Windows.Forms.Timer Timer1;
        private ColumnHeader columnHeader1;
        private ToolStrip toolStrip3;
        private ToolStripTextBox SearchBox;
        private ToolStripButton NavigateBackwardButton;
        private ToolStripButton NavigateForwardButton;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton PreviousErrorButton;
        private ToolStripButton NextErrorButton;
        private ToolStripLabel toolStripLabel1;
        private ToolStripStatusLabel ProjectStatusLoading;
        private ToolStripButton ClearSearchResultsButton;
        private Panel panel1;
        private ToolStripButton PublishButton;
        private ToolStripMenuItem CloseMenuItem;
        private ToolStripSeparator LanguagePackageSeparator;
        private ToolStripMenuItem CopyNodePathMenuItem;
        private Label LiteralValueLabel;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripStatusLabel ProjectValidationStatusLabel;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripButton SortButton;
        private Label PublishedMessageLabel;
        private System.Windows.Forms.Timer PublishedMessageTimer;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripComboBox ValidationCultureComboBox;
        private ContextMenuStrip contextMenuCultureGrid;
        private ToolStripMenuItem CopyTextMenuItem;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripMenuItem CollapseChildrenMenuItem;
        private ToolStripButton PreviousImportResultButton;
        private ToolStripButton NextImportResultButton;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripMenuItem ShowImportResultMenuItem;
        private ToolStripMenuItem ImportResultFilterUnchangedMenuItem;
        private ToolStripMenuItem ImportResultFilterNewMenuItem;
        private ToolStripMenuItem ImportResultFilterNotFoundMenuItem;
        private ToolStripMenuItem ImportResultFilterUpdatedMenuItem;
    }
}