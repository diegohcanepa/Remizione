using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// EquipmentSlot
    /// </summary>
    public abstract class EquipmentSlot : GameObject, IInputHandler
    {
        #region Private fields

        private readonly TextSprite amountText;
        private readonly ImageSprite itemImage;
        private readonly Vector2Tween itemImageScaleTween = new();
        private int lastKnownCount;
        private Item? lastKnownItem;
        private readonly InputBinding? nextInputBinding;
        private readonly GameSession session;
        private readonly ImageSprite slotImage;
        private readonly InputBinding? useInputBinding;

        #endregion

        #region Constructor

        // Constructor
        protected EquipmentSlot(GameSession session, Vector2 position, ItemCategory itemCategory, InputBinding? inputBinding)
            : base(session.Game)
        {
            this.session = session;
            this.ItemCategory = itemCategory;
            this.useInputBinding = inputBinding;

            // Bindings
            if (itemCategory == ItemCategory.LeftHand)
                nextInputBinding = InputBindings.SelectLeft;

            else if (itemCategory == ItemCategory.RightHand)
                nextInputBinding = InputBindings.SelectRight;

            // Slot image
            this.slotImage = new ImageSprite(Game, Atlases.UI.EquipmentSlot)
            {
                Opacity = .9f,
                PivotOrigin = itemCategory == ItemCategory.Gadget ? RectanglePoint.LeftTop : RectanglePoint.LeftBottom,
                Position = position
            };

            // Item image
            this.itemImage = new ImageSprite(Game)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Center, 0, -1),
                Scale = ScaleInfo.UIElement.Tiny
            };

            if (itemCategory == ItemCategory.Gadget)
                itemImage.X -= .5f;

            // Amount
            this.amountText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Top,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -4),
                Scale = ScaleInfo.Text.Large,
                Spacing = -5
            };

            InvalidateItem();
        }

        #endregion

        #region Private members

        // InvalidateItem
        private void InvalidateItem()
        {
            lastKnownItem = session.Inventory.GetEquippedItem(ItemCategory);

            if (lastKnownItem != null)
            {
                itemImage.Image = lastKnownItem.MetaItem.Image;
                itemImageScaleTween.Start(TweenStyle.Linear, new Vector2(.3f), ScaleInfo.UIElement.Tiny, 70);
                itemImage.Tweens.ScaleTween = itemImageScaleTween;
                itemImage.Opacity = 1;
            }
            else
            {
                lastKnownCount = 0;
                itemImage.Image = Atlases.UI.GetImage($"InventoryCategory{ItemCategory}");
                itemImage.Opacity = .2f;
                itemImage.Scale = ScaleInfo.UIElement.Medium;
                itemImageScaleTween.Stop();
            }

            InvalidateItemAmount(true);
        }

        // InvalidateItemAmount
        private void InvalidateItemAmount(bool enforce)
        {
            if (lastKnownItem?.Count != lastKnownCount || enforce)
            {
                lastKnownCount = lastKnownItem == null ? 0 : lastKnownItem.Count;
                itemImage.OpacityFactor = lastKnownCount == 0 && lastKnownItem != null ? .3f : 1;
                amountText.Text = lastKnownItem == null ? string.Empty : lastKnownItem.GetDisplayAmount();
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            slotImage.Draw(gameTime);
            itemImage.Draw(gameTime);
            Game.SpriteBatch.End();

            if (lastKnownItem?.MetaItem.StackMode != StackMode.None)
            {
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
                amountText.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastKnownItem != session.Inventory.GetEquippedItem(ItemCategory))
                InvalidateItem();
            else
                InvalidateItemAmount(false);

            slotImage.Update(gameTime);
            itemImage.Update(gameTime);
        }

        #endregion

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (session.Player == null || session.IsAwaiting)
                return HandleInputResult.Unhandled;

            // Use item
            if (useInputBinding?.IsPressed(PlayerIndex.One) == true)
            {
                if (lastKnownItem == null)
                {
                    if (session.Inventory.EquipPrevious(ItemCategory) == null)
                        Sound.Play(SoundNames.Error);
                    else
                        Sound.Play(SoundNames.UIHover);
                }
                else
                    session.Player.UseEquippedItem(ItemCategory);

                return HandleInputResult.Handled;
            }

            // Next item
            if (nextInputBinding?.IsPressed(PlayerIndex.One) == true)
            {
                if (session.Inventory.EquipNext(ItemCategory) != null)
                    Sound.Play(SoundNames.UIHover);

                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // ItemCategory
        public ItemCategory ItemCategory { get; }
    }
}
