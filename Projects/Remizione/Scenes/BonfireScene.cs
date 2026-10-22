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

        private readonly UIButton[] buttons;
        private static readonly string[] labels = { "Rise", "Craft", "Journey" };
        private const string prefix = "BonfireAction";

        #endregion

        #region Constructor

        // Constructor
        public BonfireScene()
        {
            buttons = new UIButton[3];
            float maxWidth = 0;
            float y = 35;
            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i] = new()
                {
                    PivotOrigin = RectanglePoint.Top,
                    Position = Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, y),
                    Text = TextRepository.GetValue($"{prefix}.{labels[i]}"),
                };

                y += buttons[i].BoundingBox.Height + 4;

                if (buttons[i].BoundingBox.Width > maxWidth)
                    maxWidth = buttons[i].BoundingBox.Width;
            }

            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].MinimumWidth = maxWidth;
            }
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

            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            for (var i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].TestPressed())
                {
                    Game.SceneManager.Pop();
                    return HandleInputResult.Handled;
                }
            }

            return base.OnHandleInput(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].Update(gameTime);
            }
        }

        #endregion
    }
}
