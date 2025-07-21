using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BagSlot
    /// </summary>
    public sealed class BagSlot : GameObject, IInputHandler
    {
        #region Private fields

        private Actor? actor;
        private readonly UITextButton button;
        private readonly ImageSprite slotImage;

        #endregion

        #region Constructor

        // Constructor
        public BagSlot(EngendroGame game)
            : base(game)
        {
            // Slot image
            this.slotImage = new ImageSprite(Game, Atlases.UI.BagSlot)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -2, -2),
            };

            // Button
            this.button = new(game, InputBindings.Inventory)
            {
                AllowPressEffect = false,
                ImageName = "BagSlot",
                PivotOrigin = RectanglePoint.RightBottom,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 3, -2),
            };
        }

        #endregion

        #region Private members
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
            if (actor != null && button.TestPressed(PlayerIndex.One))
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
