using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// TrapdoorKey
    /// </summary>
    public sealed class TrapdoorKey : PickableLoot
    {
        // Constructor
        public TrapdoorKey(GameSession session, string name)
            : base(session, name)
        {
            Collider = new Polygon("0,0;5,0;5,4;0,4");
            DisplayNameKey = $"Item.{nameof(TrapdoorKey)}.Name";
            Hotspot = new Polygon("8,-1;8,5;-1,5;-1,-1");
            RenderLayer = RenderLayer.OverBackground;

            var animation = AddAnimation("Default");
            animation.AddFrame("TrapdoorKey01", 1500);
            animation.AddFrame("TrapdoorKey02", 100);
            animation.AddFrame("TrapdoorKey03", 100);
            animation.AddFrame("TrapdoorKey04", 100);
        }
    }
}
