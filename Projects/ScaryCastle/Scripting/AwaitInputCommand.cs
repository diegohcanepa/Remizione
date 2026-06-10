using Adberration.Scripting;
using Engendro.Input;

namespace ScaryCastle.Scripting
{
    // AwaitInputCommand
    [ForceAwait]
    internal sealed class AwaitInputCommand : AwaitableCommand
    {
        // Constructor
        public AwaitInputCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 0)
        {
        }

        #region Protected members

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            MouseCursor.PerformClick();
            InputManager.DefaultPlayer.Mouse.Reset();
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting()
        {
            return !InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed();
        }
    }
}
