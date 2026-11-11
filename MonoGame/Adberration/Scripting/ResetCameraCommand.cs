namespace Adberration.Scripting
{
    // ResetCameraCommand
    internal sealed class ResetCameraCommand : NonAwaitableCommand
    {
        // Constructor
        internal ResetCameraCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            Session.Camera.Reset();
        }
    }
}
