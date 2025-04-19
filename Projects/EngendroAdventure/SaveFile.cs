using System.Globalization;
using System.IO;

namespace EngendroAdventure
{
    /// <summary>
    /// SaveFile
    /// </summary>
    public static class SaveFile
    {
        // EncodeName
        public static string EncodeName(int index)
        {
            var result = DefaultPrefix + index.ToString(CultureInfo.InvariantCulture);
            result = Path.ChangeExtension(result, DefaultExtension);
            return result;
        }

        // DefaultExtension
        public static string DefaultExtension { get; set; } = ".sav";

        // DefaultPrefix
        public static string DefaultPrefix { get; set; } = "SaveGame";
    }
}
