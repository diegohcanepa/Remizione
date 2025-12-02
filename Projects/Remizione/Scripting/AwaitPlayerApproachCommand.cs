using Adberration;
using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace Remizione.Scripting
{
    // AwaitPlayerApproachCommand
    // Syntax: [#face:] [#fast] [#target:CommonThing]
    [ForceAwait]
    internal sealed class AwaitPlayerApproachCommand : AwaitableCommand
    {
        private Actor? player;
        private int directionCooldown;
        private GameThing? target;

        // Constructor
        internal AwaitPlayerApproachCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0, FaceArg, FastArg, TargetArg)
        {
            Parser.ParseEntityArgument<GameThing>(this, TargetArg, null);
            Parser.ParseEnumArgument<FacingDirection>(this, FaceArg);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            player = session.Player;
            if (player == null || !player.CanMove)
                return;

            if (HasArg(TargetArg))
                target = Parser.ParseEntityArgument<GameThing>(this, TargetArg, null);
            else
                target = session.OutcomeTarget as GameThing;

            if (target == null)
                return;

            var destination = target.GetApproachPosition(player, true);

            if (player.MoveTo(destination))
                directionCooldown = 150;
            else
                directionCooldown = 0;
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            base.OnExecutionCompleted();

            if (player == null)
                return;

            if (HasArg(FaceArg))
                player.Direction = Parser.ParseEnumArgument<FacingDirection>(this, FaceArg);

            if (target != null)
                player.FaceTo(target);

            player = null;
            target = null;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (player != null && !player.IsMoving && directionCooldown > 0)
                directionCooldown -= gameTime.ElapsedGameTime.Milliseconds;
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting => player != null && (player.IsMoving || directionCooldown > 0);
    }
}