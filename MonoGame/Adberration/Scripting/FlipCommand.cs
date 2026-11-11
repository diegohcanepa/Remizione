namespace Adberration.Scripting
{
    // FlipCommand
    // Arguments: {Thing}
    internal sealed class FlipCommand : NonAwaitableCommand
    {
        // Constructor
        internal FlipCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            AssertEntity<Thing>(0);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (AssertEntity<Thing>(0) is Thing thing)
                thing.FlipHorizontally();
        }

        #endregion
    }
}
