using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Text;

namespace Content
{
    /// <summary>
    /// LocalizedFontTextureImporter
    /// Imports a text file of chars along with the bitmap they reference
    /// </summary>
    internal sealed class LocalizedFontTextureImporter : TextureImporter
    {
        // Import
        public override TextureContent Import(string filename, ContentImporterContext context)
        {
            // Use the base monogame texture importer
            TextureContent baseTextureContent = base.Import(filename, context);

            // Get the text file that should have been generated with the same name as the bitmap
            string textFileName = Path.ChangeExtension(filename, ".txt");

            // Read all the characters
            string allChars = File.ReadAllText(textFileName, Encoding.UTF8);
            List<char> charList =
            [
                .. allChars,
            ];
            
            LocalizedFontTextureContent localizedFontTextureContent = new(baseTextureContent, charList);

            return localizedFontTextureContent;
        }
    }
}
