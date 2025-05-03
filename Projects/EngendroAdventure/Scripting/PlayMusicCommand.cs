using Engendro.Audio;

namespace EngendroAdventure.Scripting
{
    // PlayMusicCommand
    // Arguments: {SoundName} [#clear-tag] [#fade:Integer] [#looped] [#pitch:Float] [#volume:Float]
    internal sealed class PlayMusicCommand : NonAwaitableCommand
    {
        // Constructor
        internal PlayMusicCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, ClearTagArg, FadeArg, LoopedArg, PitchArg, VolumeArg)
        {
            var name = Parser.ParseName(this, 0);

            if (Sound.Find(name) is Sound sound)
            {
                if (sound.Category != AudioManager.MusicCategory)
                {
                    throw new ScriptException(this, "The specified sound is not a music asset.");
                }
            }
            else
            {
                throw ScriptExceptionBuilder.AssetNotFound(this, name);
            }

            Parser.ParseInt32Argument(this, FadeArg);
            Parser.ParseFloatArgument(this, PitchArg);
            Parser.ParseFloatArgument(this, VolumeArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var name = Body.Clauses[0];
            var isLooped = HasArg(LoopedArg);
            var fade = Parser.ParseInt32Argument(this, FadeArg);
            var pitch = Parser.ParseFloatArgument(this, PitchArg, 0);
            var volume = Parser.ParseFloatArgument(this, VolumeArg, 1);

            AudioManager.Music.Play(name, isLooped, fade, volume, pitch);

            if (HasArg(ClearTagArg))
            {
                AudioManager.Music.CurrentTag = string.Empty;
            }
        }
    }
}
