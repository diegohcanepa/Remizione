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
        private Actor? attacker;

        // Constructor
        internal AwaitNPCAttackCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 1)
        {
            AssertEntity<Actor>(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session || session.Player == null)
                return;

            attacker = AssertEntity<Actor>(0);
            if (attacker == null || attacker.IsPlayer || attacker.IsDead)
                return;

            if (Brain.Decide(attacker) is CombatIntent combatIntent)
                attacker.Attack(combatIntent, session.Player);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (attacker != null && !attacker.IsAttacking)
                attacker = null;
        }

        // IsAwaiting
        public override bool IsAwaiting => attacker != null && attacker.IsAttacking;
    }
}
