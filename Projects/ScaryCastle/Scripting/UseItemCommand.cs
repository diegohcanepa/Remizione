using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // UseItemCommand
    // Arguments: {ItemName}
    [ForceAwait]
    internal sealed class UseItemCommand : NonAwaitableCommand
    {
        private readonly ItemDefinition definition;
        private Actor? player;

        // Constructor
        internal UseItemCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 1)
        {
            definition = Script.AssertItemDefinition(Body.Clauses[0]);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            if (session.Player == null || session.Player.IsDead || session.Player.CombatBehavior == null)
                return;

            //EffectDescriptor.Apply(definition.EffectDescriptors, player, player, EffectContext.Use);

            if (definition.Projectile != null)
                session.Player.LaunchProjectile("UsePistol", definition.Projectile);
        }
    }
}