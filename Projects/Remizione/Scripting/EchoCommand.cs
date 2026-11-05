using Adberration.Scripting;
using Engendro;

namespace Remizione.Scripting
{
    // EchoCommand
    // Arguments: {"Text"} [#lid:Integer] [#literal] [#sound:Name]
    [ForceAwait]
    internal sealed class EchoCommand : LocalizableCommand
    {
        // Constructor
        public EchoCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, LiteralArg, LocalizationIdArg, SoundArg)
        {
            Parser.ParseQuotedString(this, 0);
            Parser.ParseNameArgument(this, SoundArg);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is GameSession session)
            {
                string? title = null;
                if (session.InteractionData.Target is { } target)
                {
                    title = target.Label;
                    if (target.GraceCost > 0)
                        title += $" ({target.GraceCost}{TextRepository.GetValue("Misc.GraceSign")})";
                }

                session.Overlays.ShowEcho(GetDisplayText(), title);
            }
        }

        // TextClauseIndex
        protected override int TextClauseIndex => 0;

        #endregion

        // GetTextEmitterName
        protected override string GetTextEmitterName()
        {
            return "(Echo)";
        }

        // IsAwaiting
        public override bool IsAwaiting()
        {
            return Game.SceneManager.CurrentScene is EchoScene scene && !scene.CanClose;
        }
    }
}
