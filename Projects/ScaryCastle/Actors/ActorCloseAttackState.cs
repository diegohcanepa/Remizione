using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorCloseAttackState
    /// </summary>
    public sealed class ActorCloseAttackState : ActorAnimatedState
    {
        private bool damageTaken;

        // Constructor
        public ActorCloseAttackState()
            : base(AnimationNames.CloseAttack, false)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            damageTaken = false;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!damageTaken && Owner.Session.Player != null && Owner.AnimationPlayer.Frame?.IsEvent == true)
            {
                damageTaken = true;
                Brain.Attack(Owner, Owner.Session.Player);
                Owner.Session.InterruptAwaitingScript();
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<ActorStandState>();
        }
    }
}