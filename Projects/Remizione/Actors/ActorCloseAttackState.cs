using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorCloseAttackState
    /// </summary>
    public sealed class ActorCloseAttackState : ActorState
    {
        private bool damageTaken;

        // Constructor
        public ActorCloseAttackState(Actor owner)
            : base(owner, ActorStateNames.CloseAttack, ActorStateSettings.None)
        {
        }

        #region Protected members

        // GetAnimationName
        protected override string GetAnimationName() => Owner.AttackSkill is Item skill ? skill.Name.ToString() : string.Empty;

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Owner.Target == null || Owner.AttackSkill == null)
                return;

            if (!damageTaken && Owner.AnimationPlayer.Frame is SpriteFrame frame)
            {
                if (frame.Label == GameSettings.KeyFrame)
                {
                    damageTaken = true;
                    Owner.AttackSkill.EndUse(Owner.Target);
                }
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