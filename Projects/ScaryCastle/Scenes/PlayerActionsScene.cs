using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// PlayerActionsScene
    /// </summary>
    public sealed class PlayerActionsScene : Scene
    {
        #region Private fields

        private readonly Sprite[] icons = new Sprite[3];
        private readonly ItemContainer itemContainer;
        private readonly Sprite[] shadows = new Sprite[3];
        private readonly Sprite[] slots = new Sprite[3];

        #endregion

        // Constructor
        public PlayerActionsScene(ItemContainer itemContainer)
            : base()
        {
            this.PausePreviousScenes = false;

            this.itemContainer = itemContainer;

            for (var i = 0; i < slots.Length; i++)
            {
                slots[i] = new()
                {
                    Opacity = .9f,
                    PivotOrigin = RectanglePoint.Center,
                    Scale = new(.9f)
                };

                icons[i] = new()
                {
                    PivotOrigin = RectanglePoint.Center,
                    Scale = new(.8f)
                };

                shadows[i] = new()
                {
                    Color = Color.Black,
                    Opacity = ColorPalette.ShadowOpacity,
                    PivotOrigin = RectanglePoint.Center,
                    Scale = ScaleInfo.UIElement.Medium,
                };
            }

            slots[0].RenderImage = Atlases.UI.InventoryLiftActionSlot;
            slots[1].RenderImage = Atlases.UI.InventoryHeadbuttActionSlot;
            slots[2].RenderImage = Atlases.UI.InventoryGiftActionSlot;
        }

        #region Private members

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (itemContainer.Session.InteractionContext.HeldItem == null && GetItemAt(InputManager.DefaultPlayer.Mouse.WorldPosition(itemContainer.Session.Camera)) is Item grabbedItem)
                {
                    if (grabbedItem.Definition.Image != null)
                    {
                        itemContainer.Session.InteractionContext.HeldItem = grabbedItem;
                        MouseCursor.PerformClick(false);
                        Game.SceneManager.Pop();
                        return true;
                    }
                }
                else
                {
                    MouseCursor.PerformClick();
                    Game.SceneManager.Pop();
                }
            }

            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                if (GetItemAt(InputManager.DefaultPlayer.Mouse.WorldPosition(itemContainer.Session.Camera)) is Item item)
                {
                    itemContainer.Session.ShowItemInfo(item);
                    Sound.Play(SoundNames.Interact);
                    return true;
                }
                else
                {
                    MouseCursor.PerformClick();
                    Game.SceneManager.Pop();
                }
            }

            return false;
        }

        // Layout
        private void Layout()
        {
            if (itemContainer.Session.Player is not Actor player)
                return;

            slots[1].Position = player.GetOverheadPosition(new(0, -7));
            slots[0].Position = slots[1].BoundingBox.GetPoint(RectanglePoint.Left, -slots[0].BoundingBox.Width / 2, 7);
            slots[2].Position = slots[1].BoundingBox.GetPoint(RectanglePoint.Right, slots[0].BoundingBox.Width / 2, 7);

            for (int i = 0; i < icons.Length; i++)
            {
                icons[i].RenderImage = null;
                shadows[i].RenderImage = null;

                if (i < itemContainer.Count)
                {
                    icons[i].Position = slots[i].BoundingBox.GetPoint(RectanglePoint.Center);
                    icons[i].RenderImage = itemContainer[i].Definition.Image;

                    shadows[i].Y = slots[i].BoundingBox.Center.Y + .5f;
                    shadows[i].X = icons[i].X - .5f;
                    shadows[i].RenderImage = itemContainer[i].Definition.Image;
                }
            }
        }

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();
            MouseCursor.State = MouseCursorState.Cross;
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(itemContainer.Session.Camera);

            for (var i = 0; i < itemContainer.Capacity; i++)
            {
                slots[i].Draw(gameTime);
                shadows[i].Draw(gameTime);
                icons[i].Draw(gameTime);
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

            itemContainer.Session.InteractionContext.HeldItem = null;

            Layout();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            for (var i = 0; i < icons.Length; i++)
            {
                icons[i].Scale = ScaleInfo.UIElement.Medium;
            }

            if (GetSelectedItem() is Item item)
            {
                MouseCursor.Text = item.Definition.DisplayName;
                icons[item.Index].Scale = ScaleInfo.InventoryHeldItem;
            }
            else
            {
                MouseCursor.Text = null;
            }
        }

        #endregion

        // GetItemAt
        public Item? GetItemAt(Vector2 position)
        {
            for (int i = 0; i < itemContainer.Count; i++)
            {
                if (slots[i].BoundingBox.Contains(position))
                    return i < itemContainer.Count ? itemContainer[i] : null;
            }

            return null;
        }

        // GetSelectedItem
        public Item? GetSelectedItem()
        {
            return GetItemAt(InputManager.DefaultPlayer.Mouse.WorldPosition(itemContainer.Session.Camera));
        }
    }
}
