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
        public Placeholder(int x, int y, PlacementType placement, Ratio fillChance, PlaceholderTarget target = PlaceholderTarget.Prop)
        {
            this.Placement = placement;
            this.FillChance = fillChance;
            this.FlipImage = placement is PlacementType.WallRightBase or PlacementType.WallRightHang;
            this.Position = new(x, y);
            this.Target = target;
        }

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

        // Used
        public bool Used { get; set; }
    }
}
