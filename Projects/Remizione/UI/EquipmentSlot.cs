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

        private Actor? actor;
        private readonly TextSprite amountText;
        private readonly UITextButton button;
        private readonly ImageSprite itemImage;
        private readonly Vector2Tween itemImageScaleTween = new();
        private int lastKnownCount;
        private Item? lastKnownItem;
        private readonly InputBinding nextInputBinding;
        private readonly InputBinding previousInputBinding;
        private readonly GameSession session;
        private readonly ImageSprite slotImage;

        #endregion

        #region Constructor

        // Constructor
        protected EquipmentSlot(GameSession session, Vector2 position, InventoryCategory inventoryCategory, InputBinding inputBinding, bool horizontalCycle)
            : base(session.Game)
        {
            this.session = session;

            this.InventoryCategory = inventoryCategory;
            this.HorizontalCycle = horizontalCycle;

            if (horizontalCycle)
            {
                previousInputBinding = InputBindings.SelectLeft;
                nextInputBinding = InputBindings.SelectRight;
            }
            else
            {
                previousInputBinding = InputBindings.SelectUp;
                nextInputBinding = InputBindings.SelectDown;
            }

            // Slot image
            this.slotImage = new ImageSprite(Game, Atlases.UI.EquipmentSlot)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = position
            };

            // Item image
            this.itemImage = new ImageSprite(Game)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Center, 0, -1),
                Scale = ScaleInfo.UIElement.Small
            };

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
                ImageName = $"{inventoryCategory}Slot",
                PivotOrigin = RectanglePoint.LeftBottom,
            };

            if (horizontalCycle)
                button.Position = slotImage.BoundingBox.GetPoint(RectanglePoint.LeftBottom, -4, 0);
            else
                button.Position = slotImage.BoundingBox.GetPoint(RectanglePoint.RightBottom, -4, 0);

            InvalidateItem();
        }

        #endregion

        #region Private members

        // InvalidateItem
        private void InvalidateItem()
        {
            lastKnownItem = actor?.Inventory.GetContainer(InventoryCategory).SelectedItem;

            if (lastKnownItem != null)
            {
                itemImage.Image = lastKnownItem.MetaItem.Image;
                itemImageScaleTween.Start(TweenStyle.Linear, new Vector2(.3f), ScaleInfo.UIElement.Small, 70);
                itemImage.Tweens.ScaleTween = itemImageScaleTween;
                InvalidateItemAmount(true);
            }
            else
            {
                itemImage.Image = Atlases.UI.GetImage($"InventorySlot{InventoryCategory}Icon");
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

        // SelectNext
        private bool SelectNext()
        {
            if (actor != null)
                return actor.Inventory.GetContainer(InventoryCategory).SelectNext() != null;
            else
                return false;
        }

        // SelectPrevious
        private bool SelectPrevious()
        {
            if (actor != null)
                return actor.Inventory.GetContainer(InventoryCategory).SelectPrevious() != null;
            else
                return false;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.Room is not ProceduralRoom)
                return;

            if (!IsVisible)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            slotImage.Draw(gameTime);
            itemImage.Draw(gameTime);
            Game.SpriteBatch.End();

            if (!HideButton)
                button.Draw(gameTime);

            if (lastKnownItem?.MetaItem.Unique == false)
            {
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
                amountText.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            if (lastKnownItem != actor?.Inventory.GetContainer(InventoryCategory).SelectedItem)
                InvalidateItem();
            else
                InvalidateItemAmount(false);

            button.Update(gameTime);
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
                    lastKnownCount = -1;
                    lastKnownItem = null;
                    InvalidateItem();
                }
            }
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (actor == null || session.IsAwaiting || session.Room is not ProceduralRoom)
                return HandleInputResult.Unhandled;

            // Use item
            if (button.TestPressed(PlayerIndex.One))
            {
                if (lastKnownItem == null)
                {
                    if (actor.Inventory.GetContainer(InventoryCategory).SelectPrevious() == null)
                        Sound.Play(SoundNames.Error);
                    else
                        Sound.Play(SoundNames.UIHover);
                }
                else
                    actor.UseSelectedItem(InventoryCategory);

                return HandleInputResult.Handled;
            }

            // Previous item
            if (previousInputBinding.IsPressed(PlayerIndex.One))
            {
                if (SelectPrevious())
                    Sound.Play(SoundNames.UIHover);

                return HandleInputResult.Handled;
            }

            // Next item
            else if (nextInputBinding.IsPressed(PlayerIndex.One))
            {
                if (SelectNext())
                    Sound.Play(SoundNames.UIHover);

                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // HideButton
        public bool HideButton { get; set; }

        // HorizontalCycle
        public bool HorizontalCycle { get; }

        // InventoryCategory
        public InventoryCategory InventoryCategory { get; }

        // IsVisible
        public bool IsVisible => SceneScope == null || Game.SceneManager.CurrentScene == SceneScope;

        // SceneScope
        public Scene? SceneScope { get; set; }
    }
}
