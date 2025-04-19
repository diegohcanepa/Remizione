using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Design;
using System.Xml;
using System.Xml.Linq;

namespace TextRepositoryEditor
{
    /// <summary>
    /// ProjectNode
    /// </summary>
    public sealed class ProjectNode : CustomTreeNode
    {
        #region Private fields

        private string encryptionKey = string.Empty;
        private bool hasChanges;
        private readonly Action onChange;
        private string outputFolder = string.Empty;

        #endregion

        #region Constructor

        // Constructor
        public ProjectNode(Action onChange)
            : base("Project", AttributeName.Project)
        {
            this.onChange = onChange;

            ImageIndex = (int)NodeImage.Project;
            SelectedImageIndex = ImageIndex;

            PropertyGridNodeWrapper = new PropertyGridProjectNodeWrapper(this);
        }

        #endregion

        #region Protected members

        // CanHostNode
        protected override bool CanHostNode(CustomTreeNode node) => node is LanguagePackageFolderNode || node is FolderNode;

        // OnReadAttributes
        protected override void OnReadAttributes(XElement element)
        {
            if (element.Attribute(AttributeName.PublishVersion.ToString())?.Value is string publishVersionValue)
                PublishVersion = XmlConvert.ToInt32(publishVersionValue);

            if (element.Attribute(AttributeName.EncryptionKey.ToString())?.Value is string encryptionKeyValue)
                encryptionKey = encryptionKeyValue;

            if (element.Attribute(AttributeName.OutputFolder.ToString())?.Value is string outputFolderValue)
                outputFolder = outputFolderValue;

            if (element.Attribute(AttributeName.ValidationScope.ToString())?.Value is string validationScopeValue)
                ValidationScope = XmlConvert.ToInt32(validationScopeValue);
        }

        // OnReadChildren
        protected override void OnReadChildren(XElement element)
        {
            // Language package folder
            foreach (XElement childElement in element.Elements(AttributeName.LanguagePackageFolder.ToString()))
            {
                var node = new LanguagePackageFolderNode();
                Nodes.Add(node);
                node.Read(childElement);
            }

            // Folders
            foreach (XElement childElement in element.Elements(AttributeName.Folder.ToString()))
            {
                var node = new FolderNode(string.Empty);
                Nodes.Add(node);
                node.Read(childElement);
            }
        }

        // OnWriteAttributes
        protected override void OnWriteAttributes(XmlWriter output)
        {
            output.WriteAttributeString(AttributeName.PublishVersion.ToString(), PublishVersion.ToString());
            output.WriteAttributeString(AttributeName.EncryptionKey.ToString(), EncryptionKey);
            output.WriteAttributeString(AttributeName.OutputFolder.ToString(), OutputFolder);
            output.WriteAttributeString(AttributeName.ValidationScope.ToString(), XmlConvert.ToString(ValidationScope));
        }

        // OnWriteChildren
        protected override void OnWriteChildren(XmlWriter output)
        {
            // Language package folder
            foreach (var node in Nodes.OfType<LanguagePackageFolderNode>())
            {
                node.Write(output);
            }

            // Text nodes
            foreach (var node in Nodes.OfType<TextNode>())
            {
                node.Write(output);
            }

            // Folder nodes
            foreach (var node in Nodes.OfType<FolderNode>())
            {
                node.Write(output);
            }
        }

        #endregion

        // AllowRemove
        public override bool AllowRemove => false;

        // ContainsCulture
        public bool ContainsCulture(CultureInfo culture)
        {
            var languagePackages = new List<LanguagePackageNode>(GetLanguagePackageNodes());

            foreach (var node in languagePackages)
            {
                if (node.Culture == culture)
                    return true;
            }

            return false;
        }

        // EncryptionKey
        public string EncryptionKey
        {
            get => encryptionKey;
            set
            {
                if (value != encryptionKey)
                {
                    encryptionKey = value;
                    NotifyChange();
                }
            }
        }

        // ErrorCount
        public int ErrorCount { get; private set; }

