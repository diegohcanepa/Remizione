using Engendro.Input;

namespace Adberration.Scripting
{
    // StopVibrationCommand
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class StopVibrationCommand : NonAwaitableCommand
    {
        // Constructor
        internal StopVibrationCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            InputManager.DefaultPlayer.GamePad.StopVibration();
        }
    }
}
