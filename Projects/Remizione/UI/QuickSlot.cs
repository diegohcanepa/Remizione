using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// QuickSlot
    /// </summary>
    public sealed class QuickSlot : GameObject, IInputHandler
    {
        private Actor? actor;
        private readonly TextSprite amountText;
        private readonly ImageSprite itemImage;
        private readonly Vector2Tween itemImageScaleTween = new();
        private int lastKnownCount;
        private Item? lastKnownItem;
        private readonly ImageSprite slotImage;
        private readonly UIControl button;

        #region Constructor

        // Constructor
        public QuickSlot(EngendroGame game)
            : base(game)
        {
            // Slot image
            this.slotImage = new ImageSprite(Game, Atlases.UI.InventorySlotSelected)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 2, -2),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Item image
            this.itemImage = new ImageSprite(Game)
            {
                PivotOrigin = RectanglePoint.Top,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Top, 0, 1.5f),
                Scale = ScaleInfo.UIElement.Tiny
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

            // Button
            this.button = new(game, InputBindings.UseItem)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.RightBottom, 2, -4),
                Small = true
            };
        }

        #endregion

        #region Private members

        // SelectNextWeapon
        private void SelectNextWeapon()
        {
            if (actor == null)
                return;
         
            if (actor.Inventory.SelectNext())
            {
                Sound.Play(SoundNames.UIQuickSlot);
            }
        }

        // SelectPreviousWeapon
        private void SelectPreviousWeapon()
        {
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            slotImage.Draw(gameTime);
            itemImage.Draw(gameTime);
            Game.SpriteBatch.End();

            button.Draw(gameTime);

            if (actor?.Inventory.SelectedItem != null && actor.Inventory.SelectedItem.MetaItem.IsStackable)
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
            if (!IsVisible)
                return;

            button.Update(gameTime);

            if (lastKnownItem != actor?.Inventory.SelectedItem)
            {
                lastKnownItem = actor?.Inventory.SelectedItem;
                itemImage.Image = actor?.Inventory.SelectedItem?.MetaItem.Image;
                itemImageScaleTween.Start(TweenStyle.Linear, new Vector2(.3f), ScaleInfo.UIElement.Tiny, 70);
                itemImage.Tweens.ScaleTween = itemImageScaleTween;
            }

            if (actor?.Inventory.SelectedItem != null && actor.Inventory.SelectedItem.Count != lastKnownCount)
            {
                lastKnownCount = actor.Inventory.SelectedItem.Count;
                itemImage.Opacity = lastKnownCount == 0 ? .3f : 1;
                amountText.Text = actor.Inventory.SelectedItem.GetDisplayAmount();
                amountText.Update(gameTime);
            }

            slotImage.Update(gameTime);
            itemImage.Update(gameTime);
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
                    itemImage.Image = actor?.Inventory.SelectedItem?.MetaItem.Image;
                    lastKnownCount = -1;
                    lastKnownItem = null;
                }
            }
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (actor == null)
                return HandleInputResult.Unhandled;

            if (InputBindings.QuickSlotNextWeaponItem.IsPressed(PlayerIndex.One))
            {
                if (actor.Inventory.SelectNext())
                    Sound.Play(SoundNames.UIQuickSlot);
                
                return HandleInputResult.Handled;
            }

            else if (InputBindings.QuickSlotPreviousWeaponItem.IsPressed(PlayerIndex.One))
            {
                if (actor.Inventory.SelectPrevious())
                    Sound.Play(SoundNames.UIQuickSlot);

                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // IsVisible
        public bool IsVisible => actor != null && !actor.Inventory.IsEmpty;
    }
}
