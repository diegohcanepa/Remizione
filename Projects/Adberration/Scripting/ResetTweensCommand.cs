namespace Adberration.Scripting
{
    // ResetTweensCommand
    // Syntax: {Entity}
    internal sealed class ResetTweensCommand : NonAwaitableCommand
    {
        // Constructor
        internal ResetTweensCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            AssertEntity<Entity>(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            AssertEntity<Entity>(0)?.Tweens.Reset();
        }
    }
}
