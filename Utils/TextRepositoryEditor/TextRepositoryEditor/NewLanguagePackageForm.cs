using System.ComponentModel;
using System.Globalization;
using Windows.Foundation.Metadata;

namespace TextRepositoryEditor
{
    public partial class NewLanguagePackageForm : Form
    {
        private ProjectNode? projectNode;

        public NewLanguagePackageForm()
        {
            InitializeComponent();
        }

        // Populate
        private void Populate()
        {
            CultureListBox.Items.Clear();

            if (ProjectNode is null)
                throw new InvalidOperationException();

            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (culture.LCID == 4096)
                    continue;

                if (!ProjectNode.ContainsCulture(culture))
                    CultureListBox.Items.Add(new CultureItem(culture));
            }
        }

        // UpdateControls
        private void UpdateControls() => acceptButton.Enabled = CultureListBox.SelectedIndex >= 0;

        private void CultureListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void NewLanguagePackageForm_Load(object sender, EventArgs e)
        {
            UpdateControls();
        }

        // ProjectNode
        [DefaultValue(null)]
        public ProjectNode? ProjectNode
        {
            get => projectNode;
            set
            {
                projectNode = value;
                Populate();
                UpdateControls();
            }
        }

        // SelectedCulture
        public CultureInfo? SelectedCulture => CultureListBox.SelectedItem is CultureItem item ? item.Culture : null;
    }
}
