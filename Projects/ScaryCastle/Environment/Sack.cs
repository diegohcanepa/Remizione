using Adberration;
using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// Sack
    /// </summary>
    public sealed class Sack : Prop
    {
        // Constructor
        public Sack(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            DepthOffset = -2;
            DisplayNameKey = "Prop.Sack";
            Hotspot = new Polygon("0,0;7,0;7,7;0,7");
        }

        // OnParentChanged
        protected override void OnParentChanged(Entity? previousParent)
        {
            base.OnParentChanged(previousParent);
            if (Parent == null)
                Session.ObjectPools.Sacks.Return(this);
        }

        // Item
        public Item? Item { get; set; }
    }
}
