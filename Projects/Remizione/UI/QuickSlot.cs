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

            // TextButton
            this.button = new(game, InputBindings.UseItem)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.RightBottom, 0, -1),
                Small = true
            };
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

                if (lastKnownItem != null)
                {
                    itemImage.Image = lastKnownItem.MetaItem.Image;
                    itemImageScaleTween.Start(TweenStyle.Linear, new Vector2(.3f), ScaleInfo.UIElement.Tiny, 70);
                    itemImage.Tweens.ScaleTween = itemImageScaleTween;

                    if (lastKnownItem.MetaItem.Category == MetaItemCategory.Crafting)
                    {
                        if (actor != null && lastKnownItem.MetaItem.Craft is string entityName)
                        {
                            if (actor.Session.GetEntity<GameThing>(entityName) is GameThing thing)
                                button.Text = thing.LocalizedDisplayName;
                            else
                                button.Text = null;
                        }
                    }
                    else
                    {
                        amountText.Color = ColorPalette.Text.Default;
                    }
                }
            }

            if (lastKnownItem != null && lastKnownItem.Count != lastKnownCount)
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
            if (button.IsEnabled && InputBindings.UseItem.IsPressed(PlayerIndex.One))
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
