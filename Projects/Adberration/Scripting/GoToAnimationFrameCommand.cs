namespace Adberration.Scripting
{
    // GoToAnimationFrameCommand
    // Arguments: {Entity} {FrameIndex:Integer}
    internal sealed class GoToAnimationFrameCommand : NonAwaitableCommand
    {
        // Constructor
        internal GoToAnimationFrameCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2)
        {
            AssertEntity<Entity>(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (AssertEntity<Entity>(0) is Entity entity)
            {
                var index = Parser.ParseInt32(this, 1);
                entity.AnimationPlayer.GoTo(index);
            }
        }
    }
}
