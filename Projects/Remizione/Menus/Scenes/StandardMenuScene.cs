using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remizione.UI;

namespace Remizione.Menus
{
    /// <summary>
    /// StandardMenuScene
    /// </summary>
    public abstract class StandardMenuScene : MenuScene
    {
        #region Private fields

        private readonly TextSprite titleSprite;

        #endregion

        #region Constructor

        // Constructor
        protected StandardMenuScene(RemizioneGame game, string title)
            : base(game, Atlases.Menu.ContainerScreen)
        {
            ControlGroup = new UIControlGroup(Game) { Spacing = 5 };
            ControlGroup.Add(LocalizationHelper.GetInputBinding(InputBindings.Back), InputBindings.Back);

            titleSprite = new TextSprite(Game, Fonts.Main)
            {
                Color = ColorPalette.TextStandardMenuTitle,
                PivotOrigin = RectanglePoint.Middle,
                Position = new Vector2(Screen.Center.X, 25),
                Scale = ScaleInfo.TextMenuContainerTitle,
                Text = title
            };
        }

        #endregion

        #region Protected members

        // ControlGroup
        protected UIControlGroup ControlGroup { get; }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            titleSprite.Draw(gameTime);
            Game.SpriteBatch.End();

            ControlGroup.Draw(gameTime);
        }

        // OHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            // Back
            if (ControlGroup.Controls[0].TestPressed(0))
            {
                Pop();
                return HandleInputResult.Handled;
            }

            return base.OnHandleInput(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            ControlGroup.Update(gameTime);
            titleSprite.Update(gameTime);
        }

        #endregion
    }
}