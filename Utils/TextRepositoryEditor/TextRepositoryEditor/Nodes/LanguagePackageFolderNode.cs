using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace TextRepositoryEditor
{
    /// <summary>
    /// LanguagePackageFolderNode
    /// </summary>
    public sealed class LanguagePackageFolderNode : CustomTreeNode
    {
        // Constructor
        public LanguagePackageFolderNode()
            : base("Language Packages", AttributeName.LanguagePackageFolder)
        {
            Invalidate();
        }

        #region Protected members

        // CanHostNode
        protected override bool CanHostNode(CustomTreeNode node) => node is LanguagePackageFolderNode;

        // OnInvalidate
        protected override void OnInvalidate()
        {
            ImageIndex = IsExpanded ? (int)NodeImage.LanguagePackageFolderOpen : (int)NodeImage.LanguagePackageFolderClosed;
            SelectedImageIndex = ImageIndex;
        }

        // OnReadChildren
        protected override void OnReadChildren(XElement element)
        {
            // Language packages
            foreach (XElement childElement in element.Elements(AttributeName.LanguagePackage.ToString()))
            {
                if (childElement.Attribute(AttributeName.Text.ToString())?.Value is string lcidValue)
                {
                    if (CultureInfo.GetCultureInfo(XmlConvert.ToInt32(lcidValue)) is CultureInfo culture)
                    {
                        var childNode = new LanguagePackageNode(culture);
                        Nodes.Add(childNode);
                        childNode.Read(childElement);
                    }
                }
            }
        }

        // OnWriteChildren
        protected override void OnWriteChildren(XmlWriter output)
        {
            // Language packages
            foreach (var node in Nodes.OfType<LanguagePackageNode>())
            {
                node.Write(output);
            }
        }

        #endregion

        // AllowLabelEdit
        public override bool AllowLabelEdit => false;

        // AllowRemove
        public override bool AllowRemove => false;

        // NewNamePrefix
        public override string NewNamePrefix => "New Language Package Folder";
    }
}
