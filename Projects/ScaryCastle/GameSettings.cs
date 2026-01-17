using System.Numerics;

namespace ScaryCastle
{
    /// <summary>
    /// GameSettings
    /// </summary>
    public static class GameSettings
    {
        // Build
        public const int Build = 105;

        // CameraFollowSpeed
        public const float CameraSmoothSpeed = 2;

        // ContentRootDirectory
        public const string ContentRootDirectory = "Content";

        // DefaultGlobalLightSize
        public static readonly Vector2 DefaultGlobalLightSize = new(1.5f, 2);

        // DefaultTransitionDuration
        public const int DefaultTransitionDuration = 500;

        // GameFolder
        public const string GameFolder = "Scary Castle";

        // LogFileName
        public const string LogFileName = "ErrorLog.txt";

        // CountdownMaximum
        public const int CountdownMaximum = 120_000;

        // MaxInventoryCapacity
        public const int MaxInventoryCapacity = 16;

        // PropRevealOpacity
        public const float PropRevealOpacity = .5f;

        // SteamAppID
        public const int SteamAppID = 480;

        // TimeCritical
        public const int TimeCritical = 10_000;

        // TimeWarning
        public const int TimeWarning = 30_000;

        // Title
        public const string Title = "Scary Castle";

        // UserSettingsFileName
        public const string UserSettingsFileName = "UserSettings.cfg";
    }
}