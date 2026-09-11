using Adberration.Scripting;

namespace Remizione.Scripting
{
    // EchoCommand
    // Arguments: {"Text"} [#lid:Integer] [#literal]
    [ForceAwait]
    internal sealed class EchoCommand : LocalizableCommand
    {
        // Constructor
        public EchoCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, LiteralArg, LocalizationIdArg, VoiceArg)
        {
            Parser.ParseQuotedString(this, 0);
            Parser.ParseNameArgument(this, VoiceArg);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            var text = GetDisplayText();
            var soundName = Parser.ParseNameArgument(this, VoiceArg);

            if (Session is GameSession session)
            {
                session.ShowEcho(text, soundName);
                //session.HUD.Narrator.Play(text, Parser.ParseNameArgument(this, VoiceArg));
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
            //return false;
            return Game.SceneManager.CurrentScene is EchoScene scene && !scene.CanClose;
        }
    }
}
