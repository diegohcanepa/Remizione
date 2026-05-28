using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace ScaryCastle.Scripting
{
    // AwaitApproachCommand
    // Syntax: {Source:Actor} {Target:GameThing} [#behavior:ApproachBehavior]
    [ForceAwait]
    internal sealed class AwaitApproachCommand : AwaitableCommand
    {
        private readonly ApproachBehavior? behavior;
        private int directionCooldown;
        private Actor? source;
        private GameThing? target;

        // Constructor
        internal AwaitApproachCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, BehaviorArg)
        {
            AssertEntity<Actor>(0);
            AssertEntity<GameThing>(1);

            if (HasArg(BehaviorArg))
                behavior = Parser.ParseEnumArgument<ApproachBehavior>(this, BehaviorArg);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            source = AssertEntity<Actor>(0);
            if (source == null || !source.IsInCurrentRoom || !source.CanMove)
                return;

            target = AssertEntity<GameThing>(1);
            if (target == null || !target.IsInCurrentRoom)
                return;

            var destination = target.GetApproachPosition(source, behavior);

            directionCooldown = source.MoveTo(destination) ? 300 : 0;
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            if (source != null && target != null)
            {
                if (behavior is ApproachBehavior.FaceToFace or ApproachBehavior.ClosestSide)
                    source.FaceTo(target);

                source = null;
                target = null;
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (source != null && !source.IsMoving && directionCooldown > 0)
                directionCooldown -= gameTime.ElapsedGameTime.Milliseconds;
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting => source != null && (source.IsMoving || directionCooldown > 0);
    }
}