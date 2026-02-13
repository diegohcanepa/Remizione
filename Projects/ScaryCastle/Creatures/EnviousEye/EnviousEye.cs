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
            IsAngry = true;
            AllowMovementBehavior = true;
            AttackRate = 4000;
            scaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, Vector2.Zero, new(0, .05f), 600, -1);

            StateMachine.AddState(new ActorCloseAttackState());
        }

        #region Protected members

        // OnBeginMovementBehavior
        protected override void OnBeginMovementBehavior()
        {
            if (IsAngry && Session.Player != null)
                MoveTo(Session.Player.Position);
            else
                base.OnBeginMovementBehavior();
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

        // OnStartAttack
        protected override bool OnStartAttack()
        {
            if (Session.Player == null)
                return false;

            if (DistanceTo(Session.Player.Position) <= AttackRange)
            {
                PlaySound("EnviousEyeAttack");
                StateMachine.ChangeState<ActorCloseAttackState>();
                return true;
            }
            
            return false;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            scaleTween.Update(gameTime);
        }

        // OnUpdateAttackExecution
        protected override void OnUpdateAttackExecution(GameTime gameTime)
        {
            if (StateMachine.CurrentState is not ActorCloseAttackState)
                EndAttack();
        }

        #endregion
    }
}
