using Engendro;

namespace Adberration
{
    /// <summary>
    /// AdventureGame
    /// </summary>
    public abstract class AdventureGame : EngendroGame
    {
        // Constructor
        protected AdventureGame(string title, string contentRootDirectory, int nativeWidth, int nativeHeight, RunningPlatform platform, PlatformBridge platformBridge)
            : base(title, contentRootDirectory, nativeWidth, nativeHeight, platform)
        {
            this.PlatformBridge = platformBridge;
        }

        // PlatformBridge
        public PlatformBridge PlatformBridge { get; }
    }
}
