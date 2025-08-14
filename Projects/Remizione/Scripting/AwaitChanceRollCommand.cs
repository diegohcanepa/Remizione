using Adberration.Scripting;

namespace Remizione.Scripting
{
    // AwaitChanceRollCommand
    // Arguments: {Actor} {Prop} {Item} [#success-state:PropState]
    [ForceAwait]
    internal sealed class AwaitChanceRollCommand : AwaitableCommand
    {
        // Constructor
        internal AwaitChanceRollCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 3, SuccessStateArg)
        {
            AssertEntity<Actor>(0);
            AssertEntity<Prop>(1);
            Parser.ParseEnumArgument<PropState>(this, SuccessStateArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            // Game session
            if (Session is not GameSession session)
                return;

            // Actor
            if (AssertEntity<Actor>(0) is not Actor actor)
                return;

            // Prop
            if (AssertEntity<Prop>(1) is not Prop prop)
                return;

            // Item
            if (actor.Inventory.Find(Body.Clauses[2]) is Item item)
            {
                var successState = Parser.ParseEnumArgument<PropState>(this, SuccessStateArg);
                session.ChanceRoll.Show(actor.GetOverheadPosition(0, -3), item, prop, successState);
            }
        }

        // IsAwaiting
        public override bool IsAwaiting => Session is GameSession session && session.ChanceRoll.IsRolling;
    }
}
