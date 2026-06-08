using Engendro;

namespace Adberration.Scripting
{
    // ShakeCommand
    // Arguments: {Vector2} duration {Integer} bounces {Integer} [#tween:TweenStyle]
    internal sealed class ShakeCommand : AwaitableCommand
    {
        // Constructor
        internal ShakeCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 5, TweenArg)
        {
            Parser.ParseVector2(this, 0);
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
            var intensity = Parser.ParseVector2(this, 0);
            var duration = Parser.ParseInt32(this, 2);
            var bounces = Parser.ParseInt32(this, 4);
            var tweenStyle = Parser.ParseEnumArgument(this, TweenArg, TweenStyle.Linear);

            Session.Camera.Shake(tweenStyle, intensity, duration, bounces);
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting() => Session.Camera.ShakeState != CameraShakeState.None;

    }
}
