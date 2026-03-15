using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace ScaryCastle.Scripting
{
    // AwaitPlayerAttackCommand
    // Arguments: {Target:GameThing}
    [ScriptStatement(CodingContext.Execution)]
    [ForceAwait]
    internal sealed class AwaitPlayerAttackCommand : AwaitableCommand
    {
        private Actor? player;

        // Constructor
        internal AwaitPlayerAttackCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 1)
        {
            AssertEntity<GameThing>(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            player = (Session as GameSession)?.Player;
            if (player == null || player.IsDead)
                return;

            if (AssertEntity<GameThing>(0) is not GameThing target)
                return;

            var intent = CombatBehavior.Behaviors.Find(player.DeclaredName)?.Intents.Find("Headbutt");
            if (intent != null)
                player.Attack(intent, target);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (player != null && !player.IsAttacking)
                player = null;
        }

        // IsAwaiting
        public override bool IsAwaiting => player != null && player.IsAttacking;
    }
}