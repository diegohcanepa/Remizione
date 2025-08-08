namespace Adberration.Scripting
{
    // StopMovingCommand
    // Arguments: {Thing}
    internal sealed class StopMovingCommand : NonAwaitableCommand
    {
        // Constructor
        internal StopMovingCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            AssertEntity<Thing>(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            AssertEntity<Thing>(0)?.StopMoving();
        }
    }
}
