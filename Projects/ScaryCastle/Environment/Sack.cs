using Adberration;
using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// Sack
    /// </summary>
    public sealed class Sack : Prop, ILootContainer<ItemDefinition>, IPoolable
    {
        // Constructor
        public Sack(GameSession session, string name)
            : base(session, name)
        {
            ApproachBehavior = ApproachBehavior.ClosestSide;
            Atlas = Atlases.Props;
            DepthOffset = -2;
            Hotspot = new Polygon("0,0;7,0;7,7;0,7");
        }

        #region IPoolable interface

        // Reset
        void IPoolable.Reset()
        {
            Loot = null;
        }

        #endregion

        #region Protected members

        // GetDisplayName
        protected override string GetDisplayName()
        {
            return TextRepository.GetValue(DisplayNameKey);
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            Bounce();
        }

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

        // Bounce
        public override void Bounce()
        {
            BounceCore(.75f, 8);
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
