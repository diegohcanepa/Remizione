using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace EngendroAdventure
{
    /// <summary>
    /// This class brings a layer of abstraction, 
    /// so the game can interact with platform specific systems.
    /// </summary>
    public abstract class PlatformBridge : IUpdate
    {
        // Constructor
        protected PlatformBridge(PlatformFileSystem fileSystem)
        {
            this.FileSystem = fileSystem;
        }

        #region Protected members

        // OnSetAchievement
        protected virtual bool OnSetAchievement(Achievement achievement)
        {
            return true;
        }

        // OnUpdate
        protected virtual void OnUpdate(GameTime gameTime)
        {
        }

        #endregion

        // AllowWindowedMode
        public bool AllowWindowedMode { get; set; }

        // DetectControllerDisconnection
        public bool DetectControllerDisconnection { get; set; }

        // FileSystem
        public PlatformFileSystem FileSystem { get; }

        // GetSystemLanguageTag()
        public abstract string GetSystemLanguageTag();

        // InvertActionButton
        public bool InvertActionButton { get; set; }

        // SetAchievement
        public bool SetAchievement(string id)
        {
            if (AchievementManager.GetAchievement(id) is Achievement achievement)
            {
                return OnSetAchievement(achievement);
            }
            else
            {
                throw new InvalidOperationException($"Achievement id not found: {id}");
            }
        }

        // Update
        public void Update(GameTime gameTime)
        {
            OnUpdate(gameTime);
        }
    }
}