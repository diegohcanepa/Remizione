using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class InventoryScene : SessionOverlayScene, IInputHandler
    {
        #region Private fields

        private const float SelectionHysteresisDuration = 0.08f;
        private bool autoHide;
        private readonly Sprite background = new(Atlases.UI.QuickInventoryBackground) { PivotOrigin = RectanglePoint.LeftBottom, Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom) };
        private readonly Sprite inspectItem = new(Atlases.UI.InspectItem) { PivotOrigin = RectanglePoint.RightBottom, Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -2, -2) };
        private readonly ItemContainer itemContainer;
        private readonly TextSprite itemEffect;
        private readonly TextSprite itemLabel;
        private Item? lastSelectedItem;
        private int lastSeenContainerVersion = -1;
        private float selectionHysteresisTimer;
        private readonly InventorySlot[] slots = new InventorySlot[ItemContainer.MaximumCapacity];

        #endregion

        #region Constructor

        // Constructor
        public InventoryScene(GameSession session)
            : base(session)
        {
            this.PausePreviousScenes = false;
            this.itemContainer = session.PlayerData.Inventory;

            for (var i = 0; i < slots.Length; i++)
            {
                slots[i] = new InventorySlot();
            }

            // Item label
            itemLabel = new(Fonts.CommonOutline)
            {
                Color = Colors.Palette.Text,
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

            Refresh();
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
                else if (Session.InteractionContext.HeldItem == null && GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item grabbedItem)
                {
                    if (grabbedItem.Definition.Image != null)
                    {
                        Session.InteractionContext.HeldItem = grabbedItem;
                        Sound.Play(SoundNames.Interact);
                        Game.SceneManager.Pop();
                        return true;
                    }
                }
                else
                {
                    MouseCursor.InvalidClick();
                }
            }

            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                if (GetItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item item)
                {
                    Sound.Play(SoundNames.Interact);
                    Session.Overlays.ShowItemInfo(item);
                }
                else
                {
                    MouseCursor.InvalidClick();
                }
            }

            return false;
        }

        // Refresh
        private void Refresh()
        {
            lastSelectedItem = null;
            selectionHysteresisTimer = 0f;

            for (var i = 0; i < slots.Length; i++)
            {
                slots[i].Item = null;
            }

            float screenWidth = Screen.NativeWidth;
            int slotCount = itemContainer.Count;
            float spacing = 2;
            float slotSize = slots[0].BoundingBox.Width;

            float rowWidth = (slotCount * slotSize) + ((slotCount - 1) * spacing);
            float startingX = (screenWidth - rowWidth) / 2;
            float y = Screen.Area.Bottom - 20;

            for (int i = 0; i < ItemContainer.MaximumCapacity; i++)
            {
                if (i < itemContainer.Count)
                {
                    float x = startingX + (i * (slotSize + spacing)) + (slotSize / 2);
                    slots[i].Position = new Vector2(x, y);
                    slots[i].Item = itemContainer[i];
                }
            }

            // Actualizar la posición Y del label y descripción basándonos en la posición real fijada para los slots
            if (itemContainer.Count > 0)
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

            for (var i = 0; i < itemContainer.Count; i++)
            {
                slots[i].ResetVisualState();
            }

            lastSelectedItem = null;
            selectionHysteresisTimer = 0f;
        }

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();

            Session.HUD.BottomMessage.Hide();
            Session.InteractionContext.HeldItem = null;
            Session.InteractionContext.Refresh();
            Reset();

            if (lastSeenContainerVersion != itemContainer.Version)
            {
                lastSeenContainerVersion = itemContainer.Version;
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

            for (var i = 0; i < itemContainer.Count; i++)
            {
                slots[i].Draw(gameTime);
            }

            if (lastSelectedItem != null)
            {
                itemLabel.Draw(gameTime);
                itemEffect.Draw(gameTime);
            }

            if (lastSelectedItem != null)
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

            for (var i = 0; i < itemContainer.Count; i++)
            {
                slots[i].Update(gameTime);
            }

            Item? rawSelectedItem = GetSelectedItem();

            if (rawSelectedItem != null)
            {
                selectionHysteresisTimer = 0f;

                if (rawSelectedItem != lastSelectedItem)
                {
                    if (lastSelectedItem?.Index >= 0 && lastSelectedItem.Index < itemContainer.Count)
                    {
                        slots[lastSelectedItem.Index].SetSelected(false);
                    }

                    int currentSlotIndex = rawSelectedItem.Index;
                    float slotCenterX = slots[currentSlotIndex].BoundingBox.Center.X;

                    itemLabel.X = slotCenterX;
                    itemLabel.Color = rawSelectedItem.Definition.LabelColor;
                    itemLabel.Text = rawSelectedItem.Definition.Label;

                    itemEffect.X = slotCenterX;
                    itemEffect.Text = rawSelectedItem.Definition.EffectDescription;

                    MouseCursor.Tooltip = rawSelectedItem.Definition.Label;

                    slots[currentSlotIndex].SetSelected(true);

                    itemLabel.Tag = rawSelectedItem;
                    lastSelectedItem = rawSelectedItem;
                }
            }
            else if (lastSelectedItem != null)
            {
                selectionHysteresisTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (selectionHysteresisTimer >= SelectionHysteresisDuration)
                {
                    if (lastSelectedItem.Index >= 0 && lastSelectedItem.Index < itemContainer.Count)
                    {
                        slots[lastSelectedItem.Index].SetSelected(false);
                    }

                    itemLabel.Clear();
                    itemEffect.Clear();
                    MouseCursor.Tooltip = null;

                    lastSelectedItem = null;
                    selectionHysteresisTimer = 0f;
                }
            }
        }

        #endregion

        // AutoHideThreshold
        public const int AutoHideThreshold = 98;

        // GetItemAt
        public Item? GetItemAt(Vector2 position)
        {
            for (int i = 0; i < itemContainer.Count; i++)
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