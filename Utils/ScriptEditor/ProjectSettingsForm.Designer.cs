namespace Engendro
{
    partial class ProjectSettingsForm
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
            Accept = new System.Windows.Forms.Button();
            Cancel = new System.Windows.Forms.Button();
            Output = new System.Windows.Forms.TextBox();
            BrowseButton = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            EncryptionKey = new System.Windows.Forms.TextBox();
            UseEncryption = new System.Windows.Forms.CheckBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            BuildID = new System.Windows.Forms.NumericUpDown();
            label3 = new System.Windows.Forms.Label();
            groupBox2 = new System.Windows.Forms.GroupBox();
            FileNameLabel = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            LinesLabel = new System.Windows.Forms.Label();
            ScriptsLabel = new System.Windows.Forms.Label();
            FoldersLabel = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)BuildID).BeginInit();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // Accept
            // 
            Accept.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            Accept.DialogResult = System.Windows.Forms.DialogResult.OK;
            Accept.Location = new System.Drawing.Point(329, 442);
            Accept.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Accept.Name = "Accept";
            Accept.Size = new System.Drawing.Size(80, 28);
            Accept.TabIndex = 1;
            Accept.Text = "OK";
            Accept.UseVisualStyleBackColor = true;
            Accept.Click += Accept_Click;
            // 
            // Cancel
            // 
            Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Cancel.Location = new System.Drawing.Point(415, 442);
            Cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Cancel.Name = "Cancel";
            Cancel.Size = new System.Drawing.Size(80, 28);
            Cancel.TabIndex = 2;
            Cancel.Text = "Cancel";
            Cancel.UseVisualStyleBackColor = true;
            // 
            // Output
            // 
            Output.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Output.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            Output.Location = new System.Drawing.Point(19, 50);
            Output.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Output.Name = "Output";
            Output.Size = new System.Drawing.Size(407, 25);
            Output.TabIndex = 1;
            // 
            // BrowseButton
            // 
            BrowseButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            BrowseButton.Location = new System.Drawing.Point(432, 48);
            BrowseButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BrowseButton.Name = "BrowseButton";
            BrowseButton.Size = new System.Drawing.Size(31, 29);
            BrowseButton.TabIndex = 0;
            BrowseButton.Text = "...";
            BrowseButton.UseVisualStyleBackColor = true;
            BrowseButton.Click += BrowseButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            label1.Location = new System.Drawing.Point(19, 30);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(185, 17);
            label1.TabIndex = 0;
            label1.Text = "Script Library Output filename:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            label2.Location = new System.Drawing.Point(19, 146);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(95, 17);
            label2.TabIndex = 4;
            label2.Text = "Encryption key:";
            // 
            // EncryptionKey
            // 
            EncryptionKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            EncryptionKey.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            EncryptionKey.Location = new System.Drawing.Point(19, 167);
            EncryptionKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            EncryptionKey.Name = "EncryptionKey";
            EncryptionKey.Size = new System.Drawing.Size(407, 25);
            EncryptionKey.TabIndex = 5;
            // 
            // UseEncryption
            // 
            UseEncryption.AutoSize = true;
            UseEncryption.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            UseEncryption.Location = new System.Drawing.Point(19, 207);
            UseEncryption.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            UseEncryption.Name = "UseEncryption";
            UseEncryption.Size = new System.Drawing.Size(114, 21);
            UseEncryption.TabIndex = 6;
            UseEncryption.Text = "Use Encryption";
            UseEncryption.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(BrowseButton);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(BuildID);
            groupBox1.Controls.Add(EncryptionKey);
            groupBox1.Controls.Add(Output);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(UseEncryption);
            groupBox1.Controls.Add(label3);
            groupBox1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            groupBox1.Location = new System.Drawing.Point(12, 12);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(483, 247);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = " General ";
            // 
            // BuildID
            // 
            BuildID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            BuildID.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            BuildID.Location = new System.Drawing.Point(19, 108);
            BuildID.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            BuildID.Name = "BuildID";
            BuildID.Size = new System.Drawing.Size(146, 25);
            BuildID.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            label3.Location = new System.Drawing.Point(19, 88);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(55, 17);
            label3.TabIndex = 2;
            label3.Text = "Build ID:";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(FileNameLabel);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(LinesLabel);
            groupBox2.Controls.Add(ScriptsLabel);
            groupBox2.Controls.Add(FoldersLabel);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(label4);
            groupBox2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            groupBox2.Location = new System.Drawing.Point(12, 275);
            groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox2.Size = new System.Drawing.Size(483, 156);
            groupBox2.TabIndex = 9;
            groupBox2.TabStop = false;
            groupBox2.Text = " Information ";
            // 
            // FileNameLabel
            // 
            FileNameLabel.AutoEllipsis = true;
            FileNameLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            FileNameLabel.Location = new System.Drawing.Point(96, 31);
            FileNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            FileNameLabel.Name = "FileNameLabel";
            FileNameLabel.Size = new System.Drawing.Size(379, 17);
            FileNameLabel.TabIndex = 12;
            FileNameLabel.Text = "25";
            FileNameLabel.UseMnemonic = false;
            FileNameLabel.UseWaitCursor = true;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            label7.Location = new System.Drawing.Point(19, 30);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(66, 17);
            label7.TabIndex = 11;
            label7.Text = "File name:";
            label7.UseWaitCursor = true;
            // 
            // LinesLabel
            // 
            LinesLabel.AutoSize = true;
            LinesLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            LinesLabel.Location = new System.Drawing.Point(96, 121);
            LinesLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LinesLabel.Name = "LinesLabel";
            LinesLabel.Size = new System.Drawing.Size(22, 17);
            LinesLabel.TabIndex = 10;
            LinesLabel.Text = "25";
            LinesLabel.UseWaitCursor = true;
            // 
            // ScriptsLabel
            // 
            ScriptsLabel.AutoSize = true;
            ScriptsLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            ScriptsLabel.Location = new System.Drawing.Point(96, 91);
            ScriptsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            ScriptsLabel.Name = "ScriptsLabel";
            ScriptsLabel.Size = new System.Drawing.Size(22, 17);
            ScriptsLabel.TabIndex = 9;
            ScriptsLabel.Text = "25";
            ScriptsLabel.UseWaitCursor = true;
            // 
            // FoldersLabel
            // 
            FoldersLabel.AutoSize = true;
            FoldersLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            FoldersLabel.Location = new System.Drawing.Point(96, 62);
            FoldersLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            FoldersLabel.Name = "FoldersLabel";
            FoldersLabel.Size = new System.Drawing.Size(22, 17);
            FoldersLabel.TabIndex = 8;
            FoldersLabel.Text = "25";
            FoldersLabel.UseWaitCursor = true;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            label6.Location = new System.Drawing.Point(19, 121);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(74, 17);
            label6.TabIndex = 7;
            label6.Text = "Script lines:";
            label6.UseWaitCursor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            label5.Location = new System.Drawing.Point(19, 91);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(50, 17);
            label5.TabIndex = 6;
            label5.Text = "Scripts:";
            label5.UseWaitCursor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            label4.Location = new System.Drawing.Point(19, 61);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(54, 17);
            label4.TabIndex = 5;
            label4.Text = "Folders:";
            label4.UseWaitCursor = true;
            // 
            // ProjectSettingsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(511, 481);
            Controls.Add(groupBox2);
            Controls.Add(Cancel);
            Controls.Add(Accept);
            Controls.Add(groupBox1);
            Font = new System.Drawing.Font("Segoe UI", 9.75F);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProjectSettingsForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Project Settings";
            Load += ProjectSettingsForm_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)BuildID).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button Accept;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.TextBox Output;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox EncryptionKey;
        private System.Windows.Forms.CheckBox UseEncryption;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown BuildID;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label LinesLabel;
        private System.Windows.Forms.Label ScriptsLabel;
        private System.Windows.Forms.Label FoldersLabel;
        private System.Windows.Forms.Label FileNameLabel;
        private System.Windows.Forms.Label label7;
    }
}