using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // EchoCommand
    // Arguments: {"Text"} [#lid:Integer]
    [ForceAwait]
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
                session.ShowEcho(text);
        }

        // TextClauseIndex
        protected override int TextClauseIndex => 0;

        #endregion

        // GetTextEmitterName
        protected override string GetTextEmitterName() => "(UI Message)";
    }
}
