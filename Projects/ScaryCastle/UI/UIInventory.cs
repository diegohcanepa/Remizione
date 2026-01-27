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
        private readonly TextSprite cardAmountText;
        private readonly ImageSprite deckIcon;
        private readonly string deckOptionName = TextRepository.GetValue("Misc.Deck");
        private readonly Vector2 deckIconOriginalScale = Vector2.One;
        private readonly Vector2 deckIconSelectedScale = Vector2.One * 1.1f;
        private readonly ImageSprite[] icons;
        private readonly TextSprite itemName;
        private int lastSeenDeckVersion = -1;
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

            // Deck icon
            this.deckIcon = new(Game, Atlases.UI.Deck)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom, 7, -9),
            };

            // Card amount
            this.cardAmountText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Top,
                Position = deckIcon.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 1),
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
                    Y = slots[i].BoundingBox.Center.Y + 5,
                    Scale = ScaleInfo.Text.ExtraLarge
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
                        MouseCursor.Item = grabbedItem;
                        MouseCursor.PerformClick();
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

            /*
            if (MouseCursor.Item == null && InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                if (session.Player != null && GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item itemToDrop)
                {
                    session.Inventory.DropItem(itemToDrop, session.Player.Position);
                }
            }
            */

            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                if (GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item item)
                {
                    session.ShowEcho(item.Definition.LocalizedDescription, item.Definition.Image);
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

        // RefreshDeck
        private void RefreshDeck()
        {
            lastSeenDeckVersion = session.Deck.Count;
            cardAmountText.Text = $"{session.Deck.Count}/{session.Deck.Capacity}";
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!session.IsCurrentScene)
                return;

            if (!IsVisible)
                return;

            // Gradient
            bottomGradient.Draw(gameTime);

            deckIcon.Draw(gameTime);
            cardAmountText.Draw(gameTime);

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
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!session.IsCurrentScene)
                return;

            if (lastSeenInventoryVersion != session.Inventory.ContentVersion)
            {
                lastSeenInventoryVersion = session.Inventory.ContentVersion;
                Refresh();
            }

            if (lastSeenDeckVersion != session.Deck.ContentVersion)
            {
                lastSeenDeckVersion = session.Deck.ContentVersion;
                RefreshDeck();
            }

            if (!IsVisible)
            {
                if (InputManager.DefaultPlayer.Mouse.VirtualPosition.Y > 130)
                {
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

            deckIcon.Update(gameTime);

            for (var i = 0; i < session.Inventory.Count; i++)
            {
                icons[i].Scale = ScaleInfo.UIElement.Medium;
            }

            var cursorOverDeckIcon = deckIcon.BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition);

            if (GetSelectedItem() is Item item)
            {
                itemName.Text = item.Definition.LocalizedDisplayName;
                itemName.X = slots[item.Index].BoundingBox.Center.X;
                icons[item.Index].Scale = ScaleInfo.InventoryHeldItem;
            }
            else if (cursorOverDeckIcon)
            {
                itemName.X = deckIcon.BoundingBox.GetPoint(RectanglePoint.Top).X;
                itemName.Text = deckOptionName;

                if (itemName.BoundingBox.Left < 0)
                    itemName.X += Math.Abs(itemName.BoundingBox.Left) + 4;

            }
            else
            {
                itemName.Text = null;
            }

            if (cursorOverDeckIcon)
            {
                if (deckIcon.Scale != deckIconSelectedScale)
                {
                    deckIcon.Scale = deckIconSelectedScale;
                }
            }
            else if (deckIcon.Scale == deckIconSelectedScale)
            {
                deckIcon.Scale = deckIconOriginalScale;
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
            if (!session.IsCurrentScene)
                return HandleInputResult.Unhandled;

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
                }
            }
        }
    }
}
