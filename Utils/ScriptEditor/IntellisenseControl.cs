using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Engendro
{
    public partial class IntellisenseControl : UserControl
    {
        private TextEditor? editor;
        private readonly List<ListViewItem> items = [];
        private readonly List<TreeNode> nodes = [];
        private string text = string.Empty;

        public IntellisenseControl()
        {
            InitializeComponent();
        }

        // Apply
        private void Apply()
        {
            var textToInsert = ItemsListView.SelectedItems[0].Text;
            editor?.Document.Replace(editor.CaretOffset - text.Length, text.Length, textToInsert);
            Visible = false;
        }

        // IsCaretAtCounterCommand
        public static bool IsCaretAtCounterCommand(TextEditor editor)
        {
            var currentLine = editor.Document.GetLineByNumber(editor.TextArea.Caret.Line);
            var lineText = editor.Document.GetText(currentLine.Offset, currentLine.Length).Trim();

            return lineText.StartsWith("if-counter", StringComparison.OrdinalIgnoreCase) ||
                   lineText.StartsWith("set-counter", StringComparison.OrdinalIgnoreCase) ||
                   lineText.StartsWith("decrement-counter", StringComparison.OrdinalIgnoreCase) ||
                   lineText.StartsWith("increment-counter", StringComparison.OrdinalIgnoreCase);
        }

        // IsCaretAtFlagCommand
        public static bool IsCaretAtFlagCommand(TextEditor editor)
        {
            var currentLine = editor.Document.GetLineByNumber(editor.TextArea.Caret.Line);
            var lineText = editor.Document.GetText(currentLine.Offset, currentLine.Length).Trim();

            return lineText.StartsWith("if-flag", StringComparison.OrdinalIgnoreCase) ||
                   lineText.StartsWith("set-flag", StringComparison.OrdinalIgnoreCase) ||
                   lineText.StartsWith("toggle-flag", StringComparison.OrdinalIgnoreCase);
        }

        // IsCaretAtRoutineCommand
        public static bool IsCaretAtRoutineCommand(TextEditor editor)
        {
            var currentLine = editor.Document.GetLineByNumber(editor.TextArea.Caret.Line);
            var lineText = editor.Document.GetText(currentLine.Offset, currentLine.Length).Trim();

            return lineText.StartsWith("start-routine", StringComparison.OrdinalIgnoreCase) ||
                   lineText.StartsWith("stop-routine", StringComparison.OrdinalIgnoreCase) ||
                   lineText.StartsWith("await-routine", StringComparison.OrdinalIgnoreCase) ||
                   lineText.StartsWith("resume-routine", StringComparison.OrdinalIgnoreCase) ||
                   lineText.StartsWith("pause-routine", StringComparison.OrdinalIgnoreCase);
        }

        // IsControlCompletelyVisible
        private bool IsControlCompletelyVisible()
        {
            if (Parent == null)
            {
                return false;
            }

            // Get the rectangle of the control relative to its parent
            Rectangle controlRect = new(Location, Size);

            // Get the rectangle of the parent
            Rectangle parentRect = new(Point.Empty, Parent.Size);

            // Check if the control's rectangle is completely within the parent's rectangle
            return parentRect.Contains(controlRect);
        }

        // Populate
        private void Populate()
        {
            ItemsListView.BeginUpdate();
            ItemsListView.Items.Clear();

            for (var i = 0; i < nodes.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(text) || nodes[i].Text.Contains(text, StringComparison.OrdinalIgnoreCase))
                {
                    ItemsListView.Items.Add(items[i]);
                    items[i].Selected = false;
                }
            }

            if (ItemsListView.Items.Count > 0)
            {
                ItemsListView.Items[0].Selected = true;
            }

            ItemsListView.EndUpdate();
        }

        // Show
        public void Show(TextEditor editor, ProjectNode projectNode)
        {
            this.text = string.Empty;
            this.editor = editor;
            items.Clear();
            ItemsListView.SmallImageList = projectNode.TreeView?.ImageList;
            ItemsListView.Columns[0].Width = 400;

            var counters = IsCaretAtCounterCommand(editor);
            var flags = IsCaretAtFlagCommand(editor);
            var routines = IsCaretAtRoutineCommand(editor);

            // Collect nodes
            nodes.Clear();
            foreach (var scriptNode in projectNode.GetChildNodesRecursively().OfType<ScriptNode>())
            {
                if (flags || counters)
                {
                    if (scriptNode.ScriptType == ScriptType.Declaration)
                    {
                        var list = scriptNode.GetDeclaredVariables(counters ? VariableKind.Counter : VariableKind.Flag);
                        foreach (var variable in list)
                        {
                            var imageIndex = variable.Persistent ? 18 : 19;
                            nodes.Add(new TreeNode(variable.Name, imageIndex, imageIndex));
                        }
                    }
                }
                else if (routines)
                {
                    if (scriptNode.ScriptType == ScriptType.Routine)
                    {
                        nodes.Add(scriptNode);
                    }
                }
                else if (scriptNode.IsStaticEntity || scriptNode.ScriptType == ScriptType.Routine)
                {
                    nodes.Add(scriptNode);
                }
            }

            nodes.Sort(new TreeNodeTextComparer());

            // Cache nodes
            foreach (var node in nodes)
            {
                items.Add(ItemsListView.Items.Add(node.Text, node.ImageIndex));
            }

            Populate();

            // Get caret position
            var caretPosition = editor.TextArea.Caret.Position;

            // Get caret area
            var caretRectangle = editor.TextArea.TextView.GetVisualPosition(caretPosition, VisualYPosition.LineBottom);

            Location = new Point((int)caretRectangle.X + 32, (int)caretRectangle.Y - 7 - (int)editor.TextArea.TextView.ScrollOffset.Y);

            if (!IsControlCompletelyVisible())
            {
                Location = new Point(Location.X, Location.Y - ClientRectangle.Height);
            }

            Visible = true;
            ItemsListView.Focus();
        }

        private void ItemListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Apply();
            }
        }

        private void ItemListView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == '-' || e.KeyChar == '*')
            {
                text += e.KeyChar.ToString();
                editor?.Document.Insert(editor.CaretOffset, e.KeyChar.ToString());
                Populate();
                e.Handled = true;
                return;
            }
            else if (e.KeyChar == '\b' && text.Length > 0)
            {
                text = text.Substring(0, text.Length - 1);
                editor?.Document.Remove(editor.CaretOffset - 1, 1);
                if (text.Length > 0)
                {
                    Populate();
                    e.Handled = true;
                    return;
                }
            }

            Visible = false;
        }

        private void ItemListView_Leave(object sender, System.EventArgs e)
        {
            Visible = false;
        }

        private void ItemListView_DoubleClick(object sender, System.EventArgs e)
        {
            Apply();
        }
    }
}