        // GetLanguagePackageNodes
        public List<LanguagePackageNode> GetLanguagePackageNodes()
        {
            return this.GetChildNodesRecursively().OfType<LanguagePackageNode>().ToList();
        }

        // HasChanges
        public bool HasChanges
        {
            get => hasChanges;
            set
            {
                hasChanges = value;
                onChange();
            }
        }

        // HasLanguagePackages
        [Browsable(false)]
        public bool HasLanguagePackages
        {
            get
            {
                foreach (var node in Nodes)
                {
                    if (node is LanguagePackageNode)
                        return true;
                }

                return false;
            }
        }

        // IsLoading
        public bool IsLoading { get; private set; }

        // Load
        public void Load(string path)
        {
            TreeView.BeginUpdate();
            IsLoading = true;

            Path = path;
            XDocument doc = XDocument.Load(path);
            if (doc.Document?.Root == null)
                throw new InvalidOperationException();

            Read(doc.Document.Root);

            foreach (var node in this.GetChildNodesRecursively().OfType<CustomTreeNode>())
            {
                node.NotifyLoaded();
            }

            NotifyLoaded();
            ValidateAll();
            SortNodes();
            IsLoading = false;
            HasChanges = false;
            TreeView.EndUpdate();
        }

        // NewNamePrefix
        public override string NewNamePrefix => "New Project";

        // NotifyChange
        public void NotifyChange()
        {
            if (!IsLoading)
            {
                HasChanges = true;
                onChange?.Invoke();
            }
        }

        // OutputFolder
        public string OutputFolder
        {
            get => outputFolder;
            set
            {
                if (value != outputFolder)
                {
                    outputFolder = value;
                    NotifyChange();
                }
            }
        }

        // Path
        public string Path { get; private set; } = string.Empty;

        // PublishVersion
        public int PublishVersion { get; set; }

        // Save
        public void Save(string fileName)
        {
            using (var output = XmlWriter.Create(fileName))
            {
                output.WriteStartDocument();
                Write(output);
            }

            Path = fileName;
            HasChanges = false;
        }
        
        // SortNodes
        public void SortNodes()
        {
            var currentNode = TreeView.SelectedNode;
            TreeView.BeginUpdate();
            TreeView.Sort();
            TreeView.SelectedNode = currentNode;
            TreeView.EndUpdate();
            NotifyChange();
        }

        // ValidateAll
        public void ValidateAll()
        {
            ErrorCount = 0;
            var packageNodes = new List<LanguagePackageNode>();
            foreach (var node in this.GetChildNodesRecursively().OfType<CustomTreeNode>())
            {
                if (node is LanguagePackageNode packageNode)
                    packageNodes.Add(packageNode);
                else
                    ErrorCount += node.Validate();
            }

            foreach (var packageNode in packageNodes)
            {
                packageNode.CheckEmptyTexts();
            }
        }

        // ValidationScope
        public int ValidationScope { get; set; }

        /// <summary>
        /// PropertyGridProjectNodeWrapper
        /// </summary>
        private sealed class PropertyGridProjectNodeWrapper : PropertyGridNodeWrapper<ProjectNode>
        {
            // Constructor
            public PropertyGridProjectNodeWrapper(ProjectNode node)
                : base(node)
            {
            }

            // EncryptionKey
            [Category("Settings")]
            [Description("The encryption key to use when publishing files.")]
            [DisplayName("Encryption Key")]
            public string EncryptionKey
            {
                get => Node.EncryptionKey;
                set => Node.EncryptionKey = value;
            }

            // OutputFolder
            [Category("Settings")]
            [Description("The target folder of the published files.")]
            [DisplayName("Output Folder")]
            [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
            public string OutputFolder
            {
                get => Node.OutputFolder;
                set => Node.OutputFolder = value;
            }

            // Path
            [Category("Settings")]
            [Description("The path to the project file.")]
            public string Path => Node.Path;

            // PublishVersion
            [Category("Settings")]
            [Description("The build number of the exported files.")]
            [DisplayName("Publish Version")]
            public int PublishVersion
            {
                get => Node.PublishVersion;
                set => Node.PublishVersion = value;
            }
        }
    }
}
