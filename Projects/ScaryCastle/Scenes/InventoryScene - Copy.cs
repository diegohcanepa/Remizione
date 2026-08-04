using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// InventorySceneNew
    /// </summary>
    public sealed class InventorySceneNew : Scene, IInputHandler
    {
        #region Private fields

        private readonly Sprite[] amounts = new Sprite[ItemContainer.MaximumCapacity];
        private bool autoHide;
        private readonly DynamicWindow descriptionWindow;
        private readonly Sprite[] icons = new Sprite[ItemContainer.MaximumCapacity];
        private readonly TextSprite itemDescription;
        private readonly TextSprite itemLabel;
        private Item? lastSelectedItem;
        private int lastSeenContainerVersion = -1;
        private readonly Sprite leftMouseTipIcon;
        private readonly TextSprite leftMouseTip;
        private readonly Sprite rightMouseTipIcon;
        private readonly TextSprite rightMouseTip;
        private readonly Sprite[] shadows = new Sprite[ItemContainer.MaximumCapacity];
        private readonly Sprite[] slots = new Sprite[ItemContainer.MaximumCapacity];

        #endregion

        #region Constructor

        // Constructor
        public InventorySceneNew(ItemContainer itemContainer)
            : base()
        {
            this.PausePreviousScenes = true;

            // Description window
            this.descriptionWindow = new DynamicWindow()
            {
                BorderColor = new(20, 20, 20),
                FillColor = new(45, 37, 30),
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom),
                Height = 28,
                Width = 140
            };

            // Slots
            for (var i = 0; i < slots.Length; i++)
            {
                slots[i] = new(Atlases.UI.InventoryItemSlot)
                {
                    PivotOrigin = RectanglePoint.LeftBottom,
                    Y = Screen.Area.Bottom - 37
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
                    Y = slots[i].BoundingBox.Center.Y + 5,
                    Scale = ScaleInfo.UIElement.Medium
                };
            }

            this.ItemContainer = itemContainer;

            // Item name
            itemLabel = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Bottom,
                Position = slots[0].BoundingBox.GetPoint(RectanglePoint.Top, 0, -1),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Item description
            this.itemDescription = new(Fonts.Common)
            {
                Color = ColorPalette.Text.Highlight,
                Opacity = .7f,
                MaximumWidth = (int)descriptionWindow.InnerBounds.Width,
                Multiline = true,
                PivotOrigin = RectanglePoint.LeftTop,
                Position = descriptionWindow.InnerBounds.GetPoint(RectanglePoint.LeftTop),
                Scale = ScaleInfo.Text.Large,
                ShadowColor = ColorPalette.Shadow,
                ShadowOffset = new(.5f),
            };

            // LeftMouseTip
            leftMouseTip = new(Fonts.Common)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 10, -11),
                Scale = ScaleInfo.Text.Large,
                Text = "@Verb.Select"
            };

            // LeftMouseTipIcon
            this.leftMouseTipIcon = new(Atlases.UI.MouseLeftButtonIcon)
            {
                PivotOrigin = RectanglePoint.Right,
                Position = leftMouseTip.BoundingBox.GetPoint(RectanglePoint.Left, -1, -1),
                Scale = ScaleInfo.UIElement.Medium
            };

            // RightMouseTip
            this.rightMouseTip = new(Fonts.Common)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 10, -1),
                Scale = ScaleInfo.Text.Large,
                Text = "@Verb.Grab"
            };

            // RightMouseTipIcon
            this.rightMouseTipIcon = new(Atlases.UI.MouseRightButtonIcon)
            {
                PivotOrigin = RectanglePoint.Right,
                Position = rightMouseTip.BoundingBox.GetPoint(RectanglePoint.Left, -1, -1),
                Scale = ScaleInfo.UIElement.Medium
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
                    SelectedItem = grabbedItem;

                   // if (grabbedItem.Definition.Image != null)
                    //{
                      //  ItemContainer.Session.InteractionContext.HeldItem = grabbedItem;
                        MouseCursor.PerformClick(false);
                        //Game.SceneManager.Pop();
                        return true;
                    //}
                }

                //else if (ItemContainer.Session.InteractionContext.HeldItem == null && GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item grabbedItem)
                //{
                //    if (grabbedItem.Definition.Image != null)
                //    {
                //        ItemContainer.Session.InteractionContext.HeldItem = grabbedItem;
                //        MouseCursor.PerformClick(false);
                //        Game.SceneManager.Pop();
                //        return true;
                //    }
                //}
                else
                {
                    MouseCursor.Shake();
                }
            }

            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                if (GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item item)
                    ItemContainer.Session.InteractionContext.HeldItem = item;

                MouseCursor.PerformClick(false);
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
                        amounts[i].RenderImage = Atlases.UI.InventoryFaithAmounts[ItemContainer[i].Definition.EnergyCost - 1];
                    }
                    else if (ItemContainer[i].Definition.IsStackable || ItemContainer[i].Definition.IsDepletable)
                    {
                        if (ItemContainer[i].Amount.IsBetween(1, 5))
                        {
                            amounts[i].RenderImage = Atlases.UI.InventoryItemAmounts[ItemContainer[i].Amount - 1];
                        }
                    }
                }
            }
        }

        // Reset
        private void Reset()
        {
            SelectedItem = null;

            for (var i = 0; i < ItemContainer.Count; i++)
            {
                icons[i].Scale = ScaleInfo.UIElement.Medium;
                shadows[i].Scale = ScaleInfo.UIElement.Medium;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsCurrentScene)
                return;

            Game.SpriteBatch.Begin(Game.Camera);

            Game.Shapes.DrawRectangle(Screen.Area, ColorPalette.SceneShade);

            descriptionWindow.Draw(gameTime);

            leftMouseTipIcon.Draw(gameTime);
            leftMouseTip.Draw(gameTime);

            rightMouseTipIcon.Draw(gameTime);
            rightMouseTip.Draw(gameTime);

            itemDescription.Draw(gameTime);

            //scroll.Draw(gameTime);
            //goalText.Draw(gameTime);

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

            if (SelectedItem != null)
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

            MouseCursor.Icon = MouseCursorIcon.Arrow;

            Reset();

            autoHide = false;

            ItemContainer.Session.InteractionContext.HeldItem = null;

            if (lastSeenContainerVersion != ItemContainer.ContentVersion)
            {
                lastSeenContainerVersion = ItemContainer.ContentVersion;
                Refresh();
            }

            if (ItemContainer.Count > 0)
                SelectedItem = ItemContainer[0];
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
        }

        #endregion

        // AutoHideThreshold
        public const int AutoHideThreshold = 74;

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

        // SelectedItem
        public Item? SelectedItem
        {
            get;
            set
            {
                if (value != field)
                {
                    if (field != null)
                    {
                        icons[field.Index].Scale = ScaleInfo.UIElement.Medium;
                        shadows[field.Index].Scale = ScaleInfo.UIElement.Medium;
                    };

                    field = value;

                    if (field != null)
                    {
                        itemLabel.X = slots[field.Index].BoundingBox.Center.X;
                        itemLabel.Text = field.DisplayName;
                        itemDescription.Text = field.ShortDescription;
                        icons[field.Index].Scale = ScaleInfo.InventoryHeldItem;
                        shadows[field.Index].Scale = ScaleInfo.InventoryHeldItem;
                    }
                    else
                    {
                        itemLabel.Clear();
                    }
                }
            }
        }
    }
}
