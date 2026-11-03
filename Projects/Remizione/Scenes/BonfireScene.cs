using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BonfireScene
    /// </summary>
    public sealed class BonfireScene : Scene
    {
        #region Private fields

        private UIButton? activeButton;
        private readonly UITextButton[] buttons;
        private readonly TextSprite descriptionText;
        private static readonly string[] labels = { "Vigil", "Travel", "March" };
        private const string prefix = "BonfireAction";

        #endregion

        #region Constructor

        // Constructor
        public BonfireScene()
        {
            buttons = new UITextButton[labels.Length];
            float maxWidth = 0;
            float y = 35;
            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i] = new()
                {
                    PivotOrigin = RectanglePoint.Top,
                    Position = Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, y),
                    Text = TextRepository.GetValue($"{prefix}.{labels[i]}.Name"),
                    Tip = TextRepository.GetValue($"{prefix}.{labels[i]}.Description"),
                };

                y += buttons[i].BoundingBox.Height + 4;

                if (buttons[i].BoundingBox.Width > maxWidth)
                    maxWidth = buttons[i].BoundingBox.Width;
            }

            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].MinimumWidth = maxWidth;
            }

            buttons[2].Action = Rise;

            // Description text
            this.descriptionText = new TextSprite(Fonts.CommonOutline)
            {
                Color = Colors.Palette.Text,
                MaximumWidth = 150,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -5),
                Scale = ScaleInfo.Text.Large
            };
        }

        #endregion

        #region Private members

        // Rise
        private void Rise()
        {
            Game.SceneManager.Pop();
        }

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();
            MouseCursor.Icon = MouseCursorIcon.Cross;
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);

            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].Draw(gameTime);
            }

            descriptionText.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (InputBindings.Back.IsPressed(PlayerIndex.One))
            {
                Game.SceneManager.Pop();
                return HandleInputResult.Handled;
            }

            for (var i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].TestPressed())
                    return HandleInputResult.Handled;
            }

            return base.OnHandleInput(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            UIButton? hoveredButton = null;
            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].Update(gameTime);

                if (buttons[i].IsMouseOver && hoveredButton == null)
                    hoveredButton = buttons[i];
            }

            if (hoveredButton != activeButton)
            {
                activeButton = hoveredButton;
                descriptionText.Text = activeButton?.Tip ?? string.Empty;
            }
        }

        #endregion
    }
}
