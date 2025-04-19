namespace EngendroAdventure.Scripting
{
    // StopAnimationCommand
    // Arguments: {Entity}
    internal sealed class StopAnimationCommand : NonAwaitableCommand
    {
        // Constructor
        internal StopAnimationCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, RandomFrameArg, ReverseArg, LoopedArg)
        {
            AssertEntity<Entity>(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            AssertEntity<Entity>(0)?.AnimationPlayer.Stop();
        }
    }
}
