using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace TextRepositoryEditor
{
    /// <summary>
    /// LanguagePackageNode
    /// </summary>
    public sealed class LanguagePackageNode : CustomTreeNode
    {
        // Constructor
        public LanguagePackageNode(CultureInfo culture)
            : base(culture.DisplayName, AttributeName.LanguagePackage)
        {
            this.Culture = culture;
            this.Text = culture.DisplayName;

            ImageIndex = (int)NodeImage.LanguagePackage;
            SelectedImageIndex = ImageIndex;

            PropertyGridNodeWrapper = new PropertyGridLanguagePackageNodeWrapper(this);
        }

        #region Protected members

        // OnInvalidate
        protected override void OnInvalidate()
        {
            base.OnInvalidate();

            ImageIndex = (int)NodeImage.LanguagePackage;
            SelectedImageIndex = ImageIndex;
            StateImageIndex = HasEmptyTexts ? 1 : -1;

            if (HasEmptyTexts)
                ToolTipText = "[Warning]" + Environment.NewLine + "Language package contains empty texts." + Environment.NewLine + Environment.NewLine + ToolTipText;
        }

        // OnReadAttributes
        protected override void OnReadAttributes(XElement element)
        {
            // Last Imported
            if (element.Attribute(AttributeName.LastImported.ToString())?.Value is string lastImportedValue && long.TryParse(lastImportedValue, out long lastImportedTicks))
                LastImported = new DateTime(lastImportedTicks);

            // Last Imported File
            if (element.Attribute(AttributeName.LastImportedFile.ToString())?.Value is string lastImportedFileValue)
                LastImportedFile = lastImportedFileValue;

            // SkipValidation
            if (element.Attribute(AttributeName.AllowValidation.ToString())?.Value is string allowValidationValue)
                AllowValidation = XmlConvert.ToBoolean(allowValidationValue);
        }

        // OnRemove
        protected override void OnRemove()
        {
            foreach (var textNode in ProjectNode.GetChildNodesRecursively().OfType<TextNode>())
            {
                textNode.SetText(Culture, string.Empty);
            }
        }

        // OnTransformTextValue
        protected override string OnTransformTextValue(string text)
        {
            if (ProjectNode.IsLoading)
                return CultureInfo.GetCultureInfo(int.Parse(text)).DisplayName;
            else
                return Culture.LCID.ToString();
        }

        // OnWriteAttributes
        protected override void OnWriteAttributes(XmlWriter output)
        {
            output.WriteAttributeString(AttributeName.AllowValidation.ToString(), XmlConvert.ToString(AllowValidation));
            output.WriteAttributeString(AttributeName.LastImported.ToString(), XmlConvert.ToString(LastImported.Ticks));
            output.WriteAttributeString(AttributeName.LastImportedFile.ToString(), LastImportedFile);
        }

        #endregion

        // AllowLabelEdit
        public override bool AllowLabelEdit => false;

        // AllowValidation
        public bool AllowValidation { get; set; } = true;

        // CheckEmptyTexts
        public void CheckEmptyTexts()
        {
            HasEmptyTexts = false;

            foreach (var node in ProjectNode.GetChildNodesRecursively().OfType<TextNode>())
            {
                if (node.HasErrors && string.IsNullOrWhiteSpace(node.GetText(Culture)))
                {
                    HasEmptyTexts = true;
                    break;
                }
            }

            Invalidate();
        }

        // Culture
        public CultureInfo Culture { get; }

        // HasEmptyTexts
        public bool HasEmptyTexts { get; private set; }

        // LastImported
        public DateTime LastImported { get; set; }

        // LastImportedFile
        public string LastImportedFile { get; set; } = string.Empty;

        // NewNamePrefix
        public override string NewNamePrefix => "New Language Package";

        // ToString
        public override string ToString() => Culture.DisplayName;

        /// <summary>
        /// PropertyGridLanguagePackageNodeWrapper
        /// </summary>
        private sealed class PropertyGridLanguagePackageNodeWrapper(LanguagePackageNode node) : PropertyGridNodeWrapper<LanguagePackageNode>(node)
        {
            // CultureName
            [Category("Culture")]
            [Description("The culture name in the format 'languagecode2-country/regioncode2', where languagecode2 is a lowercase two-letter code derived from ISO 639-1 and country/regioncode2 is an uppercase two-letter code derived from ISO 3166.")]
            [DisplayName("Culture Name")]
            public string CultureName => Node.Culture.Name;

            // LastImported
            [Category("Import")]
            [Description("Last import date/time.")]
            [DisplayName("Last Imported")]
            public DateTime LastImported => Node.LastImported;

            // LastImportedFile
            [Category("Import")]
            [Description("Name of last imported file.")]
            [DisplayName("Last Imported File")]
            public string LastImportedFile => Node.LastImportedFile;

            // LCID
            [Category("Culture")]
            [Description("The culture identifier.")]
            public int LCID => Node.Culture.LCID;
        }
    }
}
