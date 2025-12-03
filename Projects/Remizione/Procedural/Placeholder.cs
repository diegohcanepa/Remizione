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
        public Placeholder(string name, float fillChance, bool flipImage, Vector2[] vertices, string[] allowTags)
        {
            this.Name = name;
            this.FillChance = fillChance;
            this.FlipImage = flipImage;
            this.Polygon = new ReadOnlyPolygon(Geometry.SimplifyPolygon(vertices));
            this.AllowTags = new(allowTags);
        }

        // AllowTags
        public Tags AllowTags { get; }

        // FillChance
        public float FillChance { get; }

        // FlipImage    
        public bool FlipImage { get; }

        // Name
        public string Name { get; }

        // Polygon
        public ReadOnlyPolygon Polygon { get; }

        // Used
        public bool Used { get; set; }
    }
}
