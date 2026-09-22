using Adberration.Scripting;

namespace Remizione.Scripting
{
    // EchoCommand
    // Arguments: {"Text"} [#lid:Integer] [#literal] [#speaker:GameThing]
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
                session.Echo(GetDisplayText());
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
