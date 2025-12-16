using Engendro;

namespace Remizione
{
    /// <summary>
    /// Placeholder
    /// </summary>
    public sealed class Placeholder : INamedObject
    {
        // Constructor
        public Placeholder(string name, Ratio fillChance, bool flipImage, string vertices, string[] allowTags, PlaceholderTarget target = PlaceholderTarget.Prop)
        {
            this.Name = name;
            this.FillChance = fillChance;
            this.FlipImage = flipImage;
            this.Polygon = new ReadOnlyPolygon(vertices);
            this.AllowTags = new(allowTags);
            this.Target = target;
        }

        // AllowTags
        public Tags AllowTags { get; set; }

        // FillChance
        public Ratio FillChance { get; set; }

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
