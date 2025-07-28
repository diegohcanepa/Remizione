using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.Menus
{
    /// <summary>
    /// TitleMenuScene
    /// </summary>
    public sealed partial class TitleMenuScene : MenuScene
    {
        #region Private fields

        private readonly ImageSprite background;
        private readonly FloatTween darknessTween = new();
        private readonly TextSprite experienceAdviceText;
        private readonly Timer fadeInTimer;
        private readonly ImageSprite foreground;
        private readonly ImageSprite foregroundLight;
        private readonly Blinker<bool> lightBlink = new(false, true);
        private readonly Countdown nextBlinkTimer = new();
        private readonly Menu menu;
        private readonly ImageSprite roofLight;

#if XBOX_ONE
        private readonly TextSprite xboxActiveUser;
        private readonly UIControl xboxSigningButton;
#endif

        #endregion

        #region Constructor

        // Constructor
        internal TitleMenuScene(RemizioneGame game)
            : base(game, null, SceneSettings.ExclusiveDraw)
        {
            // Background
            background = new ImageSprite(game, Atlases.Menu.TitleScreen)
            {
                PivotOrigin = RectanglePoint.Middle,
                Position = Screen.Center,
            };

            // Foreground
            foreground = new ImageSprite(game, Atlases.Menu.TitleScreenForeground)
            {
            };

            // ForegroundLight
            foregroundLight = new ImageSprite(game, Atlases.Menu.TitleScreenForegroundLight)
            {
            };

            // RoofLight
            roofLight = new ImageSprite(game, Atlases.Menu.TitleScreenRoofLight)
            {
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.Area.GetPoint(RectanglePoint.Top, 0, -4)
            };
            roofLight.Tweens.RotationTween = FloatTween.Create(TweenStyle.CubicInOut, -2, 2, 2000, -1);

            // Main menu
            menu = new Menu(game)
            {
            };

            menu.AddItem(MenuItemName.Start, ShowStartGame, null);
            menu.AddItem(MenuItemName.Options, ShowOptions, Atlases.Menu.OptionsIcon);
            menu.AddItem(MenuItemName.Credits, ShowCredits, null);

#if WINDOWS
            if (Game.IsDemo)
            {
                menu.AddItem(MenuItemName.WishlistNow, WishlistNow, null);
            }
#endif

            menu.AddItem(MenuItemName.Exit, Game.Exit, null);

            // Experience advice
            experienceAdviceText = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.TextWhite,
                MaximumWidth = 260,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom),
                Scale = ScaleInfo.Text.Medium,
                Text = "@Menu.Messages.BestExperienceAdvice"
            };

            this.fadeInTimer = new Timer(FadeIn);

#if XBOX_ONE
            // XBox active user
            xboxActiveUser = new TextSprite(Game, Fonts.Regular)
            { 
                Color = ColorPalette.MenuItemTextActive,
                PivotOrigin = RectanglePoint.LeftBottom
            };

            // XBox sign in button
            xboxSigningButton = new UIControl(Game, GameInput.SignIn)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Text = VladUtils.EncodePlatformMessageKey(PlatformMessageKey.SignIn)
            };
#endif
        }

        #endregion

        #region Private members

        // FadeIn
        private void FadeIn()
        {
            TransitionManager.CurrentTransition.Out(2000);

            Width = Screen.NativeWidth;
            Height = Screen.NativeHeight;

            Invalidate();
        }

        // ResetNextBlinkTimer
        private void ResetNextBlinkTimer()
        {
            nextBlinkTimer.Start(Randomizer.Next(7000, 15000));
        }

        // ShowCredits
        private void ShowCredits()
        {
            //GoToScene(new CreditsScene(Game));
        }

        // ShowOptions
        private void ShowOptions()
        {
            GoToScene(new OptionsMenuScene(Game));
        }

        // ShowStartGame
        private void ShowStartGame()
        {
            // GoToScene(new StartGameMenuScene(Game));
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            background.Draw(gameTime);

            if (!lightBlink.IsRunning || lightBlink.CurrentValue)
            {
                roofLight.Draw(gameTime);
            }

            foreground.Draw(gameTime);

            if (!lightBlink.IsRunning || lightBlink.CurrentValue)
            {
                foregroundLight.Draw(gameTime);
            }

            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            Game.Shapes.DrawRectangle(Screen.Area, Color.Black * darknessTween.CurrentValue);
            Game.SpriteBatch.End();

            menu.Draw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);

            experienceAdviceText.Draw(gameTime);
            Game.SpriteBatch.End();

#if XBOX_ONE
            xboxSigningButton.Draw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            xboxActiveUser.Draw(gameTime);
            Game.SpriteBatch.End();
#endif
            DrawVersionInformation(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {

#if XBOX_ONE
            if (xboxSigningButton.TestPressed(VladInputHelper.PlayerIndex))
            {
                // Sign In / Change User cod here
                XboxOne.ChangeSignInUser();

                return HandleInputResult.Handled;
            }
#endif

            return menu.HandleInput(gameTime);
        }

        // OnInvalidate
        protected override void OnInvalidate()
        {
#if XBOX_ONE
            xboxSigningButton.Position = Screen.SafeArea.GetPoint(RectanglePoint.LeftBottom);
            xboxSigningButton.Text = string.IsNullOrWhiteSpace(Game.ActiveUser) ? VladUtils.EncodePlatformMessageKey(PlatformMessageKey.SignIn) : VladUtils.EncodePlatformMessageKey(PlatformMessageKey.ChangeUser);

            xboxActiveUser.Position = xboxSigningButton.BoundingBox.GetPoint(RectanglePoint.LeftTop, 0, -2);
            xboxActiveUser.Text = Game.ActiveUser;
            xboxActiveUser.Scale = xboxSigningButton.TextScale;
#endif
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            darknessTween.Start(TweenStyle.CubicInOut, .2f, .4f, 3000, -1);

            TransitionManager.CurrentTransition.In(0);
            fadeInTimer.Start(500);

            Invalidate();

            ResetNextBlinkTimer();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

#if XBOX_ONE
            xboxActiveUser.Update(gameTime);
            xboxSigningButton.Update(gameTime);
#endif

            lightBlink.Update(gameTime);
            darknessTween.Update(gameTime);
            foreground.Update(gameTime);
            foregroundLight.Update(gameTime);
            roofLight.Update(gameTime);
            background.Update(gameTime);
            fadeInTimer.Update(gameTime);
            menu.Update(gameTime);
            experienceAdviceText.Update(gameTime);
        }

        #endregion

        // WishlistNow
        public static void WishlistNow()
        {
#if WINDOWS
            System.Diagnostics.ProcessStartInfo info = new()
            {
                FileName = $"steam://store/{GameSettings.SteamAppID}",
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(info);
#endif
        }
    }
}