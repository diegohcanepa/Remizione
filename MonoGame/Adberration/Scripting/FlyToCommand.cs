using Engendro;

namespace Adberration.Scripting
{
    // FlyToCommand
    // Arguments: {Thing} {zoom:Float} {duration:Integer} [#relative] [#tween:TweenStyle]
    internal sealed class FlyToCommand : AwaitableCommand
    {
        // Constructor
        internal FlyToCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3, RelativePositionArg, RelativeZoomArg, TweenArg)
        {
            AssertEntity<Thing>(0);
            Parser.ParseFloat(this, 1);
            Parser.ParseInt32(this, 2);
            Parser.ParseEnumArgument<TweenStyle>(this, TweenArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (AssertEntity<Thing>(0) is not Thing thing)
                return;

            var destination = thing.Position;
            var zoom = Parser.ParseFloat(this, 1);
            var duration = Parser.ParseInt32(this, 2);
            var tweenStyle = Parser.ParseEnumArgument(this, TweenArg, TweenStyle.Linear);

            if (duration <= 0)
                return;

            if (HasArg(RelativeZoomArg))
                zoom += Session.Camera.Zoom;

            if (HasArg(RelativePositionArg))
                destination += Session.Camera.Position;

            Session.Camera.FlyTo(tweenStyle, destination, zoom, duration);
        }

        // IsAwaiting
        public override bool IsAwaiting()
        {
            return Session.Camera.IsFlying;
        }
    }
}
