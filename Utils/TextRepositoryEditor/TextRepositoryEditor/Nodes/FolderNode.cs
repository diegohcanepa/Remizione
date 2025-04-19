using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Xml;
using System.Xml.Linq;

namespace TextRepositoryEditor
{
    /// <summary>
    /// FolderNode
    /// </summary>
    public sealed class FolderNode : CustomTreeNode
    {
        // Constructor
        public FolderNode(string text = "")
            : base(text, AttributeName.Folder)
        {
            Invalidate();
        }

        #region Private members

        // CloneFolder
        private FolderNode CloneFolder(FolderNode sourceFolderNode)
        {
            var clonedFolder = new FolderNode();

            foreach (TreeNode node in sourceFolderNode.Nodes)
            {
                if (node is TextNode textNode)
                {
                    var clonedTextNode = textNode.CloneNode();
                    clonedTextNode.Text = textNode.Text;
                    clonedFolder.Nodes.Add(clonedTextNode);
                }
                else if (node is FolderNode folderNode)
                {
                    var clonedChildFolder = CloneFolder(folderNode);
                    clonedChildFolder.Text = folderNode.Text;
                    clonedFolder.Nodes.Add(clonedChildFolder);
                }
            }

            return clonedFolder;
        }

        #endregion

        #region Protected members

        // CanHostNode
        protected override bool CanHostNode(CustomTreeNode node) => node is FolderNode || node is TextNode;

        // CloneNodeCore
        protected override CustomTreeNode? CloneNodeCore() => CloneFolder(this);

        // OnInvalidate
        protected override void OnInvalidate()
        {
            if (IsPredefinedFolder)
                ImageIndex = IsExpanded ? (int)NodeImage.ImportsFolderOpen : (int)NodeImage.ImportsFolderClosed;
            else
                ImageIndex = IsExpanded ? (int)NodeImage.FolderOpen : (int)NodeImage.FolderClosed;

            SelectedImageIndex = ImageIndex;
        }

        // OnReadChildren
        protected override void OnReadChildren(XElement element)
        {
            // Texts
            foreach (XElement childElement in element.Elements(AttributeName.LocalizableText.ToString()))
            {
                var childNode = new TextNode(string.Empty);
                Nodes.Add(childNode);
                childNode.Read(childElement);
            }

            // Folders
            foreach (XElement childElement in element.Elements(AttributeName.Folder.ToString()))
            {
                var childNode = new FolderNode(string.Empty);
                Nodes.Add(childNode);
                childNode.Read(childElement);
            }
        }

        // OnWriteChildren
        protected override void OnWriteChildren(XmlWriter output)
        {
            // Texts
            foreach (var node in Nodes.OfType<TextNode>())
            {
                node.Write(output);
            }

            // Folders
            foreach (var node in Nodes.OfType<FolderNode>())
            {
                node.Write(output);
            }
        }

        #endregion

        // AllowDrag
        public override bool AllowDrag => !IsPredefinedFolder;

        // AllowLabelEdit
        public override bool AllowLabelEdit => !IsPredefinedFolder;

        // AllowRemove
        public override bool AllowRemove => !IsPredefinedFolder;

        // CanClone
        public override bool CanClone => !IsPredefinedFolder;

        // ImportsNamePrefix
        public const string ImportsNamePrefix = "<IMPORTS>";

        // IsPredefinedFolder
        public bool IsPredefinedFolder => Text == ImportsNamePrefix;

        // NewNamePrefix
        public override string NewNamePrefix => "New Folder";
    }
}
