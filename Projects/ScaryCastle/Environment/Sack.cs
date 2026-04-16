using Adberration;
using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// Sack
    /// </summary>
    public sealed class Sack : Prop, ILoot<ItemDefinition>, IPoolable
    {
        // Constructor
        public Sack(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            DepthOffset = -2;
            Hotspot = new Polygon("0,0;7,0;7,7;0,7");
        }

        #region IPoolable interface

        // Reset
        void IPoolable.Reset() => Loot = null;

        #endregion

        #region Protected members

        // OnParentChanged
        protected override void OnParentChanged(Entity? previousParent)
        {
            base.OnParentChanged(previousParent);
            if (Parent == null)
                Unload();
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            Session.ObjectPools.Sacks.Return(this);
        }

        #endregion

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
