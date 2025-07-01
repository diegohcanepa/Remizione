using Engendro;

namespace Remizione
{
    /// <summary>
    /// GameSettings
    /// </summary>
    public static class GameSettings
    {
        // Build
        public const int Build = 105;

        // CameraFollowSpeed
        public const float CameraSmoothSpeed = .04f;

        // ContentRootDirectory
        public const string ContentRootDirectory = "Content";

        // CycleDuration
        public const int CycleDuration = 30_000;

        // DefaultTransitionDuration
        public const int DefaultTransitionDuration = 500;

        // GameFolder
        public const string GameFolder = "Remizione";

        // LogFileName
        public const string LogFileName = "ErrorLog.txt";

        // PropRevealOpacity
        public const float PropRevealOpacity = .5f;

        // RainDurationRange
        public static readonly Int32Range RainDurationRange = new(180000, 300000);

        // SteamAppID
        public const int SteamAppID = 480;

        // Title
        public const string Title = "Remizione";

        // UserSettingsFileName
        public const string UserSettingsFileName = "UserSettings.cfg";
    }
}