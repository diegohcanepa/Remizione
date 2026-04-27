using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // ApplyItemEffectsCommand
    // Arguments: {ItemName} [#context:{EffectContext}]
    internal sealed class ApplyItemEffectsCommand : NonAwaitableCommand
    {
        // Constructor
        internal ApplyItemEffectsCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 1, ContextArg)
        {
            Script.AssertItemDefinition(Body.Clauses[0]);
            Parser.ParseEnumArgument<EffectContext>(this, ContextArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            if (session.Player is not Actor player)
                return;

            if (session.PlayerInventory.Find(Body.Clauses[0]) is Item item && session.OutcomeTarget is GameThing target)
            {
                var context = Parser.ParseEnumArgument(this, ContextArg, EffectContext.Use);
                item.ApplyEffects(player, target, context);
            }
        }
    }
}
