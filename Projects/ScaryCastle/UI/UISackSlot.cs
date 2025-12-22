using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UISackSlot
    /// </summary>
    public sealed class UISackSlot : GameObject, IInputHandler
    {
        #region Private fields

        private readonly TextSprite amountText;
        private readonly ImageSprite flyingIcon;
        private readonly ImageSprite icon;
        private readonly Vector2 iconScale = Vector2.One;
        private int lastKnownCount;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();
        private readonly GameSession session;
        private readonly ImageSprite slotImage;

        #endregion

        #region Constructor

        // Constructor
        public UISackSlot(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Slot image
            this.slotImage = new ImageSprite(Game, Atlases.UI.SackSlot)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -2, -3),
            };

            // Icon
            this.icon = new(Game, Atlases.UI.SackIcon)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Center, -.5f, 0)
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

            // Flying icon
            this.flyingIcon = new(Game)
            {
                PivotOrigin = RectanglePoint.Center,
            };
        }

        #endregion

        #region Private members

        // EatItem
        private void EatItem()
        {
            Sound.Play(SoundNames.ItemAdded);

            rotationTween.Start(TweenStyle.QuadraticInOut, 0, 15, 50, 6);
            icon.Tweens.RotationTween = rotationTween;

            scaleTween.Start(TweenStyle.QuadraticInOut, iconScale, iconScale * 1.3f, 150, 2);
            icon.Tweens.ScaleTween = scaleTween;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            slotImage.Draw(gameTime);
            flyingIcon.Draw(gameTime);
            icon.Draw(gameTime);
            amountText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            slotImage.Update(gameTime);
            flyingIcon.Update(gameTime);
            icon.Update(gameTime);

            if (lastKnownCount != session.Inventory.Count)
            {
                lastKnownCount = session.Inventory.Count;
                amountText.Color = session.InventoryFull ? ColorPalette.Text.Orange : ColorPalette.Text.Default;
                amountText.Text = $"{session.Inventory.Count}/{Inventory.MaximumSize}";
            }
        }

        #endregion

        // AnimateItem
        public void AnimateItem(MetaItem metaItem, Vector2 startPosition)
        {
            if (metaItem.Image is null)
                return;

            flyingIcon.Image = metaItem.Image;
            flyingIcon.Position = startPosition - Game.Camera.Offset;
            flyingIcon.Scale = ScaleInfo.UIElement.Tiny;
            flyingIcon.Tweens.PositionTween = Vector2Tween.Create(TweenStyle.CubicInOut, startPosition, icon.BoundingBox.Center, 1000);
            flyingIcon.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, icon.Scale * .3f, ScaleInfo.UIElement.Large, 400, 2, EatItem);
        }

        // BoundingBox
        public RectangleF BoundingBox => slotImage.BoundingBox;

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (InputBindings.Inventory.IsPressed(PlayerIndex.One))
            {
                session.ShowInventory();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }
    }
}
