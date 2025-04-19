using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework.Audio;

namespace EngendroAdventure.Scripting
{
    // PlaySoundCommand
    // Arguments: {Name} [#delay:Integer] [#emitter:Entity] [#fade:Integer] [#index:Integer] [#scope:LifetimeScope] [#looped] [#no-caption] [#pan:Float] [#pause-aware:Boolean] [#pitch:Float] [#transition-aware:Boolean] [#volume:RatioRange]
    internal sealed class PlaySoundCommand : AwaitableCommand
    {
        private SoundInstance? instance;

        // Constructor
        internal PlaySoundCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, DelayArg, EmitterArg, FadeArg, IndexArg, ScopeArg, LoopedArg, NoCaptionArg, PanArg, PauseAwareArg, PitchArg, TransitionAwareArg, VolumeArg)
        {
            var name = Parser.ParseName(this, 0);

            if (Sound.Find(name) is Sound sound)
            {
                if (sound.Category == AudioManager.MusicCategory)
                    throw new ScriptException(this, "Use 'play-music' for music assets.");
            }
            else
                throw ScriptExceptionBuilder.AssetNotFound(this, name);

            Parser.ParseInt32Argument(this, DelayArg);
            Parser.ParseInt32Argument(this, FadeArg);
            Parser.ParseInt32Argument(this, IndexArg);
            Parser.ParseEnumArgument<LifetimeScope>(this, ScopeArg);
            Parser.ParseFloatArgument(this, PanArg);
            Parser.ParseFloatArgument(this, PitchArg);
            Parser.ParseFloatRangeArgument(this, VolumeArg);
            Parser.ParseEntityArgument<Entity>(this, EmitterArg, null);
            Parser.ParseBooleanArgument(this, PauseAwareArg);
            Parser.ParseBooleanArgument(this, TransitionAwareArg);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            // Name
            var name = Body.Clauses[0];
            var index = Parser.ParseInt32Argument(this, IndexArg, -1);

            // Get sound instance
            instance = Sound.Find(name)?.PopInstance(index);
            if (instance == null)
                return;

            var scope = Parser.ParseEnumArgument(this, ScopeArg, LifetimeScope.Room);

            instance.Emitter = Parser.ParseEntityArgument<Entity>(this, EmitterArg, null);
            instance.IsLooped = HasArg(LoopedArg);
            instance.Pan = Parser.ParseFloatArgument(this, PanArg, instance.Pan);
            instance.Pitch = Parser.ParseFloatArgument(this, PitchArg, instance.Pitch);

            if (HasArg(PauseAwareArg))
                instance.PauseAware = Parser.ParseBooleanArgument(this, PauseAwareArg);

            if (HasArg(TransitionAwareArg))
                instance.TransitionAware = Parser.ParseBooleanArgument(this, TransitionAwareArg);

            var volume = Parser.ParseFloatRangeArgument(this, VolumeArg, new FloatRange(instance.Volume.Master));

            instance.Volume.Master = volume.Random();

            // Fade
            if (HasArg(FadeArg))
            {
                var duration = Parser.ParseInt32Argument(this, FadeArg);
                instance.Volume.FadeIn(duration);
            }

            var delay = Parser.ParseInt32Argument(this, DelayArg);
            if (delay > 0)
                instance.PlayDelayed(delay);
            else
                instance.Play();

            if (instance.Sound.Caption.Length > 0 && !HasArg(NoCaptionArg))
                Session.ShowSoundCaption(instance);

            if (scope == LifetimeScope.Room)
                Session.Room?.RegisterSound(instance);

            // Await
            if (!Body.Await)
                instance = null;
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting => instance != null && instance.State == SoundState.Playing;
    }
}
