using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// EnviousEye
    /// </summary>
    public sealed class EnviousEye : ProceduralActor
    {
        private readonly Vector2Tween scaleTween;

        // Constructor
        public EnviousEye(GameSession session, string name)
            : base(session, name)
        {
            AnimationSettings.SupressAll();
            BodySize = BodySize.Small;
            FastMoveFactor = 3;
            Guts = 7;
            scaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, Vector2.Zero, new(0, .1f), 600, -1);

            BodyMachine.AddState(new BodyCloseAttackState());
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
