using Engendro.Nodes;
using System.ComponentModel;
using System.Windows.Forms;

namespace Engendro
{
    public partial class DocumentSelectorForm : Form
    {
        private ProjectNode? projectNode;

        public DocumentSelectorForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        // InvalidaNode
        private static void InvalidaNode(TreeNode node)
        {
            if (node.Tag is TreeNode n)
            {
                node.Text = n.Text;
                node.ImageIndex = n.ImageIndex;
                node.SelectedImageIndex = n.ImageIndex;
            }
        }

        // InvalidateLabel
        private void InvalidateLabel()
        {
            if (DocumentSelector.SelectedNode is TreeNode node)
            {
                ActiveDocumentLabel.Text = "     " + node.Text;
                ActiveDocumentLabel.ImageIndex = node.ImageIndex;
            }
        }

        private void DocumentSelectorForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Control && projectNode?.DocumentExplorer != null)
            {
                if (DocumentSelector.SelectedNode is TreeNode selectedNode && selectedNode.Tag is DocumentNode documentNode)
                {
                    SelectedDocument = documentNode;
                }

                DialogResult = DialogResult.OK;
            }
        }

        private void DocumentSelectorForm_Activated(object sender, System.EventArgs e)
        {
            DocumentSelector.BeginUpdate();

            DocumentSelector.Nodes.Clear();

            // Populate
            if (projectNode != null)
            {
                TreeNode? selectedNode = null;
                foreach (var node in projectNode.Documents)
                {
                    var newNode = DocumentSelector.Nodes.Add(node.Text);
                    newNode.Tag = node;
                    InvalidaNode(newNode);

                    if (node.IsSelected)
                    {
                        selectedNode = newNode;
                    }
                }


                if (selectedNode != null)
                {
                    if (selectedNode.NextNode != null)
                    {
                        DocumentSelector.SelectedNode = selectedNode.NextNode;
                    }
                }

            }

            DocumentSelector.EndUpdate();

            InvalidateLabel();
        }

        private void DocumentSelector_KeyDown(object sender, KeyEventArgs e)
        {
            if (DocumentSelector.SelectedNode is not TreeNode selectedNode)
            {
                return;
            }

            if (e.KeyCode == Keys.Tab)
            {
                if (e.Shift)
                {
                    if (selectedNode.PrevNode == null)
                    {
                        DocumentSelector.SelectedNode = DocumentSelector.Nodes[DocumentSelector.Nodes.Count - 1];
                    }
                    else
                    {
                        DocumentSelector.SelectedNode = selectedNode.PrevNode;
                    }
                }
                else
                {
                    if (selectedNode.NextNode == null)
                    {
                        DocumentSelector.SelectedNode = DocumentSelector.Nodes[0];
                    }
                    else
                    {
                        DocumentSelector.SelectedNode = selectedNode.NextNode;
                    }
                }
            }
        }

        private void DocumentSelector_AfterSelect(object sender, TreeViewEventArgs e)
        {
            InvalidateLabel();
        }

        #region Protected members

        // CreateParams
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ClassStyle |= 0x00020000;
                return cp;
            }
        }

        #endregion

        // SelectedDocument
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DocumentNode? SelectedDocument { get; private set; }

        // Show
        public void Show(ProjectNode projectNode)
        {
            this.projectNode = projectNode;
            DocumentSelector.ImageList = projectNode?.DocumentExplorer?.ImageList;
            ActiveDocumentLabel.ImageList = projectNode?.DocumentExplorer?.ImageList;
            SelectedDocument = null;
            ShowDialog();
        }
    }
}
