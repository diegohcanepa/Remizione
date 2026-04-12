using Microsoft.Xna.Framework;

namespace Adberration.Scripting
{
    // MoveCommand
    // Arguments: {Thing} to {Vector2} [#face:FacingDirection] [#follow] [#relative]
    internal sealed class MoveCommand : AwaitableCommand
    {
        private Thing? thing;
        private readonly FacingDirection direction;
        private int directionCooldown;

        // Constructor
        internal MoveCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3, FaceArg, FollowArg, RelativeArg)
        {
            AssertEntity<Thing>(0);
            AssertKeyword(1, "to");
            Parser.ParsePosition(this, 2, Vector2.Zero);
            direction = Parser.ParseEnumArgument(this, FaceArg, FacingDirection.Right);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            thing = AssertEntity<Thing>(0);
            if (thing == null || !thing.CanMove)
                return;

            var destination = Parser.ParsePosition(this, 2, thing.Position);

            // Relative
            if (HasArg(RelativeArg))
                destination = thing.Position + destination;

            // Follow
            if (HasArg(FollowArg))
                Session.Camera.Follow(thing);

            if (thing.MoveTo(destination))
                directionCooldown = 150;
            else
                directionCooldown = 0;
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            base.OnExecutionCompleted();

            if (thing == null)
                return;

            if (HasArg(FaceArg))
                thing.Direction = direction;

            thing = null;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (thing != null && !thing.IsMoving && directionCooldown > 0)
                directionCooldown -= gameTime.ElapsedGameTime.Milliseconds;
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting => thing != null && (thing.IsMoving || directionCooldown > 0);
    }
}
