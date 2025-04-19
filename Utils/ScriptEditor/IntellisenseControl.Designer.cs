namespace Engendro
{
    partial class IntellisenseControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ItemsListView = new System.Windows.Forms.ListView();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            SuspendLayout();
            // 
            // ItemsListView
            // 
            ItemsListView.BackColor = System.Drawing.Color.WhiteSmoke;
            ItemsListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ItemsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1 });
            ItemsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            ItemsListView.FullRowSelect = true;
            ItemsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            ItemsListView.LabelWrap = false;
            ItemsListView.Location = new System.Drawing.Point(3, 3);
            ItemsListView.MultiSelect = false;
            ItemsListView.Name = "ItemsListView";
            ItemsListView.ShowGroups = false;
            ItemsListView.Size = new System.Drawing.Size(586, 459);
            ItemsListView.TabIndex = 13;
            ItemsListView.UseCompatibleStateImageBehavior = false;
            ItemsListView.View = System.Windows.Forms.View.Details;
            ItemsListView.DoubleClick += ItemListView_DoubleClick;
            ItemsListView.KeyDown += ItemListView_KeyDown;
            ItemsListView.KeyPress += ItemListView_KeyPress;
            ItemsListView.Leave += ItemListView_Leave;
            // 
            // columnHeader1
            // 
            columnHeader1.Width = 400;
            // 
            // IntellisenseControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(ItemsListView);
            Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "IntellisenseControl";
            Size = new System.Drawing.Size(592, 465);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.ListView ItemsListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}
