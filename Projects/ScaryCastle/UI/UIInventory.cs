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
        private readonly TextSprite[] amounts;
        private readonly ImageSprite[] icons;
        private readonly Inventory inventory;
        private readonly ImageSprite[] slots;

        #region Constructor

        // Constructor
        public UIInventory(Inventory inventory)
            : base(inventory.Session.Game)
        {
            this.inventory = inventory;
            this.amounts = new TextSprite[GameSettings.MaxInventoryCapacity];
            this.icons = new ImageSprite[GameSettings.MaxInventoryCapacity];
            this.slots = new ImageSprite[GameSettings.MaxInventoryCapacity];

            for (var i = 0; i < slots.Length; i++)
            {
                slots[i] = new(Game, Atlases.UI.InventorySlot)
                {
                    PivotOrigin = RectanglePoint.Bottom,
                    Y = Screen.Area.Bottom - 7
                };

                icons[i] = new(Game)
                {
                    PivotOrigin = RectanglePoint.Center,
                    Scale = ScaleInfo.UIElement.Medium,
                    Y = slots[i].BoundingBox.Center.Y
                };

                // Amount text
                amounts[i] = new(Game, Fonts.CommonOutline)
                {
                    Color = ColorPalette.Text.Default,
                    PivotOrigin = RectanglePoint.Top,
                    Y = slots[i].BoundingBox.Center.Y + 4,
                    Scale = ScaleInfo.Text.ExtraLarge
                };
            }

            Layout();
        }

        #endregion

        #region Private members

        // GetInventoryItemAt
        private Item? GetInventoryItemAt(Vector2 position)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].BoundingBox.Contains(position))
                    return i < inventory.Count ? inventory[i] : null;
            }

            return null;
        }

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (GetInventoryItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is Item grabbedItem)
                {
                    if (grabbedItem.Definition.Image != null)
                    {
                        MouseCursor.AnimateClick();
                        Sound.Play(SoundNames.UISelectC);
                        inventory.HeldItem = grabbedItem;
                        return true;
                    }
                }
            }

            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed() && IsVisible)
            {
                if (inventory.HeldItem != null)
                {
                    inventory.HeldItem = null;
                }
                else
                {
                    IsVisible = false;
                }

                return true;
            }

            return false;
        }

        // Layout
        private void Layout()
        {
            float anchoPantalla = Screen.NativeWidth;
            int cantidadRects = inventory.Capacity;
            float anchoRect = slots[0].BoundingBox.Width;
            float separacion = 2;

            // 2. Calcular el ancho total de la fila
            // (Cantidad * ancho) + (Espacios intermedios * separacion)
            float anchoTotalFila = (cantidadRects * anchoRect) + ((cantidadRects - 1) * separacion);

            // 3. Calcular el punto de inicio (X) para que quede centrado
            float xInicial = (anchoPantalla - anchoTotalFila) / 2;

            for (int i = 0; i < cantidadRects; i++)
            {
                // La posición X es el inicio + el desplazamiento de los rectángulos previos y sus espacios
                slots[i].X = xInicial + (i * (anchoRect + separacion));

                if (i < inventory.Count)
                {
                    icons[i].X = slots[i].BoundingBox.Center.X;
                    icons[i].Image = inventory[i].Definition.Image;

                    amounts[i].X = icons[i].X;
                    amounts[i].Text = inventory[i].Count < 2 ? null : inventory[i].Count.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            for (var i = 0; i < inventory.Capacity; i++)
            {
                slots[i].Draw(gameTime);

                if (inventory.HeldItem?.Index == i)
                    continue;

                icons[i].Draw(gameTime);
                amounts[i].Draw(gameTime);
            }
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            // TODO: Must be called only when inventory changes
            Layout();
        }

        #endregion

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
        public bool IsVisible { get; set; }
    }
}
