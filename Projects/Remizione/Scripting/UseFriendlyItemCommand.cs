using Adberration.Scripting;

namespace Remizione.Scripting
{
    // UseFriendlyItemCommand
    // Arguments: {Actor} title {"Action"}
    [ForceAwait]
    internal sealed class UseFriendlyItemCommand : LocalizableCommand
    {
        // Constructor
        internal UseFriendlyItemCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 3)
        {
            AssertEntity<Actor>(0);
            AssertKeyword(1, "title");
            Parser.ParseQuotedString(this, 2);
        }

        // TextClauseIndex
        protected override int TextClauseIndex => 2;

        // OnExecute
        protected override void OnExecute()
        {
            // Actor
            if (AssertEntity<Actor>(0) is not Actor actor)
                return;

            actor.ChooseFriendlyItem(Parser.ParseQuotedString(this, 2));
        }
    }
}
