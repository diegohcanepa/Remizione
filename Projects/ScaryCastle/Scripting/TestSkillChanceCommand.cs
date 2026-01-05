using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // TestSkillChanceCommand
    // Arguments: {Prop} {MetaItem} [#success-state:PropState]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class TestSkillChanceCommand : NonAwaitableCommand
    {
        // Constructor
        internal TestSkillChanceCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 2, SuccessStateArg)
        {
            AssertEntity<Prop>(0);

            if (MetaItem.Find(Body.Clauses[1]) == null)
                throw new ScriptException(this, $"MetaItem '{Body.Clauses[1]}' does not exist.");

            Parser.ParseEnumArgument<PropState>(this, SuccessStateArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            // Game session
            if (Session is not GameSession session)
                return;

            // Player
            if (session.Player == null)
                return;

            // Prop
            if (AssertEntity<Prop>(0) is not Prop prop)
                return;

            var propState = Parser.ParseEnumArgument<PropState>(this, SuccessStateArg);

            // Item
            if (session.Inventory.Find(Body.Clauses[1]) is Item item)
                prop.TestSkillChance(session.Player, item, propState);
        }
    }
}
