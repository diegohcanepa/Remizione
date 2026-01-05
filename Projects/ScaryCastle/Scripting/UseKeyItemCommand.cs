using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // UseKeyItemCommand
    // Arguments: {"Action"}
    [ForceAwait]
    [ScriptStatement(CodingContext.Execution)]
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
            (Session as GameSession)?.ChooseKeyItem(Parser.ParseQuotedString(this, 0));
        }
    }
}
