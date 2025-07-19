using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class InventoryScene : Scene
    {
        #region Private fields

        private const int maxInfoTextWidth = 80;

        private InventoryGrid activeGrid;
        private readonly UITextButton buttonClose;
        private readonly UITextButton buttonConsume;
        private readonly UITextButton buttonDiscard;
        private readonly UITextButton buttonEquip;
        private readonly List<InventoryCategory> categories = [InventoryCategory.Consumables, InventoryCategory.Equipment, InventoryCategory.KeyItems];
        private readonly TextSprite categoryText;
        private readonly ImageSprite checkMark;
        private readonly ImageSprite checkMarkItemInfo;
        private InventoryCategory currentCategory;
        private Item? equippedItem;
        private readonly ImageSprite gridContainer;
        private readonly Dictionary<InventoryCategory, InventoryGrid> grids = [];
        private readonly UIHealthMeter healthMeter;
        private readonly ImageSprite infoContainer;
        private readonly TextSprite itemDescription;
        private readonly TextSprite itemName;
        private readonly TextSprite itemStats;
        private InputMethod lastKnownInput;
        private readonly ImageSprite navigationBar;
        private readonly UITextButton nextCategoryButton;
        private readonly UITextButton previousCategoryButton;

        #endregion

        #region Constructor

        // Constructor
        public InventoryScene(Actor owner)
            : base(owner.Game, SceneSettings.PausePreviousScenes | SceneSettings.ExclusiveDraw)
        {
            this.Owner = owner;

            BackgroundColor = Color.Black;

            // Health meter
            this.healthMeter = new(Game)
            {
                Actor = owner,
            };

            // Checkmark
            this.checkMark = new(Game, Atlases.UI.CheckMark)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Scale = ScaleInfo.UIElement.Medium
            };

            // Grid container
            this.gridContainer = new(Game, Atlases.UI.InventoryGridContainer)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Position = new(10, 24),
            };

            // Grids
            grids[InventoryCategory.Consumables] = new InventoryGrid(owner.GetInventory(InventoryCategory.Consumables), 6, 4);
            grids[InventoryCategory.Equipment] = new InventoryGrid(owner.GetInventory(InventoryCategory.Equipment), 6, 4);
            grids[InventoryCategory.KeyItems] = new InventoryGrid(owner.GetInventory(InventoryCategory.KeyItems), 6, 4);

            foreach (var grid in grids.Values)
            {
                grid.Position = gridContainer.BoundingBox.GetPoint(RectanglePoint.LeftTop, 3, 3);
            }

            activeGrid = grids[InventoryCategory.Consumables];

            // Navigation bar
            this.navigationBar = new(Game, Atlases.UI.InventoryNavigationBar)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = gridContainer.BoundingBox.GetPoint(RectanglePoint.Top, 0, 0)
            };

            // Previous tab button
            this.previousCategoryButton = new(Game, InputBindings.PreviousTab)
            {
                ImageName = nameof(InputBindings.PreviousTab),
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = gridContainer.BoundingBox.GetPoint(RectanglePoint.LeftTop, 6, 0),
            };

            // Next tab button
            this.nextCategoryButton = new(Game, InputBindings.NextTab)
            {
                ImageName = nameof(InputBindings.NextTab),
                PivotOrigin = RectanglePoint.RightBottom,
                Position = gridContainer.BoundingBox.GetPoint(RectanglePoint.RightTop, -6, 0)
            };

            // Category
            this.categoryText = new(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Middle,
                Scale = ScaleInfo.Text.Huge,
                Position = navigationBar.BoundingBox.GetPoint(RectanglePoint.Middle, 0, .5f),
                ShadowOffset = new Vector2(0, .75f)
            };

            // Info container
            this.infoContainer = new(Game, Atlases.UI.InventoryInfoContainer)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Position = gridContainer.BoundingBox.GetPoint(RectanglePoint.RightTop, 3, 0)
            };

            // Item name
            this.itemName = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.LeftTop,
                Position = new Vector2(6, 3),
                Scale = ScaleInfo.Text.ExtraLarge,
                VisualParent = infoContainer,
                ShadowOffset = new Vector2(0, .75f)
            };

            // Item description
            this.itemDescription = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Dark,
                PivotOrigin = RectanglePoint.LeftTop,
                MaximumWidth = maxInfoTextWidth,
                Scale = ScaleInfo.Text.VeryLarge,
                ShadowOffset = new Vector2(0, .75f)
            };

            // Item stats
            this.itemStats = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.LeftTop,
                MaximumWidth = maxInfoTextWidth,
                Scale = ScaleInfo.Text.VeryLarge,
                ShadowOffset = new Vector2(0, .75f)
            };

            // Checkmark item info
            this.checkMarkItemInfo = new(Game, Atlases.UI.CheckMark)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.RightTop, -4, 3),
            };

            // Close button
            buttonClose = new UITextButton(owner.Game, InputBindings.Close)
            {
                AllowPressEffect = false,
                PivotOrigin = RectanglePoint.RightTop,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.RightBottom, 0, 7),
            };

            // Consume button
            buttonConsume = new UITextButton(owner.Game, InputBindings.UseItem)
            {
                AllowSound = false,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.RightBottom, -3, -3)
            };

            // Discard button
            buttonDiscard = new UITextButton(owner.Game, InputBindings.Discard)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 5, -3),
                TextColor = ColorPalette.Text.TerraLight,
            };

            // Equip button
            buttonEquip = new UITextButton(owner.Game, InputBindings.Equip)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.RightBottom, -3, -3)
            };
        }

        #endregion

        #region Private members

        // DrawButtons
        private void DrawButtons(GameTime gameTime)
        {
            buttonClose.Draw(gameTime);

            if (activeGrid.SelectedSlot.Item is not Item item)
                return;

            //buttonDiscard.Draw(gameTime);

            if (item.MetaItem.Category == InventoryCategory.Equipment)
            {
                buttonEquip.Draw(gameTime);
            }
            else if (item.MetaItem.Category == InventoryCategory.Consumables)
            {
                buttonConsume.Draw(gameTime);
            }
        }

        // InvalidateCategory
        private void InvalidateCategory()
        {
            activeGrid = grids[currentCategory];
            categoryText.Text = Localization.GetValue(currentCategory);
            InvalidateItemInfo();
            InvalidateEquippedItem();
        }

        // InvalidateEquippedItem
        private void InvalidateEquippedItem()
        {
            if (equippedItem != null && grids[InventoryCategory.Equipment].GetSlot(equippedItem) is InventorySlot slot)
            {
                checkMark.Image = Atlases.UI.CheckMark;
                checkMark.Position = slot.BoundingBox.GetPoint(RectanglePoint.RightBottom, -1, -8);
            }
            else
                checkMark.Image = null;
        }

        // InvalidateItemInfo
        private void InvalidateItemInfo()
        {
            if (activeGrid.SelectedSlot?.Item is Item item)
            {
                itemName.Text = TextRepository.GetValue($"Item.{item.Name}.Name") + (item.Level == 0 ? string.Empty : $" +{item.Level}");
                itemDescription.Text = $"@Item.{item.Name}.Description";
                itemDescription.Position = itemName.BoundingBox.GetPoint(RectanglePoint.LeftBottom);
                itemStats.Text = item.GetLocalizedInfo();
                itemStats.Position = itemDescription.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, 1);
            }
            else
            {
                itemName.Clear();
                itemDescription.Clear();
                itemStats.Clear();
            }
        }

        // NextCategory
        private void NextCategory()
        {
            var index = categories.IndexOf(currentCategory);
            if (index == categories.Count - 1)
                currentCategory = categories[0];
            else
                currentCategory = categories[index + 1];

            InvalidateCategory();
        }

        // PreviousCategory
        private void PreviousCategory()
        {
            var index = categories.IndexOf(currentCategory);
            if (index == 0)
                currentCategory = categories[^1];
            else
                currentCategory = categories[index - 1];

            InvalidateCategory();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            healthMeter.Draw(gameTime);

            // Containers
            Game.SpriteBatch.Begin(Game.Camera);
            navigationBar.Draw(gameTime);
            gridContainer.Draw(gameTime);
            infoContainer.Draw(gameTime);
            Game.SpriteBatch.End();

            DrawButtons(gameTime);

            activeGrid.Draw(gameTime);

            if (equippedItem != null && equippedItem.MetaItem.Category == currentCategory)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                checkMark.Draw(gameTime);

                if (equippedItem == activeGrid.SelectedSlot.Item)
                    checkMarkItemInfo.Draw(gameTime);

                Game.SpriteBatch.End();
            }

            nextCategoryButton.Draw(gameTime);
            previousCategoryButton.Draw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearWrap);
            categoryText.Draw(gameTime);
            itemName.Draw(gameTime);
            itemDescription.Draw(gameTime);
            itemStats.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            // Grid
            if (activeGrid.HandleInput(gameTime) == HandleInputResult.Handled)
            {
                InvalidateItemInfo();
                return HandleInputResult.Handled;
            }

            // Close
            if (buttonClose.TestPressed(PlayerIndex.One))
            {
                SceneController.Pop();
                return HandleInputResult.Handled;
            }

            if (activeGrid.SelectedSlot.Item != null)
            {
                // Consume
                if (activeGrid.Inventory.Category == InventoryCategory.Consumables)
                {
                    if (buttonConsume.TestPressed(PlayerIndex.One))
                    {
                        activeGrid.SelectedSlot.PerformDefaultAction();
                        InvalidateItemInfo();
                        return HandleInputResult.Handled;
                    }
                }

                // Equip
                if (activeGrid.Inventory.Category == InventoryCategory.Equipment)
                {
                    if (buttonEquip.TestPressed(PlayerIndex.One))
                    {
                        equippedItem = activeGrid.SelectedSlot.Item;
                        InvalidateEquippedItem();
                        return HandleInputResult.Handled;
                    }
                }
            }

            // Next category
            if (nextCategoryButton.TestPressed(PlayerIndex.One))
            {
                NextCategory();
                return HandleInputResult.Handled;
            }

            // Previous category
            if (previousCategoryButton.TestPressed(PlayerIndex.One))
            {
                PreviousCategory();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();

            Sound.Play(SoundNames.UIInventoryOpen);

            // Populate grids
            foreach (var grid in grids.Values)
            {
                grid.Populate();
            }

            currentCategory = InventoryCategory.Consumables;
            equippedItem = Owner.Equipment.SelectedItem;
            InvalidateEquippedItem();
            lastKnownInput = InputMethod.None;

            InvalidateCategory();
        }

        // OnUnloadContent
        protected override void OnUnloadContent()
        {
            base.OnUnloadContent();

            if (equippedItem != null)
                Owner.Equipment.Select(equippedItem);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            buttonClose.Update(gameTime);
            buttonConsume.Update(gameTime);
            buttonDiscard.Update(gameTime);
            buttonEquip.Update(gameTime);
            categoryText.Update(gameTime);
            activeGrid.Update(gameTime);
            gridContainer.Update(gameTime);
            healthMeter.Update(gameTime);
            itemName.Update(gameTime);
            itemDescription.Update(gameTime);
            itemStats.Update(gameTime);
            nextCategoryButton.Update(gameTime);
            previousCategoryButton.Update(gameTime);

            if (lastKnownInput != InputManager.DefaultPlayer.LastInputMethod)
                lastKnownInput = InputManager.DefaultPlayer.LastInputMethod;
        }

        #endregion

        // Owner
        public Actor Owner { get; set; }
    }
}
