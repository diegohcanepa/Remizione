namespace Adberration.Scripting
{
    // PopSceneCommand
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class PopSceneCommand : NonAwaitableCommand
    {
        // Constructor
        internal PopSceneCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            Game.SceneManager.Pop();
        }
    }
}
