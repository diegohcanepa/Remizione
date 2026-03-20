using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Rat
    /// </summary>
    public sealed class Rat : Actor
    {
        private readonly Vector2Tween scaleTween;

        // Constructor
        public Rat(GameSession session, string name)
            : base(session, name)
        {
            AnimationSettings.SupressAll();
            BodySize = BodySize.Small;
            FastMoveFactor = 3;
            Guts = 4;
            Scale = new(.8f);
            scaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, Vector2.Zero, new(0, .05f), 200, -1);
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Sprite.ScaleFactor += scaleTween.CurrentValue;
            base.OnDraw(gameTime);
            Sprite.ScaleFactor -= scaleTween.CurrentValue;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            scaleTween.Update(gameTime);
        }

        #endregion
    }
}
