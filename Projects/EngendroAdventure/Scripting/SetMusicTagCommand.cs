using Engendro.Audio;

namespace EngendroAdventure.Scripting
{
    // SetMusicTagCommand
    // Arguments: {Tag}
    internal sealed class SetMusicTagCommand : NonAwaitableCommand
    {
        // Constructor
        internal SetMusicTagCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            var tag = Parser.ParseName(this, 0);
            if (tag != ScriptSyntax.NullValue && !Sound.AvailableTags.Contains(tag))
            {
                throw ScriptExceptionBuilder.UndeclaredTag(this, tag);
            }
        }

        // OnExecute
        protected override void OnExecute()
        {
            var tag = Body.Clauses[0];
            if (tag == ScriptSyntax.NullValue)
            {
                tag = string.Empty;
            }

            AudioManager.Music.CurrentTag = tag;
        }
    }
}
