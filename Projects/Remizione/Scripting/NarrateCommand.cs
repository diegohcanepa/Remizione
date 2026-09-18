using Adberration.Scripting;

namespace Remizione.Scripting
{
    // NarrateCommand
    // Arguments: {"Text"} [#lid:Integer] [#literal] [#speaker:GameThing]
    [ForceAwait]
    internal sealed class NarrateCommand : LocalizableCommand
    {
        // Constructor
        public NarrateCommand(Script script, string source, StatementBody body)
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
                session.Narrate(text, soundName);
        }

        // TextClauseIndex
        protected override int TextClauseIndex => 0;

        #endregion

        // GetTextEmitterName
        protected override string GetTextEmitterName()
        {
            return "(Narrate)";
        }

        // IsAwaiting
        public override bool IsAwaiting()
        {
            return Game.SceneManager.CurrentScene is NarrationScene scene && !scene.CanClose;
        }
    }
}
