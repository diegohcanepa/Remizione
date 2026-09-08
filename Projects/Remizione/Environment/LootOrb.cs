using Engendro;

namespace Remizione
{
    /// <summary>
    /// LootOrb
    /// </summary>
    public sealed class LootOrb : PickableLoot
    {
        // Constructor
        public LootOrb(GameSession session, string name)
            : base(session, name)
        {
            ApproachBehavior = ApproachBehavior.Over;
            Atlas = Atlases.Environment;
            Color = new(240, 181, 65);
            DepthOffset = -2;
            Hotspot = new Polygon("0,0;7,0;7,7;0,7");
            Opacity = .6f;
            RenderLayer = RenderLayer.Default;

            this.AttachedLight = new("Light")
            {
                Color = this.Color,
                PivotOrigin = RectanglePoint.Center,
                LightKind = LightKind.LootOrb,
            };

            AttachedLightPosition = new(3, 4);
        }

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicOut, 0, .75f, 1000);
        }

        // OnLootChanged
        protected override void OnLootChanged()
        {
            base.OnLootChanged();
            DisplayNameKey = Loot != null ? $"Item.{Loot.Name}.Name" : string.Empty;
        }

        #endregion
    }
}
