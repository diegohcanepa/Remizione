using Engendro;
using System;

namespace Adberration.Scripting
{
    // FocusCommand
    // Arguments: {Thing} [#duration:Integer] [#follow] [#tween:TweenStyle]
    internal sealed class FocusCommand : AwaitableCommand
    {
        private Thing? thing;

        // Constructor
        internal FocusCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, DurationArg, FollowArg, TweenArg)
        {
            AssertEntity<Thing>(0);
            Parser.ParseInt32Argument(this, DurationArg);
            Parser.ParseEnumArgument<TweenStyle>(this, TweenArg);
        }

        #region Protected members

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            base.OnExecutionCompleted();

            if (thing != null && HasArg(FollowArg))
            {
                Session.Camera.Follow(thing);
                thing = null;
            }
        }

        // OnExecute
        protected override void OnExecute()
        {
            thing = AssertEntity<Thing>(0);
            if (thing == null)
                return;

            if (HasArg(FollowArg))
                Session.Camera.StopFollowing();

            // Duration
            var duration = Parser.ParseInt32Argument(this, DurationArg);

            // TweenStyle
            var tweenStyle = Parser.ParseEnumArgument(this, TweenArg, TweenStyle.QuadraticInOut);

            var position = thing.Position;

            if ((Math.Abs(position.X - Session.Camera.Position.X) <= 0) && (Math.Abs(position.Y - Session.Camera.Position.Y) <= 0))
                duration = 0;

            if (duration > 0)
            {
                Session.Camera.MoveTo(tweenStyle, position, duration);
            }
            else
            {
                Session.Camera.StopMoving();
                Session.Camera.Position = position;
            }
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting => Session.Camera.IsMoving;
    }
}
