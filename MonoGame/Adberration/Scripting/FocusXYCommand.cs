using Engendro;

namespace Adberration.Scripting
{
    // FocusXYCommand
    // Arguments: {Vector2} [#duration:Integer] [#tween:TweenStyle]
    internal sealed class FocusXYCommand : AwaitableCommand
    {
        // Constructor
        internal FocusXYCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, DurationArg, RelativeArg, TweenArg)
        {
            Parser.ParseVector2(this, 0);
            Parser.ParseInt32Argument(this, DurationArg);
            Parser.ParseEnumArgument<TweenStyle>(this, TweenArg);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            var destination = Parser.ParseVector2(this, 0, Session.Camera.Position);

            if (HasArg(RelativeArg))
                destination = Session.Camera.Position + destination;

            var duration = Parser.ParseInt32Argument(this, DurationArg);
            var tweenStyle = Parser.ParseEnumArgument(this, TweenArg, TweenStyle.QuadraticInOut);

            if (duration > 0)
            {
                Session.Camera.MoveTo(tweenStyle, destination, duration);
            }
            else
            {
                Session.Camera.Position = destination;
            }
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting()
        {
            return Session.Camera.IsMoving;
        }
    }
}
