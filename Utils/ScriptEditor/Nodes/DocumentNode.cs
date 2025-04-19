using System.Windows.Forms;

namespace Engendro.Nodes
{
    /// <summary>
    /// DocumentNode
    /// </summary>
    public sealed class DocumentNode : TreeNode
    {
        private bool isPinned;

        // Constructor
        public DocumentNode(ObjectModelNode node)
            : base()
        {
            this.ObjectModelNode = node;
        }

        // IsPinned
        public bool IsPinned
        {
            get => isPinned;
            set
            {
                if (value != isPinned)
                {
                    isPinned = value;
                    StateImageIndex = isPinned ? 0 : -1;
                }
            }
        }

        // KeyName
        public string KeyName => $"{Text}-{ImageIndex}";

        // ObjectModelNode
        public ObjectModelNode ObjectModelNode { get; }
    }
}
