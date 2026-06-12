using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// Pill
    /// </summary>
    public abstract class Pill : PickableLoot
    {
        // Constructor
        protected Pill(GameSession session, string name)
            : base(session, name)
        {
            ApproachBehavior = ApproachBehavior.Over;
            Collider = new Polygon("0,0;5,0;5,4;0,4");
            Hotspot = new Polygon("8,-1;8,5;-1,5;-1,-1");
            RenderLayer = RenderLayer.OverBackground;
            Scale = new(.66f);
        }
    }
}
