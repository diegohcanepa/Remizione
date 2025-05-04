using EngendroAdventure;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatChargeState
    /// </summary>
    public sealed class CombatChargeState : CombatState
    {
        // Constructor
        public CombatChargeState(CombatStateMachine stateMachine)
            : base(stateMachine)
        {
        }

        #region Private members

        // MoveTowardsTarget
        private void MoveTowardsTarget()
        {
            if (Actor.Target is GameThing target)
            {
                if (target is Actor actorTarget && actorTarget.IsAlert)
                    actorTarget.FaceTo(Actor);

                Actor.FastMove = true;

                var front = target.Direction == FacingDirection.Right && Actor.X > target.X ||
                            target.Direction == FacingDirection.Left && Actor.X < target.X;

                Actor.MoveTo(target.GetApproachPosition(target, front));
            }
        }
        
        #endregion

        // Enter
        public override void Enter()
        {
            base.Enter();
            MoveTowardsTarget();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Actor.Anger <= 0)
                StateMachine.ExecuteAction(CombatStateSignal.Fatigue);

            else if (!Actor.IsMoving)
                StateMachine.ExecuteAction(CombatStateSignal.CloseAttack);
        }
    }
}
