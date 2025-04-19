using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Engendro
{
    /// <summary>
    /// ScriptNode
    /// </summary>
    public sealed class ScriptNode : ObjectModelNode, IDisposable
    {
        #region Private fields

        private int caretOffset = -1;
        private static readonly string[] definitions = [ScriptType.Room.ToString(), ScriptType.Thing.ToString(), ScriptType.Routine.ToString(), ScriptType.Outcome.ToString()];
        private bool isInvalidating;
        private bool nameChanged;

        #endregion

        #region Constructor

        // Constructor
        public ScriptNode(TreeView? documentExplorer)
            : base(string.Empty, XmlAttributeName.Script, false, documentExplorer)
        {
            Invalidate();
        }

        #endregion

        #region Private members

        // DecomposeLine
        private static string[] DecomposeLine(string line)
        {
            var parts = Regex.Matches(line, @"[^\s""]+|""[^""]*""");

            var result = new string[parts.Count];

            // Remove blanks
            for (var i = 0; i < parts.Count; i++)
            {
                result[i] = parts[i].Value;
            }

            return result;
        }

        // EnsureTextEditor
        private void EnsureTextEditor()
        {
            if (TextEditor != null)
            {
                return;
            }

            // Code editor
            this.TextEditor = new TextEditor
            {
                HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
                ShowLineNumbers = true,
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 16,
                Tag = this,
                Text = this.SourceCode
            };

            TextEditor.Options.HighlightCurrentLine = true;

            // Highlighting
            var fileName = Path.Combine(Application.StartupPath, "HighLighting.xshd");
            var stm = File.OpenRead(fileName);
            this.TextEditor.SyntaxHighlighting = HighlightingLoader.Load(XmlReader.Create(stm), HighlightingManager.Instance);
            this.TextEditor.TextChanged += TextEditorChanged;

            if (caretOffset != -1)
            {
                this.TextEditor.CaretOffset = caretOffset;
                this.TextEditor.TextArea.Caret.BringCaretToView();
            }

            this.SearchPanel = SearchPanel.Install(TextEditor);
        }

        // Invalidate
        private void Invalidate()
        {
            if (isInvalidating)
            {
                return;
            }

            isInvalidating = true;

            var previousName = Text;

            var newText = "<Unknown>";
            ScriptType = ScriptType.Unknown;

            try
            {
                if (SourceCode.Length == 0)
                {
                    return;
                }

                var index = SourceCode.IndexOf(Environment.NewLine, StringComparison.InvariantCultureIgnoreCase);
                var line = index == -1 ? SourceCode : SourceCode.Substring(0, index);

                DefinitionHeader = line;

                var words = DecomposeLine(line);

                if (words.Length > 0)
                {
                    if (Enum.TryParse(words[0], out ScriptType result))
                    {
                        ScriptType = result;

                        if (!line.Contains(" Persistent"))
                        {
                            if (ScriptType == ScriptType.Thing)
                            {
                                ScriptType = ScriptType.TransientThing;
                            }
                            else if (ScriptType == ScriptType.Room)
                            {
                                ScriptType = ScriptType.TransientRoom;
                            }
                        }
                    }
                    else
                    {
                        return;
                    }

                    if (words.Length > 1 && result != ScriptType.Unknown)
                    {
                        newText = words[1];
                    }
                    else if (words.Length == 1 && result == ScriptType.EnterRoom)
                    {
                        newText = words[0];
                    }
                    else if (words.Length == 1 && result == ScriptType.NewSession)
                    {
                        newText = words[0];
                    }
                }

                HasTask = SourceCode.Contains("TODO:");
                StateImageIndex = HasTask ? 0 : -1;
            }
            finally
            {
                ImageIndex = (int)ScriptType + 3;
                SelectedImageIndex = ImageIndex;
                InvalidateDocumentNode();
                isInvalidating = false;
                nameChanged = previousName != newText;

                if (nameChanged)
                {
                    Text = newText;
                    InvalidateDocumentNode();
                }
            }
        }

        // TextEditorChanged
        private void TextEditorChanged(object? sender, EventArgs e)
        {
            SourceCode = TextEditor?.Text ?? string.Empty;
            Invalidate();
            DocumentHasChanges = true;
            if (nameChanged)
            {
                DocumentExplorer?.SortTree();
            }

            if (TreeView != null && TreeView.TopNode is ProjectNode projectNode && !projectNode.IsLoading)
            {
                projectNode.NotifyScriptChange();
            }

            NotifyProjectChange();
        }

        #endregion

        #region Protected members

        // OnCloseDocument
        protected override void OnCloseDocument()
        {
            if (DocumentNode != null)
            {
                DocumentNode.IsPinned = false;
                DocumentExplorer?.Nodes.Remove(DocumentNode);
            }
        }

        // OnInvalidateDocumentNode
        protected override void OnInvalidateDocumentNode()
        {
            if (DocumentNode != null)
            {
                DocumentNode.ImageIndex = ImageIndex;
                DocumentNode.SelectedImageIndex = ImageIndex;
                DocumentNode.Text = Text;
            }
        }

        // OnOpenDocument
        protected override void OnOpenDocument(bool selectDocument)
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            EnsureTextEditor();

            InvalidateDocumentNode();
        }

        // OnRead
        protected override void OnRead(ProjectNode projectNode, XElement element)
        {
            if (element.Attribute(XmlAttributeName.CaretOffset.ToString())?.Value is string caretOffsetValue)
            {
                this.caretOffset = XmlConvert.ToInt32(caretOffsetValue);
            }

            if (element.Attribute(XmlAttributeName.SourceCode.ToString())?.Value is string sourceCode)
            {
                this.SourceCode = sourceCode;
            }

            Invalidate();
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            if (TextEditor != null)
            {
                output.WriteAttributeString(XmlAttributeName.CaretOffset.ToString(), XmlConvert.ToString(TextEditor.CaretOffset));
            }

            output.WriteAttributeString(XmlAttributeName.SourceCode.ToString(), SourceCode);
        }

        #endregion

        // DefinitionHeader
        public string DefinitionHeader { get; private set; } = string.Empty;

        // Dispose
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            CloseDocument();
            TextEditor?.Clear();

            IsDisposed = true;
        }

        // DocumentHasChanges
        public bool DocumentHasChanges { get; private set; }

        // GetCurrentWord
        public string GetCurrentWord()
        {
            if (TextEditor == null || IsDisposed)
            {
                return string.Empty;
            }

            var caret = TextEditor.TextArea.Caret;
            var document = TextEditor.Document;

            // Obtener la línea actual
            var currentLine = document.GetLineByNumber(caret.Line);
            var lineText = document.GetText(currentLine.Offset, currentLine.Length);

            // Obtener la posición del caret dentro de la línea
            var offsetInLine = caret.Offset - currentLine.Offset;

            // Usar una expresión regular para encontrar la palabra en la posición del caret
            Regex wordRegex = new(@"\b[\w\*\-]+\b");

            var matches = wordRegex.Matches(lineText);

            foreach (Match match in matches)
            {
                if (match.Index <= offsetInLine && match.Index + match.Length >= offsetInLine)
                {
                    return match.Value;
                }
            }

            return string.Empty;
        }

        // GetDeclaredVariables
        public List<VarInfo> GetDeclaredVariables(VariableKind variableKind)
        {
            List<VarInfo> result = [];
            var keyword = variableKind.ToString().ToLowerInvariant();

            // Define the regular expression patterns
            var patternTransient = $@"{keyword}\s+(\w+)\s*=\s*.*#transient";
            var patternNonTransient = $@"{keyword}\s+(\w+)\s*=\s*(?!.*#transient)";

            // Create Regex objects
            Regex regexTransient = new(patternTransient);
            Regex regexNonTransient = new(patternNonTransient);

            // Find matches for transient variables
            var matchesTransient = regexTransient.Matches(SourceCode);
            foreach (Match match in matchesTransient)
            {
                result.Add(new VarInfo(match.Groups[1].Value, false));
            }

            // Find matches for persistent variables
            var matchesNonTransient = regexNonTransient.Matches(SourceCode);
            foreach (Match match in matchesNonTransient)
            {
                result.Add(new VarInfo(match.Groups[1].Value, true));
            }

            return result;
        }

        // HasTask
        public bool HasTask { get; private set; }

        // IsDefinition
        public bool IsDefinition
        {
            get
            {
                for (var i = 0; i < definitions.Length; i++)
                {
                    if (DefinitionHeader.StartsWith(definitions[i], StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        // IsDisposed
        public bool IsDisposed { get; private set; }

        // IsEmpty
        public bool IsEmpty => string.IsNullOrEmpty(SourceCode);

        // IsPersistentEntity
        public bool IsPersistentEntity { get; private set; }

        // IsStaticEntity
        public bool IsStaticEntity => ScriptType == ScriptType.Room ||
                       ScriptType == ScriptType.TransientRoom ||
                       ScriptType == ScriptType.Thing ||
                       ScriptType == ScriptType.TransientThing;

        // ScriptType
        public ScriptType ScriptType { get; private set; }

        // SearchPanel
        public SearchPanel? SearchPanel { get; private set; }

        // SourceCode
        public string SourceCode { get; private set; } = string.Empty;

        // TextEditor
        public TextEditor? TextEditor { get; private set; }

        /// <summary>
        /// VarInfo
        /// </summary>
        public sealed class VarInfo
        {
            // Constructor
            internal VarInfo(string name, bool persistent)
            {
                this.Name = name;
                this.Persistent = persistent;
            }

            // Name
            public string Name { get; }

            // Persistent
            public bool Persistent { get; }
        }
    }
}
