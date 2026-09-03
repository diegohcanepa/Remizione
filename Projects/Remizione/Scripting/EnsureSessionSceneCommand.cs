using Adberration.Scripting;

namespace Remizione.Scripting
{
    // EnsureSessionSceneCommand
    internal sealed class EnsureSessionSceneCommand : NonAwaitableCommand
    {
        // Constructor
        internal EnsureSessionSceneCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            Game.SceneManager.PopUntil(Session);
        }
    }
}
