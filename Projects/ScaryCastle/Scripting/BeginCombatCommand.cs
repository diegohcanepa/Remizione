using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // BeginCombatCommand
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class BeginCombatCommand : NonAwaitableCommand
    {
        // Constructor
        public BeginCombatCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            (Session as GameSession)?.BeginCombat();
        }
    }
}
