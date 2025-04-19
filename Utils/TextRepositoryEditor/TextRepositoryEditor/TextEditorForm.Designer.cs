namespace TextRepositoryEditor
{
    partial class TextEditorForm
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
            this.ButtonOK = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.CommentsLabel = new System.Windows.Forms.Label();
            this.CommentImage = new System.Windows.Forms.PictureBox();
            this.LocalizableTextBox = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CommentImage)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonOK
            // 
            this.ButtonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ButtonOK.Location = new System.Drawing.Point(664, 19);
            this.ButtonOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(101, 31);
            this.ButtonOK.TabIndex = 6;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            this.ButtonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(772, 19);
            this.ButtonCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(101, 31);
            this.ButtonCancel.TabIndex = 7;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.CommentsLabel);
            this.panel1.Controls.Add(this.CommentImage);
            this.panel1.Controls.Add(this.ButtonCancel);
            this.panel1.Controls.Add(this.ButtonOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(5, 352);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(886, 68);
            this.panel1.TabIndex = 8;
            // 
            // CommentsLabel
            // 
            this.CommentsLabel.AutoEllipsis = true;
            this.CommentsLabel.Location = new System.Drawing.Point(29, 7);
            this.CommentsLabel.Name = "CommentsLabel";
            this.CommentsLabel.Size = new System.Drawing.Size(630, 53);
            this.CommentsLabel.TabIndex = 9;
            this.CommentsLabel.Text = "label1";
            // 
            // CommentImage
            // 
            this.CommentImage.Image = global::TextRepositoryEditor.Properties.Resources.Comment;
            this.CommentImage.Location = new System.Drawing.Point(7, 7);
            this.CommentImage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CommentImage.Name = "CommentImage";
            this.CommentImage.Size = new System.Drawing.Size(16, 16);
            this.CommentImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.CommentImage.TabIndex = 8;
            this.CommentImage.TabStop = false;
            // 
            // LocalizableTextBox
            // 
            this.LocalizableTextBox.AcceptsReturn = true;
            this.LocalizableTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LocalizableTextBox.Location = new System.Drawing.Point(5, 5);
            this.LocalizableTextBox.Multiline = true;
            this.LocalizableTextBox.Name = "LocalizableTextBox";
            this.LocalizableTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.LocalizableTextBox.Size = new System.Drawing.Size(886, 347);
            this.LocalizableTextBox.TabIndex = 9;
            this.LocalizableTextBox.TabStop = false;
            this.LocalizableTextBox.WordWrap = false;
            this.LocalizableTextBox.TextChanged += new System.EventHandler(this.LocalizableTextBox_TextChanged);
            // 
            // TextEditorForm
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(896, 425);
            this.Controls.Add(this.LocalizableTextBox);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(912, 464);
            this.Name = "TextEditorForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TextEditorForm";
            this.Shown += new System.EventHandler(this.TextEditorForm_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CommentImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button ButtonOK;
        private Button ButtonCancel;
        private Panel panel1;
        private PictureBox CommentImage;
        private Label CommentsLabel;
        private TextBox LocalizableTextBox;
    }
}