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
            BodySize = ActorSize.Small;
            FastMoveFactor = 3;
            Guts = 7;
            AttackRange = 10;
            scaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, Vector2.Zero, new(0, .05f), 600, -1);
            Sensor.ViewAngle = 360;

            BodyMachine.AddState(new BodyCloseAttackState());
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            SupressOnTransformNotification++;
            Scale += scaleTween.CurrentValue;
            base.OnDraw(gameTime);
            Scale -= scaleTween.CurrentValue;
            SupressOnTransformNotification--;
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
