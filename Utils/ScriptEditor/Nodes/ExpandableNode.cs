using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Engendro
{
    /// <summary>
    /// ExpandableNode
    /// </summary>
    public abstract class ExpandableNode : ObjectModelNode
    {
        // Constructor
        protected ExpandableNode(string text, XmlAttributeName attributeName, bool serializeName, TreeView? documentExplorer)
            : base(text, attributeName, serializeName, documentExplorer)
        {
        }

        #region Protected members

        // OnRead
        protected override void OnRead(ProjectNode projectNode, XElement element)
        {
            // Scripts
            foreach (var scriptElement in element.Elements(XmlAttributeName.Script.ToString()))
            {
                ScriptNode scriptNode = new(projectNode.DocumentExplorer);
                Nodes.Add(scriptNode);
                scriptNode.Read(projectNode, scriptElement);
            }

            // Folders
            foreach (var folderElement in element.Elements(XmlAttributeName.Folder.ToString()))
            {
                FolderNode folderNode = new(string.Empty);
                Nodes.Add(folderNode);
                folderNode.Read(projectNode, folderElement);
            }
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            // Scripts
            foreach (var scriptNode in Nodes.OfType<ScriptNode>())
            {
                scriptNode.Write(output);
            }

            // Parent nodes
            foreach (var parentNode in Nodes.OfType<ExpandableNode>())
            {
                parentNode.Write(output);
            }
        }

        #endregion

        // Remove
        public new void Remove()
        {
            foreach (var childNode in this.GetChildNodesRecursively().OfType<ScriptNode>())
            {
                childNode.Dispose();
            }

            base.Remove();
        }

        // Text
        public new string Text
        {
            get => base.Text;
            set
            {
                if (value != Text)
                {
                    base.Text = value;
                    NotifyProjectChange();
                }
            }
        }
    }
}
