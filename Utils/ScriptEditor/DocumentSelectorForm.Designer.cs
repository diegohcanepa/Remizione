namespace Engendro
{
    partial class DocumentSelectorForm
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
            DocumentSelector = new System.Windows.Forms.TreeView();
            ActiveDocumentLabel = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // DocumentSelector
            // 
            DocumentSelector.BackColor = System.Drawing.SystemColors.Control;
            DocumentSelector.BorderStyle = System.Windows.Forms.BorderStyle.None;
            DocumentSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            DocumentSelector.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            DocumentSelector.FullRowSelect = true;
            DocumentSelector.ItemHeight = 28;
            DocumentSelector.Location = new System.Drawing.Point(10, 75);
            DocumentSelector.Name = "DocumentSelector";
            DocumentSelector.ShowLines = false;
            DocumentSelector.ShowPlusMinus = false;
            DocumentSelector.ShowRootLines = false;
            DocumentSelector.Size = new System.Drawing.Size(316, 293);
            DocumentSelector.TabIndex = 0;
            DocumentSelector.AfterSelect += DocumentSelector_AfterSelect;
            DocumentSelector.KeyDown += DocumentSelector_KeyDown;
            // 
            // ActiveDocumentLabel
            // 
            ActiveDocumentLabel.Dock = System.Windows.Forms.DockStyle.Top;
            ActiveDocumentLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            ActiveDocumentLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            ActiveDocumentLabel.Location = new System.Drawing.Point(10, 10);
            ActiveDocumentLabel.Name = "ActiveDocumentLabel";
            ActiveDocumentLabel.Size = new System.Drawing.Size(316, 38);
            ActiveDocumentLabel.TabIndex = 1;
            ActiveDocumentLabel.Text = "label1";
            // 
            // label1
            // 
            label1.Dock = System.Windows.Forms.DockStyle.Top;
            label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label1.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            label1.Location = new System.Drawing.Point(10, 48);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(316, 27);
            label1.TabIndex = 2;
            label1.Text = "Active Files";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DocumentSelectorForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(336, 378);
            Controls.Add(DocumentSelector);
            Controls.Add(label1);
            Controls.Add(ActiveDocumentLabel);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DocumentSelectorForm";
            Padding = new System.Windows.Forms.Padding(10);
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "DocumentSelectorForm";
            TopMost = true;
            Activated += DocumentSelectorForm_Activated;
            KeyUp += DocumentSelectorForm_KeyUp;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TreeView DocumentSelector;
        private System.Windows.Forms.Label ActiveDocumentLabel;
        private System.Windows.Forms.Label label1;
    }
}