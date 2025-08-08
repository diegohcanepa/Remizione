using Engendro;
using Adberration;
using Steamworks;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// WindowsBridge 
    /// </summary>
    public sealed class WindowsBridge : PlatformBridge
    {
        // Constructor
        public WindowsBridge(PlatformFileSystem fileSystemBridge)
            : base(fileSystemBridge)
        {
            AllowWindowedMode = true;
        }

        #region Protected members

        // OnSetAchievement
        protected override bool OnSetAchievement(Achievement achievement)
        {
            // Is Steam Loaded? if no, can't get stats, done
            if (!WindowsGame.IsSteamRunning)
            {
                return false;
            }

            var achievementName = $"Achievement_{achievement.Index + 1}";
            SteamUserStats.SetAchievement(achievementName);

            // Store stats in the Steam database if necessary
            // If this failed, we never sent anything to the server, try again later.
            return SteamUserStats.StoreStats();
        }

        #endregion

        // GetSystemLanguageTag
        public override string GetSystemLanguageTag()
        {
            return CultureInfo.CurrentCulture.Name;
        }
    }
}
