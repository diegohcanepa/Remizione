using Engendro.Audio;

namespace Adberration.Scripting
{
    // StopSoundCommand
    // Arguments: {Name} [#fade:Integer]
    internal sealed class StopSoundCommand : NonAwaitableCommand
    {
        // Constructor
        internal StopSoundCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, FadeArg)
        {
            Parser.ParseName(this, 0);
            Parser.ParseInt32Argument(this, FadeArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var name = Body.Clauses[0];
            var fade = Parser.ParseInt32Argument(this, FadeArg, 0);

            if (Sound.Find(name) is Sound sound)
            {
                sound.Stop(fade);
            }
        }
    }
}
