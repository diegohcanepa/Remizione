namespace TextRepositoryEditor
{
    partial class NewLanguagePackageForm
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.acceptButton = new System.Windows.Forms.Button();
            this.CultureListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(473, 420);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(91, 34);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // acceptButton
            // 
            this.acceptButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.acceptButton.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.acceptButton.Location = new System.Drawing.Point(375, 420);
            this.acceptButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.Size = new System.Drawing.Size(91, 34);
            this.acceptButton.TabIndex = 6;
            this.acceptButton.Text = "OK";
            this.acceptButton.UseVisualStyleBackColor = true;
            // 
            // CultureListBox
            // 
            this.CultureListBox.FormattingEnabled = true;
            this.CultureListBox.ItemHeight = 20;
            this.CultureListBox.Location = new System.Drawing.Point(14, 14);
            this.CultureListBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CultureListBox.Name = "CultureListBox";
            this.CultureListBox.Size = new System.Drawing.Size(550, 384);
            this.CultureListBox.Sorted = true;
            this.CultureListBox.TabIndex = 8;
            this.CultureListBox.SelectedIndexChanged += new System.EventHandler(this.CultureListBox_SelectedIndexChanged);
            // 
            // NewLanguagePackageForm
            // 
            this.AcceptButton = this.acceptButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(579, 468);
            this.Controls.Add(this.CultureListBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.acceptButton);
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewLanguagePackageForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Language Package";
            this.Load += new System.EventHandler(this.NewLanguagePackageForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Button cancelButton;
        private Button acceptButton;
        private ListBox CultureListBox;
    }
}