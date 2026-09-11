using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// LootOrb
    /// </summary>
    public sealed class LootOrb : Pickable
    {
        private readonly FloatTween fadeTween = new();

        // Constructor
        public LootOrb(GameSession session, string name)
            : base(session, name)
        {
            ApproachBehavior = ApproachBehavior.Over;
            Atlas = Atlases.Environment;
            DepthOffset = -2;
            Hotspot = new Polygon("0,0;7,0;7,7;0,7");
            RenderLayer = RenderLayer.Default;

            this.AttachedLight = new("Light", LightKind.LootOrb)
            {
                PivotOrigin = RectanglePoint.Center,
            };

            this.Color = AttachedLight.Color;

            AttachedLightPosition = new(3, 4);
        }

        #region Protected members

        // OnItemRewardChanged
        protected override void OnItemRewardChanged()
        {
            base.OnItemRewardChanged();
            DisplayNameKey = ItemReward != null ? $"Item.{ItemReward.Name}.Name" : string.Empty;
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            fadeTween.Start(TweenStyle.CubicIn, 0, 1, 1000);
            Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, .5f, .6f, 90, -1);
            Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicIn, .1f, .75f, 400);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (fadeTween.IsRunning)
            {
                fadeTween.Update(gameTime);
                OpacityFactor = fadeTween.CurrentValue;
            }
        }

        #endregion
    }
}
