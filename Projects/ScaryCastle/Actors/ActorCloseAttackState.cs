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
        public ActorCloseAttackState(Actor owner)
            : base(owner, ActorStateNames.CloseAttack, false)
        {
        }

        #region Protected members

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!damageTaken && Owner.Session.Player != null && Owner.AnimationPlayer.Frame?.IsEvent == true)
            {
                damageTaken = true;
                Brain.Attack(Owner, Owner.Session.Player);
            }
        }

        #endregion

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (!Owner.AnimationPlayer.IsPlaying)
                return ActorStateNames.Stand;
            else
                return base.CheckTransitions();
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            damageTaken = false;
        }
    }
}