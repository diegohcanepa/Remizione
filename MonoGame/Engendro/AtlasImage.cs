using Microsoft.Xna.Framework;

namespace Engendro
{
    /// <summary>
    /// AtlasImage
    /// </summary>
    public sealed class AtlasImage : INamedObject
    {
        // Constructor
        internal AtlasImage(string name, Atlas atlas, Rectangle textureArea)
        {
            this.Name = name;
            this.Atlas = atlas;
            this.TextureArea = textureArea;
        }

        // Atlas
        public Atlas Atlas { get; }

        // Name
        public string Name { get; }

        // TextureArea
        public Rectangle TextureArea { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
