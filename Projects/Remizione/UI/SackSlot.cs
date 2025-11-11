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

        private readonly UIButton button;
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
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -8, -2),
            };

            // Button
            this.button = new(Game, InputBindings.PilgrimSack)
            {
                AllowPressEffect = false,
                ImageName = nameof(SackSlot),
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.RightBottom, -3, 0),
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            /*
            Game.SpriteBatch.Begin(Game.Camera);
            slotImage.Draw(gameTime);
            Game.SpriteBatch.End();

            button.Draw(gameTime);
            */
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            button.Update(gameTime);
            slotImage.Update(gameTime);
        }

        #endregion

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (button.TestPressed(PlayerIndex.One))
            {
                session.ShowPilgrimSack();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }
    }
}
