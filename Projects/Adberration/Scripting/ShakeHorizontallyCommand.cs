using Engendro;

namespace Adberration.Scripting
{
    // ShakeHorizontallyCommand
    // Arguments: {Float} duration {Integer} bounces {Integer} [#tween:TweenStyle]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class ShakeHorizontallyCommand : AwaitableCommand
    {
        // Constructor
        internal ShakeHorizontallyCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 5, TweenArg)
        {
            Parser.ParseFloat(this, 0);
            AssertKeyword(1, "duration");
            Parser.ParseInt32(this, 2);
            AssertKeyword(3, "bounces");
            Parser.ParseInt32(this, 4);
            Parser.ParseEnumArgument<TweenStyle>(this, TweenArg);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            var intensity = Parser.ParseFloat(this, 0);
            var duration = Parser.ParseInt32(this, 2);
            var bounces = Parser.ParseInt32(this, 4);
            var tweenStyle = Parser.ParseEnumArgument(this, TweenArg, TweenStyle.Linear);

            Session.Camera.ShakeHorizontally(tweenStyle, intensity, duration, bounces);
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting => Session.Camera.ShakeState == CameraShakeState.X;

    }
}
