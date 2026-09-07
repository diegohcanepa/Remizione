using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class InventoryScene : Scene, IInputHandler
    {
        #region Private fields

        private readonly TextSprite[] amounts = new TextSprite[ItemContainer.MaximumCapacity];
        private bool autoHide;
        private readonly Sprite background = new(Atlases.UI.QuickInventoryBackground) { PivotOrigin = RectanglePoint.LeftBottom, Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom) };
        private readonly Sprite[] icons = new Sprite[ItemContainer.MaximumCapacity];
        private readonly TextSprite itemLabel;
        private readonly TextSprite itemDescription;
        private Item? lastSelectedItem;
        private int lastSeenContainerVersion = -1;
        private const int slotSize = 14;

        #endregion

        #region Constructor

        // Constructor
        public InventoryScene(ItemContainer itemContainer)
            : base()
        {
            this.PausePreviousScenes = false;

            // Icons
            for (var i = 0; i < icons.Length; i++)
            {
                icons[i] = new()
                {
                    PivotOrigin = RectanglePoint.Center,
                    Y = Screen.Area.Bottom - 18
                };

                // Amount
                amounts[i] = new(Fonts.Common)
                {
                    Color = ColorPalette.Text.Terra,
                    PivotOrigin = RectanglePoint.LeftBottom,
                    Scale = ScaleInfo.Text.Large
                };
            }

            this.ItemContainer = itemContainer;

            // Item name
            itemLabel = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.MouseCursor,
                PivotOrigin = RectanglePoint.Bottom,
                Y = icons[0].Y - slotSize,
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Item description
            itemDescription = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
                MaximumWidth = 200,
                Multiline = false,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -22),
                Scale = ScaleInfo.Text.Medium,
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
                if (GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item item)
                {
                    // Discard??
                }
            }

            return false;
        }

        // Refresh
        private void Refresh()
        {
            float screenWidth = Screen.NativeWidth;
            int slotCount = ItemContainer.Count;
            float spacing = 0;

            float rowWidth = (slotCount * slotSize) + ((slotCount - 1) * spacing);
            float startingX = (screenWidth - rowWidth) / 2;

            for (int i = 0; i < ItemContainer.Count; i++)
            {
                var x = startingX + (i * (slotSize + spacing)) + (slotSize / 2);
                icons[i].RenderImage = null;
                amounts[i].Text = null;

                if (i < ItemContainer.Count)
                {
                    icons[i].X = x;
                    icons[i].RenderImage = ItemContainer[i].Definition.Image;

                    amounts[i].Position = icons[i].BoundingBox.GetPoint(RectanglePoint.RightBottom, -3, 2);

                    if (ItemContainer[i].Definition.HPCost.IsBetween(1, 3))
                    {
                        amounts[i].Text = "x" + ItemContainer[i].Definition.HPCost.ToString(CultureInfo.InvariantCulture);
                    }
                    else if (ItemContainer[i].Definition.IsStackable || ItemContainer[i].Definition.IsDepletable)
                    {
                        if (ItemContainer[i].Amount.IsBetween(1, 5))
                        {
                            amounts[i].Text = "x" + ItemContainer[i].Amount.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
        }

        // Reset
        private void Reset()
        {
            itemLabel.Clear();

            MouseCursor.Tooltip = null;

            for (var i = 0; i < ItemContainer.Count; i++)
            {
                icons[i].Scale = ScaleInfo.UIElement.Large;
            }
        }

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();
            Reset();
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsCurrentScene)
                return;

            Game.SpriteBatch.Begin(Game.Camera);

            background.Draw(gameTime);

            for (var i = 0; i < ItemContainer.Count; i++)
            {
                //slots[i].Draw(gameTime);

                if (ItemContainer.Session.InteractionContext.HeldItem?.Index == i)
                {
                    if (!ItemContainer[i].Definition.IsStackable)
                        continue;
                }

                icons[i].Draw(gameTime);
                amounts[i].Draw(gameTime);
            }

            if (lastSelectedItem != null)
            {
                itemLabel.Draw(gameTime);
                itemDescription.Draw(gameTime);
            }

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

            if (lastSeenContainerVersion != ItemContainer.Version)
            {
                lastSeenContainerVersion = ItemContainer.Version;
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
                        icons[lastSelectedItem.Index].Scale = ScaleInfo.UIElement.Large;
                    }
                    else
                    {
                        lastSelectedItem = null;
                    }

                    itemLabel.X = icons[item.Index].BoundingBox.Center.X;
                    itemLabel.Text = item.Definition.DisplayName;
                    MouseCursor.Tooltip = item.Definition.DisplayName;
                    itemDescription.Text = item.Definition.EffectDescription;
                    icons[item.Index].Scale = ScaleInfo.InventoryHeldItem;
                    itemLabel.Tag = item;
                    lastSelectedItem = item;
                }
            }
            else if (lastSelectedItem != null)
            {
                if (lastSelectedItem.Index >= 0)
                    icons[lastSelectedItem.Index].Scale = ScaleInfo.UIElement.Large;

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
                if (icons[i].BoundingBox.Contains(position))
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
