using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // TerminateDialogBlockCommand
    internal sealed class TerminateDialogBlockCommand : NonAwaitableCommand
    {
        // Constructor
        internal TerminateDialogBlockCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            var scenes = Session.Game.SceneManager.GetScenes();
            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i] is DialogBlockScene dialogBlockScene)
                {
                    dialogBlockScene.Terminate();
                    break;
                }
            }
        }
    }
}
