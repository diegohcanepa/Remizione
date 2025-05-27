using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // BeginRainCommand
    // Arguments: [#duration:Integer]
    internal sealed class BeginRainCommand : NonAwaitableCommand
    {
        // Constructor
        internal BeginRainCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0, DurationArg)
        {
            Parser.ParseInt32Argument(this, DurationArg);
            Parser.ParseInt32Argument(this, FadeArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is GameSession session)
            {
                var duration = Parser.ParseInt32Argument(this, DurationArg, GameSettings.RainDurationRange.Random());
                session.Environment.Rain.Begin(duration, false);
            }
        }
    }
}
