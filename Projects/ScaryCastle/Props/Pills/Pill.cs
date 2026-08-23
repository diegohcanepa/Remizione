using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// Pill
    /// </summary>
    public sealed class Pill : PickableLoot
    {
        // Constructor
        public Pill(GameSession session, string name)
            : base(session, name)
        {
            Collider = new Polygon("0,0;5,0;5,4;0,4");
            DisplayNameKey = $"Item.{DeclaredName}.Name";
            Hotspot = new Polygon("8,-1;8,5;-1,5;-1,-1");
            RenderLayer = RenderLayer.OverBackground;
            Scale = new(.66f);
        }
    }
}
