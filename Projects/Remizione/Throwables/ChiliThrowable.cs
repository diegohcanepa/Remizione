using Engendro.Audio;

namespace Remizione
{
    /// <summary>
    /// ChiliThrowable
    /// </summary>
    public sealed class ChiliThrowable : Throwable
    {
        // Constructor
        public ChiliThrowable(GameSession session)
            : base(session, 60, 400, true)
        {
            const string basePrefix = "ThrowableChili";

            ImpactSound = Sound.Find(basePrefix);
            Shadow.Image = Atlas?.GetImage(basePrefix + "Shadow");

            var animation = AddAnimation(AnimationNames.Default);
            animation.AddFrame(basePrefix, 1000);
        }
    }
}
