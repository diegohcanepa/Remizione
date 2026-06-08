using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace ScaryCastle.Scripting
{
    // AwaitNPCTurnCommand
    // Syntax: {Source:Actor}
    [ForceAwait]
    internal sealed class AwaitNPCTurnCommand : AwaitableCommand
    {
        private Actor? actor;
        private bool awaitAttack;
        private bool awaitMove;

        // Constructor
        internal AwaitNPCTurnCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            AssertEntity<Actor>(0);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            actor = AssertEntity<Actor>(0);
            if (actor == null || actor.CombatDecision is not { } decision)
                return;

            if (decision.Type is CombatDecisionType.None or CombatDecisionType.Bullying)
                return;

            awaitMove = true;

            if (decision.Type == CombatDecisionType.Attack)
            {
                if (decision.Target != null)
                {
                    var pos = decision.Target.GetApproachPosition(actor, ApproachBehavior.ClosestSide);
                    actor.MoveTo(pos);
                }
            }
            else if (decision.Type == CombatDecisionType.Charge)
            {
                if (decision.Target != null)
                    actor.Charge(decision.Target.Position);
            }
            else if (decision.Type == CombatDecisionType.Flee)
            {
                actor.Flee();
            }
            else if (decision.Type == CombatDecisionType.RandomMove)
            {
                actor.MoveRandomly();
            }
            else if (decision.Type == CombatDecisionType.MoveNearby)
            {
                if (decision.Target != null)
                    actor.MoveNearby(decision.Target);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting()
        {
            if (actor == null)
                return false;

            if (awaitMove)
            {
                if (!actor.IsMoving)
                {
                    awaitMove = false;
                    awaitAttack = actor.CombatDecisionType is CombatDecisionType.Attack;
                    if (awaitAttack && actor.CombatDecision?.Intent is { } intent)
                        actor.ExecuteAction(intent, actor.CombatDecision.Target);
                }

                return true;
            }

            if (awaitAttack)
                return !actor.IsStanding;

            return false;
        }
    }
}