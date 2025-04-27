using Engendro;
using Engendro.Input;
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
        protected override string GetAnimationName() => AnimationName;

        // Target
        public GameThing? Target { get; set; } = null;

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!damageTaken && Target != null && Owner.DistanceTo(Target) <= Owner.CloseAttackItem.Range && Owner.AnimationPlayer.Frame is SpriteFrame frame)
            {
                if (frame.Label == GameSettings.KeyFrame)
                {
                    damageTaken = true;
                    Owner.CloseAttackItem?.EndUse(Target);
                }
            }
        }

        #endregion

        // AnimationName
        public string AnimationName { get; set; } = string.Empty;

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

        // Exit
        public override void Exit()
        {
            base.Exit();

            if (Owner.IsPlayer)
                Target = null;

            if (Owner.Session.CombatManager.CurrentActor == Owner)
                Owner.Session.CombatManager.AdvanceTurn();
        }
    }
}