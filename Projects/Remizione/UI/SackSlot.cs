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

        private readonly TextSprite amountText;
        private int lastKnownCount;
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
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -2, -3),
            };

            // Amount
            this.amountText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Top,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -2),
                Scale = ScaleInfo.Text.Large,
                Spacing = -5
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            slotImage.Draw(gameTime);
            amountText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            slotImage.Update(gameTime);

            if (lastKnownCount != session.PilgrimSack.Count)
            {
                lastKnownCount = session.PilgrimSack.Count;
                amountText.Text = $"{session.PilgrimSack.Count}/{session.PilgrimSack.Size}";
            }
        }

        #endregion

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (InputBindings.PilgrimSack.IsPressed(PlayerIndex.One))
            {
                session.ShowPilgrimSack();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }
    }
}
