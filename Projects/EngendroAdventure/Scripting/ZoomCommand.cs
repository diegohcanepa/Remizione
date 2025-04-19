using Engendro;

namespace EngendroAdventure.Scripting
{
    // ZoomCommand
    // Arguments: {zoom:Float} [#duration:Integer] [#bounces:Integer] [#relative] [#tween:TweenStyle]
    internal sealed class ZoomCommand : AwaitableCommand
    {
        // Constructor
        internal ZoomCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, BouncesArg, DurationArg, RelativeArg, TweenArg)
        {
            Parser.ParseFloat(this, 0);
            Parser.ParseInt32Argument(this, BouncesArg);
            Parser.ParseInt32Argument(this, DurationArg);
            Parser.ParseEnumArgument<TweenStyle>(this, TweenArg);
        }

        // IsAwaiting
        public override bool IsAwaiting => Session.Camera.ZoomState != ZoomState.None;

        // OnExecute
        protected override void OnExecute()
        {
            var zoom = Parser.ParseFloat(this, 0);
            var bounces = Parser.ParseInt32Argument(this, BouncesArg, 0);
            var duration = Parser.ParseInt32Argument(this, DurationArg);
            var tweenStyle = Parser.ParseEnumArgument(this, TweenArg, TweenStyle.Linear);

            if (HasArg(RelativeArg))
            {
                zoom += Session.Camera.Zoom;
            }

            if (duration > 0)
            {
                Session.Camera.ZoomTo(tweenStyle, zoom, duration, bounces);
            }
            else
            {
                Session.Camera.Zoom = zoom;
            }
        }
    }
}
