using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// FullInventoryScene
    /// </summary>
    public sealed class FullInventoryScene : Scene
    {
        private readonly UITextButton buttonClose;
        private readonly TextSprite categoryText;
        private readonly InventoryGrid grid;
        private readonly UITextButton nextTabButton;
        private readonly UITextButton previousTabButton;


        #region Constructor

        // Constructor
        public FullInventoryScene(Actor owner)
            : base(owner.Game, SceneSettings.PausePreviousScenes | SceneSettings.ExclusiveDraw)
        {
            this.Owner = owner;
            this.grid = new(owner.Inventory, 6, 5)
            {
                Position = new(20, 26)
            };

            BackgroundColor = Color.Black;

            // Close button
            buttonClose = new UITextButton(owner.Game, InputBindings.Close)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -2),
            };

            // Previous tab button
            this.previousTabButton = new(Game, InputBindings.PreviousTab)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = grid.BoundingBox.GetPoint(RectanglePoint.LeftTop, 4, -1)
            };

            // Next tab button
            this.nextTabButton = new(Game, InputBindings.NextTab)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = grid.BoundingBox.GetPoint(RectanglePoint.RightTop, -6, -1)
            };

            // Category
            this.categoryText = new(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.Huge,
                Position = grid.BoundingBox.GetPoint(RectanglePoint.Top, 0, -1)
            };
        }

        #endregion

        #region Private members

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                return false;

            /*
            if (GetInventoryItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is InventoryItem item)
            {
                Select(items.IndexOf(item));
                MouseCursor.Instance.AnimateClick();
                Sound.Play(SoundNames.UIHover);
            }
            */

            return false;
        }

        // SelectCategory
        private void SelectCategory(MetaItemCategory category)
        {
            grid.Fill(MetaItemCategory.Consumable);
            categoryText.Text = Localization.GetValue(category);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            buttonClose.Draw(gameTime);
            grid.Draw(gameTime);
            nextTabButton.Draw(gameTime);
            previousTabButton.Draw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearWrap);
            categoryText.Draw(gameTime);
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

            // Close
            if (buttonClose.TestPressed(PlayerIndex.One))
            {
                SceneController.Pop();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;   
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();
            SelectCategory(MetaItemCategory.Consumable);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            buttonClose.Update(gameTime);
            categoryText.Update(gameTime);
            grid.Update(gameTime);
            nextTabButton.Update(gameTime);
            previousTabButton.Update(gameTime);
        }

        #endregion

        // Owner
        public Actor Owner { get; set; }
    }
}
