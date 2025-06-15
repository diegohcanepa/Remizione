using Engendro.Audio;

namespace Remizione
{
    /// <summary>
    /// DuckThrowable
    /// </summary>
    public sealed class DuckThrowable : Throwable
    {
        // Constructor
        public DuckThrowable(GameSession session)
            : base(session, 40, 300, true)
        {
            const string basePrefix = "ThrowableDuck";

            ImpactSound = Sound.Find(basePrefix);
            Shadow.Image = Atlas?.GetImage(basePrefix + "Shadow");

            var animation = AddAnimation(AnimationNames.Default);
            animation.AddFrame(basePrefix, 1000);
        }
    }
}
