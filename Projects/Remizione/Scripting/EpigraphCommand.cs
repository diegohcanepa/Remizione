using Adberration.Scripting;

namespace Remizione.Scripting
{
    // EpigraphCommand
    // Arguments: {"Text"} [#lid:Integer] [#literal]
    [ForceAwait]
    internal sealed class EpigraphCommand : LocalizableCommand
    {
        // Constructor
        public EpigraphCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, LiteralArg, LocalizationIdArg)
        {
            Parser.ParseQuotedString(this, 0);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is GameSession session)
                session.Epigrah(GetDisplayText());
        }

        // TextClauseIndex
        protected override int TextClauseIndex => 0;

        #endregion

        // GetTextEmitterName
        protected override string GetTextEmitterName()
        {
            return "(Epigrah)";
        }

        // IsAwaiting
        public override bool IsAwaiting()
        {
            return Game.SceneManager.CurrentScene is EpigrahScene scene && !scene.CanClose;
        }
    }
}
