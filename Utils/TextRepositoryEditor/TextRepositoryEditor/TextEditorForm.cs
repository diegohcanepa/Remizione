using System.Globalization;

namespace TextRepositoryEditor
{
    public partial class TextEditorForm : Form
    {
        private CultureInfo? culture;
        private bool isNormalizingLineFeeds;
        private TextNode? textNode;

        public TextEditorForm()
        {
            InitializeComponent();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (textNode != null && culture != null)
                textNode.SetText(culture, LocalizableTextBox.Text);
        }

        // Prepare
        public void Prepare(CultureInfo culture, TextNode textNode)
        {
            this.culture = culture;
            this.textNode = textNode;

            Text = string.Format("Editing '{0}' - {1}", textNode.GetPath(), culture.DisplayName);
            LocalizableTextBox.Text = textNode.GetText(culture);
            LocalizableTextBox.SelectAll();
            CommentsLabel.Text = textNode.Comments;
        }

        private void TextEditorForm_Shown(object sender, EventArgs e)
        {
            LocalizableTextBox.Focus();
        }

        private void LocalizableTextBox_TextChanged(object sender, EventArgs e)
        {
            if (isNormalizingLineFeeds)
                return;

            isNormalizingLineFeeds = true;
            LocalizableTextBox.Text = LocalizableTextBox.Text.ReplaceLineEndings();
            isNormalizingLineFeeds = false;
        }
    }
}
