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
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 2, -2),
            };

            // Item image
            this.itemImage = new ImageSprite(Game)
            {
                PivotOrigin = RectanglePoint.Middle,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Middle),
                Scale = ScaleInfo.UIElement.Medium
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
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.RightBottom, -3, -2),
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
                itemImageScaleTween.Start(TweenStyle.Linear, new Vector2(.3f), ScaleInfo.UIElement.Medium, 70);
                itemImage.Tweens.ScaleTween = itemImageScaleTween;
                button.Text = null;
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
                button.IsEnabled = true;
            }
        }

        // SelectFirst
        private bool SelectFirst()
        {
            if (actor != null)
                return actor.Inventory.SelectFirst(MetaItemCategory.Equipment) != null;
            else
                return false;
        }

        // SelectLast
        private bool SelectLast()
        {
            if (actor != null)
                return actor.Inventory.SelectLast(MetaItemCategory.Equipment) != null;
            else
                return false;
        }

        // SelectNext
        private bool SelectNext()
        {
            if (actor != null)
                return actor.Inventory.SelectNext(MetaItemCategory.Equipment) != null;
            else
                return false;
        }

        // SelectPrevious
        private bool SelectPrevious()
        {
            if (actor != null)
                return actor.Inventory.SelectPrevious(MetaItemCategory.Equipment) != null;
            else
                return false;
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
                //button.Draw(gameTime);

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

            // First item
            if (InputBindings.SelectUp.IsPressed(PlayerIndex.One))
            {
                if (SelectFirst())
                    Sound.Play(SoundNames.UIHover);

                return HandleInputResult.Handled;
            }

            // Last item
            else if (InputBindings.SelectDown.IsPressed(PlayerIndex.One))
            {
                if (SelectLast())
                    Sound.Play(SoundNames.UIHover);

                return HandleInputResult.Handled;
            }

            // Previous item
            else if (InputBindings.SelectLeft.IsPressed(PlayerIndex.One))
            {
                if (SelectPrevious())
                    Sound.Play(SoundNames.UIHover);

                return HandleInputResult.Handled;
            }

            // Last item
            else if (InputBindings.SelectRight.IsPressed(PlayerIndex.One))
            {
                if (SelectNext())
                    Sound.Play(SoundNames.UIHover);

                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // IsVisible
        public bool IsVisible => actor != null && !actor.Inventory.IsEmpty && actor.Session.IsCurrentScene;
    }
}
