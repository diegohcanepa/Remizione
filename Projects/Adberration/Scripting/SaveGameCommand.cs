namespace Adberration.Scripting
{
    // SaveGameCommand
    // Argumemts: [#disable]
    internal sealed class SaveGameCommand : NonAwaitableCommand
    {
        // Constructor
        internal SaveGameCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0, DisableArg)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            Session.Save(HasArg(DisableArg));
        }
    }
}
