namespace TextRepositoryEditor
{
    partial class ImportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            ListViewGroup listViewGroup1 = new ListViewGroup("New", HorizontalAlignment.Left);
            ListViewGroup listViewGroup2 = new ListViewGroup("Updated", HorizontalAlignment.Left);
            ListViewGroup listViewGroup3 = new ListViewGroup("Orphan (not found in scripts)", HorizontalAlignment.Left);
            ImportButton = new Button();
            ButtonClose = new Button();
            ListView1 = new ListView();
            IdHeader = new ColumnHeader();
            Value1Header = new ColumnHeader();
            label1 = new Label();
            CurrentValueLabel = new Label();
            CurrentValueTitleLabel = new Label();
            toolTip1 = new ToolTip(components);
            SuspendLayout();
            // 
            // ImportButton
            // 
            ImportButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ImportButton.DialogResult = DialogResult.OK;
            ImportButton.Location = new Point(886, 536);
            ImportButton.Name = "ImportButton";
            ImportButton.Size = new Size(109, 34);
            ImportButton.TabIndex = 1;
            ImportButton.Text = "Import";
            ImportButton.UseVisualStyleBackColor = true;
            // 
            // ButtonClose
            // 
            ButtonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ButtonClose.Location = new Point(1001, 536);
            ButtonClose.Name = "ButtonClose";
            ButtonClose.Size = new Size(109, 34);
            ButtonClose.TabIndex = 2;
            ButtonClose.Text = "Close";
            ButtonClose.UseVisualStyleBackColor = true;
            // 
            // ListView1
            // 
            ListView1.Columns.AddRange(new ColumnHeader[] { IdHeader, Value1Header });
            ListView1.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            ListView1.FullRowSelect = true;
            listViewGroup1.CollapsedState = ListViewGroupCollapsedState.Collapsed;
            listViewGroup1.Header = "New";
            listViewGroup1.Name = "AdditionGroup";
            listViewGroup2.CollapsedState = ListViewGroupCollapsedState.Collapsed;
            listViewGroup2.Header = "Updated";
            listViewGroup2.Name = "UpdateGroups";
            listViewGroup3.Header = "Orphan (not found in scripts)";
            listViewGroup3.Name = "OrphanGroup";
            ListView1.Groups.AddRange(new ListViewGroup[] { listViewGroup1, listViewGroup2, listViewGroup3 });
            ListView1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            ListView1.LabelWrap = false;
            ListView1.Location = new Point(19, 20);
            ListView1.MultiSelect = false;
            ListView1.Name = "ListView1";
            ListView1.Size = new Size(1091, 488);
            ListView1.Sorting = SortOrder.Ascending;
            ListView1.TabIndex = 0;
            ListView1.UseCompatibleStateImageBehavior = false;
            ListView1.View = View.Details;
            ListView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
            // 
            // IdHeader
            // 
            IdHeader.Text = "ID";
            IdHeader.Width = 400;
            // 
            // Value1Header
            // 
            Value1Header.Text = "Value";
            Value1Header.Width = 650;
            // 
            // label1
            // 
            label1.Location = new Point(246, -2);
            label1.Name = "label1";
            label1.Size = new Size(552, 28);
            label1.TabIndex = 3;
            label1.Text = "No additions or updates found in the imported file.";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CurrentValueLabel
            // 
            CurrentValueLabel.AutoEllipsis = true;
            CurrentValueLabel.Location = new Point(19, 536);
            CurrentValueLabel.Name = "CurrentValueLabel";
            CurrentValueLabel.Size = new Size(846, 37);
            CurrentValueLabel.TabIndex = 4;
            CurrentValueLabel.Text = "label2";
            // 
            // CurrentValueTitleLabel
            // 
            CurrentValueTitleLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            CurrentValueTitleLabel.Location = new Point(19, 515);
            CurrentValueTitleLabel.Name = "CurrentValueTitleLabel";
            CurrentValueTitleLabel.Size = new Size(846, 21);
            CurrentValueTitleLabel.TabIndex = 5;
            CurrentValueTitleLabel.Text = "Original value:";
            // 
            // toolTip1
            // 
            toolTip1.IsBalloon = true;
            // 
            // ImportForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = ButtonClose;
            ClientSize = new Size(1129, 582);
            Controls.Add(CurrentValueTitleLabel);
            Controls.Add(CurrentValueLabel);
            Controls.Add(label1);
            Controls.Add(ListView1);
            Controls.Add(ButtonClose);
            Controls.Add(ImportButton);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ImportForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Import Preview";
            ResumeLayout(false);
        }

        #endregion

        private Button ImportButton;
        private Button ButtonClose;
        private ListView ListView1;
        private ColumnHeader IdHeader;
        private ColumnHeader Value1Header;
        private Label label1;
        private Label CurrentValueLabel;
        private Label CurrentValueTitleLabel;
        private ToolTip toolTip1;
    }
}