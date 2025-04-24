using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remizione.Inventory;
using System;
using System.Collections.Generic;

namespace Remizione.Scenes
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    internal sealed class InventoryScene : Scene
    {
        private const int maxInfoTextWidth = 85;

        #region Private fields

        private Actor? actor;
        private readonly ImageSprite categoryMarkerImage;
        private readonly UIControl[] categoryIcons;
        private readonly TextSprite categoryText;
        private readonly ItemCategory[] categories;
        private readonly List<InventoryGrid> grids = [];
        private readonly ImageSprite gridContainerImage;
        private readonly ImageSprite infoContainerImage;
        private readonly UIControl itemActionButton;
        private readonly TextSprite itemDescription;
        private readonly TextSprite itemName;
        private readonly TextSprite itemUpgradeInfo;
        private readonly UIControl nextTabButton;
        private readonly TextSprite noItemSelectedText;
        private readonly UIControl previousTabButton;
        private readonly GameSession session;
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 100 };

        #endregion

        #region Contructor

        // Constructor
        public InventoryScene(GameSession session)
            : base(session.Game, SceneSettings.PausePreviousScenes | SceneSettings.ExclusiveDraw)
        {
            this.session = session;

            var values = Enum.GetValues<ItemCategory>();
            categories = new ItemCategory[values.Length];
            for (var i = 0; i < categories.Length; i++)
            {
                categories[i] = (ItemCategory)i;
            }

            BackgroundColor = new(10, 10, 25);

            // Create grids
            for (int i = 0; i < Enum.GetValues<ItemCategory>().Length; i++)
            {
                grids.Add(new InventoryGrid(Game, 7, 6, 6) { Position = new Vector2(15, 30) });
            }

            // Grid container
            this.gridContainerImage = new(Game, Atlases.UI.InventoryGridContainer)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Position = new(10, 18),
            };

            // Category marker
            this.categoryMarkerImage = new(Game, Atlases.UI.InventoryCategoryMarker)
            {
                Color = ColorPalette.Text.Green,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = new Vector2(.75f)
            };

            // Info container
            this.infoContainerImage = new(Game, Atlases.UI.InventoryInfoContainer)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Position = new(130, 18),
            };

            // Item action button
            this.itemActionButton = new UIControl(Game, InputBindings.ItemAction, null)
            {
                AllowContainer = true,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = infoContainerImage.BoundingBox.GetPoint(RectanglePoint.RightBottom, -5, -3),
                Size = UIControlSize.Large
            };

            // Previous tab button
            this.previousTabButton = new UIControl(Game, InputBindings.PreviousTab)
            {
                ImageSource = ControlImageSource.InputBindingName,
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = gridContainerImage.BoundingBox.GetPoint(RectanglePoint.LeftTop, 4, -1)
            };

            // Next tab button
            this.nextTabButton = new UIControl(Game, InputBindings.NextTab)
            {
                ImageSource = ControlImageSource.InputBindingName,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = gridContainerImage.BoundingBox.GetPoint(RectanglePoint.RightTop, -4, -1)
            };

            // Category text
            this.categoryText = new TextSprite(Game, Fonts.Regular)
            {
                Color = ColorPalette.Text.Title,
                PivotOrigin = RectanglePoint.Top,
                Position = gridContainerImage.BoundingBox.GetPoint(RectanglePoint.Top, 0, 4),
                Scale = ScaleInfo.Text.VeryLarge,
                ShadowOffset = new Vector2(0, .75f)
            };

            // Item name
            this.itemName = new TextSprite(Game, Fonts.Regular)
            {
                Color = ColorPalette.Text.Title,
                PivotOrigin = RectanglePoint.LeftTop,
                Position = new Vector2(6, 4),
                Scale = ScaleInfo.Text.Large,
                VisualParent = infoContainerImage,
                ShadowOffset = new Vector2(0, .75f)
            };

            // Item description
            this.itemDescription = new TextSprite(Game, Fonts.Regular)
            {
                Color = ColorPalette.Text.Dark,
                PivotOrigin = RectanglePoint.LeftTop,
                MaximumWidth = maxInfoTextWidth,
                Scale = ScaleInfo.Text.Medium,
            };

            // Upgrade info
            this.itemUpgradeInfo = new TextSprite(Game, Fonts.Regular)
            {
                Color = ColorPalette.Text.Green,
                PivotOrigin = RectanglePoint.RightBottom,
                MaximumWidth = maxInfoTextWidth,
                Scale = ScaleInfo.Text.Large,
            };

            // No item selected
            this.noItemSelectedText = new TextSprite(Game, Fonts.Regular)
            {
                Color = ColorPalette.Text.Dark,
                PivotOrigin = RectanglePoint.Middle,
                Position = infoContainerImage.BoundingBox.Center,
                MaximumWidth = maxInfoTextWidth,
                Scale = ScaleInfo.Text.Medium,
                Text = "@Messages.InventoryNoItemSelected"
            };

            // Category icons
            var names = Enum.GetNames<ItemCategory>();
            this.categoryIcons = new UIControl[names.Length];

            var x = 41;
            for (var i = 0; i < names.Length; i++)
            {
                this.categoryIcons[i] = new(Game, null)
                {
                    ImageSource = ControlImageSource.ImageName,
                    ImageName = "Inventory" + names[i],
                    PivotOrigin = RectanglePoint.Middle,
                    Position = new(x, 12),
                    Size = UIControlSize.Large
                };

                x += 12;
            }
        }

        #endregion

        #region Private members

        // ActiveGrid
        private InventoryGrid? ActiveGrid => grids[(int)session.SelectedItemCategory];

        // InvalidateCategory
        private void InvalidateCategory()
        {
            categoryText.Text = TextRepository.GetValue($"ItemCategory.{session.SelectedItemCategory}").ToUpper(TextRepository.LanguagePackage?.CultureInfo);
            categoryMarkerImage.Position = categoryIcons[(int)session.SelectedItemCategory].BoundingBox.GetPoint(RectanglePoint.Top, 0, -2);
            InvalidateItemInfo();
        }

        // InvalidateItemInfo
        private void InvalidateItemInfo()
        {
            Sound.Play(SoundNames.UINavigation);

            if (ActiveGrid?.SelectedSlot?.Item is Item item)
            {
                itemName.Text = TextRepository.GetValue($"Items.{item}.Name").ToUpper(TextRepository.LanguagePackage?.CultureInfo) + (item.Level == 0 ? string.Empty : $" +{item.Level}");
                itemDescription.Text = $"@Items.{item}.Description";
                itemDescription.Position = itemName.BoundingBox.GetPoint(RectanglePoint.LeftBottom);
                itemActionButton.TextColor = ColorPalette.Text.Light;
                itemActionButton.Text = item.MetaItem.Action == ItemAction.None ? null : $"@ItemActions.{item.MetaItem.Action}";

                if (item.MetaItem.Action == ItemAction.Upgrade && item.HasUpgrade)
                {
                    itemUpgradeInfo.Text = item.GetLocalizedUpgradeDescription();
                    itemUpgradeInfo.Position = itemActionButton.BoundingBox.GetPoint(RectanglePoint.RightTop, -1, 0);
                    //itemActionButton.TextColor = ColorPalette.Text.Green;
                }
                else
                    itemUpgradeInfo.Text = null;
            }
            else
            {
                itemName.Text = null;
                itemDescription.Text = null;
                itemActionButton.Text = null;
                itemUpgradeInfo.Text = null;
            }
        }

        // NextCategory
        private void NextCategory()
        {
            var index = (int)session.SelectedItemCategory;
            if (index == categories.Length - 1)
                session.SelectedItemCategory = categories[0];
            else
                session.SelectedItemCategory = categories[index + 1];

            InvalidateCategory();
        }

        // PreviousCategory
        private void PreviousCategory()
        {
            var index = (int)session.SelectedItemCategory;
            if (index == 0)
                session.SelectedItemCategory = categories[^1];
            else
                session.SelectedItemCategory = categories[index - 1];

            InvalidateCategory();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            gridContainerImage.Draw(gameTime);
            infoContainerImage.Draw(gameTime);
            categoryMarkerImage.Draw(gameTime);
            Game.SpriteBatch.End();

            for (var i = 0; i < categoryIcons.Length; i++)
            {
                categoryIcons[i].Draw(gameTime);
            }

            ActiveGrid?.Draw(gameTime);
            nextTabButton.Draw(gameTime);
            previousTabButton.Draw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            categoryText.Draw(gameTime);
            itemName.Draw(gameTime);
            itemDescription.Draw(gameTime);

            if (itemDescription.IsEmpty)
                noItemSelectedText.Draw(gameTime);

            itemUpgradeInfo.Draw(gameTime);

            Game.SpriteBatch.End();

            if (itemActionButton.Text != null)
                itemActionButton.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (InputBindings.ShowInventory.IsPressed(PlayerIndex.One))
                SceneController.Pop();

            if (ActiveGrid is not InventoryGrid grid)
                return base.OnHandleInput(gameTime);

            if (ActiveGrid.HandleInput(gameTime) == HandleInputResult.Handled)
            {
                InvalidateItemInfo();
                return HandleInputResult.Handled;
            }

            for (var i = 0; i < categoryIcons.Length; i++)
            {
                if (categoryIcons[i].TestPressed(PlayerIndex.One))
                {
                    session.SelectedItemCategory = (ItemCategory)i;
                    InvalidateCategory();
                    return HandleInputResult.Handled;
                }
            }

            // Previous category
            if (previousTabButton.TestPressed(PlayerIndex.One))
            {
                PreviousCategory();
                return HandleInputResult.Handled;
            }

            // Next category
            if (nextTabButton.TestPressed(PlayerIndex.One))
            {
                NextCategory();
                return HandleInputResult.Handled;
            }

            // Move up
            if (InputBindings.SelectUp.IsPressed(0) || stick.IsUp(PlayerIndex.One))
            {
                grid.MoveUp();
                InvalidateItemInfo();
                return HandleInputResult.Handled;
            }

            // Move left
            else if (InputBindings.SelectLeft.IsPressed(0) || stick.IsLeft(PlayerIndex.One))
            {
                grid.MoveLeft();
                InvalidateItemInfo();
                return HandleInputResult.Handled;
            }

            // Move down
            if (InputBindings.SelectDown.IsPressed(0) || stick.IsDown(PlayerIndex.One))
            {
                grid.MoveDown();
                InvalidateItemInfo();
                return HandleInputResult.Handled;
            }

            // Move right
            if (InputBindings.SelectRight.IsPressed(0) || stick.IsRight(PlayerIndex.One))
            {
                grid.MoveRight();
                InvalidateItemInfo();
                return HandleInputResult.Handled;
            }

            return base.OnHandleInput(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            nextTabButton.Update(gameTime);
            previousTabButton.Update(gameTime);
            stick.Update(gameTime);
            ActiveGrid?.Update(gameTime);
            categoryText.Update(gameTime);
            itemActionButton.Update(gameTime);
            itemName.Update(gameTime);
            itemDescription.Update(gameTime);
            noItemSelectedText.Update(gameTime);
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get => actor;
            set
            {
                actor = value;

                /*
                for (int i = 0; i < Enum.GetValues<ItemCategory>().Length; i++)
                {
                    if (actor != null)
                        grids[i].Fill(actor.GetItemStorage((ItemCategory)i));
                    else
                        grids[i].Clear();
                }
                */

                InvalidateCategory();
            }
        }
    }
}
