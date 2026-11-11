namespace Adberration.Scripting
{
    // AwaitScriptCommand
    [ForceAwait]
    internal sealed class AwaitScriptCommand : AwaitableCommand
    {
        // Constructor
        internal AwaitScriptCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            Session.AwaitScript(Script);
        }

        #endregion
    }
}
