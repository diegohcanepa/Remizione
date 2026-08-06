using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// InventoryScene2
    /// </summary>
    public sealed class InventoryScene : Scene, IInputHandler
    {
        #region Private fields

        private readonly Sprite[] amounts = new Sprite[ItemContainer.MaximumCapacity];
        private bool autoHide;
        private readonly Sprite background = new(Atlases.UI.QuickInventoryBackground) { PivotOrigin = RectanglePoint.LeftBottom, Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom) };
        private readonly Sprite[] icons = new Sprite[ItemContainer.MaximumCapacity];
        private readonly TextSprite itemLabel;
        private readonly TextSprite itemDescription;
        private Item? lastSelectedItem;
        private int lastSeenContainerVersion = -1;
        private readonly Sprite[] shadows = new Sprite[ItemContainer.MaximumCapacity];
        private readonly Sprite[] slots = new Sprite[ItemContainer.MaximumCapacity];

        #endregion

        #region Constructor

        // Constructor
        public InventoryScene(ItemContainer itemContainer)
            : base()
        {
            this.PausePreviousScenes = false;

            // Slots
            for (var i = 0; i < slots.Length; i++)
            {
                slots[i] = new(Atlases.UI.InventoryItemSlot)
                {
                    PivotOrigin = RectanglePoint.LeftBottom,
                    Y = Screen.Area.Bottom - 22
                };

                icons[i] = new()
                {
                    PivotOrigin = RectanglePoint.Center,
                    Y = slots[i].BoundingBox.Center.Y - .5f
                };

                shadows[i] = new()
                {
                    Color = Color.Black,
                    Opacity = ColorPalette.ShadowOpacity,
                    PivotOrigin = RectanglePoint.Center,
                    Scale = ScaleInfo.UIElement.Medium,
                    Y = slots[i].BoundingBox.Center.Y + .5f
                };

                // Amount
                amounts[i] = new Sprite()
                {
                    PivotOrigin = RectanglePoint.Top,
                    Scale = ScaleInfo.UIElement.Medium
                };
            }

            this.ItemContainer = itemContainer;

            // Item name
            itemLabel = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.MouseCursor,
                PivotOrigin = RectanglePoint.Bottom,
                Position = slots[0].BoundingBox.GetPoint(RectanglePoint.Top, 0, -1),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Item description
            itemDescription = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                MaximumWidth = 200,
                Multiline = false,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -2),
                Scale = ScaleInfo.Text.Large,
            };
        }

        #endregion

        #region Private members

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (InputManager.DefaultPlayer.Mouse.VirtualPosition.Y < AutoHideThreshold)
                {
                    MouseCursor.PerformClick(false);
                    Game.SceneManager.Pop();
                }
                else if (ItemContainer.Session.InteractionContext.HeldItem == null && GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item grabbedItem)
                {
                    if (grabbedItem.Definition.Image != null)
                    {
                        ItemContainer.Session.InteractionContext.HeldItem = grabbedItem;
                        MouseCursor.PerformClick(false);
                        Game.SceneManager.Pop();
                        return true;
                    }
                }
                else
                {
                    MouseCursor.Shake();
                }
            }

            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                MouseCursor.PerformClick();
                Game.SceneManager.Pop();
            }

            return false;
        }

        // Refresh
        private void Refresh()
        {
            float screenWidth = Screen.NativeWidth;
            int slotCount = ItemContainer.Capacity;
            float slotWidth = slots[0].BoundingBox.Width;
            float spacing = 1;

            float rowWidth = (slotCount * slotWidth) + ((slotCount - 1) * spacing);
            float startingX = (screenWidth - rowWidth) / 2;

            for (int i = 0; i < slotCount; i++)
            {
                slots[i].X = startingX + (i * (slotWidth + spacing));
                icons[i].RenderImage = null;
                shadows[i].RenderImage = null;
                amounts[i].RenderImage = null;

                if (i < ItemContainer.Count)
                {
                    icons[i].X = slots[i].BoundingBox.Center.X;
                    icons[i].RenderImage = ItemContainer[i].Definition.Image;

                    shadows[i].X = icons[i].X - .5f;
                    shadows[i].RenderImage = ItemContainer[i].Definition.Image;

                    amounts[i].X = icons[i].X;

                    if (ItemContainer[i].Definition.EnergyCost.IsBetween(1, 3))
                    {
                        amounts[i].Scale = ScaleInfo.UIElement.Large;
                        amounts[i].Y = slots[i].BoundingBox.Center.Y + 8;
                        amounts[i].RenderImage = Atlases.UI.FaithCosts[ItemContainer[i].Definition.EnergyCost - 1];
                    }
                    else if (ItemContainer[i].Definition.IsStackable || ItemContainer[i].Definition.IsDepletable)
                    {
                        if (ItemContainer[i].Amount.IsBetween(1, 5))
                        {
                            amounts[i].Scale = ScaleInfo.UIElement.Medium;
                            amounts[i].Y = slots[i].BoundingBox.Center.Y + 6;
                            amounts[i].RenderImage = Atlases.UI.InventoryItemAmounts[ItemContainer[i].Amount - 1];
                        }
                    }
                }
            }
        }

        // Reset
        private void Reset()
        {
            itemLabel.Clear();

            for (var i = 0; i < ItemContainer.Count; i++)
            {
                icons[i].Scale = ScaleInfo.UIElement.Medium;
                shadows[i].Scale = ScaleInfo.UIElement.Medium;
            }
        }

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();
            MouseCursor.Icon = MouseCursorIcon.Cross;
            Reset();
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsCurrentScene)
                return;

            Game.SpriteBatch.Begin(Game.Camera);

            background.Draw(gameTime);

            itemDescription.Draw(gameTime);

            for (var i = 0; i < ItemContainer.Capacity; i++)
            {
                slots[i].Draw(gameTime);

                if (ItemContainer.Session.InteractionContext.HeldItem?.Index == i)
                {
                    if (!ItemContainer[i].Definition.IsStackable)
                        continue;
                }

                shadows[i].Draw(gameTime);
                icons[i].Draw(gameTime);
                amounts[i].Draw(gameTime);
            }

            if (lastSelectedItem != null)
                itemLabel.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            // Mouse input
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
            {
                if (HandleMouseInput())
                    return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();

            autoHide = false;

            ItemContainer.Session.InteractionContext.HeldItem = null;

            if (lastSeenContainerVersion != ItemContainer.ContentVersion)
            {
                lastSeenContainerVersion = ItemContainer.ContentVersion;
                Refresh();
            }
        }

        // OnUnloadContent
        protected override void OnUnloadContent()
        {
            Reset();
            base.OnUnloadContent();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (autoHide)
            {
                if (InputManager.DefaultPlayer.Mouse.VirtualPosition.Y < AutoHideThreshold)
                {
                    Game.SceneManager.Pop();
                    return;
                }
            }
            else
            {
                autoHide = InputManager.DefaultPlayer.Mouse.VirtualPosition.Y >= AutoHideThreshold;
            }

            if (GetSelectedItem() is Item item)
            {
                if (item != lastSelectedItem)
                {
                    if (lastSelectedItem?.Index >= 0)
                    {
                        icons[lastSelectedItem.Index].Scale = ScaleInfo.UIElement.Medium;
                        shadows[lastSelectedItem.Index].Scale = ScaleInfo.UIElement.Medium;
                    }
                    else
                    {
                        lastSelectedItem = null;
                    }

                    itemLabel.X = slots[item.Index].BoundingBox.Center.X;
                    itemLabel.Text = item.DisplayName;
                    itemDescription.Text = item.ShortDescription;
                    icons[item.Index].Scale = ScaleInfo.InventoryHeldItem;
                    shadows[item.Index].Scale = ScaleInfo.InventoryHeldItem;
                    itemLabel.Tag = item;
                    lastSelectedItem = item;
                }
            }
            else if (lastSelectedItem != null)
            {
                if (lastSelectedItem.Index >= 0)
                {
                    icons[lastSelectedItem.Index].Scale = ScaleInfo.UIElement.Medium;
                    shadows[lastSelectedItem.Index].Scale = ScaleInfo.UIElement.Medium;
                }

                lastSelectedItem = null;
            }
        }

        #endregion

        // AutoHideThreshold
        public const int AutoHideThreshold = 98;

        // ItemContainer
        public ItemContainer ItemContainer
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Refresh();
                }
            }
        }

        // GetItemAt
        public Item? GetItemAt(Vector2 position)
        {
            for (int i = 0; i < ItemContainer.Count; i++)
            {
                if (slots[i].BoundingBox.Contains(position))
                    return i < ItemContainer.Count ? ItemContainer[i] : null;
            }

            return null;
        }

        // GetSelectedItem
        public Item? GetSelectedItem()
        {
            return GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition);
        }
    }
}
