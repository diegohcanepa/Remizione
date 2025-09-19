using Adberration.Scripting;

namespace Remizione.Scripting
{
    // UseKeyItemCommand
    // Arguments: {"Action"}
    [ForceAwait]
    internal sealed class UseKeyItemCommand : LocalizableCommand
    {
        // Constructor
        internal UseKeyItemCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 1)
        {
            Parser.ParseQuotedString(this, 0);
        }

        // TextClauseIndex
        protected override int TextClauseIndex => 0;

        // OnExecute
        protected override void OnExecute()
        {
            if ((Session as GameSession)?.Player is Actor actor)
                actor.ChooseKeyItem(Parser.ParseQuotedString(this, 0));
        }
    }
}
