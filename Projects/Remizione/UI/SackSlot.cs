using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BagSlot
    /// </summary>
    public sealed class SackSlot : GameObject, IInputHandler
    {
        #region Private fields

        private Actor? actor;
        private readonly UITextButton button;
        private readonly GameSession session;
        private readonly ImageSprite slotImage;

        #endregion

        #region Constructor

        // Constructor
        public SackSlot(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Slot image
            this.slotImage = new ImageSprite(Game, Atlases.UI.SackSlot)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -2, -2),
            };

            // Button
            this.button = new(Game, InputBindings.Inventory)
            {
                AllowPressEffect = false,
                ImageName = "BagSlot",
                PivotOrigin = RectanglePoint.RightBottom,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 3, -2),
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            slotImage.Draw(gameTime);
            Game.SpriteBatch.End();

            button.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            var opacity = actor?.Session.IsAwaiting == true ? .3f : 1f;    

            button.ButtonOpacity = opacity;
            slotImage.Opacity = opacity;
            button.IsEnabled = opacity == 1;

            button.Update(gameTime);
            slotImage.Update(gameTime);
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get => actor;
            set
            {
                if (value != actor)
                {
                    actor = value;
                }
            }
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (actor == null || session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (button.TestPressed(PlayerIndex.One))
            {
                actor.ShowInventory();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // IsVisible
        public bool IsVisible => actor != null && actor.Session.IsCurrentScene;
    }
}
