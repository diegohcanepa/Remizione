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

            if (decision.Type is CombatDecisionType.None or CombatDecisionType.Curse)
                return;

            awaitAttack = false;
            awaitMove = false;

            if (decision.Type == CombatDecisionType.Attack)
            {
                if (decision.Target != null)
                {
                    awaitMove = true;
                    var pos = decision.Target.GetApproachPosition(actor, ApproachBehavior.ClosestSide);
                    actor.MoveTo(pos);
                }
            }
            else if (decision.Type == CombatDecisionType.Charge)
            {
                if (decision.Target != null)
                {
                    awaitMove = true;
                    actor.Charge(decision.Target.Position);
                }
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

        #endregion

        // IsAwaiting
        public override bool IsAwaiting()
        {
            if (actor == null || actor.IsDead)
                return false;

            if (awaitMove)
            {
                if (!actor.IsMoving)
                {
                    awaitMove = false;
                    awaitAttack = actor.CombatDecisionType is CombatDecisionType.Attack or CombatDecisionType.Charge;
                    if (awaitAttack && actor.CombatDecision?.Intent is { } intent)
                        actor.ExecuteAction(intent, actor.CombatDecision.Target);
                }

                return true;
            }

            if (awaitAttack)
            {
                if (actor.IsPerformingAction)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
    }
}