using Engendro.Audio;
using Microsoft.Xna.Framework.Audio;

namespace Adberration.Scripting
{
    // AwaitMusicCommand
    [ForceAwait]
    internal sealed class AwaitMusicCommand : AwaitableCommand
    {
        // Constructor
        internal AwaitMusicCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // IsAwaiting
        public override bool IsAwaiting() => AudioManager.Music.State == SoundState.Playing && !AudioManager.Music.IsLooped;
    }
}
