using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Placeholder
    /// </summary>
    public sealed class Placeholder
    {
        // Constructor
        public Placeholder(string name, PlaceholderType placeholderType, PlaceholderSize placeholderSize, bool flipImage, params Vector2[] vertices)
        {
            this.Name = name;
            this.PlaceholderType = placeholderType;
            this.Size = placeholderSize;
            this.FlipImage = flipImage;
            this.Polygon = new ReadOnlyPolygon(Geometry.SimplifyPolygon(vertices));
        }

        // FlipImage    
        public bool FlipImage { get; }

        // Name
        public string Name { get; }

        // PlaceholderType
        public PlaceholderType PlaceholderType { get; }

        // Polygon
        public ReadOnlyPolygon Polygon { get; }

        // Size
        public PlaceholderSize Size { get; }

        // Used
        public bool Used { get; set; }
    }
}
