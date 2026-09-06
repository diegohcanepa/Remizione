using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Rat
    /// </summary>
    public class Rat : Actor
    {
        private readonly Vector2Tween scaleTween;

        // Constructor
        public Rat(GameSession session, string name)
            : base(session, name)
        {
            AnimationSettings.SupressAll();
            BodySize = BodySize.Small;
            FastMoveFactor = 3;
            scaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, Vector2.Zero, new(0, .05f), 200, -1);
            Scale = new(.5f);
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
