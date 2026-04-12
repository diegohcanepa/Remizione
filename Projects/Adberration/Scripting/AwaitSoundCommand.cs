using Engendro.Audio;

namespace Adberration.Scripting
{
    // AwaitSoundCommand
    // Arguments: {Name}
    [ForceAwait]
    internal sealed class AwaitSoundCommand : AwaitableCommand
    {
        private SoundInstance? soundInstance;

        // Constructor
        internal AwaitSoundCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            Parser.ParseName(this, 0);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            soundInstance = SoundInstance.FindRunningInstance(Body.Clauses[0]);
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting => soundInstance != null && soundInstance.RemainingTime > 0;
    }
}
