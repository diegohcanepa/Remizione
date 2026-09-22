using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Fleshiness
    /// </summary>
    public sealed class Fleshiness : Prop
    {
        private readonly FloatTween fadeTween = new();

        // Constructor
        public Fleshiness(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            DepthOffset = -2;
            LabelKey = "Prop.Fleshiness";
            Hotspot = new Polygon("0,0;13,0;13,5;0,5");
            RenderLayer = RenderLayer.Default;

            /*
            this.AttachedLight = new("Light", LightKind.Fleshiness)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = new(2, .5f)
            };

            this.Color = AttachedLight.Color;
            */

            AttachedLightPosition = new(3, 2);
        }

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            if (AttachedLight != null)
            {
                AttachedLight.Unlit(true);
                AttachedLight.Lit();
            }

            fadeTween.Start(TweenStyle.CubicIn, 0, 1, 1000);
            Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, .5f, .6f, 90, -1);
            Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicIn, 0, .75f, 400);
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
