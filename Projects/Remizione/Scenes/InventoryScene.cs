using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class InventoryScene : Scene, IInputHandler
    {
        #region Private fields

        private bool autoHide;
        private readonly Sprite background = new(Atlases.UI.QuickInventoryBackground) { PivotOrigin = RectanglePoint.LeftBottom, Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom) };
        private readonly Sprite inspectItem = new(Atlases.UI.InspectItem) { PivotOrigin = RectanglePoint.RightBottom, Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -2, -2) };
        private readonly TextSprite itemLabel;
        private readonly TextSprite itemEffect;
        private Item? lastSelectedItem;
        private int lastSeenContainerVersion = -1;
        private readonly InventorySlot[] slots = new InventorySlot[ItemContainer.MaximumCapacity];

        #endregion

        #region Constructor

        // Constructor
        public InventoryScene(ItemContainer itemContainer)
            : base()
        {
            this.PausePreviousScenes = false;

            for (var i = 0; i < slots.Length; i++)
            {
                slots[i] = new InventorySlot();
            }

            this.ItemContainer = itemContainer;

            // Item label
            itemLabel = new(Fonts.CommonOutline)
            {
                Color = Colors.MouseCursor.Tooltip,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Item effect
            itemEffect = new(Fonts.CommonOutline)
            {
                Color = Colors.Inventory.EffectDescription,
                MaximumWidth = 200,
                Multiline = false,
                PivotOrigin = RectanglePoint.Bottom,
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
                    Game.SceneManager.Pop();
                }
                else if (ItemContainer.Session.InteractionContext.HeldItem == null && GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item grabbedItem)
                {
                    if (grabbedItem.Definition.Image != null)
                    {
                        ItemContainer.Session.InteractionContext.HeldItem = grabbedItem;
                        Sound.Play(SoundNames.Interact);
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
                    Sound.Play(SoundNames.Interact);
                    ItemContainer.Session.Overlays.ShowItemInfo(item);
                }
                else
                {
                    MouseCursor.Shake();
                }
            }

            return false;
        }

        // Refresh
        private void Refresh()
        {
            lastSelectedItem = null;

            for (var i = 0; i < slots.Length; i++)
            {
                slots[i].Item = null;
            }

            float screenWidth = Screen.NativeWidth;
            int slotCount = ItemContainer.Count;
            float spacing = 2;
            float slotSize = slots[0].BoundingBox.Width;

            float rowWidth = (slotCount * slotSize) + ((slotCount - 1) * spacing);
            float startingX = (screenWidth - rowWidth) / 2;
            float y = Screen.Area.Bottom - 20;

            for (int i = 0; i < ItemContainer.MaximumCapacity; i++)
            {
                if (i < ItemContainer.Count)
                {
                    float x = startingX + (i * (slotSize + spacing)) + (slotSize / 2);
                    slots[i].Position = new Vector2(x, y);
                    slots[i].Item = ItemContainer[i];
                }
            }

            // Actualizar la posición Y del label y descripción basándonos en la posición real fijada para los slots
            if (ItemContainer.Count > 0)
            {
                itemLabel.Y = slots[0].BoundingBox.Top - 8;
                itemEffect.Y = slots[0].BoundingBox.Top - 2;
            }
        }

        // Reset
        private void Reset()
        {
            itemLabel.Clear();
            itemEffect.Clear();

            MouseCursor.Tooltip = null;

            for (var i = 0; i < ItemContainer.Count; i++)
            {
                slots[i].ResetVisualState();
            }

            lastSelectedItem = null;
        }

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();
            ItemContainer.Session.InteractionContext.HeldItem = null;
            ItemContainer.Session.InteractionContext.Refresh();
            Reset();

            if (lastSeenContainerVersion != ItemContainer.Version)
            {
                lastSeenContainerVersion = ItemContainer.Version;
                Refresh();
            }
        }

        // OnDeactivate
        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            itemLabel.Clear();
            itemEffect.Clear();
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
                slots[i].Draw(gameTime);
            }

            if (lastSelectedItem != null)
            {
                itemLabel.Draw(gameTime);
                itemEffect.Draw(gameTime);
            }

            inspectItem.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            // Mouse input
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
            {
                if (HandleMouseInput())
                    return HandleInputResult.Handled;
            }

            if (InputBindings.Back.IsPressed(PlayerIndex.One))
            {
                Game.SceneManager.Pop();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();
            autoHide = false;
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

            for (var i = 0; i < ItemContainer.Count; i++)
            {
                slots[i].Update(gameTime);
            }

            if (GetSelectedItem() is Item item)
            {
                if (item != lastSelectedItem)
                {
                    if (lastSelectedItem?.Index >= 0 && lastSelectedItem.Index < ItemContainer.Count)
                    {
                        slots[lastSelectedItem.Index].SetSelected(false);
                    }

                    int currentSlotIndex = item.Index;
                    float slotCenterX = slots[currentSlotIndex].BoundingBox.Center.X;

                    itemLabel.X = slotCenterX;
                    itemLabel.Color = item.Definition.IsKeyItem ? Colors.Inventory.KeyItem : Colors.Inventory.Item;
                    itemLabel.Text = item.Definition.Label;

                    itemEffect.X = slotCenterX;
                    itemEffect.Text = item.Definition.EffectDescription;

                    MouseCursor.Tooltip = item.Definition.Label;

                    slots[currentSlotIndex].SetSelected(true);

                    itemLabel.Tag = item;
                    lastSelectedItem = item;
                }
            }
            else if (lastSelectedItem != null)
            {
                if (lastSelectedItem.Index >= 0 && lastSelectedItem.Index < ItemContainer.Count)
                {
                    slots[lastSelectedItem.Index].SetSelected(false);
                }

                itemLabel.Clear();
                itemEffect.Clear();
                MouseCursor.Tooltip = null;

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
                if (slots[i].Contains(position))
                    return slots[i].Item;
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