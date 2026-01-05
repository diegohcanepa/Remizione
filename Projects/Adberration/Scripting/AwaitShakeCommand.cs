using Engendro;

namespace Adberration.Scripting
{
    // AwaitShakeCommand
    [ForceAwait]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class AwaitShakeCommand : AwaitableCommand
    {
        // Constructor
        internal AwaitShakeCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // IsAwaiting
        public override bool IsAwaiting => Session.Camera.ShakeState != CameraShakeState.None;
    }
}
