using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ItemInfoScene
    /// </summary>
    internal class ItemInfoScene : Scene
    {
        private readonly UIButton buttonClose;
        private readonly TextSprite categoryName;
        private readonly UIHPBonus hpBonus;
        private readonly ImageSprite infoContainer;
        private readonly ImageSprite infoTitleContainer;
        private readonly TextSprite itemDescription;
        private readonly ImageSprite itemIcon;
        private readonly TextSprite itemName;

        // Constructor
        public ItemInfoScene(EngendroGame game)
            : base(game, SceneSettings.PausePreviousScenes)
        {
            BackgroundColor = Color.Transparent;

            // Heart bonus
            this.hpBonus = new(Game);

            // Info container
            this.infoContainer = new(Game, Atlases.UI.InventoryInfoContainer)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.Center
            };

            // Info title container
            this.infoTitleContainer = new(Game, Atlases.UI.InventoryInfoTitleContainer)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.Top)
            };

            // Item description
            this.itemDescription = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Dark,
                PivotOrigin = RectanglePoint.LeftTop,
                MaximumWidth = 98,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.LeftTop, 6, 4),
                Scale = ScaleInfo.Text.VeryLarge,
                ShadowOffset = new Vector2(0, .75f)
            };

            // Item icon
            this.itemIcon = new(Game)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = infoTitleContainer.BoundingBox.GetPoint(RectanglePoint.LeftBottom, -4, 2),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Item name
            this.itemName = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Center,
                Position = infoTitleContainer.BoundingBox.GetPoint(RectanglePoint.Center, 0, 1.3f),
                Scale = ScaleInfo.Text.ExtraLarge,
                ShadowOffset = new Vector2(0, .75f)
            };

            // Category name
            this.categoryName = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 6, -3),
                Scale = ScaleInfo.Text.ExtraLarge,
                ShadowOffset = new Vector2(0, .75f)
            };

            // Close button
            buttonClose = new UIButton(Game, InputBindings.Close)
            {
                AllowPressEffect = false,
                PivotOrigin = RectanglePoint.RightTop,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.RightBottom, 0, 1),
            };
        }

        #region Private members

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                SceneController.Pop();
                return true;
            }

            return false;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Item == null)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            Game.Shapes.DrawRectangle(Screen.Area, ColorPalette.SceneShade);
            infoContainer.Draw(gameTime);
            infoTitleContainer.Draw(gameTime);
            itemName.Draw(gameTime);
            itemDescription.Draw(gameTime);
            itemIcon.Draw(gameTime);
            categoryName.Draw(gameTime);
            Game.SpriteBatch.End();
            hpBonus.Draw(gameTime);
            buttonClose.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (HandleMouseInput())
                return HandleInputResult.Handled;

            // Close
            if (buttonClose.TestPressed(PlayerIndex.One))
            {
                SceneController.Pop();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // OnInvalidate
        protected override void OnInvalidate()
        {
            if (Item == null)
                return;

            itemIcon.Image = Item.MetaItem.Image;
            itemName.Text = TextRepository.GetValue($"Item.{Item.Name}.Name");
            itemDescription.Opacity = 1;
            itemDescription.PivotOrigin = RectanglePoint.LeftTop;
            itemDescription.Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.LeftTop, 5, 3);
            itemDescription.Text = $"@Item.{Item.Name}.Description";

            categoryName.Text = Localization.GetValue(Item.MetaItem.Category);

            // HP
            if (Item.MetaItem.Effect.HP != null)
            {
                hpBonus.Position = itemDescription.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, 1);
                hpBonus.Amount = Item.MetaItem.Effect.HP.MaximumValue;
            }
            else
                hpBonus.Amount = 0;
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();
            Sound.Play(SoundNames.UIInventoryOpen);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            buttonClose.Update(gameTime);
        }

        #endregion

        // Item
        public Item? Item
        {
            get;
            set
            {
                field = value;
                Invalidate();
            }
        }

    }
}
