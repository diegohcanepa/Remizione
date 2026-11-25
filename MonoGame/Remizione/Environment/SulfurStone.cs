using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// SulfurStone
    /// </summary>
    public sealed class SulfurStone : Prop, ITransientLight
    {
        private static readonly Vector2 maxLightScale = new(9, 6);
        private static readonly Vector2 minLightScale = new(4, 3);

        #region Constructor

        // Constructor
        public SulfurStone(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            RenderLayer = RenderLayer.Default;

            AttachedLight ??= new("Light", LightKind.CobaltPebble)
            {
                PivotOrigin = RectanglePoint.Center,
            };

            AttachedLightPosition = new(2, 2);

            Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, 1, .8f, 70, -1);
        }

        #endregion

        #region ITransientLight interface

        // Reset
        void ITransientLight.Reset()
        {
            AttachedLight?.Unlit(true);
        }

        #endregion


        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            AttachedLight?.Lit(2000);
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
