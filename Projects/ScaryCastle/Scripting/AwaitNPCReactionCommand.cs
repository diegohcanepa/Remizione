using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace ScaryCastle.Scripting
{
    // AwaitNPCReactionCommand
    // Arguments: {Actor}
    [ScriptStatement(CodingContext.Execution)]
    [ForceAwait]
    internal sealed class AwaitNPCReactionCommand : AwaitableCommand
    {
        private Actor? npc;

        // Constructor
        internal AwaitNPCReactionCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 1)
        {
            AssertEntity<Actor>(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session || session.Player == null)
                return;

            npc = AssertEntity<Actor>(0);
            if (npc == null || npc.IsPlayer || npc.IsDead)
                return;

            if (Brain.Decide(npc) is CombatDecision decision)
            {
                if (decision.Type == CombatDecisionType.Attack)
                {
                    if (decision.Intent != null)
                        npc.Attack(decision.Intent, session.Player);
                }
                else if (decision.Type == CombatDecisionType.Flee)
                {
                    npc.MoveRandomly();
                }
            }
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            base.OnExecutionCompleted();
            npc?.PendingReaction = false;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (npc != null && !npc.IsAttacking)
                npc = null;
        }

        // IsAwaiting
        public override bool IsAwaiting => npc != null && npc.IsAttacking;
    }
}
