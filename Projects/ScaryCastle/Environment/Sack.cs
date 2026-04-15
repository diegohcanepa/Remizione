using Adberration;
using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// Sack
    /// </summary>
    public sealed class Sack : Prop, ILoot<ItemDefinition>
    {
        // Constructor
        public Sack(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            DepthOffset = -2;
            Hotspot = new Polygon("0,0;7,0;7,7;0,7");
        }

        // OnParentChanged
        protected override void OnParentChanged(Entity? previousParent)
        {
            base.OnParentChanged(previousParent);
            
            if (Parent == null)
                Session.ObjectPools.Sacks.Return(this);
        }

        // Loot
        public ItemDefinition? Loot
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    DisplayNameKey = Loot != null ? $"Item.{Loot.Name}.Name" : string.Empty;
                }
            }
        }
    }
}
