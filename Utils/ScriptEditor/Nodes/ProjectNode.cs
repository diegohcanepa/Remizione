using Engendro.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Engendro
{
    /// <summary>
    /// ProjectNode
    /// </summary>
    public sealed class ProjectNode : ExpandableNode
    {
        #region Private fields

        private readonly List<DocumentNode> documents = [];
        private string encryptionKey = string.Empty;
        private readonly Action onChange;
        private readonly List<string> openDocuments = [];
        private string outputFileName = string.Empty;
        private bool useEncryption;

        #endregion

        #region Constructor

        // Constructor
        public ProjectNode(TreeView documentExplorer, Action onChange)
            : base("Project", XmlAttributeName.Project, true, documentExplorer)
        {
            this.onChange = onChange;

            ImageIndex = (int)NodeImage.Project;
            SelectedImageIndex = ImageIndex;

            Documents = new ReadOnlyCollection<DocumentNode>(documents);
        }

        #endregion

        #region Protected members

        // OnReadAttributes
        protected override void OnReadAttributes(XElement element)
        {
            if (element.Attribute(XmlAttributeName.Build.ToString())?.Value is string buildIDValue)
            {
                BuildID = XmlConvert.ToInt32(buildIDValue);
            }

            if (element.Attribute(XmlAttributeName.EncryptionKey.ToString())?.Value is string encryptionKeyValue)
            {
                encryptionKey = encryptionKeyValue;
            }

            if (element.Attribute(XmlAttributeName.OutputFileName.ToString())?.Value is string outputFileNameValue)
            {
                OutputFileName = outputFileNameValue;
            }

            openDocuments.Clear();
            if (element.Attribute(XmlAttributeName.Documents.ToString())?.Value is string activeDocuments)
            {
                var docs = activeDocuments.Split(';');
                foreach (var keyName in docs)
                {
                    if (!openDocuments.Contains(keyName))
                    {
                        openDocuments.Add(keyName);
                    }
                }
            }
        }

        // OnWriteAttributes
        protected override void OnWriteAttributes(XmlWriter output)
        {
            output.WriteAttributeString(XmlAttributeName.Build.ToString(), BuildID.ToString(CultureInfo.InvariantCulture));
            output.WriteAttributeString(XmlAttributeName.EncryptionKey.ToString(), EncryptionKey);
            output.WriteAttributeString(XmlAttributeName.OutputFileName.ToString(), OutputFileName);

            List<string> docs = [];
            foreach (var doc in Documents)
            {
                if (!doc.Text.Contains(';'))
                {
                    if (!docs.Contains(doc.KeyName))
                    {
                        docs.Add(doc.KeyName);
                    }
                }
            }

            output.WriteAttributeString(XmlAttributeName.Documents.ToString(), string.Join(';', docs.ToArray()));
        }

        #endregion

        // AddDocument
        public void AddDocument(DocumentNode node)
        {
            if (IsLoading)
            {
                return;
            }

            documents.Remove(node);
            documents.Insert(0, node);
        }

        // BuildID
        public int BuildID { get; set; }

        // DocumentNodes
        public ReadOnlyCollection<DocumentNode> Documents { get; }

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

        // Export
        public bool Export()
        {
            if (string.IsNullOrWhiteSpace(OutputFileName))
            {
                return false;
            }

            // Create directory
            if (!Directory.Exists(Path.GetDirectoryName(OutputFileName)))
            {
                Dialogs.ShowMessage($"Cannot create script library file.\n\n{OutputFileName}", MessageBoxIcon.Warning);
                return false;
            }

            StringBuilder sb = new();

            sb.AppendLine(BuildID.ToString(CultureInfo.InvariantCulture));

            foreach (var scriptNode in this.GetChildNodesRecursively().OfType<ScriptNode>())
            {
                sb.AppendLine(scriptNode.SourceCode);
                sb.AppendLine();
            }

            var text = XOREncryptor.AsString(sb.ToString(), EncryptionKey);

            File.WriteAllText(OutputFileName, text);

            return true;
        }

        // FileName
        public string FileName { get; private set; } = string.Empty;

        // HasChanges
        public bool HasChanges { get; private set; }

        // HasScriptChanges
        public bool HasScriptChanges { get; private set; }

        // IsLoading
        public bool IsLoading { get; private set; }

        // NotifyChange
        public void NotifyChange()
        {
            HasChanges = true;
            onChange?.Invoke();
        }

        // NotifyScriptChange
        public void NotifyScriptChange()
        {
            if (!HasScriptChanges)
            {
                HasScriptChanges = true;
                BuildID++;
            }
        }

        // Load
        public void Load(string fileName)
        {
            IsLoading = true;

            this.FileName = fileName;
            XDocument doc = XDocument.Load(fileName);

            if (doc.Document?.Root == null)
            {
                throw new InvalidOperationException("No root document.");
            }

            Read(this, doc.Document.Root);

            // Notify children
            foreach (var node in this.GetChildNodesRecursively().OfType<ObjectModelNode>())
            {
                node.NotifyLoadedCompleted();
            }

            NotifyLoadedCompleted();

            if (DocumentExplorer != null)
            {
                foreach (var text in openDocuments)
                {
                    foreach (var documentNode in DocumentExplorer.Nodes.OfType<DocumentNode>())
                    {
                        if (documentNode.KeyName == text)
                        {
                            documents.Add(documentNode);
                            break;
                        }
                    }
                }
            }

            HasChanges = false;

            IsLoading = false;
        }

        // OutputFileName
        public string OutputFileName
        {
            get => outputFileName;
            set
            {
                if (value != outputFileName)
                {
                    this.outputFileName = value;
                    NotifyChange();
                }
            }
        }

        // RemoveDocument
        public void RemoveDocument(DocumentNode node)
        {
            documents.Remove(node);
        }

        // Save
        public void Save(string fileName)
        {
            using (XmlWriter output = XmlWriter.Create(fileName))
            {
                output.WriteStartDocument();
                Write(output);
            }

            this.FileName = fileName;
            HasChanges = false;
            HasScriptChanges = false;
        }

        // TaskCount
        public int TaskCount { get; set; }

        // UseEncryption
        public bool UseEncryption
        {
            get => useEncryption;
            set
            {
                if (value != useEncryption)
                {
                    useEncryption = value;
                    NotifyChange();
                }
            }
        }
    }
}
