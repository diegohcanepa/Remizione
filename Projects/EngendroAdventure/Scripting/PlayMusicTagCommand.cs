using Engendro.Audio;

namespace EngendroAdventure.Scripting
{
    // PlayMusicTagCommand
    // Arguments: {Tag} [#fade:Integer] [#scope:MusicTagScope]
    internal sealed class PlayMusicTagCommand : NonAwaitableCommand
    {
        // Constructor
        internal PlayMusicTagCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, FadeArg, ScopeArg)
        {
            var tag = Parser.ParseName(this, 0);
            Parser.ParseInt32Argument(this, FadeArg);
            Parser.ParseEnumArgument<MusicTagScope>(this, ScopeArg);

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

            var fade = Parser.ParseInt32Argument(this, FadeArg);
            var scope = Parser.ParseEnumArgument(this, ScopeArg, MusicTagScope.Session);

            Session.PlayMusicTag(tag, scope, fade);
        }
    }
}
