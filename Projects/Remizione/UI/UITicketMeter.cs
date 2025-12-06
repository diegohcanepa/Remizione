using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// UITicketMeter
    /// </summary>
    public class UITicketMeter : GameObject
    {
        private readonly ImageSprite icon;
        private readonly UIScore score;
        private readonly ImageSprite slot;

        // Constructor
        public UITicketMeter(EngendroGame game)
            : base(game)
        {
            // Slot
            this.slot = new ImageSprite(Game, Atlases.UI.TicketSlot)
            {
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 206, 112)
            };

            // Icon
            this.icon = new ImageSprite(Game, Atlases.UI.TicketIcon)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = slot.BoundingBox.GetPoint(RectanglePoint.Center),
                Scale = ScaleInfo.UIElement.Tiny
            };

            // Score
            this.score = new UIScore(game, ColorPalette.Text.Default, ScaleInfo.Text.Large, false)
            {
                PivotOrigin = RectanglePoint.Top,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Bottom)
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
            slot.Draw(gameTime);
            icon.Draw(gameTime);
            score.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            score.Update(gameTime);
        }

        #endregion

        // SetInitialValue
        public void SetInitialValue(int value)
        {
            score.SetInitialValue(value);
        }

        // Value
        public int Value
        {
            get => score.Value;
            set => score.Value = value;
        }
    }
}
