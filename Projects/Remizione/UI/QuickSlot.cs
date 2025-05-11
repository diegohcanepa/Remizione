using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.UI
{
    /// <summary>
    /// QuickSlot
    /// </summary>
    public sealed class QuickSlot : GameObject, IInputHandler
    {
        private readonly TextSprite amountText;
        private readonly InputBinding? inputBinding;
        private readonly ImageSprite itemImage;
        private readonly Vector2Tween itemImageScaleTween = new();
        private ItemContainer? items;
        private static readonly Vector2 itemScale = new(.4f);
        private int lastKnownCount;
        private Item? lastKnownItem;
        private readonly ImageSprite shadowImage;
        private readonly ColorTween slotColorTween = new();
        private readonly ImageSprite slotImage;

        // Constructor
        public QuickSlot(EngendroGame game, InputBinding? inputBinding)
            : base(game)
        {
            this.inputBinding = inputBinding;

            // Slot image
            this.slotImage = new ImageSprite(Game, Atlases.UI.QuickSlot)
            {
                Opacity = .8f,
                PivotOrigin = RectanglePoint.Middle,
                Scale = new(.75f)
            };

            // Slot shadow
            this.shadowImage = new ImageSprite(Game, Atlases.UI.QuickSlotShadow)
            {
                Opacity = .2f,
                PivotOrigin = RectanglePoint.Middle,
                Scale = new(.75f)
            };

            // Item image
            this.itemImage = new ImageSprite(Game)
            {
                PivotOrigin = RectanglePoint.Top,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Top, 0, 1.5f),
                Scale = itemScale,
                VisualParent = slotImage
            };

            // Amount
            this.amountText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.TextWhite,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.RightBottom, -1.5f, -.5f),
                Scale = ScaleInfo.TextQuickSlot,
                Spacing = -10,
                VisualParent = slotImage
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            shadowImage.Draw(gameTime);
            slotImage.Draw(gameTime);
            itemImage.Draw(gameTime);
            Game.SpriteBatch.End();

            if (items?.SelectedItem != null && items.SelectedItem.MetaItem.Maximum != 1)
            {
                if (lastKnownCount > 0)
                {
                    Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
                    amountText.Draw(gameTime);
                    Game.SpriteBatch.End();
                }
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastKnownItem != items?.SelectedItem)
            {
                lastKnownItem = items?.SelectedItem;
                itemImage.Image = items?.SelectedItem?.IconImage;

                slotColorTween.Start(TweenStyle.Linear, Color.Green, Color.White, 70);
                slotImage.Tweens.ColorTween = slotColorTween;

                itemImageScaleTween.Start(TweenStyle.Linear, new Vector2(.3f), itemScale, 70);
                itemImage.Tweens.ScaleTween = itemImageScaleTween;
            }

            if (items?.SelectedItem != null && items.SelectedItem.Count != lastKnownCount)
            {
                lastKnownCount = items.SelectedItem.Count;
                itemImage.Opacity = lastKnownCount == 0 ? .3f : 1;
                amountText.Text = lastKnownCount.ToString();
                amountText.Update(gameTime);
            }

            slotImage.Update(gameTime);
            itemImage.Update(gameTime);
        }

        #endregion

        // CanHandleInput
        public bool CanHandleInput => inputBinding != null && Items != null;

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (inputBinding != null && inputBinding.IsPressed(0))
            {
                Sound.Play(SoundNames.UIQuickSlot);
                Items?.SelectNext();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // Items
        public ItemContainer? Items
        {
            get => items;
            set
            {
                if (value != items)
                {
                    items = value;
                    itemImage.Image = items?.SelectedItem?.IconImage;
                    lastKnownCount = -1;
                    lastKnownItem = null;
                }
            }
        }

        // Position
        public Vector2 Position
        {
            get => slotImage.Position;
            set
            {
                slotImage.Position = value;
                shadowImage.Position = value + Vector2.UnitY / 2;
            }
        }
    }
}
