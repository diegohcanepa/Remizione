using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Placeholder
    /// </summary>
    public sealed class Placeholder
    {
        // Constructor
        public Placeholder(Vector2 position, PlacementType placement, Ratio fillChance, Tags allowTags, PlaceholderTarget target = PlaceholderTarget.Prop)
        {
            this.Position = position;
            this.Placement = placement;
            this.FillChance = fillChance;
            this.AllowTags = allowTags;
            this.FlipImage = placement is PlacementType.WallRightBase or PlacementType.WallRightHang;
            this.Target = target;
        }

        // AllowTags
        public Tags AllowTags { get; }

        // FillChance
        public Ratio FillChance { get; set; }

        // FlipImage    
        public bool FlipImage { get; }

        // Placement
        public PlacementType Placement { get; }

        // Position
        public Vector2 Position { get; }

        // Target
        public PlaceholderTarget Target { get; set; }
    }
}
