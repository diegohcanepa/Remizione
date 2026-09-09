using Adberration.Scripting;

namespace Remizione.Scripting
{
    // SetItemRewardCommand
    // Syntax: {GameThing} {ItemName}
    internal sealed class SetItemRewardCommand : NonAwaitableCommand
    {
        // Constructor
        internal SetItemRewardCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2)
        {
            AssertEntity<GameThing>(0);
            Script.AssertItemDefinition(Body.Clauses[1]);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            if (AssertEntity<GameThing>(0) is not { } target)
                return;

            target.ItemReward = GameData.Items.Find(Body.Clauses[1]);
        }
    }
}
