using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // ApplyEnemyCombatIntentCommand 
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class ApplyEnemyCombatIntentCommand : NonAwaitableCommand
    {
        // Constructor
        internal ApplyEnemyCombatIntentCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            // TODO: Check

            /*
            if ((Session as GameSession)?.CombatManager is not CombatManager combatManager)
                return;

            if (combatManager.Enemy.CombatIntent != null)
                EffectDescriptor.Apply(combatManager.Enemy.CombatIntent.EffectDescriptors, combatManager.Enemy, combatManager.Player);
            */
        }
    }
}
