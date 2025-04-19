namespace Engendro
{
    /// <summary>
    /// FolderNode
    /// </summary>
    public sealed class FolderNode : ExpandableNode
    {
        // Constructor
        public FolderNode(string text)
            : base(text, XmlAttributeName.Folder, true, null)
        {
            Invalidate();
        }

        // Invalidate
        public void Invalidate()
        {
            ImageIndex = IsExpanded ? (int)NodeImage.FolderOpened : (int)NodeImage.FolderClosed;
            SelectedImageIndex = ImageIndex;
        }
    }
}

