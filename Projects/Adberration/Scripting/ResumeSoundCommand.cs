using Engendro.Audio;

namespace Adberration.Scripting
{
    // ResumeSoundCommand
    // Arguments: {Name}
    internal sealed class ResumeSoundCommand : NonAwaitableCommand
    {
        // Constructor
        internal ResumeSoundCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            Parser.ParseName(this, 0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var name = Body.Clauses[0];
            if (Sound.Find(name) is Sound sound)
                sound.Resume();
        }
    }
}
