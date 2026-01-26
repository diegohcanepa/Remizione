using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace ScaryCastle.Scripting
{
    // AwaitPlayerApproachCommand
    // Syntax: {GameThing} [#behavior:ApproachBehavior]
    [ForceAwait]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class AwaitPlayerApproachCommand : AwaitableCommand
    {
        private readonly ApproachBehavior? behavior;
        private Actor? player;
        private int directionCooldown;
        private GameThing? target;

        // Constructor
        internal AwaitPlayerApproachCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, BehaviorArg)
        {
            AssertEntity<GameThing>(0);

            if (HasArg(BehaviorArg))
                behavior = Parser.ParseEnumArgument<ApproachBehavior>(this, BehaviorArg);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            target = AssertEntity<GameThing>(0);
            if (target == null || !target.IsInCurrentRoom)
                return;

            player = session.Player;
            if (player == null || !player.CanMove)
                return;

            var destination = target.GetApproachPosition(player, behavior);

            directionCooldown = player.MoveTo(destination) ? 300 : 0;
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            if (player != null && target != null)
            {
                if (behavior is ApproachBehavior.FaceToFace or ApproachBehavior.ClosestSide)
                    player.FaceTo(target);

                player = null;
                target = null;
            }
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