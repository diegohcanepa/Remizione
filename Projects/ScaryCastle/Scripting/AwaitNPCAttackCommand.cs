using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace ScaryCastle.Scripting
{
    // AwaitNPCAttackCommand
    // Arguments: {Actor}
    [ScriptStatement(CodingContext.Execution)]
    [ForceAwait]
    internal sealed class AwaitNPCAttackCommand : AwaitableCommand
    {
        private Actor? npc;

        // Constructor
        internal AwaitNPCAttackCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 1)
        {
            AssertEntity<Actor>(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            /*
            if (Session is not GameSession session || session.Player == null)
                return;

            npc = AssertEntity<Actor>(0);
            if (npc != null && npc.Reaction is CombatDecision decision)
            {
                if (decision.Type == CombatDecisionType.Attack)
                {
                    if (decision.Intent != null)
                        npc.Attack(decision.Intent, session.Player);
                }
            }
            */
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (npc != null && !npc.IsAttacking)
                npc = null;
        }

        // IsAwaiting
        public override bool IsAwaiting => npc?.IsAttacking == true;
    }
}
