using Adberration;
using Adberration.Scripting;
using Microsoft.Xna.Framework;
using System.Windows.Forms.Design.Behavior;

namespace ScaryCastle.Scripting
{
    // AwaitPlayerApproachCommand
    // Syntax: [#behavior:ApproachBehavior] [#target:GameThing]
    [ForceAwait]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class AwaitPlayerApproachCommand : AwaitableCommand
    {
        private readonly ApproachBehavior behavior;
        private Actor? player;
        private int directionCooldown;
        private GameThing? target;

        // Constructor
        internal AwaitPlayerApproachCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0, BehaviorArg, TargetArg)
        {
            Parser.ParseEntityArgument<GameThing>(this, TargetArg, null);
            behavior = Parser.ParseEnumArgument<ApproachBehavior>(this, BehaviorArg, ApproachBehavior.InFront);
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

            var destination = target.GetApproachPosition(player, behavior);

            if (player.MoveTo(destination))
                directionCooldown = 3500;
            else
                directionCooldown = 0;
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            if (player == null)
                return;

            /*
            if (target != null)
            {
                if (behavior is ApproachBehavior.FaceToFace or ApproachBehavior.ClosestSide)
                    player.FaceTo(target);
            }
            */

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