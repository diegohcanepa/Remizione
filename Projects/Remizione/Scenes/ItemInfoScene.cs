using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ItemInfoScene
    /// </summary>
    public sealed class ItemInfoScene : Scene
    {
        #region Private fields

        private readonly UIButton button;
        private readonly TextSprite descriptionText;
        private readonly Sprite image;
        private readonly Sprite imageSlot;
        private Item? item;
        private readonly GameSession session;
        private readonly Sprite shadow;
        private readonly TextSprite nameText;

        #endregion

        #region Constructor

        // Constructor
        public ItemInfoScene(GameSession session)
        {
            this.session = session;
            this.BackgroundColor = Color.Black;

            PausePreviousScenes = true;
            ExclusiveDraw = true;

            // Image slot
            this.imageSlot = new()
            {
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.Area.GetPoint(RectanglePoint.Top, 0, 20),
                RenderImage = Atlases.UI.InventoryItemSlots[0]
            };

            // Image
            this.image = new()
            {
                PivotOrigin = RectanglePoint.Center,
                Position = imageSlot.BoundingBox.GetPoint(RectanglePoint.Center, 0, -1),
            };

            // Shadow
            shadow = new()
            {
                Color = Color.Black,
                Opacity = .3f,
                PivotOrigin = RectanglePoint.Center,
                Y = image.BoundingBox.Center.Y + 1,
                X = image.BoundingBox.Center.X - .5f
            };

            // Name text
            this.nameText = new(Fonts.Common)
            {
                Color = ColorPalette.MouseCursor.Tooltip,
                PivotOrigin = RectanglePoint.Top,
                Position = imageSlot.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 3),
                Scale = ScaleInfo.Text.Giant,
                Text = "Text"
            };

            // Description
            this.descriptionText = new(Fonts.Common)
            {
                Color = ColorPalette.Inventory.EffectDescription,
                MaximumWidth = 190,
                PivotOrigin = RectanglePoint.Top,
                Position = nameText.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 1),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Button
            this.button = new()
            {
                HoverColor = ColorPalette.Text.TerraLight,
                TextColor = ColorPalette.Text.Terra,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -20, -3),
                Text = Localization.GetValue(UserAction.Drop)
            };
        }

        #endregion

        #region Private members

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.LastInputMethod != InputMethod.Mouse)
                return false;

            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed() ||
                InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                MouseCursor.PerformClick();
                Game.SceneManager.Pop();
                return true;
            }

            return false;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            imageSlot.Draw(gameTime);
            shadow.Draw(gameTime);
            image.Draw(gameTime);
            nameText.Draw(gameTime);
            descriptionText.Draw(gameTime);
            button.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            if (button.TestPressed())
            {
                if (item != null && session.Room is { } room && session.Player is { } player)
                {
                    item.Remove();
                    Sound.Play(SoundNames.ItemDiscard);
                    ItemOrb.Drop(item.Definition, item.Amount, room, player.Position);
                }

                Game.SceneManager.Pop();
                return HandleInputResult.Handled;
            }

            if (HandleMouseInput())
                return HandleInputResult.Handled;

            return base.OnHandleInput();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            button.Update(gameTime);
            nameText.Update(gameTime);
            descriptionText.Update(gameTime);
        }

        #endregion

        // Show
        public void Show(Item item)
        {
            this.item = item;
            this.nameText.Color = item.Definition.IsKeyItem ? ColorPalette.Inventory.KeyItem : ColorPalette.Inventory.Item;
            this.nameText.Text = item.Definition.GetLabel(item.Amount);
            this.descriptionText.Text = item.Definition.Description;
            this.image.RenderImage = item.Definition.Image;
            this.shadow.RenderImage = item.Definition.Image;
        }
    }
}
