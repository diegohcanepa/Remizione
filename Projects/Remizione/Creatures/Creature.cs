using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Creature
    /// </summary>
    public class Creature : Actor
    {
        private readonly Vector2Tween scaleTween;

        // Constructor
        public Creature(GameSession session, string name)
            : base(session, name)
        {
            scaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, Vector2.Zero, new(0, .1f), 600, -1);
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
