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
            : base(script, source, body, 1, LiteralArg, LocalizationIdArg, SpeakerArg, SoundArg)
        {
            Parser.ParseQuotedString(this, 0);
            Parser.ParseEntityArgument<GameThing>(this, SpeakerArg, null);
            Parser.ParseNameArgument(this, SoundArg);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            var speaker = Parser.ParseEntityArgument<GameThing>(this, SpeakerArg, null);
            var text = GetDisplayText();
            var soundName = Parser.ParseNameArgument(this, SoundArg);

            if (Session is GameSession session)
                session.Echo(text, speaker, soundName);
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
