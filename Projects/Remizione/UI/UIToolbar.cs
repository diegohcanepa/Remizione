using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione.UI
{
    /// <summary>
    /// UIToolbar
    /// </summary>
    public sealed class UIToolbar : GameObject, IInputHandler
    {
        private readonly HUDButton inventoryButton;
        private readonly GameSession session;

        // Constructor
        public UIToolbar(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Inventory button
            this.inventoryButton = new HUDButton(Game, "InventoryIcon", Localization.GetValue(InGameMenuOptionName.Inventory))
            {
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.InventoryButton)
                inventoryButton.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (session.InventoryButton)
                inventoryButton.Update(gameTime);
        }

        #endregion

        // GetHoveredButton
        public UIToolbarButton GetHoveredButton()
        {
            if (inventoryButton.IsMouseOver)
                return UIToolbarButton.Inventory;

            return UIToolbarButton.None;
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (inventoryButton.TestPressed(0))
            {
                session.ShowItemContainerScene(ItemContainerCategory.Inventory);
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // Invalidate
        public void Invalidate()
        {
            var pos = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 4, -4);

            if (session.InventoryButton)
            {
                inventoryButton.Position = pos;
                pos.X += inventoryButton.BoundingBox.Width;
            }
        }
    }
}
