namespace Adberration.Scripting
{
    // SaveGameCommand
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class SaveGameCommand : NonAwaitableCommand
    {
        // Constructor
        internal SaveGameCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            Session.Save();
        }
    }
}
