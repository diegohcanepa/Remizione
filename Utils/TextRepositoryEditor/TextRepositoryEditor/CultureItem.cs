using System.Globalization;

namespace TextRepositoryEditor
{
    /// <summary>
    /// CultureItem
    /// </summary>
    internal class CultureItem
    {
        // Constructor
        public CultureItem(CultureInfo culture)
        {
            this.Culture = culture;
        }

        // Culture
        public CultureInfo Culture { get; }

        // ToString
        public override string ToString() => Culture.DisplayName;
    }
}
