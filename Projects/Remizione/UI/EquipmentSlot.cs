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
        private readonly UITextButton button;
        private readonly ImageSprite itemImage;
        private readonly Vector2Tween itemImageScaleTween = new();
        private int lastKnownCount;
        private Item? lastKnownItem;
        private readonly InputBinding? nextInputBinding;
        private readonly InputBinding? previousInputBinding;
        private readonly GameSession session;
        private readonly ImageSprite slotImage;

        #endregion

        #region Constructor

        // Constructor
        protected EquipmentSlot(GameSession session, Vector2 position, ItemCategory itemCategory, InputBinding? inputBinding)
            : base(session.Game)
        {
            this.session = session;

            this.ItemCategory = itemCategory;

            // Bindings
            if (itemCategory == ItemCategory.Junk)
            {
                previousInputBinding = InputBindings.SelectLeft;
                nextInputBinding = InputBindings.SelectRight;
            }
            else if (itemCategory == ItemCategory.Gadgets)
            {
                previousInputBinding = InputBindings.SelectUp;
                nextInputBinding = InputBindings.SelectDown;
            }

            // Slot image
            this.slotImage = new ImageSprite(Game, itemCategory == ItemCategory.Trinkets ? Atlases.UI.TrinketSlot : Atlases.UI.EquipmentSlot)
            {
                PivotOrigin = itemCategory == ItemCategory.Trinkets ? RectanglePoint.LeftTop : RectanglePoint.LeftBottom,
                Position = position
            };

            // Item image
            this.itemImage = new ImageSprite(Game)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Center, 0, -1),
                Scale = ScaleInfo.UIElement.Small
            };

            if (itemCategory == ItemCategory.Trinkets)
                itemImage.X -= .5f;

            // Amount
            this.amountText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Top,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -3),
                Scale = ScaleInfo.Text.Large,
                Spacing = -5
            };

            // Button
            this.button = new(Game, inputBinding)
            {
                AllowSound = false,
                ImageName = $"{itemCategory}Slot",
            };

            if (itemCategory == ItemCategory.Junk)
            {
                button.PivotOrigin = RectanglePoint.RightBottom;
                button.Position = slotImage.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 3, -1);
            }
            else if (itemCategory == ItemCategory.Gadgets)
            {
                button.PivotOrigin = RectanglePoint.LeftBottom;
                button.Position = slotImage.BoundingBox.GetPoint(RectanglePoint.RightBottom, -3, -1);
            }

            InvalidateItem();
        }

        #endregion

        #region Private members

        // InvalidateItem
        private void InvalidateItem()
        {
            lastKnownItem = session.PilgrimSack.GetEquippedItem(ItemCategory);

            if (lastKnownItem != null)
            {
                itemImage.Image = lastKnownItem.MetaItem.Image;
                itemImageScaleTween.Start(TweenStyle.Linear, new Vector2(.3f), ScaleInfo.UIElement.Small, 70);
                itemImage.Tweens.ScaleTween = itemImageScaleTween;
                InvalidateItemAmount(true);
            }
            else
            {
                itemImage.Image = Atlases.UI.GetImage($"EquipmentSlot{ItemCategory}Icon");
                itemImage.Scale = ScaleInfo.UIElement.Medium;
                itemImageScaleTween.Stop();
            }
        }

        // InvalidateItemAmount
        private void InvalidateItemAmount(bool enforce)
        {
            if (lastKnownItem != null && (lastKnownItem.Count != lastKnownCount || enforce))
            {
                lastKnownCount = lastKnownItem.Count;
                itemImage.OpacityFactor = lastKnownCount == 0 ? .3f : 1;
                amountText.Text = lastKnownItem.GetDisplayAmount();
                button.IsEnabled = true;
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

            if (session.IsCurrentScene)
                button.Draw(gameTime);

            if (lastKnownItem?.MetaItem.AllowEmpty == true)
            {
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
                amountText.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastKnownItem != session.PilgrimSack.GetEquippedItem(ItemCategory))
                InvalidateItem();
            else
                InvalidateItemAmount(false);

            button.Update(gameTime);
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
            if (button.TestPressed(PlayerIndex.One))
            {
                if (lastKnownItem == null)
                {
                    if (session.PilgrimSack.EquipPrevious(ItemCategory) == null)
                        Sound.Play(SoundNames.Error);
                    else
                        Sound.Play(SoundNames.UIHover);
                }
                else
                    session.Player.UseEquippedItem(ItemCategory);

                return HandleInputResult.Handled;
            }

            // Previous item
            if (previousInputBinding?.IsPressed(PlayerIndex.One) == true)
            {
                if (session.PilgrimSack.EquipPrevious(ItemCategory) != null)
                    Sound.Play(SoundNames.UIHover);

                return HandleInputResult.Handled;
            }

            // Next item
            else if (nextInputBinding?.IsPressed(PlayerIndex.One) == true)
            {
                if (session.PilgrimSack.EquipNext(ItemCategory) != null)
                    Sound.Play(SoundNames.UIHover);

                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // ItemCategory
        public ItemCategory ItemCategory { get; }
    }
}
