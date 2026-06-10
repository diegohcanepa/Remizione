namespace Adberration.Scripting
{
    // AwaitCameraCommand
    [ForceAwait]
    internal sealed class AwaitCameraCommand : AwaitableCommand
    {
        // Constructor
        internal AwaitCameraCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // IsAwaiting
        public override bool IsAwaiting()
        {
            return Session.Camera.IsMoving ||
                                           (Session.Camera.Target != null && !Session.Camera.IsTargetFocused);
        }
    }
}
