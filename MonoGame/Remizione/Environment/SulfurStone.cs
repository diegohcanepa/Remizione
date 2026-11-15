using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// SulfurStone
    /// </summary>
    public sealed class SulfurStone : Prop
    {
        private static readonly Vector2 maxLightScale = new(18, 12);
        private static readonly Vector2 minLightScale = new(9, 6);

        #region Constructor

        // Constructor
        public SulfurStone(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            IsStatic = true;
            RenderLayer = RenderLayer.Default;

            AttachedLight ??= new("Light", LightKind.CobaltPebble)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = new(7, 5)
            };

            AttachedLightPosition = new(2, 2);
        }

        #endregion

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            AttachedLight?.Lit(3000);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (RemainingTime >= 0)
            {
                RemainingTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (RemainingTime <= 0)
                {
                    AttachedLight?.Unlit(1000);
                    Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, Opacity, 0, 1000, Unparent);
                }
                else
                {
                    if (AttachedLight != null)
                    {
                        float progress = MathHelper.Clamp(RemainingTime / GameData.WorldSettings.SulfurStoneDuration, 0f, 1f);
                        AttachedLight.Scale = Vector2.Lerp(minLightScale, maxLightScale, progress);
                    }
                }
            }
        }

        #endregion

        // RemainingTime
        public float RemainingTime { get; set; } = GameData.WorldSettings.SulfurStoneDuration;
    }
}
