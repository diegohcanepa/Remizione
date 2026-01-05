using Engendro.Input;

namespace Adberration.Scripting
{
    // SuspendVibrationCommand
    // Arguments: {Duration:Integer}
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class SuspendVibrationCommand : NonAwaitableCommand
    {
        // Constructor
        internal SuspendVibrationCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            Parser.ParseInt32(this, 0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            InputManager.DefaultPlayer.GamePad.SuspendVibration(Parser.ParseInt32(this, 0));
        }
    }
}
