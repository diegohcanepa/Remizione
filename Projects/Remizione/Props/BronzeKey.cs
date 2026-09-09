using Engendro;

namespace Remizione
{
    /// <summary>
    /// BronzeKey
    /// </summary>
    public sealed class BronzeKey : Pickable
    {
        // Constructor
        public BronzeKey(GameSession session, string name)
            : base(session, name)
        {
            Collider = new Polygon("0,0;5,0;5,4;0,4");
            DisplayNameKey = $"Item.{nameof(BronzeKey)}.Name";
            Hotspot = new Polygon("8,-1;8,5;-1,5;-1,-1");
            RenderLayer = RenderLayer.OverBackground;

            var animation = AddAnimation("Default");
            animation.AddFrame("BronzeKey01", 1500);
            animation.AddFrame("BronzeKey02", 100);
            animation.AddFrame("BronzeKey03", 100);
            animation.AddFrame("BronzeKey04", 100);
        }
    }
}
