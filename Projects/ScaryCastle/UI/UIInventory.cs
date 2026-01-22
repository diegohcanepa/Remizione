using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// UIInventory
    /// </summary>
    public sealed class UIInventory : GameObject, IInputHandler
    {
        #region Private fields

        private readonly TextSprite[] amounts;
        private readonly ImageSprite bottomGradient;
        private readonly TextSprite diceAmountText;
        private readonly ImageSprite diceIcon;
        private readonly string diceOptionName = TextRepository.GetValue("Misc.Dice");
        private readonly Vector2 diceIconOriginalScale = Vector2.One;
        private readonly Vector2 diceIconSelectedScale = Vector2.One * 1.1f;
        private readonly ImageSprite[] icons;
        private readonly TextSprite itemDescription;
        private readonly TextSprite itemName;
        private int lastSeenDiceBagVersion = -1;
        private int lastSeenInventoryVersion = -1;
        private readonly GameSession session;
        private readonly ImageSprite[] shadows;
        private readonly ImageSprite[] slots;

        #endregion

        #region Constructor

        // Constructor
        public UIInventory(GameSession session)
            : base(session.Game)
        {
            this.session = session;
            this.amounts = new TextSprite[Inventory.MaximumCapacity];
            this.icons = new ImageSprite[Inventory.MaximumCapacity];
            this.shadows = new ImageSprite[Inventory.MaximumCapacity];
            this.slots = new ImageSprite[Inventory.MaximumCapacity];

            // Bottom gradient
            bottomGradient = new ImageSprite(Game, Atlases.UI.GetImage("InventoryContainer"))
            {
                Opacity = .8f,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom),
            };

            // Dice icon
            this.diceIcon = new(Game, Atlases.UI.Dice)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom, 7, -11),
            };

            // Dice amount
            this.diceAmountText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Top,
                Position = diceIcon.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -1),
                Scale = ScaleInfo.Text.ExtraLarge,
                Spacing = -6
            };

            // Slots
            for (var i = 0; i < slots.Length; i++)
            {
                slots[i] = new(Game, Atlases.UI.InventorySlot)
                {
                    PivotOrigin = RectanglePoint.LeftBottom,
                    Y = Screen.Area.Bottom - 10
                };

                icons[i] = new(Game)
                {
                    PivotOrigin = RectanglePoint.Center,
                    Scale = ScaleInfo.UIElement.Medium,
                    Y = slots[i].BoundingBox.Center.Y
                };

                shadows[i] = new(Game)
                {
                    Color = Color.Black,
                    Opacity = ColorPalette.ShadowOpacity,
                    PivotOrigin = RectanglePoint.Center,
                    Scale = ScaleInfo.UIElement.Medium,
                    Y = slots[i].BoundingBox.Center.Y + 1
                };

                // Amount text
                amounts[i] = new(Game, Fonts.CommonOutline)
                {
                    Color = ColorPalette.Text.Highlight,
                    PivotOrigin = RectanglePoint.Top,
                    Y = slots[i].BoundingBox.Center.Y + 4,
                    Scale = ScaleInfo.Text.Large
                };
            }

            // Item name
            this.itemName = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Bottom,
                Y = slots[0].BoundingBox.Top - 3,
                Scale = ScaleInfo.UISentence
            };

            // Item description
            this.itemDescription = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Bottom,
                MaximumLines = 2,
                MaximumWidth = 140,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom, 0, -5),
                Scale = ScaleInfo.Text.Large
            };
        }

        #endregion

        #region Private members

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (!IsVisible)
                return false;

            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (MouseCursor.Item == null && GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item grabbedItem)
                {
                    if (grabbedItem.Definition.Image != null)
                    {
                        session.Player?.Stand();
                        Sound.Play(SoundNames.Interact);
                        MouseCursor.Item = grabbedItem;
                        MouseCursor.AnimateClick();
                        IsVisible = false;
                        return true;
                    }
                }
                else
                {
                    MouseCursor.Shake();
                    Sound.Play(SoundNames.Error);
                }
            }

            if (MouseCursor.Item == null && InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                if (session.Player != null && GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item itemToDrop)
                {
                    session.Inventory.DropItem(itemToDrop, session.Player.Position);
                }
            }

            return false;
        }

        // Refresh
        private void Refresh()
        {
            float screenWidth = Screen.NativeWidth;
            int slotCount = session.Inventory.Capacity;
            float slotWidth = slots[0].BoundingBox.Width;
            float spacing = 1;

            float rowWidth = (slotCount * slotWidth) + ((slotCount - 1) * spacing);
            float startingX = (screenWidth - rowWidth) / 2;

            for (int i = 0; i < slotCount; i++)
            {
                slots[i].X = startingX + (i * (slotWidth + spacing));
                icons[i].Image = null;
                shadows[i].Image = null;
                amounts[i].Text = null;

                if (i < session.Inventory.Count)
                {
                    icons[i].X = slots[i].BoundingBox.Center.X;
                    icons[i].Image = session.Inventory[i].Definition.Image;

                    shadows[i].X = icons[i].X - 1;
                    shadows[i].Image = session.Inventory[i].Definition.Image;

                    amounts[i].X = icons[i].X;
                    amounts[i].Text = session.Inventory[i].Definition.IsStackable ? session.Inventory[i].Count.ToString(CultureInfo.InvariantCulture) : null;
                }
            }

            MouseCursor.Item = MouseCursor.Item;
        }

        // RefreshDiceBag
        private void RefreshDiceBag()
        {
            lastSeenDiceBagVersion = session.Deck.Count;
            diceAmountText.Text = $"{session.Deck.Count}/{session.Deck.Capacity}";
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            // Gradient
            bottomGradient.Draw(gameTime);

            diceIcon.Draw(gameTime);
            diceAmountText.Draw(gameTime);

            for (var i = 0; i < session.Inventory.Capacity; i++)
            {
                slots[i].Draw(gameTime);

                if (MouseCursor.Item?.Index == i)
                {
                    if (!session.Inventory[i].Definition.IsStackable)
                        continue;
                }

                shadows[i].Draw(gameTime);
                icons[i].Draw(gameTime);
                amounts[i].Draw(gameTime);
            }

            itemName.Draw(gameTime);
            //itemDescription.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!IsVisible)
            {
                if (InputManager.DefaultPlayer.Mouse.VirtualPosition.Y > 130)
                {
                    session.Player?.Stand();    
                    IsVisible = true;
                    MouseCursor.Item = null;
                    return;
                }
            }
            else
            {
                if (InputManager.DefaultPlayer.Mouse.VirtualPosition.Y < 105)
                {
                    IsVisible = false;
                    return;
                }
            }

            if (!IsVisible)
                return;

            if (lastSeenInventoryVersion != session.Inventory.ContentVersion)
            {
                lastSeenInventoryVersion = session.Inventory.ContentVersion;
                Refresh();
            }

            if (lastSeenDiceBagVersion != session.Deck.ContentVersion)
            {
                lastSeenDiceBagVersion = session.Deck.ContentVersion;
                RefreshDiceBag();
            }

            diceIcon.Update(gameTime);

            for (var i = 0; i < session.Inventory.Count; i++)
            {
                icons[i].Scale = ScaleInfo.UIElement.Medium;
            }

            var cursorOverDeckIcon = diceIcon.BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition);

            if (GetSelectedItem() is Item item)
            {
                itemName.Text = item.Definition.LocalizedDisplayName;
                itemName.X = slots[item.Index].BoundingBox.Center.X;

                itemDescription.Text = item.Definition.LocalizedDescription;

                icons[item.Index].Scale = ScaleInfo.InventoryHeldItem;
            }
            else if (cursorOverDeckIcon)
            {
                itemName.X = diceIcon.BoundingBox.GetPoint(RectanglePoint.Top).X;
                itemName.Text = diceOptionName;

                if (itemName.BoundingBox.Left < 0)
                    itemName.X += Math.Abs(itemName.BoundingBox.Left) + 4;

            }
            else
            {
                itemName.Text = null;
                itemDescription.Text = null;
            }

            if (cursorOverDeckIcon)
            {
                if (diceIcon.Scale != diceIconSelectedScale)
                {
                    diceIcon.Scale = diceIconSelectedScale;
                }
            }
            else if (diceIcon.Scale == diceIconSelectedScale)
            {
                diceIcon.Scale = diceIconOriginalScale;
            }
        }

        #endregion

        // GetItemAt
        public Item? GetItemAt(Vector2 position)
        {
            for (int i = 0; i < session.Inventory.Count; i++)
            {
                if (slots[i].BoundingBox.Contains(position))
                    return i < session.Inventory.Count ? session.Inventory[i] : null;
            }

            return null;
        }

        // GetSelectedItem
        public Item? GetSelectedItem()
        {
            return GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition);
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            // Mouse input
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
            {
                if (HandleMouseInput())
                    return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // IsVisible
        public bool IsVisible
        {
            get;
            private set
            {
                if (field != value)
                {
                    field = value;
                    itemName.Text = null;
                    itemDescription.Text = null;
                }
            }
        }
    }
}
