using Adberration.Scripting;

namespace Remizione.Scripting
{
    // AwaitChanceRollCommand
    // Arguments: {Prop} {MetaItem} [#success-state:PropState]
    [ForceAwait]
    internal sealed class AwaitChanceRollCommand : AwaitableCommand
    {
        // Constructor
        internal AwaitChanceRollCommand(Script script, string source, StatementBody args)
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

            // Item
            if (session.Player.Inventory.Find(Body.Clauses[1]) is Item item)
            {
                var successState = Parser.ParseEnumArgument<PropState>(this, SuccessStateArg);
                session.HUD.ChanceRoll.Show(item, prop, successState);
            }
        }

        // IsAwaiting
        public override bool IsAwaiting => Session is GameSession session && session.HUD.ChanceRoll.IsVisible;
    }
}
