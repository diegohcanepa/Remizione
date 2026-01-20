using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIDeck
    /// </summary>
    public sealed class UIDeck : GameObject
    {
        #region Private fields

        private readonly TextSprite amountText;
        private readonly ImageSprite icon;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public UIDeck(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Icon
            this.icon = new(Game, Atlases.UI.Deck)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.Area.GetPoint(RectanglePoint.RightBottom, -7, -7),
            };

            // Amount
            this.amountText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Right,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Left, -1, 1),
                Scale = ScaleInfo.Text.ExtraLarge,
                Spacing = -6
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            amountText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            icon.Update(gameTime);
            if (lastKnownCount != session.Inventory.Count)
            {
                lastKnownCount = session.Inventory.Count;
                amountText.Text = $"{session.Inventory.Count}/{session.Inventory.Capacity}";
            }
        }

        #endregion
    }
}
