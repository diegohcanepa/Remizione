using Engendro.Audio;

namespace EngendroAdventure.Scripting
{
    // StopMusicCommand
    // Arguments: [#clear-tag] [#fade:Integer]
    internal sealed class StopMusicCommand : NonAwaitableCommand
    {
        // Constructor
        internal StopMusicCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0, ClearTagArg, FadeArg)
        {
            Parser.ParseInt32Argument(this, FadeArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var fadeDuration = Parser.ParseInt32Argument(this, FadeArg, 0);

            if (HasArg(ClearTagArg))
            {
                AudioManager.Music.PlayingTag = string.Empty;
            }

            AudioManager.Music.Stop(fadeDuration);
        }
    }
}
