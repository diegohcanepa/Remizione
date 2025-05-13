using Engendro;
using Engendro.Input;
using EngendroAdventure;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Steamworks;
using System;
using System.IO;
using System.Threading;

namespace Remizione
{
    /// <summary>
    /// WindowsGame
    /// </summary>
    public sealed partial class WindowsGame : RemizioneGame
    {
        // Constructor
        public WindowsGame(PlatformBridge platformBridge)
            : base(RunningPlatform.Windows, platformBridge)
        {
            AppDomain.CurrentDomain.UnhandledException += ProgramUnhandledException;

            Graphics.HardwareModeSwitch = false;
            GamePadDevice.Style = GamePadStyle.Xbox;

            // The following lines restart your game through the Steam-client in case someone started it by double-clicking the exe.
            try
            {
                if (SteamAPI.RestartAppIfNecessary((AppId_t)GameSettings.SteamAppID))
                {
                    //Console.Out.WriteLine("Game wasn't started by Steam-client. Restarting.");
                    Exit();
                }
            }
            catch (DllNotFoundException)
            {
                // We check this here as it will be the first instance of it.
                //Console.Out.WriteLine("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib." +
                //                    " It's likely not in the correct location. Refer to the README for more details.\n" + e);
                Exit();
            }
        }

        #region Private members

        // ProgramUnhandledException
        private static void ProgramUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var path = GameSettings.LogFileName;
            File.WriteAllText(path, e.ExceptionObject.ToString());
        }

        #endregion

        #region Protected members

        // OnExiting
        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            base.OnExiting(sender, args);

            SteamAPI.Shutdown();
        }

        // OnInitialize
        protected override void OnInitialize()
        {
            try
            {
                if (SteamAPI.Init())
                {
                    IsSteamRunning = true;
                    SteamUtils.SetOverlayNotificationPosition(ENotificationPosition.k_EPositionTopRight);
                }
            }
            catch (DllNotFoundException e)
            {
                Console.WriteLine(e);
            }

            base.OnInitialize();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            /*
#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
                return;
            }
#endif
            */

            if (IsSteamRunning)
            {
                SteamAPI.RunCallbacks();
            }
        }

        #endregion

        // IsSteamRunning
        public static bool IsSteamRunning { get; private set; }

        // Mutex
        public static Mutex? Mutex { get; set; } = null;
    }
}
