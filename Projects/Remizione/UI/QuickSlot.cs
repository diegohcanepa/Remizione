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
        #region Private fields

        private Actor? actor;
        private readonly TextSprite amountText;
        private readonly UITextButton button;
        private readonly ImageSprite itemImage;
        private readonly Vector2Tween itemImageScaleTween = new();
        private int lastKnownCount;
        private Item? lastKnownItem;
        private readonly ImageSprite slotImage;
        private readonly UIDerivedStatModifier statModifier;

        #endregion

        #region Constructor

        // Constructor
        public QuickSlot(EngendroGame game)
            : base(game)
        {
            // Slot image
            this.slotImage = new ImageSprite(Game, Atlases.UI.QuickSlot)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 2, -6),
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
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 0),
                Scale = ScaleInfo.Text.Large,
                Spacing = -5
            };

            // Button
            this.button = new(game, InputBindings.UseItem)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.RightBottom, 1, -1),
                Small = true
            };

            // Stat icon
            this.statModifier = new(Game, DerivedStat.Faith)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = button.BoundingBox.GetPoint(RectanglePoint.LeftTop, -.5f, -1)
            };
        }

        #endregion

        #region Private members

        // InvalidateItem
        private void InvalidateItem(GameTime gameTime)
        {
            lastKnownItem = actor?.Inventory.SelectedItem;

            if (lastKnownItem != null)
            {
                itemImage.Image = lastKnownItem.MetaItem.Image;
                itemImageScaleTween.Start(TweenStyle.Linear, new Vector2(.3f), ScaleInfo.UIElement.Tiny, 70);
                itemImage.Tweens.ScaleTween = itemImageScaleTween;

                if (lastKnownItem.MetaItem.CraftProp != null)
                {
                    button.Text = lastKnownItem.MetaItem.CraftProp.LocalizedDisplayName;
                    statModifier.Amount = lastKnownItem.MetaItem.Faith != null ? lastKnownItem.MetaItem.Faith.MaximumValue : 0;
                }
                else
                {
                    amountText.Color = ColorPalette.Text.Default;
                    button.Text = null;
                }

                InvalidateItemAmount(gameTime, true);
            }
        }

        // InvalidateItemAmount
        private void InvalidateItemAmount(GameTime gameTime, bool enforce)
        {
            if (lastKnownItem != null && (lastKnownItem.Count != lastKnownCount || enforce))
            {
                lastKnownCount = lastKnownItem.Count;
                itemImage.Opacity = lastKnownCount == 0 ? .3f : 1;
                amountText.Text = lastKnownItem.GetDisplayAmount();
                amountText.Update(gameTime);

                if (lastKnownItem.MetaItem.Category == MetaItemCategory.Crafting)
                {
                    button.IsEnabled = lastKnownItem.IsStackFull;
                    amountText.Color = button.IsEnabled ? ColorPalette.Text.Green : amountText.Color = ColorPalette.Text.Terra;
                    button.TextColor = button.IsEnabled ? ColorPalette.Text.Green : ColorPalette.Text.Default;
                }
                else
                {
                    amountText.Color = ColorPalette.Text.Default;
                    button.IsEnabled = true;
                }
            }
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

            if (lastKnownItem != null)
            {
                button.Draw(gameTime);

                if (lastKnownItem.MetaItem.IsStackable)
                {
                    Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
                    amountText.Draw(gameTime);
                    Game.SpriteBatch.End();
                }

                if (lastKnownItem.MetaItem.CraftProp != null && lastKnownItem.MetaItem.Faith != null)
                    statModifier.Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            button.Update(gameTime);

            if (lastKnownItem != actor?.Inventory.SelectedItem)
                InvalidateItem(gameTime);
            else
                InvalidateItemAmount(gameTime, false);

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

            // Use item
            if (InputBindings.UseItem.IsPressed(PlayerIndex.One))
            {
                actor.UseSelectedItem();
                return HandleInputResult.Handled;
            }

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
        public bool IsVisible => actor != null && !actor.Inventory.IsEmpty && actor.Session.IsCurrentScene;
    }
}
