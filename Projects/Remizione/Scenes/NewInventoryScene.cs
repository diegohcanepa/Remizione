using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class NewInventoryScene : Scene
    {
        #region Private fields

        private readonly TextSprite amountText;
        private const float animationSpeed = 14;
        private readonly ImageSprite bottomGradient;
        private readonly UITextButton buttonClose;
        private readonly UITextButton buttonViewAll;
        private readonly TextSprite itemNameText;
        private readonly List<InventoryItem> items = [];
        private int selectedIndex;
        private readonly ImageSprite slotImage;
        private static readonly Vector2 slotPosition = new(Screen.Center.X, Screen.HUDArea.Bottom - 18);
        private const int spaceBetweenIcons = 15;
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 150 };
        private const int visibleRange = 13;
        private float visualIndex;

        #endregion

        #region Constructor

        // Constructor
        public NewInventoryScene(Actor owner)
            : base(owner.Game)
        {
            this.Owner = owner;

            // Bottom gradient
            bottomGradient = new ImageSprite(owner.Game, Atlases.UI.BottomGradient)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom),
                Scale = new Vector2(1, 1.2f)
            };

            // SlotImage
            this.slotImage = new(Game, Atlases.UI.InventorySlot)
            {
                PivotOrigin = RectanglePoint.Middle,
                Position = slotPosition
            };

            // Item name
            itemNameText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Top, 0, -1),
                Scale = ScaleInfo.Text.ExtraLarge
            };

            // Amount text
            amountText = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Top,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -1),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Close button
            buttonClose = new UITextButton(owner.Game, InputBindings.Close)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -2),
                //Small = true
            };

            // View all button
            buttonViewAll = new UITextButton(owner.Game, InputBindings.ViewAll)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = buttonClose.BoundingBox.GetPoint(RectanglePoint.RightBottom, 0, -10),
                //Small = true
            };
        }

        #endregion

        #region Private members

        // DrawItems
        private void DrawItems(GameTime gameTime)
        {
            for (int i = -visibleRange; i <= visibleRange; i++)
            {
                int index = (int)visualIndex + i;
                if (index < 0 || index >= items.Count)
                    continue;

                // desplazamiento relativo animado
                float offset = i - (visualIndex - (int)visualIndex);
                items[index].Position = slotPosition + new Vector2(offset * spaceBetweenIcons, 0);

                // Escala y opacidad basadas en distancia
                float distance = MathF.Abs(offset);
                float scale = MathF.Max(.6f, .8f - distance * .2f); // escala mínima 0.6
                float alpha = MathF.Max(.3f, 1 - distance * .3f); // transparencia mínima 0.3

                items[index].Opacity = alpha;
                items[index].Scale = new(scale);

                items[index].Draw(gameTime);
            }
        }

        // GetInventoryItemAt
        private InventoryItem? GetInventoryItemAt(Vector2 position)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].BoundingBox.Contains(position))
                    return items[i];
            }

            return null;
        }

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                SceneController.Pop();
                return true;
            }

            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                return false;

            if (GetInventoryItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is InventoryItem item)
            {
                Select(items.IndexOf(item));
                MouseCursor.Instance.AnimateClick();
                Sound.Play(SoundNames.UINavigation);
            }

            return false;
        }

        // Select
        private void Select(string name)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Item.Name == name)
                {
                    Select(i);
                    return;
                }
            }
        }

        // Select
        private void Select(int index)
        {
            if (index >= items.Count)
                return;

            selectedIndex = index;
            itemNameText.Text = items[index].Item.DisplayText;
            amountText.Text = items[index].Item.GetDisplayAmount();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsCurrentScene || Owner.Session.IsOutcomeInProgress)
                return;

            // Gradient
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            bottomGradient.Draw(gameTime);
            itemNameText.Draw(gameTime);
            amountText.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera);
            slotImage.Draw(gameTime);
            DrawItems(gameTime);
            Game.SpriteBatch.End();

            buttonClose.Draw(gameTime);
            buttonViewAll.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
            {
                if (HandleMouseInput())
                    return HandleInputResult.Handled;
            }

            // Close
            if (buttonClose.TestPressed(PlayerIndex.One))
            {
                SceneController.Pop();
                return HandleInputResult.Handled;
            }

            if (InputBindings.SelectLeft.IsPressed(PlayerIndex.One) || stick.IsLeft(PlayerIndex.One))
            {
                Select(Math.Max(0, selectedIndex - 1));
                Sound.Play(SoundNames.UINavigation);
                return HandleInputResult.Handled;
            }

            else if (InputBindings.SelectRight.IsPressed(PlayerIndex.One) || stick.IsRight(PlayerIndex.One))
            {
                Select(Math.Min(items.Count - 1, selectedIndex + 1));
                Sound.Play(SoundNames.UINavigation);
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Handled;
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            Owner.Stand();
            base.OnLoadContent();

            // Load items
            items.Clear();
            for (var i = 0; i < Owner.Inventory.Items.Count; i++)
            {
                var obj = Owner.Inventory.Items[i];
                if (obj.MetaItem.Category != MetaItemCategory.Misc)
                    items.Add(new(obj));
            }

            if (Owner.Inventory.SelectedItem is Item item)
            { 
                Select(item.Name);
                visualIndex = selectedIndex;
            }
        }

        // OnUnloadContent
        protected override void OnUnloadContent()
        {
            base.OnUnloadContent();

            if (selectedIndex >= 0)
                Owner.Inventory.Select(items[selectedIndex].Item);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            bottomGradient.Update(gameTime);
            stick.Update(gameTime);
            
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            visualIndex += (selectedIndex - visualIndex) * MathF.Min(1f, animationSpeed * deltaTime);

            base.OnUpdate(gameTime);

            buttonClose.Update(gameTime);
            buttonViewAll.Update(gameTime);
        }

        #endregion

        // Owner
        public Actor Owner { get; set; }

        // InventoryItem
        private sealed class InventoryItem : GameObject
        {
            private readonly ImageSprite image;

            // Constructor
            public InventoryItem(Item item)
                : base(item.Owner.Game)
            {
                this.Item = item;

                // Image
                this.image = new(Game, item.MetaItem.Image)
                {
                    PivotOrigin = RectanglePoint.Middle
                };
            }

            // OnDraw
            protected override void OnDraw(GameTime gameTime)
            {
                image.Draw(gameTime);
            }

            // OnUpdate
            protected override void OnUpdate(GameTime gameTime)
            {
            }

            // BoundingBox
            public RectangleF BoundingBox => image.BoundingBox;

            // Item
            public Item Item { get; }

            // Opacity
            public float Opacity
            {
                get => image.Opacity;
                set => image.Opacity = value;
            }

            // Position
            public Vector2 Position
            { 
                get => image.Position; 
                set => image.Position = value;
            }

            // Scale
            public Vector2 Scale
            {
                get => image.Scale;
                set => image.Scale = value;
            }
        }
    }
}
