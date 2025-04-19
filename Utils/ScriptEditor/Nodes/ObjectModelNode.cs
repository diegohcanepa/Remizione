using Engendro.Nodes;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Engendro
{
    /// <summary>
    /// ObjectModelNode
    /// </summary>
    public abstract class ObjectModelNode : TreeNode
    {
        #region Private members

        private readonly XmlAttributeName attributeName;
        private bool expandAfterLoading;
        private bool isOpeningDocument;
        private readonly bool serializeName;

        #endregion

        #region Constructor

        // Constructor
        protected ObjectModelNode(string text, XmlAttributeName attributeName, bool serializeName, TreeView? documentExplorer)
            : base(text)
        {
            this.attributeName = attributeName;
            this.serializeName = serializeName;
            this.DocumentExplorer = documentExplorer;

            if (documentExplorer != null)
            {
                this.DocumentNode = new DocumentNode(this);
            }
        }

        #endregion

        #region Protected members

        // InvalidateDocumentNode
        protected void InvalidateDocumentNode()
        {
            OnInvalidateDocumentNode();
        }

        // NotifyLoadedCompleted
        protected internal void NotifyLoadedCompleted()
        {
            if (expandAfterLoading)
            {
                Expand();
            }

            OnLoadCompleted();
        }

        // NotifyProjectChange
        protected void NotifyProjectChange()
        {
            if (TreeView == null)
                return;

            if (TreeView.TopNode is ProjectNode projectNode && !projectNode.IsLoading)
            {
                projectNode.NotifyChange();
            }
        }

        // OnCloseDocument
        protected virtual void OnCloseDocument()
        {
        }

        // OnInvalidateDocumentNode
        protected virtual void OnInvalidateDocumentNode()
        {
        }

        // OnLoadCompleted
        protected virtual void OnLoadCompleted()
        {
        }

        // OnOpenDocument
        protected virtual void OnOpenDocument(bool selectDocument)
        {
        }

        // OnRead
        protected virtual void OnRead(ProjectNode projectNode, XElement element)
        {
        }

        // OnReadAttributes
        protected virtual void OnReadAttributes(XElement element)
        {
        }

        // OnWrite
        protected virtual void OnWrite(XmlWriter output)
        {
        }

        // OnWriteAttributes
        protected virtual void OnWriteAttributes(XmlWriter output)
        {
        }

        // Read
        protected internal void Read(ProjectNode projectNode, XElement element)
        {
            // Name
            if (element.Attribute(XmlAttributeName.Name.ToString())?.Value is string textValue)
            {
                Text = textValue;
            }

            // IsExpanded
            expandAfterLoading = element.Attribute(XmlAttributeName.IsExpanded.ToString())?.Value is not null;

            OnReadAttributes(element);
            OnRead(projectNode, element);

            // IsSelected
            if (element.Attribute(XmlAttributeName.IsSelected.ToString())?.Value is not null && TreeView != null)
            {
                TreeView.SelectedNode = this;
            }

            // IsDocumentOpen
            if (element.Attribute(XmlAttributeName.IsDocumentOpen.ToString())?.Value is not null)
            {
                var select = element.Attribute(XmlAttributeName.IsDocumentSelected.ToString())?.Value is not null;
                OpenDocument(select);
            }

            // IsDocumentPinned
            if (DocumentNode != null && element.Attribute(XmlAttributeName.IsDocumentPinned.ToString())?.Value is not null)
            {
                DocumentNode.IsPinned = true;
            }
        }

        // SerializeNodeName
        protected virtual bool SerializeNodeName => true;

        // Write
        protected internal void Write(XmlWriter output)
        {
            output.WriteStartElement(attributeName.ToString());

            if (serializeName)
            {
                output.WriteAttributeString(XmlAttributeName.Name.ToString(), Text);
            }

            // IsSelected
            if (IsSelected)
            {
                output.WriteAttributeString(XmlAttributeName.IsSelected.ToString(), XmlConvert.ToString(true));
            }

            // IsDocumentOpen
            if (IsDocumentOpen)
            {
                output.WriteAttributeString(XmlAttributeName.IsDocumentOpen.ToString(), XmlConvert.ToString(true));
            }

            // IsDocumentSelected
            if (IsDocumentSelected)
            {
                output.WriteAttributeString(XmlAttributeName.IsDocumentSelected.ToString(), XmlConvert.ToString(true));
            }

            // IsDocumentPinned
            if (IsDocumentPinned)
            {
                output.WriteAttributeString(XmlAttributeName.IsDocumentPinned.ToString(), XmlConvert.ToString(true));
            }

            // IsExpanded
            if (IsExpanded)
            {
                output.WriteAttributeString(XmlAttributeName.IsExpanded.ToString(), XmlConvert.ToString(true));
            }

            OnWriteAttributes(output);
            OnWrite(output);

            output.WriteEndElement();
        }

        #endregion

        // AutomaticallySortDocumentExplorer
        public static bool AutomaticallySortDocumentExplorer { get; set; } = true;

        // CloseDocument
        public void CloseDocument()
        {
            OnCloseDocument();
        }

        // DocumentExplorer
        public TreeView? DocumentExplorer { get; }

        // DocumentNode
        public DocumentNode? DocumentNode { get; }

        // IsDocumentOpen
        public bool IsDocumentOpen => DocumentNode?.TreeView != null;

        // IsDocumentPinned
        public bool IsDocumentPinned => DocumentNode?.TreeView != null && DocumentNode.IsPinned;

        // IsDocumentSelected
        public bool IsDocumentSelected => DocumentNode?.TreeView != null && DocumentNode.IsSelected;

        // OpenDocument
        public void OpenDocument(bool select)
        {
            if (DocumentExplorer is null || DocumentNode == null || isOpeningDocument)
            {
                return;
            }

            isOpeningDocument = true;

            if (!IsDocumentOpen)
            {
                OnOpenDocument(select);
                DocumentExplorer.Nodes.Add(DocumentNode);
            }

            if (select)
            {
                DocumentExplorer.SelectedNode = DocumentNode;
            }

            if (AutomaticallySortDocumentExplorer)
            {
                DocumentExplorer.SortTree();
            }

            isOpeningDocument = false;
        }

        // Remove
        public new void Remove()
        {
            NotifyProjectChange();
            base.Remove();
        }
    }
}
