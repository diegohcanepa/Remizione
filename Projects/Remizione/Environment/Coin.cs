using Engendro;

namespace Remizione
{
    /// <summary>
    /// Coin
    /// </summary>
    public sealed class Coin : PickableLoot
    {
        // Constructor
        public Coin(GameSession session, string name)
            : base(session, name)
        {
            Collider = new Polygon("0,0;5,0;5,4;0,4");
            DepthOffset = -3;
            DisplayNameKey = "Prop.Coin";
            Hotspot = new Polygon("0,0;5,0;5,4;0,4");

            var animation = AddAnimation("Default");
            animation.AddFrame("Coin01", 1500);
            animation.AddFrame("Coin02", 100);
            animation.AddFrame("Coin03", 100);
            animation.AddFrame("Coin04", 100);
        }
    }
}