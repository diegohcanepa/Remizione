using Adberration.Scripting;

namespace Remizione.Scripting
{
    // AwaitChanceRollCommand
    // Arguments: {Actor} {Prop} {Item}
    [ForceAwait]
    internal sealed class AwaitChanceRollCommand : AwaitableCommand
    {
        // Constructor
        internal AwaitChanceRollCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 3)
        {
            AssertEntity<Actor>(0);
            AssertEntity<Prop>(1);
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
                session.ChanceRoll.Show(actor.GetOverheadPosition(), item.Chance - prop.ChancePenalty);
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            if (AssertEntity<Actor>(0)?.Inventory.Find(Body.Clauses[2]) is Item item)
            {
                item.Use();
                if (item.MetaItem.IsStackable && Session is GameSession session)
                    session.HUD.Log.Show(LogVerb.Lost, item.DisplayText, item.MetaItem.Image);
            }
        }

        // IsAwaiting
        public override bool IsAwaiting => Session is GameSession session && session.ChanceRoll.IsRolling;
    }
}
