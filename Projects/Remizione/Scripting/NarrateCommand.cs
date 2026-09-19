using Adberration.Scripting;

namespace Remizione.Scripting
{
    // NarrateCommand
    // Arguments: {"Text"} {SoundName} [#lid:Integer]
    [ForceAwait]
    internal sealed class NarrateCommand : LocalizableCommand
    {
        // Constructor
        public NarrateCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, LocalizationIdArg)
        {
            Parser.ParseQuotedString(this, 0);
            Parser.ParseName(this, 1);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            var text = GetDisplayText();
            var soundName = Parser.ParseName(this, 1);

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
