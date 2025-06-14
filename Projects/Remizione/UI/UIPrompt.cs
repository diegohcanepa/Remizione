using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione.UI
{
    /// <summary>
    /// UIPrompt
    /// </summary>
    public sealed class UIPrompt : GameObject
    {
        private readonly UIControl control;
        private readonly GameSession session;

        // Constructor
        public UIPrompt(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Control
            this.control = new(Game, InputBindings.Interact)
            {
                AllowContainer = true,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom)
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.IsCurrentScene && control.Tag != null)
                control.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (session.IsCurrentScene && MouseCursor.Instance.State != MouseCursorState.Wait && session.Player?.InteractiveTarget is GameThing target)
            {
                if (target != control.Tag)
                {
                    control.Tag = target;
                    control.Text = target.LocalizedDisplayName;
                }
            }
            else
            {
                control.Text = null;
                control.Tag = null;
            }

            control.Update(gameTime);
        }

        #endregion
    }
}
