using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// GoldenKey
    /// </summary>
    public sealed class GoldenKey : PickableLoot
    {
        // Constructor
        public GoldenKey(GameSession session, string name)
            : base(session, name)
        {
            Collider = new Polygon("0,0;5,0;5,4;0,4");
            DisplayNameKey = $"Item.{nameof(GoldenKey)}.Name";
            Hotspot = new Polygon("8,-1;8,5;-1,5;-1,-1");
            RenderLayer = RenderLayer.OverBackground;

            var animation = AddAnimation("Default");
            animation.AddFrame("GoldenKey01", 1500);
            animation.AddFrame("GoldenKey02", 100);
            animation.AddFrame("GoldenKey03", 100);
            animation.AddFrame("GoldenKey04", 100);
        }
    }
}
