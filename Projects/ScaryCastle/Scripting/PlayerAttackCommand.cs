using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace ScaryCastle.Scripting
{
    // PlayerAttackCommand
    // Arguments: {CombatIntent} {Target:GameThing}
    [ScriptStatement(CodingContext.Execution)]
    [ForceAwait]
    internal sealed class PlayerAttackCommand : NonAwaitableCommand
    {
        private Actor? player;

        // Constructor
        internal PlayerAttackCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 2)
        {
            Parser.ParseName(this, 0);
            AssertEntity<GameThing>(1);
        }

        // OnExecute
        protected override void OnExecute()
        {
            player = (Session as GameSession)?.Player;
            if (player == null || player.IsDead || player.CombatBehavior == null)
                return;

            var combatIntentName = Parser.ParseName(this, 0);

            if (AssertEntity<GameThing>(1) is not GameThing target)
                return;

            var intent = player.CombatBehavior.Intents.Find(combatIntentName);
            if (intent != null)
                player.Attack(intent, target);
        }
    }
}