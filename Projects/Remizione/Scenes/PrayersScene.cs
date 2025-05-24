namespace Remizione
{
    /// <summary>
    /// PrayersScene
    /// </summary>
    public sealed class PrayersScene : ItemContainerScene
    {
        // Constructor
        public PrayersScene(RemizioneGame game)
            : base(game, ItemContainerCategory.Prayers, false, true)
        {
        }
    }
}
