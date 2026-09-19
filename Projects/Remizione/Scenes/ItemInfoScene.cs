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
        private readonly TextSprite titleText;

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
                RenderImage = Atlases.UI.InventoryItemSlot
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

            // Title text
            this.titleText = new(Fonts.Common)
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
                Color = ColorPalette.Text.Terra,
                MaximumWidth = 190,
                PivotOrigin = RectanglePoint.Top,
                Position = titleText.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 1),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Button
            this.button = new()
            {
                HoverColor = ColorPalette.Text.OrangeLight,
                TextColor = ColorPalette.Text.Orange,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -3, -3),
                Text = Localization.GetValue(UserAction.Discard)
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
            titleText.Draw(gameTime);
            descriptionText.Draw(gameTime);
            button.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            if (button.TestPressed())
            {
                if (item != null)
                {
                    item.Remove();
                    Sound.Play(SoundNames.ItemDiscard);
                    session.Player?.ShowFlyOff($"-{item.Definition.DisplayName}", ColorPalette.Text.Terra);
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
            titleText.Update(gameTime);
            descriptionText.Update(gameTime);
        }

        #endregion

        // Show
        public void Show(Item item)
        {
            this.item = item;
            this.titleText.Text = item.Definition.DisplayName;
            this.descriptionText.Text = item.Definition.Description;
            this.image.RenderImage = item.Definition.Image;
            this.shadow.RenderImage = item.Definition.Image;
        }
    }
}
