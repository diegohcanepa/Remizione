using Adberration.Scripting;
using Remizione.Menus;

namespace Remizione.Scripting
{
    // ExitSessionCommand
    internal sealed class ExitSessionCommand : NonAwaitableCommand
    {
        // Constructor
        internal ExitSessionCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session.Game is RemizioneGame game)
            {
                game.DisposeSession(new TitleMenuScene(game));
            }
        }
    }
}
