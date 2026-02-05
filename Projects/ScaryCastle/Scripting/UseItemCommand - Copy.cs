using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // ApplyCombatIntentCommand
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class ApplyCombatIntentCommand : NonAwaitableCommand
    {
        // Constructor
        internal ApplyCombatIntentCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            if ((Session as GameSession)?.CombatManager is not CombatManager combatManager)
                return;

            if (combatManager.Enemy.CombatIntent != null)
                EffectDescriptor.Apply(combatManager.Enemy.CombatIntent.EffectDescriptors, combatManager.Enemy, combatManager.Player);
        }
    }
}
