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

                var destination = target.GetApproachPosition(Actor, false);
                Actor.MoveTo(destination);
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
        }
    }
}
