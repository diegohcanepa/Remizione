using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // BeginCombatCommand
    // Arguments: {Enemy:Actor}
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class BeginCombatCommand : NonAwaitableCommand
    {
        // Constructor
        public BeginCombatCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 1)
        {
            AssertEntity<Actor>(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (AssertEntity<Actor>(0) is Actor enemy)
                (Session as GameSession)?.BeginCombat(enemy);
        }
    }
}
