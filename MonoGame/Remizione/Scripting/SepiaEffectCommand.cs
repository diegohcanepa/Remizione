using Adberration.Scripting;

namespace Remizione.Scripting
{
    // SepiaEffectCommand
    // Arguments: {Intensity:Float} [#bounces:Integer] [#duration:Integer]
    internal sealed class SepiaEffectCommand : NonAwaitableCommand
    {
        // Constructor
        internal SepiaEffectCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, BouncesArg, DurationArg)
        {
            Parser.ParseFloat(this, 0);
            Parser.ParseInt32Argument(this, BouncesArg, 0);
            Parser.ParseInt32Argument(this, DurationArg, 0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is GameSession session)
            {
                var intensity = Parser.ParseFloat(this, 0);
                var duration = Parser.ParseInt32Argument(this, DurationArg, 0);
                var bounces = Parser.ParseInt32Argument(this, BouncesArg, 0);
                session.SepiaEffect.Start(intensity, duration, bounces);
            }
        }
    }
}
