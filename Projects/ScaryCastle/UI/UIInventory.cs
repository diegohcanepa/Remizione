using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
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
        private readonly TextSprite deckAmountText;
        private readonly ImageSprite deckIcon;
        private readonly Vector2 deckIconOriginalScale = Vector2.One;
        private readonly Vector2 deckIconSelectedScale = Vector2.One * 1.1f;
        private readonly ImageSprite[] icons;
        private readonly TextSprite itemDescription;
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
            this.amounts = new TextSprite[Inventory.MaxCapacity];
            this.icons = new ImageSprite[Inventory.MaxCapacity];
            this.shadows = new ImageSprite[Inventory.MaxCapacity];
            this.slots = new ImageSprite[Inventory.MaxCapacity];

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

            // Deck amount
            this.deckAmountText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Top,
                Position = deckIcon.BoundingBox.GetPoint(RectanglePoint.Bottom),
                Scale = ScaleInfo.Text.Large,
                Spacing = -6
            };

            // Slots
            for (var i = 0; i < slots.Length; i++)
            {
                slots[i] = new(Game, Atlases.UI.InventorySlot)
                {
                    PivotOrigin = RectanglePoint.Bottom,
                    Y = Screen.Area.Bottom - 8
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
                Y = slots[0].BoundingBox.Top - 2,
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
                        Sound.Play(SoundNames.UISelectC);
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

            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed() && IsVisible)
            {
                if (MouseCursor.Item != null)
                {
                    MouseCursor.Item = null;
                    Sound.Play(SoundNames.UISelectD);
                }
                else
                {
                    IsVisible = false;
                }

                return true;
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
            deckAmountText.Text = $"{session.Deck.Count}/{session.Deck.Capacity}";
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            Game.SpriteBatch.Begin(Game.Camera);

            // Gradient
            bottomGradient.Draw(gameTime);

            deckIcon.Draw(gameTime);
            deckAmountText.Draw(gameTime);

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

            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!IsVisible)
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

            deckIcon.Update(gameTime);

            for (var i = 0; i < session.Inventory.Count; i++)
            {
                icons[i].Scale = ScaleInfo.UIElement.Medium;
            }

            if (GetSelectedItem() is Item item)
            {
                itemName.Text = item.Definition.LocalizedDisplayName;
                itemName.X = slots[item.Index].X;

                itemDescription.Text = item.Definition.LocalizedDescription;

                icons[item.Index].Scale = ScaleInfo.InventoryHeldItem;
            }
            else
            {
                itemName.Text = null;
                itemDescription.Text = null;
            }

            var cursorOverDeckIcon = deckIcon.BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition);   

            if (cursorOverDeckIcon)
            {
                if (deckIcon.Scale != deckIconSelectedScale)
                {
                    Sound.Play(SoundNames.CardFlap);
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
            set
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
