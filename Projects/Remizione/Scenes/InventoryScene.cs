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
        private readonly List<InventoryCategory> categories = [InventoryCategory.Junk, InventoryCategory.Consumables, InventoryCategory.KeyItems, InventoryCategory.Trinkets, InventoryCategory.Traits];
        private readonly ImageSprite[] categoryIcons;
        private readonly TextSprite categoryText;
        private readonly ImageSprite checkMark;
        private InventoryCategory currentCategory;
        private Item? equippedJunk;
        private Item? equippedTrinket;
        private readonly ImageSprite gridContainer;
        private readonly Dictionary<InventoryCategory, InventoryGrid> grids = [];
        private readonly UIHealthMeter healthMeter;
        private readonly UIHealthBonus heartBonus;
        private readonly ImageSprite infoContainer;
        private readonly ImageSprite infoTitleContainer;
        private readonly TextSprite itemDescription;
        private readonly ImageSprite itemIcon;
        private readonly TextSprite itemName;
        private InputMethod lastKnownInput;
        private readonly ImageSprite navigationBar;
        private readonly UITextButton nextCategoryButton;
        private readonly UITextButton previousCategoryButton;
        private readonly UITicketsMeter ticketsMeter;
        private readonly TrinketSlot trinketSlot;

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

            // Trinket slot
            this.trinketSlot = new(Game)
            {
                Actor = owner,
            };

            // Tickets meter
            this.ticketsMeter = new(Game)
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
                Position = new(10, 30),
            };

            // Create grids for each category
            var gridPos = gridContainer.BoundingBox.GetPoint(RectanglePoint.LeftTop, 3, 3);
            foreach (var category in categories)
            {
                grids[category] = new InventoryGrid(owner.Inventory.GetContainer(category), 6, 4)
                {
                    Position = gridPos
                };
            }

            activeGrid = grids[InventoryCategory.Consumables];

            // Navigation bar
            this.navigationBar = new(Game, Atlases.UI.InventoryNavigationBar)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = gridContainer.BoundingBox.GetPoint(RectanglePoint.Top, 0, 0)
            };

            // Category icons
            categoryIcons = new ImageSprite[categories.Count];
            for (int i = 0; i < categoryIcons.Length; i++)
            {
                categoryIcons[i] = new(Game, Atlases.UI.GetImage($"InventoryCategory{categories[i]}"))
                {
                    PivotOrigin = RectanglePoint.Center,
                    Scale = ScaleInfo.UIElement.Medium
                };
            }

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
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.Text.ExtraLarge,
                Position = navigationBar.BoundingBox.GetPoint(RectanglePoint.Center, 0, .5f),
                ShadowOffset = new Vector2(0, .75f)
            };

            // Heart bonus
            this.heartBonus = new(Game);

            // Info container
            this.infoContainer = new(Game, Atlases.UI.InventoryInfoContainer)
            {
                Position = gridContainer.BoundingBox.GetPoint(RectanglePoint.RightTop, 3, 0)
            };

            // Info title container
            this.infoTitleContainer = new(Game, Atlases.UI.InventoryInfoTitleContainer)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.Top)
            };

            // Item icon
            this.itemIcon = new(Game)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = infoTitleContainer.BoundingBox.GetPoint(RectanglePoint.LeftBottom, -4, 1),
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

            // Item description
            this.itemDescription = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Dark,
                PivotOrigin = RectanglePoint.LeftTop,
                MaximumWidth = maxInfoTextWidth,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.LeftTop, 6, 3),
                Scale = ScaleInfo.Text.VeryLarge,
                ShadowOffset = new Vector2(0, .75f)
            };

            // Close button
            buttonClose = new UITextButton(owner.Game, InputBindings.Close)
            {
                AllowPressEffect = false,
                PivotOrigin = RectanglePoint.RightTop,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.RightBottom, 0, 4),
            };

            // Consume button
            buttonConsume = new UITextButton(owner.Game, InputBindings.ConsumeItem)
            {
                AllowSound = false,
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 5, -3)
            };

            // Discard button
            buttonDiscard = new UITextButton(owner.Game, InputBindings.Discard)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Position = gridContainer.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, 4)
            };

            // Equip button
            buttonEquip = new UITextButton(owner.Game, InputBindings.EquipItem)
            {
                AllowSound = false,
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 5, -3)
            };

            LayoutCategoryIcons();
        }

        #endregion

        #region Private members

        // DrawButtons
        private void DrawButtons(GameTime gameTime)
        {
            buttonClose.Draw(gameTime);

            if (activeGrid.SelectedItem is not Item item)
                return;

            if (!item.MetaItem.PreventDiscard)
                buttonDiscard.Draw(gameTime);

            if (item.MetaItem.Category == InventoryCategory.Junk || item.MetaItem.Category == InventoryCategory.Trinkets)
            {
                buttonEquip.Draw(gameTime);
            }
            else if (item.MetaItem.Category == InventoryCategory.Consumables)
            {
                buttonConsume.Draw(gameTime);
            }
        }

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                for (var i = 0; i < categoryIcons.Length; i++)
                {
                    if (categoryIcons[i].BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition))
                    {
                        currentCategory = categories[i];
                        Sound.Play(SoundNames.UISelectA);
                        InvalidateCategory();
                        return true;
                    }
                }
            }

            return false;
        }

        // InvalidateCategory
        private void InvalidateCategory()
        {
            activeGrid = grids[currentCategory];
            categoryText.Text = Localization.GetValue(currentCategory);
            InvalidateItemInfo();

            if (activeGrid.ItemContainer.Category == InventoryCategory.Junk)
            {
                InvalidateEquippedJunk();
            }
            else if (activeGrid.ItemContainer.Category == InventoryCategory.Trinkets)
            {
                InvalidateEquippedTrinket();
            }

            foreach (var category in categories)
            {
                categoryIcons[categories.IndexOf(category)].Opacity = currentCategory == category ? 1f : .4f;
            }
        }

        // InvalidateEquippedJunk
        private void InvalidateEquippedJunk()
        {
            if (equippedJunk != null && grids[InventoryCategory.Junk].GetSlot(equippedJunk) is InventorySlot slot)
            {
                buttonEquip.Text = Localization.GetValue(InventoryVerb.Unequip);
                checkMark.Image = Atlases.UI.CheckMark;
                checkMark.Position = slot.BoundingBox.GetPoint(RectanglePoint.RightBottom, -1, -8);
            }
            else
            {
                buttonEquip.Text = Localization.GetValue(InventoryVerb.Equip);
                checkMark.Image = null;
            }
        }

        // InvalidateEquippedTrinket
        private void InvalidateEquippedTrinket()
        {
            if (equippedTrinket != null && grids[InventoryCategory.Trinkets].GetSlot(equippedTrinket) is InventorySlot slot)
            {
                buttonEquip.Text = Localization.GetValue(InventoryVerb.Unequip);
                checkMark.Image = Atlases.UI.CheckMark;
                checkMark.Position = slot.BoundingBox.GetPoint(RectanglePoint.RightBottom, -1, -8);
            }
            else
            {
                buttonEquip.Text = Localization.GetValue(InventoryVerb.Equip);
                checkMark.Image = null;
            }
        }

        // InvalidateItemInfo
        private void InvalidateItemInfo()
        {
            if (activeGrid.SelectedItem is Item item)
            {
                itemName.Text = TextRepository.GetValue($"Item.{item.Name}.Name") + (item.Level == 0 ? string.Empty : $" +{item.Level}");
                itemDescription.Text = $"@Item.{item.Name}.Description";
                heartBonus.Position = itemDescription.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, 1);
                heartBonus.Value = item.MetaItem.Health;
                itemIcon.Image = item.MetaItem.Image;
            }
            else
            {
                itemName.Clear();
                itemDescription.Clear();
                itemIcon.Image = null;
                heartBonus.Value = null;
            }
        }

        // LayoutCategoryIcons
        private void LayoutCategoryIcons()
        {
            const int spacing = 2;

            var iconWidth = categoryIcons[0].BoundingBox.Width;
            var totalWidth = categoryIcons.Length * iconWidth + (categoryIcons.Length - 1) * spacing;
            float x = (navigationBar.BoundingBox.GetPoint(RectanglePoint.Top).X - totalWidth / 2) + (iconWidth / 2);
            float y = navigationBar.BoundingBox.GetPoint(RectanglePoint.Top, 0, -5).Y;

            for (var i = 0; i < categoryIcons.Length; i++)
            {
                categoryIcons[i].X = x + i * (iconWidth + spacing);
                categoryIcons[i].Y = y;
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

            if (Owner.Session.Room is ProceduralRoom)
            {
                trinketSlot.Draw(gameTime);
                healthMeter.Draw(gameTime);
                ticketsMeter.Draw(gameTime);
            }

            // Containers
            Game.SpriteBatch.Begin(Game.Camera);
            navigationBar.Draw(gameTime);
            gridContainer.Draw(gameTime);
            infoTitleContainer.Draw(gameTime);
            infoContainer.Draw(gameTime);

            for (int i = 0; i < categoryIcons.Length; i++)
            {
                categoryIcons[i].Draw(gameTime);
            }

            itemIcon.Draw(gameTime);

            Game.SpriteBatch.End();

            DrawButtons(gameTime);

            activeGrid.Draw(gameTime);

            if ( (equippedJunk != null && equippedJunk.MetaItem.Category == currentCategory) ||
                 (equippedTrinket != null && equippedTrinket.MetaItem.Category == currentCategory))
            {
                Game.SpriteBatch.Begin(Game.Camera);
                checkMark.Draw(gameTime);
                Game.SpriteBatch.End();
            }

            nextCategoryButton.Draw(gameTime);
            previousCategoryButton.Draw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            categoryText.Draw(gameTime);
            itemName.Draw(gameTime);
            itemDescription.Draw(gameTime);
            Game.SpriteBatch.End();

            if (heartBonus.Value != null)
                heartBonus.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (HandleMouseInput())
                return HandleInputResult.Handled;

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

            if (activeGrid.SelectedItem is Item selectedItem)
            {
                // Discard
                if (!selectedItem.MetaItem.PreventDiscard && buttonDiscard.TestPressed(PlayerIndex.One))
                {
                    Sound.Play(SoundNames.ItemDiscard);
                    activeGrid.DiscardSelectedItem();
                    
                    if (selectedItem == equippedJunk)
                    {
                        equippedJunk = activeGrid.SelectedItem;
                        InvalidateEquippedJunk();
                    }
                    else if (selectedItem == equippedTrinket)
                    {
                        equippedTrinket = activeGrid.SelectedItem;
                        InvalidateEquippedTrinket();
                    }

                    InvalidateItemInfo();
                    return HandleInputResult.Handled;
                }

                // Consume
                if (activeGrid.ItemContainer.Category == InventoryCategory.Consumables)
                {
                    if (buttonConsume.TestPressed(PlayerIndex.One))
                    {
                        activeGrid.SelectedSlot.PerformDefaultAction();
                        InvalidateItemInfo();
                        return HandleInputResult.Handled;
                    }
                }

                // Equip junk
                if (activeGrid.ItemContainer.Category == InventoryCategory.Junk)
                {
                    if (buttonEquip.TestPressed(PlayerIndex.One))
                    {
                        Sound.Play(SoundNames.ItemEquip);

                        if (equippedJunk == null)
                        {
                            equippedJunk = selectedItem;
                            Owner.Inventory.Junk.Select(equippedJunk);
                        }
                        else
                        {
                            equippedJunk = null;
                            Owner.Inventory.Junk.ClearSelection();
                        }

                        InvalidateEquippedJunk();
                        return HandleInputResult.Handled;
                    }
                }

                // Equip trinket
                if (activeGrid.ItemContainer.Category == InventoryCategory.Trinkets)
                {
                    if (buttonEquip.TestPressed(PlayerIndex.One))
                    {
                        Sound.Play(SoundNames.ItemEquip);

                        if (equippedTrinket == null)
                        {
                            equippedTrinket = selectedItem;
                            Owner.Inventory.Trinkets.Select(equippedTrinket);
                        }
                        else
                        {
                            equippedTrinket = null;
                            Owner.Inventory.Trinkets.ClearSelection();
                        }

                        InvalidateEquippedTrinket();
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

            currentCategory = InventoryCategory.Junk;
            
            equippedJunk = Owner.Inventory.Junk.SelectedItem;
            equippedTrinket = Owner.Inventory.Trinkets.SelectedItem;
            lastKnownInput = InputMethod.None;

            InvalidateCategory();
        }

        // OnUnloadContent
        protected override void OnUnloadContent()
        {
            base.OnUnloadContent();

            if (equippedJunk != null)
                Owner.Inventory.Junk.Select(equippedJunk);
            else
                Owner.Inventory.Junk.ClearSelection();

            if (equippedTrinket != null)
                Owner.Inventory.Trinkets.Select(equippedTrinket);
            else
                Owner.Inventory.Trinkets.ClearSelection();
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
            trinketSlot.Update(gameTime);
            healthMeter.Update(gameTime);
            ticketsMeter.Update(gameTime);
            itemName.Update(gameTime);
            itemDescription.Update(gameTime);
            nextCategoryButton.Update(gameTime);
            previousCategoryButton.Update(gameTime);

            if (lastKnownInput != InputManager.DefaultPlayer.LastInputMethod)
                lastKnownInput = InputManager.DefaultPlayer.LastInputMethod;

            if (MouseCursor.Instance.State == MouseCursorState.Cross)
            {
                for (var i = 0; i < categoryIcons.Length; i++)
                {
                    if (categoryIcons[i].BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition))
                    {
                        MouseCursor.Instance.State = MouseCursorState.CrossOn;
                        break;
                    }
                }
            }
        }

        #endregion

        // Owner
        public Actor Owner { get; set; }
    }
}
