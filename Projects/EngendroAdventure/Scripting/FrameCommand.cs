using System.Globalization;

namespace EngendroAdventure.Scripting
{
    // FrameCommand
    // Arguments: {Range:Int32Range} duration {Int32} [#goto:Label] [#label:Name] [#repeat:Integer] [#sound:Name] [#sub-area:Rectangle] [#speed-factor:Float]
    internal sealed class FrameCommand : NonAwaitableCommand
    {
        // Constructor
        internal FrameCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3, EventFrameArg, FootstepArg, GotoArg, LabelArg, RepeatArg, SoundArg, SubAreaArg, SpeedFactorArg)
        {
            if (AnimationCommand.ActiveAnimation == null)
                throw ScriptExceptionBuilder.AnimationNotActive(this);
            
            var range = Parser.ParseInt32Range(this, 0);
            var duration = Parser.ParseInt32(this, 2);
            var isEventFrame = HasArg(EventFrameArg);
            var label = Parser.ParseNameArgument(this, LabelArg) ?? string.Empty;
            var repeat = Parser.ParseInt32Argument(this, RepeatArg, 1);
            var sound = Parser.ParseNameArgument(this, SoundArg) ?? string.Empty;
            var speedFactor = Parser.ParseFloatArgument(this, SpeedFactorArg, 1);
            var footstep = HasArg(FootstepArg);
            var gotoLabel = Parser.ParseNameArgument(this, GotoArg) ?? string.Empty;
            var subArea = Parser.ParseRectangleArgument(this, SubAreaArg);

            // Add frames
            var prefix = AnimationCommand.ActiveAnimationFramePrefix ?? AnimationCommand.ActiveAnimation.Name;

            for (int i = 0; i < repeat; i++)
            {
                for (var j = range.Minimum; j <= range.Maximum; j++)
                {
                    var imageName = prefix + j.ToString(CultureInfo.InvariantCulture).PadLeft(AnimationCommand.ZeroPaddingLength, '0');
                    AnimationCommand.ActiveAnimation.AddFrame(imageName, duration, isEventFrame, label, speedFactor, sound, subArea, footstep, gotoLabel);
                }
            }
        }
    }
}
