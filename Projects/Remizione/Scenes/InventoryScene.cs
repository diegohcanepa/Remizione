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
        private readonly ImageSprite gridContainerImage;
        private Item? equippedItem;
        private readonly Dictionary<InventoryCategory, InventoryGrid> grids = [];
        private readonly ImageSprite checkMark;
        private readonly UIHealthMeter healthMeter;
        private readonly ImageSprite infoContainerImage;
        private readonly TextSprite itemDescription;
        private readonly TextSprite itemName;
        private InputMethod lastKnownInput;
        private readonly UITextButton nextTabButton;
        private readonly UITextButton previousTabButton;

        #endregion

        #region Constructor

        // Constructor
        public InventoryScene(Actor owner)
            : base(owner.Game, SceneSettings.PausePreviousScenes | SceneSettings.ExclusiveDraw)
        {
            this.Owner = owner;

            BackgroundColor = Color.Black;

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
            this.gridContainerImage = new(Game, Atlases.UI.InventoryGridContainer)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Position = new(10, 28),
            };

            // Grids
            grids[InventoryCategory.Consumables] = new InventoryGrid(owner.GetInventory(InventoryCategory.Consumables), 6, 4);
            grids[InventoryCategory.Equipment] = new InventoryGrid(owner.GetInventory(InventoryCategory.Equipment), 6, 4);
            grids[InventoryCategory.KeyItems] = new InventoryGrid(owner.GetInventory(InventoryCategory.KeyItems), 6, 4);

            foreach (var grid in grids.Values)
            {
                grid.Position = gridContainerImage.BoundingBox.GetPoint(RectanglePoint.LeftTop, 3, 3);
            }

            activeGrid = grids[InventoryCategory.Consumables];

            // Previous tab button
            this.previousTabButton = new(Game, InputBindings.PreviousTab)
            {
                ImageName = nameof(InputBindings.PreviousTab),
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = gridContainerImage.BoundingBox.GetPoint(RectanglePoint.LeftTop, 4, -1),
            };

            // Next tab button
            this.nextTabButton = new(Game, InputBindings.NextTab)
            {
                ImageName = nameof(InputBindings.NextTab),
                PivotOrigin = RectanglePoint.RightBottom,
                Position = gridContainerImage.BoundingBox.GetPoint(RectanglePoint.RightTop, -6, -1)
            };

            // Category
            this.categoryText = new(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.Huge,
                Position = gridContainerImage.BoundingBox.GetPoint(RectanglePoint.Top, 0, -1)
            };

            // Info container
            this.infoContainerImage = new(Game, Atlases.UI.InventoryInfoContainer)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Position = gridContainerImage.BoundingBox.GetPoint(RectanglePoint.RightTop, 3, 0)
            };

            // Item name
            this.itemName = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Terra,
                PivotOrigin = RectanglePoint.LeftTop,
                Position = new Vector2(6, 4),
                Scale = ScaleInfo.Text.VeryLarge,
                VisualParent = infoContainerImage,
                ShadowOffset = new Vector2(0, .75f)
            };

            // Item description
            this.itemDescription = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Dark,
                PivotOrigin = RectanglePoint.LeftTop,
                MaximumWidth = maxInfoTextWidth,
                Scale = ScaleInfo.Text.Large
            };

            // Close button
            buttonClose = new UITextButton(owner.Game, InputBindings.Close)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -2),
            };

            // Consume button
            buttonConsume = new UITextButton(owner.Game, InputBindings.ConsumeItem)
            {
                AllowSound = false,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = infoContainerImage.BoundingBox.GetPoint(RectanglePoint.RightBottom, -3, -3)
            };

            // Discard button
            buttonDiscard = new UITextButton(owner.Game, InputBindings.Discard)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = infoContainerImage.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 3, -3)
            };

            // Equip button
            buttonEquip = new UITextButton(owner.Game, InputBindings.Equip)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = infoContainerImage.BoundingBox.GetPoint(RectanglePoint.RightBottom, -3, -3)
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

            buttonDiscard.Draw(gameTime);

            if (item.MetaItem.Category == InventoryCategory.Equipment)
            {
                buttonEquip.IsEnabled = equippedItem != item;
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
            activeGrid = grids[Owner.Session.SelectedInventoryCategory];
            categoryText.Text = Localization.GetValue(Owner.Session.SelectedInventoryCategory);
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
            }
            else
            {
                itemName.Text = null;
                itemDescription.Text = null;
            }
        }

        // NextCategory
        private void NextCategory()
        {
            var index = categories.IndexOf(Owner.Session.SelectedInventoryCategory);
            if (index == categories.Count - 1)
                Owner.Session.SelectedInventoryCategory = categories[0];
            else
                Owner.Session.SelectedInventoryCategory = categories[index + 1];

            InvalidateCategory();
        }

        // PreviousCategory
        private void PreviousCategory()
        {
            var index = categories.IndexOf(Owner.Session.SelectedInventoryCategory);
            if (index == 0)
                Owner.Session.SelectedInventoryCategory = categories[^1];
            else
                Owner.Session.SelectedInventoryCategory = categories[index - 1];

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
            gridContainerImage.Draw(gameTime);
            infoContainerImage.Draw(gameTime);
            Game.SpriteBatch.End();

            DrawButtons(gameTime);

            activeGrid.Draw(gameTime);

            if (equippedItem != null && equippedItem.MetaItem.Category == Owner.Session.SelectedInventoryCategory)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                checkMark.Draw(gameTime);
                Game.SpriteBatch.End();
            }

            nextTabButton.Draw(gameTime);
            previousTabButton.Draw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearWrap);
            categoryText.Draw(gameTime);
            itemName.Draw(gameTime);
            itemDescription.Draw(gameTime);
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
            if (nextTabButton.TestPressed(PlayerIndex.One))
            {
                NextCategory();
                return HandleInputResult.Handled;
            }

            // Previous category
            if (previousTabButton.TestPressed(PlayerIndex.One))
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

            foreach (var grid in grids.Values)
            {
                grid.Populate();
            }

            equippedItem = Owner.Equipment.SelectedItem;
            InvalidateEquippedItem();

            Sound.Play(SoundNames.UIInventoryOpen);
            lastKnownInput = InputMethod.None;

            InvalidateCategory();
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
            gridContainerImage.Update(gameTime);
            healthMeter.Update(gameTime);
            itemName.Update(gameTime);
            itemDescription.Update(gameTime);
            nextTabButton.Update(gameTime);
            previousTabButton.Update(gameTime);

            if (lastKnownInput != InputManager.DefaultPlayer.LastInputMethod)
                lastKnownInput = InputManager.DefaultPlayer.LastInputMethod;
        }

        #endregion

        // Owner
        public Actor Owner { get; set; }
    }
}
