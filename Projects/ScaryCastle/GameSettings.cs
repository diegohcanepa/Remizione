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

        // ConditionCooldown
        public const int ConditionCooldown = 10000;

        // ContentRootDirectory
        public const string ContentRootDirectory = "Content";

        // CountdownDuration
        public const int CountdownDuration = 66;

        // CountdownCritical
        public const int CountdownCritical = 9;

        // DeathCoooldown
        public const int DeathCoooldown = 15000;

        // DefaultGlobalLightSize
        public static readonly Vector2 DefaultGlobalLightSize = new(1.5f, 2);

        // DefaultTransitionDuration
        public const int DefaultTransitionDuration = 500;

        // GameFolder
        public const string GameFolder = "Scary Castle";

        // InitialBibleCapacity
        public const int InitialBibleCapacity = 20;

        // LogFileName
        public const string LogFileName = "ErrorLog.txt";

        // SteamAppID
        public const int SteamAppID = 480;

        // Title
        public const string Title = "Scary Castle";

        // UserSettingsFileName
        public const string UserSettingsFileName = "UserSettings.cfg";
    }
}