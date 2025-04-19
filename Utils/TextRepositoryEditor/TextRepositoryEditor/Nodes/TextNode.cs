using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing.Design;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace TextRepositoryEditor
{
    /// <summary>
    /// TextNode
    /// </summary>
    public sealed class TextNode : CustomTreeNode
    {
        private string context = string.Empty;
        private string emitter = string.Empty;
        private ImportResult importResult;
        private bool isLiteral;
        private string literalText = string.Empty;
        private int maximumLength;
        private readonly Dictionary<CultureInfo, string> texts = [];

        // Constructor
        public TextNode(string text)
            : base(text, AttributeName.LocalizableText)
        {
            PropertyGridNodeWrapper = new PropertyGridTextNodeWrapper(this);
            Invalidate();
        }

        #region Private members

        // HasEmptyValues
        private bool HasEmptyValues() => HasEmptyValues(null);

        // HasEmptyValues
        private bool HasEmptyValues(CultureInfo? cultureInfo)
        {
            foreach (var languagePackageNode in ProjectNode.GetLanguagePackageNodes())
            {
                if (!languagePackageNode.AllowValidation)
                    continue;

                if (cultureInfo == null || languagePackageNode.Culture == cultureInfo)
                {
                    if (!texts.ContainsKey(languagePackageNode.Culture))
                        return true;

                    else if (texts.TryGetValue(languagePackageNode.Culture, out string? value) && string.IsNullOrWhiteSpace(value))
                        return true;
                }
            }

            return false;
        }

        #endregion

        #region Protected members

        // CloneNodeCore
        protected override CustomTreeNode CloneNodeCore()
        {
            var result = new TextNode(string.Empty)
            {
                IsLiteral = IsLiteral,
                LiteralText = LiteralText
            };

            foreach (KeyValuePair<CultureInfo, string> keyValue in texts)
            {
                result.SetText(keyValue.Key, keyValue.Value);
            }

            return result;
        }

        // OnInvalidate
        protected override void OnInvalidate()
        {
            if (HasErrors)
                ImageIndex = (int)NodeImage.TextWithErrors;
            else
                ImageIndex = IsLiteral ? (int)NodeImage.TextLiteral : (int)NodeImage.Text;

            SelectedImageIndex = ImageIndex;
            StateImageIndex = string.IsNullOrWhiteSpace(Comments) ? -1 : 0;

            ToolTipText = string.Empty;

            for (int i = 0; i < Errors.Count; i++)
            {
                ToolTipText += Errors[i];
                if (i < Errors.Count - 1)
                    ToolTipText += Environment.NewLine;
            }
        }

        // OnReadAttributes
        protected override void OnReadAttributes(XElement element)
        {
            foreach (var languagePackage in ProjectNode.GetLanguagePackageNodes())
            {
                var attributeName = $"LCID{languagePackage.Culture.LCID}";
                if (element.Attribute(attributeName)?.Value is string textValue)
                    SetText(languagePackage.Culture, textValue);
            }

            // Context
            if (element.Attribute(AttributeName.Context.ToString())?.Value is string contextValue)
                Context = contextValue;

            // Emitter
            if (element.Attribute(AttributeName.Emitter.ToString())?.Value is string emitterValue)
                Emitter = emitterValue;

            // IsLiteral
            if (element.Attribute(AttributeName.IsLiteral.ToString())?.Value is string isLiteralValue)
                IsLiteral = XmlConvert.ToBoolean(isLiteralValue);

            // LiteralText
            if (element.Attribute(AttributeName.LiteralText.ToString())?.Value is string literalTextValue)
                LiteralText = literalTextValue;

            // ImportResult
            if (element.Attribute(AttributeName.ImportResult.ToString())?.Value is string importResultValue)
                ImportResult = Enum.TryParse<ImportResult>(importResultValue, out var result) ? result : ImportResult.None;
        }

        // OnValidate
        protected override string[] OnValidate()
        {
            var result = new List<string>();

            if (IsLiteral)
            {
                if (string.IsNullOrWhiteSpace(LiteralText))
                    result.Add("Literal text is empty.");
            }
            else if (HasEmptyValues())
                result.Add("Contains empty values.");

            if (IsMaximumLengthExceeded)
                result.Add("Maximum length exceeded.");

            return result.ToArray();
        }

        // OnWriteAttributes
        protected override void OnWriteAttributes(XmlWriter output)
        {
            var cultures = from languagePackageNode in ProjectNode.GetLanguagePackageNodes()
                           orderby languagePackageNode.Culture.NativeName.ToUpper()
                           select languagePackageNode.Culture;

            foreach (var culture in cultures)
            {
                var text = GetText(culture);

                if (text != string.Empty)
                    output.WriteAttributeString($"LCID{culture.LCID}", GetText(culture));
            }

            output.WriteAttributeString(AttributeName.Context.ToString(), Context);
            output.WriteAttributeString(AttributeName.Emitter.ToString(), Emitter);
            output.WriteAttributeString(AttributeName.IsLiteral.ToString(), XmlConvert.ToString(IsLiteral));
            output.WriteAttributeString(AttributeName.LiteralText.ToString(), LiteralText);
            output.WriteAttributeString(AttributeName.ImportResult.ToString(), ImportResult.ToString());
        }

        #endregion

        // AllowDrag
        public override bool AllowDrag => true;

        // CanClone
        public override bool CanClone => true;

        // ClearTexts
        public void ClearTexts() => ClearTexts(null);

        // ClearTexts
        public void ClearTexts(CultureInfo? preserveCulture)
        {
            foreach (var key in texts.Keys)
            {
                if (preserveCulture != null && key == preserveCulture)
                    continue;

                texts[key] = string.Empty;
            }

            NotifyChangeCore();
        }

        // Contains
        public bool Contains(CultureInfo? culture, string value, bool ignoreCase)
        {
            var comparisonType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            if (culture == null)
            {
                foreach (var languagePackageNode in ProjectNode.GetLanguagePackageNodes())
                {
                    if (GetText(languagePackageNode.Culture) is string text)
                    {
                        if (text.Contains(value, comparisonType))
                            return true;
                    }
                }

                return false;
            }
            else
            {
                string? text = GetText(culture);
                return text is not null && text.Contains(value, comparisonType);
            }
        }

        // Context
        public string Context
        {
            get => context;
            set
            {
                if (value != context)
                {
                    context = value;
                    Invalidate();
                    ProjectNode.NotifyChange();
                }
            }
        }

        // Emitter
        public string Emitter
        {
            get => emitter;
            set
            {
                if (value != emitter)
                {
                    emitter = value;
                    NotifyChangeCore();
                }
            }
        }

        // HasContext
        public bool HasContext => !string.IsNullOrWhiteSpace(Context);

        // ImportResult
        public ImportResult ImportResult
        {
            get => importResult;
            set
            {
                if (importResult != value)
                {
                    importResult = value;

                    if (importResult == ImportResult.New)
                        ForeColor = Color.Green;

                    else if (importResult == ImportResult.Updated)
                        ForeColor = Color.Blue;

                    else if (importResult == ImportResult.NotFound)
                        ForeColor = Color.Red;

                    else
                        ForeColor = Color.Black;
                }
            }
        }

        // NewNamePrefix
        public override string NewNamePrefix => "New Text";

        // GetText
        public string? GetText(CultureInfo culture) => texts.TryGetValue(culture, out string? value) ? value : null;

        // HasValidationErrors
        public override bool HasValidationErrors
        {
            get
            {
                if (IsMaximumLengthExceeded)
                    return true;

                return base.HasValidationErrors;
            }
        }

        // IsLiteral
        public bool IsLiteral
        {
            get => isLiteral;
            set
            {
                if (value != isLiteral)
                {
                    isLiteral = value;
                    NotifyChangeCore();
                }
            }
        }

        // IsMaximumLengthExceeded
        public bool IsMaximumLengthExceeded
        {
            get
            {
                if (MaximumLength <= 0)
                    return false;

                if (IsLiteral && LiteralText.Length > MaximumLength)
                    return true;

                foreach (var languagePackageNode in ProjectNode.GetLanguagePackageNodes())
                {
                    if (texts.TryGetValue(languagePackageNode.Culture, out string? value))
                    {
                        if (value.Length > MaximumLength)
                            return true;
                    }
                }

                return false;
            }
        }

        // LiteralText
        public string LiteralText
        {
            get => literalText;
            set
            {
                if (value != literalText)
                {
                    literalText = value;
                    NotifyChangeCore();
                }
            }
        }

        // MaximumLength
        public int MaximumLength
        {
            get => maximumLength;
            set
            {
                if (value != maximumLength)
                {
                    maximumLength = value;
                    NotifyChangeCore();
                }
            }
        }

        // NormalizeText
        public static string NormalizeText(string value)
        {
            var result = Regex.Replace(value, "\r\n|\n|\r", Environment.NewLine);
            result = result.ReplaceLineEndings();
            return result;
        }

        // SetText
        public bool SetText(CultureInfo culture, string value)
        {
            if (GetText(culture) != value)
            {
                // Normalize carriage returns and line feeds
                texts[culture] = NormalizeText(value);
                NotifyChangeCore();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// PropertyGridTextNodeWrapper
        /// </summary>
        private sealed class PropertyGridTextNodeWrapper(TextNode node) : PropertyGridNodeWrapper<TextNode>(node)
        {

            // Comments
            [Category("Misc")]
            [DefaultValue(false)]
            [Description("User-defined comment.")]
            [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
            public string Comments
            {
                get => Node.Comments;
                set => Node.Comments = value;
            }

            // Context
            [Category("Misc")]
            [DefaultValue(false)]
            [Description("User-defined text context annotation.")]
            [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
            public string Context
            {
                get => Node.Context;
                set => Node.Context = value;
            }

            // Emitter
            [Category("Misc")]
            [DefaultValue(false)]
            [Description("An optional value specifying who or what emits the text.")]
            public string Emitter
            {
                get => Node.Emitter;
                set => Node.Emitter = value;
            }

            // ImportResult
            [Category("Import")]
            [DefaultValue(false)]
            [Description("Result of the last import operation.")]
            [DisplayName("Last Import Result")]
            public ImportResult ImportResult => Node.ImportResult;

            // IsLiteral
            [Category("Attributes")]
            [DefaultValue(false)]
            [Description("Indicates whether value requires to be translated.")]
            [DisplayName("Is Literal")]
            public bool IsLiteral
            {
                get => Node.IsLiteral;
                set => Node.IsLiteral = value;
            }

            // LiteralText
            [Category("Attributes")]
            [DefaultValue(false)]
            [Description("The text to be used in all cultures.")]
            [DisplayName("Literal Text")]
            public string LiteralText
            {
                get => Node.LiteralText;
                set => Node.LiteralText = value;
            }

            // MaximumLength
            [Category("Attributes")]
            [DefaultValue(0)]
            [Description("Maximum length constraint.")]
            [DisplayName("Maximum Length")]
            public int MaximumLength
            {
                get => Node.MaximumLength;
                set => Node.MaximumLength = value;
            }
        }
    }
}
