using Engendro.Audio;

namespace EngendroAdventure.Scripting
{
    // ChangeSoundSettingsCommand
    // Arguments: {SoundName} {#emitter:Thing} [#pan:Float] [#pitch:Float] [#volume:Float]
    internal sealed class ChangeSoundSettingsCommand : NonAwaitableCommand
    {
        // Constructor
        internal ChangeSoundSettingsCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, EmitterArg, PanArg, PitchArg, VolumeArg)
        {
            Parser.ParseName(this, 0);
            Parser.ParseEntityArgument<Thing>(this, EmitterArg, null);
            Parser.ParseFloatArgument(this, PanArg);
            Parser.ParseFloatArgument(this, PitchArg);
            Parser.ParseFloatArgument(this, VolumeArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (SoundInstance.GetRunningInstance(Parser.ParseName(this, 0)) is SoundInstance soundInstance)
            {
                // Emitter
                if (Body.Args.GetArg(EmitterArg)?.Value is string emitter)
                {
                    soundInstance.Emitter = Parser.ParseEntity<Thing>(this, emitter);
                }

                soundInstance.Pan = Parser.ParseFloatArgument(this, PanArg, soundInstance.Pan);
                soundInstance.Pitch = Parser.ParseFloatArgument(this, PitchArg, soundInstance.Pitch);
                soundInstance.Volume.Master = Parser.ParseFloatArgument(this, VolumeArg, soundInstance.Volume.Master);
                soundInstance.Volume.Reset();
            }
        }
    }
}
