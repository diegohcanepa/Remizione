using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIPrompt
    /// </summary>
    public sealed class UIPrompt : GameObject
    {
        private readonly UITextButton button;
        private readonly GameSession session;
        private GameThing? target;

        // Constructor
        public UIPrompt(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Button
            this.button = new(Game, InputBindings.Interact)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -4)
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.IsCurrentScene && target != null)
                button.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (session.IsCurrentScene && session.Player?.InteractiveTarget is GameThing currentTarget)
            {
                if (currentTarget != target)
                {
                    target = currentTarget;
                    button.Text = currentTarget.LocalizedDisplayName;
                    //Sound.Play(SoundNames.UIPrompt);
                }
            }
            else
            {
                button.Text = null;
                target = null;
            }

            button.Update(gameTime);
        }

        #endregion
    }
}
