namespace Remizione
{
    /// <summary>
    /// GameSettings
    /// </summary>
    public static class GameSettings
    {
        // Build
        public const int Build = 1255;

        // CameraFollowSpeed
        public const float CameraSmoothSpeed = 2;

        // ContentRootDirectory
        public const string ContentRootDirectory = "Content";

        // DarknessMissChancePenalty
        public const float DarknessMissChancePenalty = .2f;

        // DefaultTransitionDuration
        public const int DefaultTransitionDuration = 500;

        // GameFolder
        public const string GameFolder = "Remizione";

        // HeavyMoveThreshold
        public const int HeavyMoveThreshold = 150;

        // LogFileName
        public const string LogFileName = "ErrorLog.txt";

        // MaxItemAmount
        public const int MaxItemAmount = 5;

        // PlayerDefaults
        internal static class PlayerDefaults
        {
            public const int InventoryCapacity = 5;
            public const int MaxEnergy = 4;
            public const int MaxHP = 4;
            public const int MaxStamina = 5;
        }

        // StaminaRechargeMoveThreshold
        public const int StaminaRechargeMoveThreshold = 100;

        // SteamAppID
        public const int SteamAppID = 480;

        // Title
        public const string Title = "Remizione";

        // UserSettingsFileName
        public const string UserSettingsFileName = "UserSettings.cfg";

        // WalkThreshold
        public const int WalkThreshold = 50;
    }
}