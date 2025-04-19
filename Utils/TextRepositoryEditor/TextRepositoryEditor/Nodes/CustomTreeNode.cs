using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Linq;

namespace TextRepositoryEditor
{
    /// <summary>
    /// CustomTreeNode
    /// </summary>
    public abstract class CustomTreeNode : TreeNode
    {
        private string comments = string.Empty;
        private readonly List<string> errors = [];
        private bool expandAfterRead;
        private bool isValidating;

        #region Constructor

        // Constructor
        protected CustomTreeNode(string text, AttributeName attributeName)
            : base(text)
        {
            this.AttributeName = attributeName;
            this.Errors = new ReadOnlyCollection<string>(errors);
            NotifyChangeCore();
        }

        #endregion

        #region Protected members

        // CanHostNode
        protected virtual bool CanHostNode(CustomTreeNode node) => false;

        // CloneNodeCore
        protected virtual CustomTreeNode? CloneNodeCore() => null;

        // HasProjectNode
        protected bool HasProjectNode => TreeView?.TopNode is ProjectNode;

        // NotifyChangeCore
        protected void NotifyChangeCore()
        {
            if (TreeView?.TopNode is not null)
            {
                ProjectNode.NotifyChange();
                Validate();
                Invalidate();
            }
        }

        // OnInvalidate
        protected virtual void OnInvalidate()
        {
        }

        // OnReadAttributes
        protected virtual void OnReadAttributes(XElement element)
        {
        }

        // OnReadChildren
        protected virtual void OnReadChildren(XElement element)
        {
        }

        // OnRemove
        protected virtual void OnRemove()
        {
        }

        // OnValidate
        protected virtual string[] OnValidate() => [];

        // OnWriteAttributes
        protected virtual void OnWriteAttributes(XmlWriter output)
        {
        }

        // OnWriteChildren
        protected virtual void OnWriteChildren(XmlWriter output)
        {
        }

        // OnReadTextValue
        protected virtual string OnReadTextValue() => Text;

        // OnTransformTextValue
        protected virtual string OnTransformTextValue(string text) => text;

        #endregion

        // AllowDrag
        public virtual bool AllowDrag => false;

        // AllowLabelEdit
        public virtual bool AllowLabelEdit => !Text.StartsWith('<');

        // AllowRemove
        public virtual bool AllowRemove => true;

        // AttributeName
        public AttributeName AttributeName { get; }

        // CanBeParentOf
        public bool CanBeParentOf(CustomTreeNode node)
        {
            if (TreeView == null || this == node || node.IsParentOf(this))
                return false;

            return CanHostNode(node);
        }

        // CanClone
        public virtual bool CanClone => false;

        // CloneNode
        public CustomTreeNode CloneNode()
        {
            if (!CanClone)
                throw new InvalidOperationException();

            var result = CloneNodeCore() ?? throw new InvalidOperationException();
            return result;
        }

        // Comments
        public string Comments
        {
            get => comments;
            set
            {
                if (value != comments)
                {
                    comments = value;
                    Invalidate();
                    ProjectNode.NotifyChange();
                }
            }
        }

        // Errors
        public ReadOnlyCollection<string> Errors { get; }

        // GenerateUniqueName
        public void GenerateUniqueName() => GenerateUniqueName(NewNamePrefix);

        // GenerateUniqueName
        public void GenerateUniqueName(string prefix)
        {
            if (Parent is not TreeNode parentNode)
                return;

            var index = 1;
            var newText = prefix;
            while (true)
            {
                if (parentNode.FindChild<TreeNode>(newText) != null)
                {
                    newText = $"{prefix} ({index})";
                    index++;
                }
                else
                    break;
            }

            Text = newText;
        }

        // GetPath
        public string GetPath() => GetPath(".", false);

        // GetPath
        public string GetPath(bool excludeProject) => GetPath(".", excludeProject);

        // GetPath
        public string GetPath(string separator, bool excludeProject)
        {
            var names = new List<string>();

            TreeNode node = this;
            while (node != null)
            {
                names.Insert(0, node.Text);
                node = node.Parent;

                if (node is ProjectNode && excludeProject)
                    break;
            }

            return string.Join(separator, names);
        }

        // HasComments
        public bool HasComments => !string.IsNullOrWhiteSpace(Comments);

        // HasErrors
        public bool HasErrors => Errors.Count > 0;

        // HasValidationErrors
        public virtual bool HasValidationErrors => false;

        // Invalidate
        public void Invalidate()
        {
            StateImageIndex = HasComments ? 0 : -1;
            ToolTipText = HasComments ? ("[Comments]" + Environment.NewLine + comments) : string.Empty;
            OnInvalidate();
        }

        // IsParentOf
        public bool IsParentOf(TreeNode node)
        {
            // Same node
            if (this == node || node.Parent == null)
                return false;

            if (node.Parent == this)
                return true;

            // If the parent node is not null or equal to the first node,   
            // call the ContainsNode method recursively using the parent of   
            // the second node.  
            return IsParentOf(node.Parent);
        }

        // NewNamePrefix
        public abstract string NewNamePrefix { get; }

        // NotifyLoaded
        public void NotifyLoaded()
        {
            if (expandAfterRead)
                Expand();
        }

        // ProjectNode
        public ProjectNode ProjectNode
        {
            get
            {
                if (TreeView.Nodes.Count > 0 && TreeView.Nodes[0] is ProjectNode projectNode)
                    return projectNode;
                else
                    throw new InvalidOperationException();
            }
        }

        // PropertyGridNodeWrapper
        public object? PropertyGridNodeWrapper { get; protected set; }

        // Read
        public void Read(XElement element)
        {
            if (element.Attribute(AttributeName.Text.ToString())?.Value is string textValue)
                Text = OnTransformTextValue(textValue);
            else
                throw new InvalidOperationException();

            // Comments
            if (element.Attribute(AttributeName.Comments.ToString())?.Value is string commentsValue)
                Comments = commentsValue;

            // IsExpanded
            if (element.Attribute(AttributeName.IsExpanded.ToString())?.Value is string isExpandedValue)
                expandAfterRead = XmlConvert.ToBoolean(isExpandedValue);

            OnReadAttributes(element);
            OnReadChildren(element);
        }

        // Remove
        public new void Remove()
        {
            OnRemove();
            ProjectNode.NotifyChange();
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
                    if (HasProjectNode)
                        ProjectNode.NotifyChange();
                }
            }
        }

        // Validate
        public int Validate()
        {
            if (isValidating)
                return 0;

            isValidating = true;
            errors.Clear();
            errors.AddRange(OnValidate());
            isValidating = false;
            Invalidate();

            return errors.Count;
        }

        // Write
        public void Write(XmlWriter output)
        {
            output.WriteStartElement(AttributeName.ToString());
            output.WriteAttributeString(AttributeName.Text.ToString(), OnTransformTextValue(Text));
            output.WriteAttributeString(AttributeName.Comments.ToString(), Comments);
            output.WriteAttributeString(AttributeName.IsExpanded.ToString(), XmlConvert.ToString(IsExpanded));
            OnWriteAttributes(output);
            OnWriteChildren(output);
            output.WriteEndElement();
        }
    }
}
