using Engendro;

namespace Adberration.Scripting
{
    // GoToFramePositionCommand
    // Arguments: {Entity} {FramePosition}
    internal sealed class GoToFramePositionCommand : NonAwaitableCommand
    {
        // Constructor
        internal GoToFramePositionCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2)
        {
            AssertEntity<Entity>(0);
            Parser.ParseEnum<FramePosition>(this, 1);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (AssertEntity<Entity>(0) is Entity entity)
            {
                var framePosition = Parser.ParseEnum<FramePosition>(this, 1);
                entity.AnimationPlayer.GoTo(framePosition);
            }
        }
    }
}
