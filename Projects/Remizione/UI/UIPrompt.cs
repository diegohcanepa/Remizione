using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.UI
{
    /// <summary>
    /// UIPrompt
    /// </summary>
    public sealed class UIPrompt : GameObject
    {
        private readonly UITextButton control;
        private readonly TextSprite label;
        private readonly GameSession session;
        private GameThing? target;

        // Constructor
        public UIPrompt(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Control
            this.control = new(Game, InputBindings.Interact)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -2)
            };

            // Label
            this.label = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Middle,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -7),
                Scale = ScaleInfo.Text.Huge
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.IsCurrentScene && target != null)
            {
                if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.GamePad)
                {
                    control.Draw(gameTime);
                }
                else
                {
                    Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
                    label.Draw(gameTime);
                    Game.SpriteBatch.End();
                }
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (session.IsCurrentScene && MouseCursor.Instance.State != MouseCursorState.Wait && session.Player?.InteractiveTarget is GameThing currentTarget)
            {
                if (currentTarget != target)
                {
                    target = currentTarget;
                    control.Text = currentTarget.LocalizedDisplayName;
                    label.Text = currentTarget.LocalizedDisplayName;
                }
            }
            else
            {
                control.Text = null;
                label.Clear();
                target = null;
            }

            control.Update(gameTime);
        }

        #endregion
    }
}
