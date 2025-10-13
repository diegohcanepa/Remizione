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

        private const int maxInfoTextWidth = 100;

        private InventoryGrid activeGrid;
        private readonly UITextButton buttonClose;
        private readonly UITextButton buttonConsume;
        private readonly UITextButton buttonDiscard;
        private readonly UITextButton buttonEquip;
        private readonly List<InventoryCategory> categories = [InventoryCategory.Junk, InventoryCategory.Thingies, InventoryCategory.Consumables, InventoryCategory.Trinkets, InventoryCategory.KeyItems];
        private readonly ImageSprite[] categoryIcons;
        private readonly ImageSprite[] categoryMarkers;
        private readonly TextSprite categoryText;
        private readonly ImageSprite checkMark;
        private InventoryCategory currentCategory;
        private readonly JunkSlot junkSlot;
        private readonly Dictionary<InventoryCategory, Item?> equippedItems = [];
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
        private readonly ThingieSlot thingiesSlot;
        private readonly TrinketSlot trinketSlot;

        #endregion

        #region Constructor

        // Constructor
        public InventoryScene(Actor owner)
            : base(owner.Game, SceneSettings.PausePreviousScenes)
        {
            this.Owner = owner;

            equippedItems[InventoryCategory.Junk] = null;
            equippedItems[InventoryCategory.Thingies] = null;
            equippedItems[InventoryCategory.Trinkets] = null;

            // Health meter
            this.healthMeter = new(Game)
            {
                Actor = owner,
            };

            // Junk slot
            this.junkSlot = new(Owner.Session)
            {
                Actor = owner,
                HideButton = true,
                SceneScope = this
            };

            // Thingies slot
            this.thingiesSlot = new(Owner.Session)
            {
                Actor = owner,
                HideButton = true,
                SceneScope = this
            };

            // Trinket slot
            this.trinketSlot = new(Game)
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
                Position = new(22, 36),
            };

            // Create grids for each category
            var gridPos = gridContainer.BoundingBox.GetPoint(RectanglePoint.LeftTop, 3, 3);
            foreach (var category in categories)
            {
                grids[category] = new InventoryGrid(owner.Inventory.GetContainer(category), 4, 3)
                {
                    Position = gridPos
                };
            }

            activeGrid = grids[InventoryCategory.Thingies];

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

            // Category markers
            categoryMarkers = new ImageSprite[categories.Count];
            for (int i = 0; i < categoryMarkers.Length; i++)
            {
                categoryMarkers[i] = new(Game)
                {
                    PivotOrigin = RectanglePoint.RightBottom,
                    Scale = ScaleInfo.UIElement.Medium
                };
            }

            // Previous tab button
            this.previousCategoryButton = new(Game, InputBindings.PreviousTab)
            {
                ImageName = nameof(InputBindings.PreviousTab),
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = gridContainer.BoundingBox.GetPoint(RectanglePoint.LeftTop),
            };

            // Next tab button
            this.nextCategoryButton = new(Game, InputBindings.NextTab)
            {
                ImageName = nameof(InputBindings.NextTab),
                PivotOrigin = RectanglePoint.RightBottom,
                Position = gridContainer.BoundingBox.GetPoint(RectanglePoint.RightTop)
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
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -2),
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
                PivotOrigin = RectanglePoint.RightBottom,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.RightBottom, -4, -3)
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

            if (item.MetaItem.Category == InventoryCategory.Consumables)
            {
                buttonConsume.Draw(gameTime);
            }
            else if (item.MetaItem.IsEquipment)
            {
                buttonEquip.Draw(gameTime);
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
            else if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                SceneController.Pop();
                return true;
            }

            return false;
        }

        // InvalidateCategory
        private void InvalidateCategory()
        {
            activeGrid = grids[currentCategory];
            categoryText.Text = Localization.GetValue(currentCategory);
            InvalidateItemInfo();

            if (activeGrid.ItemContainer.Category == InventoryCategory.Junk ||
                activeGrid.ItemContainer.Category == InventoryCategory.Thingies ||
                activeGrid.ItemContainer.Category == InventoryCategory.Trinkets)
            {
                InvalidateEquippedItem(activeGrid.ItemContainer.SelectedItem);
            }

            foreach (var category in categories)
            {
                categoryIcons[categories.IndexOf(category)].Opacity = currentCategory == category ? 1f : .5f;
                categoryIcons[categories.IndexOf(category)].Scale = new(.8f);
            }
        }

        // InvalidateEquippedItem
        private void InvalidateEquippedItem(Item? item)
        {
            if (item != null && grids[item.MetaItem.Category].GetSlot(item) is InventorySlot slot)
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
            heartBonus.Value = null;

            if (activeGrid.SelectedItem is Item item)
            {
                itemName.Text = TextRepository.GetValue($"Item.{item.Name}.Name") + (item.Level == 0 ? string.Empty : $" +{item.Level}");
                itemDescription.Opacity = 1;
                itemDescription.PivotOrigin = RectanglePoint.LeftTop;
                itemDescription.Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.LeftTop, 6, 3);
                itemDescription.Text = $"@Item.{item.Name}.Description";
                itemIcon.Image = item.MetaItem.Image;

                var pos = itemDescription.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, 1);

                // Health
                if (item.MetaItem.HP != null)
                {
                    heartBonus.Position = itemDescription.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, 1);
                    heartBonus.Value = item.MetaItem.HP;
                    pos.Y += heartBonus.BoundingBox.Height;
                }
            }
            else
            {
                itemDescription.PivotOrigin = RectanglePoint.Center;
                itemDescription.Opacity = .5f;
                itemDescription.Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.Center, 0, -3);
                itemDescription.Text = "@Misc.NoInventoryItem";
                itemName.Clear();
                itemIcon.Image = null;
            }
        }

        // LayoutCategoryIcons
        private void LayoutCategoryIcons()
        {
            const int spacing = 3;

            var iconWidth = categoryIcons[0].BoundingBox.Width;
            var totalWidth = categoryIcons.Length * iconWidth + (categoryIcons.Length - 1) * spacing;
            float x = (navigationBar.BoundingBox.GetPoint(RectanglePoint.Top).X - totalWidth / 2) + (iconWidth / 2);
            float y = navigationBar.BoundingBox.GetPoint(RectanglePoint.Top, 0, -7).Y;

            for (var i = 0; i < categoryIcons.Length; i++)
            {
                categoryIcons[i].X = x + i * (iconWidth + spacing);
                categoryIcons[i].Y = y;
                categoryMarkers[i].Image = Owner.Inventory.GetContainer(categories[i]).Count == 0 ? null : Atlases.UI.InventoryCategoryNotEmpty;
                categoryMarkers[i].Position = categoryIcons[i].BoundingBox.GetPoint(RectanglePoint.RightBottom, 0, 1.5f);
            }
        }

        // NextCategory
        private void NextCategory()
        {
            buttonDiscard.IsBeating = false;

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
            buttonDiscard.IsBeating = false;

            var index = categories.IndexOf(currentCategory);
            if (index == 0)
                currentCategory = categories[^1];
            else
                currentCategory = categories[index - 1];

            InvalidateCategory();
        }

        // TestDiscard
        private bool TestDiscard(Item item)
        {
            if (!item.MetaItem.PreventDiscard && buttonDiscard.TestPressed(PlayerIndex.One))
            {
                if (buttonDiscard.IsBeating)
                {
                    Sound.Play(SoundNames.ItemDiscard);
                    activeGrid.DiscardSelectedItem();

                    if (equippedItems.ContainsKey(item.MetaItem.Category))
                    {
                        if (item == equippedItems[item.MetaItem.Category])
                        {
                            equippedItems[item.MetaItem.Category] = activeGrid.SelectedItem;
                            InvalidateEquippedItem(equippedItems[item.MetaItem.Category]);
                        }
                    }

                    LayoutCategoryIcons();
                    InvalidateItemInfo();
                }
                else
                    buttonDiscard.IsBeating = true;

                return true;
            }

            return false;
        }

        // TestConsume
        private bool TestConsume(Item selectedItem)
        {
            if (selectedItem.MetaItem.Category == InventoryCategory.Consumables)
            {
                if (buttonConsume.TestPressed(PlayerIndex.One))
                {
                    activeGrid.SelectedSlot.PerformDefaultAction();
                    InvalidateItemInfo();
                    LayoutCategoryIcons();
                    return true;
                }
            }

            return false;
        }

        // TestEquip
        private bool TestEquip(Item selectedItem)
        {
            if (selectedItem.MetaItem.IsEquipment)
            {
                if (buttonEquip.TestPressed(PlayerIndex.One))
                {
                    Sound.Play(SoundNames.ItemEquip);

                    if (equippedItems[activeGrid.ItemContainer.Category] == null)
                    {
                        equippedItems[activeGrid.ItemContainer.Category] = selectedItem;
                        activeGrid.ItemContainer.Select(selectedItem);
                    }
                    else
                    {
                        equippedItems[activeGrid.ItemContainer.Category] = null;
                        activeGrid.ItemContainer.ClearSelection();
                    }

                    InvalidateEquippedItem(equippedItems[activeGrid.ItemContainer.Category]);
                    
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera);
            Game.Shapes.DrawRectangle(Screen.Area, ColorPalette.BackgroundShade);
            Game.SpriteBatch.End();

            if (Owner.Session.Room is ProceduralRoom)
            {
                trinketSlot.Draw(gameTime);
                healthMeter.Draw(gameTime);
            }

            junkSlot.Draw(gameTime);
            thingiesSlot.Draw(gameTime);

            // Containers
            Game.SpriteBatch.Begin(Game.Camera);
            navigationBar.Draw(gameTime);
            gridContainer.Draw(gameTime);
            infoTitleContainer.Draw(gameTime);
            infoContainer.Draw(gameTime);

            itemIcon.Draw(gameTime);

            Game.SpriteBatch.End();

            for (int i = 0; i < categoryIcons.Length; i++)
            {
                Effect? shader = null;
                if (currentCategory != categories[i] && categoryIcons[i].BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition))
                {
                    RemizioneGame.Effects.ColorSaturation.SetColor(.7f, .7f, .7f, 1);
                    shader = RemizioneGame.Effects.ColorSaturation.Effect;
                }

                Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp, shader);
                categoryIcons[i].Draw(gameTime);
                categoryMarkers[i].Opacity = shader == null ? categoryIcons[i].Opacity : 1;
                categoryMarkers[i].Draw(gameTime);
                Game.SpriteBatch.End();
            }

            DrawButtons(gameTime);

            activeGrid.Draw(gameTime);

            if (equippedItems[InventoryCategory.Junk]?.MetaItem.Category == currentCategory ||
                equippedItems[InventoryCategory.Thingies]?.MetaItem.Category == currentCategory ||
                equippedItems[InventoryCategory.Trinkets]?.MetaItem.Category == currentCategory)
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
                buttonDiscard.IsBeating = false;
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
                if (TestDiscard(selectedItem))
                    return HandleInputResult.Handled;

                // Equip
                if (TestEquip(selectedItem))
                {
                    buttonDiscard.IsBeating = false;
                    return HandleInputResult.Handled;
                }

                // Consume
                if (TestConsume(selectedItem))
                {
                    buttonDiscard.IsBeating = false;
                    return HandleInputResult.Handled;
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

            LayoutCategoryIcons();

            buttonDiscard.IsBeating = false;

            Sound.Play(SoundNames.UIInventoryOpen);

            // Populate grids
            foreach (var grid in grids.Values)
            {
                grid.Populate();
            }

            currentCategory = Owner.HP < Owner.MaxHP ? InventoryCategory.Consumables : InventoryCategory.Junk;

            equippedItems[InventoryCategory.Junk] = Owner.Inventory.Junk.SelectedItem;
            equippedItems[InventoryCategory.Thingies] = Owner.Inventory.Thingies.SelectedItem;
            equippedItems[InventoryCategory.Trinkets] = Owner.Inventory.Trinkets.SelectedItem;

            lastKnownInput = InputMethod.None;

            InvalidateCategory();
        }

        // OnUnloadContent
        protected override void OnUnloadContent()
        {
            base.OnUnloadContent();

            foreach (var keyValue in equippedItems)
            {
                if (keyValue.Value != null)
                    keyValue.Value.Select();
                else
                    Owner.Inventory.GetContainer(keyValue.Key).ClearSelection();
            }
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
            junkSlot.Update(gameTime);
            thingiesSlot.Update(gameTime);
            trinketSlot.Update(gameTime);
            healthMeter.Update(gameTime);
            itemName.Update(gameTime);
            itemDescription.Update(gameTime);
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
