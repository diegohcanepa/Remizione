using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // EchoCommand
    // Arguments: {"Text"} [#lid:Integer]
    [ForceAwait]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class EchoCommand : LocalizableCommand
    {
        // Constructor
        public EchoCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, LiteralArg, LocalizationIdArg)
        {
            Parser.ParseQuotedString(this, 0);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            string text = GetDisplayText();

            if (Session is GameSession session)
                session.ShowEcho(text, null);
        }

        // TextClauseIndex
        protected override int TextClauseIndex => 0;

        #endregion

        // GetTextEmitterName
        protected override string GetTextEmitterName()
        {
            return "(UI Message)";
        }

        // IsAwaiting
        public override bool IsAwaiting => Game.SceneManager.CurrentScene is EchoScene;
    }
}
