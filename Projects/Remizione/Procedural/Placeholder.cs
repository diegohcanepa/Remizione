using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// Placeholder
    /// </summary>
    public sealed class Placeholder
    {
        // Constructor
        public Placeholder(string name, PlaceholderType placeholderType, PlaceholderSize placeholderSize, float fillChance, bool flipImage, Vector2[] vertices, params string[] allowedTags)
        {
            this.Name = name;
            this.PlaceholderType = placeholderType;
            this.Size = placeholderSize;
            this.FillChance = fillChance;
            this.FlipImage = flipImage;
            this.Polygon = new ReadOnlyPolygon(Geometry.SimplifyPolygon(vertices));
            this.AllowedTags = new(allowedTags);
        }

        // AllowedTags
        public ReadOnlyCollection<string> AllowedTags { get; }

        // FillChance
        public float FillChance { get; }

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
