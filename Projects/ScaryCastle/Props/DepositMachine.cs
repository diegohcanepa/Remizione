using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// DepositMachine
    /// </summary>
    public sealed class DepositMachine : Prop
    {
        private readonly ImageSprite lightLayer;

        // Constructor
        public DepositMachine(GameSession session, string name)
            : base(session, name)
        {
            this.HitEffect = HitEffect.Shake;
            this.HitTestPolygon = TestPolygon.Hotspot;
            this.lightLayer = new(Game);
            this.lightLayer.Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, 1, .85f, 80, -1);

            this.AttachedLight = new Light(Game, "")
            {
                PivotOrigin = RectanglePoint.Center
            };

            HighlightInteraction = false;
            PropState = PropState.Locked;
        }

        #region Protected members

        // OnDrawLights
        protected override void OnDrawLights(GameTime gameTime)
        {
            base.OnDrawLights(gameTime);
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            if (PropState != PropState.TurnedOff)
            {
                lightLayer.Position += GetShakeOffset();
                lightLayer.Draw(gameTime);
                lightLayer.Position -= GetShakeOffset();
            }
        }

        /*
        // OnPropStateChanged
        protected override void OnPropStateChanged(PropState previousState)
        {
            AnimationPlayer.Play(PropState == PropState.Locked ? AnimationNames.Locked : AnimationNames.Unlocked, false);

            if (PropState == PropState.Locked)
            {
                lightLayer.Image = Atlas?.GetImage($"{StaticName}LightRed");
                if (AttachedLight != null)
                {
                    AttachedLight.TurnOn(true);
                    AttachedLight.Color = Color.Red;
                }
            }
            else if (PropState == PropState.Unlocked)
            {
                lightLayer.Image = Atlas?.GetImage($"{StaticName}LightGreen");
                if (AttachedLight != null)
                {
                    AttachedLight.TurnOn(true);
                    AttachedLight.Color = Color.Green;
                }
            }
            else
                AttachedLight?.TurnOff(true);
        }
        */

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);
            lightLayer?.MatchTransform(Sprite);
            if (AttachedLight != null)
                AttachedLight.Position = BoundingBox.GetPoint(RectanglePoint.LeftTop, 6, 23);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            lightLayer.Update(gameTime);
        }

        #endregion
    }
}
