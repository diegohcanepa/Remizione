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
            FastMoveFactor = 3;
            Guts = 7;
            ShadowSpotSize = 0;
            AllowMovementBehavior = true;
            AttackRate = 2000;
            scaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, Vector2.Zero, new(0, .05f), 600, -1);

            StateMachine.RegisterState(new ActorCloseAttackState(this));
        }

        #region Protected members

        // BeginAttackExecution
        protected override void BeginAttackExecution()
        {
            PlaySound("EnviousEyeAttack");
            StateMachine.ChangeState(ActorStateNames.CloseAttack);
        }

        // BeginMovementBehavior
        protected override void BeginMovementBehavior()
        {
            if (IsNervous && Session.Player != null)
                MoveTo(Session.Player.Position);
            else
                base.BeginMovementBehavior();
        }

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

        // UpdateAttackExecution
        protected override void UpdateAttackExecution(GameTime gameTime)
        {
            if (StateMachine.CurrentState is not ActorCloseAttackState)
                EndAttack();
        }

        #endregion
    }
}
