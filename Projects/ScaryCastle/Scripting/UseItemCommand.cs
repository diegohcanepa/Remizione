using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // UseItemCommand
    // Arguments: {ItemName} [#context:{EffectContext}]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class UseItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal UseItemCommand(Script script, string source, StatementBody args)
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

            if (session.Inventory.Find(Body.Clauses[0]) is Item item && session.OutcomeTarget is GameThing target)
            {
                var context = Parser.ParseEnumArgument<EffectContext>(this, ContextArg, EffectContext.Use);
                item.Use(player, target, context);
            }
        }
    }
}
