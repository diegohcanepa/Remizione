using Engendro.Audio;

namespace Adberration.Scripting
{
    // FadeSoundCommand
    // Arguments: {SoundName} to {Volume:Float} duration {Integer} [#relative]
    internal sealed class FadeSoundCommand : NonAwaitableCommand
    {
        // Constructor
        internal FadeSoundCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 5, RelativeArg)
        {
            Parser.ParseName(this, 0);
            AssertKeyword(1, "to");
            Parser.ParseFloat(this, 2);
            AssertKeyword(3, "duration");
            Parser.ParseInt32(this, 4);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (SoundInstance.GetRunningInstance(Parser.ParseName(this, 0)) is SoundInstance soundInstance)
            {
                var finalVolume = Parser.ParseFloat(this, 2);

                if (HasArg(RelativeArg))
                {
                    finalVolume = soundInstance.Volume.Current + finalVolume;
                }

                var duration = Parser.ParseInt32(this, 4);
                soundInstance.Volume.FadeTo(duration, finalVolume);
            }
        }
    }
}
