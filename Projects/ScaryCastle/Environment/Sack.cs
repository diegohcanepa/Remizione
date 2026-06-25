using Adberration;
using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// Sack
    /// </summary>
    public sealed class Sack : PickableLoot
    {
        // Constructor
        public Sack(GameSession session, string name)
            : base(session, name)
        {
            ApproachBehavior = ApproachBehavior.ClosestSide;
            Atlas = Atlases.Props;
            DepthOffset = -2;
            DisplayNameKey = "Prop.Sack";
            Hotspot = new Polygon("0,0;7,0;7,7;0,7");
            RenderLayer = RenderLayer.Default;
        }

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

        #endregion

        // Bounce
        public override void Bounce()
        {
            BounceCore(.75f, 8);
        }
    }
}
