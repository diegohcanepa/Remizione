using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.Menus
{
    /// <summary>
    /// OptionsMenuScene
    /// </summary>
    public sealed partial class OptionsMenuScene : StandardMenuScene
    {
        private readonly TextSprite optionDescription;
        private readonly OptionMenu menu;
        private readonly TextSprite mouseInfo;

        #region Constructor

        // Constructor
        public OptionsMenuScene(RemizioneGame game)
            : base(game, "@Menu.Titles.Options")
        {
            this.mouseInfo = new TextSprite(game, Fonts.Main) { Scale = ScaleInfo.Text.Medium, Color = Color.White, PivotOrigin = RectanglePoint.LeftTop };

            menu = new OptionMenu(game, 6)
            {
                Position = new Vector2(Screen.Center.X, 60)
            };

            if (game.CurrentSession == null)
            {
                menu.AddOption(new LanguageOption(game));
            }

            if (game.PlatformBridge.AllowWindowedMode)
            {
                menu.AddOption(new FullscreenOption(game));
            }

            menu.AddOption(new VibrationOption(game));
            menu.AddOption(new AudioOption(game));
            menu.AddOption(new GamePadOption(game));

            //?
            /*
            if (InputManager.Keyboard.Allowed)
                menu.AddOption(new KeyboardOption(game));
            */

            // Option description
            this.optionDescription = new TextSprite(game, Fonts.Main)
            {
                Color = ColorPalette.TextWhite,
                MaximumWidth = 100,
                Scale = ScaleInfo.Text.Small
            };
        }

        #endregion

        #region Private members

        // InvalidateSelectedOptionText
        private void InvalidateSelectedOptionText()
        {
            if (menu.SelectedOption != null)
            {
                optionDescription.Text = menu.SelectedOption.Description;
                optionDescription.Position = menu.SelectedOption.RightArrowBoundingBox.GetPoint(RectanglePoint.RightTop, 20, 0);
            }
            else
            {
                optionDescription.Text = null;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            menu.Draw(gameTime);

            if (menu.SelectedOption != null)
            {
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
                optionDescription.Draw(gameTime);
                Game.SpriteBatch.End();
            }

            DrawVersionInformation(gameTime);

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            mouseInfo.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (menu.HandleInput(gameTime) == HandleInputResult.Handled)
            {
                InvalidateSelectedOptionText();
                return HandleInputResult.Handled;
            }
            else
            {
                return base.OnHandleInput(gameTime);
            }
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();
            InvalidateSelectedOptionText();
        }

        // OnUnloadContent
        protected override void OnUnloadContent()
        {
            UserSettingsData.SaveCurrentSystemSettings(Game);
            base.OnUnloadContent();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            menu.Update(gameTime);
            optionDescription.Update(gameTime);
        }

        #endregion
    }
}
