using Engendro.Input;
using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // VibrateCommand
    // Arguments: {Duration:Integer} [#left:float] [#right:float] [#left-trigger:float] [#right-trigger:float]
    internal sealed class VibrateCommand : NonAwaitableCommand
    {
        // Constructor
        public VibrateCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, LeftArg, LeftTriggerArg, RightArg, RightTriggerArg)
        {
            Parser.ParseInt32(this, 0);
            Parser.ParseRatioArgument(this, LeftArg);
            Parser.ParseRatioArgument(this, LeftTriggerArg);
            Parser.ParseRatioArgument(this, RightArg);
            Parser.ParseRatioArgument(this, RightTriggerArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var duration = Parser.ParseInt32(this, 0);
            var leftMotor = Parser.ParseRatioArgument(this, LeftArg, 0);
            var rightMotor = Parser.ParseRatioArgument(this, RightArg, 0);
            var leftTrigger = Parser.ParseRatioArgument(this, LeftTriggerArg, 0);
            var rightTrigger = Parser.ParseRatioArgument(this, RightTriggerArg, 0);

            InputManager.DefaultPlayer.GamePad.Vibrate(duration, leftMotor, rightMotor, leftTrigger, rightTrigger);
        }
    }
}