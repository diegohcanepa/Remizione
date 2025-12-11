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
        public Placeholder(string name, float fillChance, bool flipImage, Vector2[] vertices, string[] allowTags, PlaceholderTarget target = PlaceholderTarget.Prop)
        {
            this.Name = name;
            this.FillChance = fillChance;
            this.FlipImage = flipImage;
            this.Polygon = new ReadOnlyPolygon(Geometry.SimplifyPolygon(vertices));
            this.AllowTags = new(allowTags);
            this.Target = target;
        }

        // AllowTags
        public Tags AllowTags { get; set; }

        // FillChance
        public float FillChance { get; set; }

        // FlipImage    
        public bool FlipImage { get; }

        // Name
        public string Name { get; }

        // Polygon
        public ReadOnlyPolygon Polygon { get; }

        // Target
        public PlaceholderTarget Target { get; set; }

        // Used
        public bool Used { get; set; }
    }
}
