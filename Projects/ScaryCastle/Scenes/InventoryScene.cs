using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class InventoryScene : Scene
    {
        #region Private fields

        private const int maxInfoTextWidth = 78;

        private readonly UIButton buttonClose;
        private readonly UIButton buttonConsume;
        private readonly UIButton buttonDiscard;
        private readonly UIButton buttonEquip;
        private readonly List<ItemCategory?> categories = [null, ItemCategory.LeftHand, ItemCategory.RightHand, ItemCategory.Consumable, ItemCategory.Gadget, ItemCategory.KeyItem];
        private readonly List<ImageSprite> categoryIcons = [];
        private readonly Vector2Tween categoryIconTween = Vector2Tween.Create(TweenStyle.Linear, .8f, .9f, 200, -1);
        private readonly ImageSprite[] categoryMarkers;
        private readonly TextSprite categoryText;
        private ItemCategory? currentCategory;
        private readonly ItemGrid grid;
        private readonly ImageSprite gridContainer;
        private readonly UIHPBonus hpBonus;
        private readonly ImageSprite infoContainer;
        private readonly ImageSprite infoTitleContainer;
        private readonly Inventory inventory;
        private readonly TextSprite itemDescription;
        private readonly ImageSprite itemIcon;
        private readonly TextSprite itemName;
        private InputMethod lastKnownInput;
        private readonly ImageSprite navigationBar;
        private readonly UIButton nextCategoryButton;
        private readonly UIButton previousCategoryButton;

        #endregion

        #region Constructor

        // Constructor
        public InventoryScene(Inventory inventory)
            : base(inventory.Session.Game, SceneSettings.PausePreviousScenes)
        {
            this.inventory = inventory;

            // Grid container
            this.gridContainer = new(Game, Atlases.UI.InventoryGridContainer)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Position = new(32, 38),
            };

            // Create grid
            var gridPos = gridContainer.BoundingBox.GetPoint(RectanglePoint.LeftTop, 5, 3);
            grid = new ItemGrid(inventory, null, 4, 3)
            {
                Position = gridPos
            };

            // Navigation bar
            this.navigationBar = new(Game, Atlases.UI.InventoryNavigationBar)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = gridContainer.BoundingBox.GetPoint(RectanglePoint.Top, 0, 0)
            };

            // Category icons
            for (int i = 0; i < categories.Count; i++)
            {
                ImageSprite image;
                if (categories[i] == null)
                {
                    image = new(Game, Atlases.UI.FindImage("InventoryCategoryAll"))
                    {
                        PivotOrigin = RectanglePoint.Center,
                        Scale = ScaleInfo.UIElement.Medium
                    };
                }
                else
                {
                    image = new(Game, Atlases.UI.FindImage($"InventoryCategory{categories[i]}"))
                    {
                        PivotOrigin = RectanglePoint.Center,
                        Scale = ScaleInfo.UIElement.Medium
                    };
                }

                categoryIcons.Add(image);
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

            // Category text
            this.categoryText = new(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.Text.ExtraLarge,
                Position = navigationBar.BoundingBox.GetPoint(RectanglePoint.Center, 0, .75f),
                ShadowOffset = new Vector2(0, .75f)
            };

            // Heart bonus
            this.hpBonus = new(Game);

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
            buttonClose = new UIButton(Game, InputBindings.Close)
            {
                AllowPressEffect = false,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -2),
            };

            // Consume button
            buttonConsume = new UIButton(Game, InputBindings.ConsumeItem)
            {
                AllowSound = false,
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 3, -3)
            };

            // Discard button
            buttonDiscard = new UIButton(Game, InputBindings.Discard)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.RightBottom, -4, -3)
            };

            // Equip button
            buttonEquip = new UIButton(Game, InputBindings.EquipItem)
            {
                AllowSound = false,
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 3, -3)
            };

            LayoutCategoryIcons();
        }

        #endregion

        #region Private members

        // DrawButtons
        private void DrawButtons(GameTime gameTime)
        {
            buttonClose.Draw(gameTime);

            if (grid.SelectedItem is not Item item)
                return;

            if (!item.MetaItem.PreventDiscard)
                buttonDiscard.Draw(gameTime);

            if (item.MetaItem.Category == ItemCategory.Consumable)
            {
                buttonConsume.Draw(gameTime);
            }
            else if (item.MetaItem.IsEquipment)
            {
                if (!item.IsEquipped || item.MetaItem.Category == ItemCategory.Gadget)
                    buttonEquip.Draw(gameTime);
            }
        }

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                for (var i = 0; i < categoryIcons.Count; i++)
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
            categoryText.Text = currentCategory is ItemCategory c ? Localization.GetValue(c) : TextRepository.GetValue("Misc.All");

            foreach (var category in categories)
            {
                categoryIcons[categories.IndexOf(category)].Tweens.Reset();
                if (category == currentCategory)
                    categoryIcons[categories.IndexOf(category)].Tweens.ScaleTween = categoryIconTween;
                else
                    categoryIcons[categories.IndexOf(category)].Scale = categoryIconTween.StartValue;
            }

            grid.CategoryFilter = currentCategory;

            InvalidateItemInfo();
        }

        // InvalidateItemInfo
        private void InvalidateItemInfo()
        {
            hpBonus.Amount = 0;

            if (grid.SelectedItem is Item item)
            {
                itemName.Text = TextRepository.GetValue($"Item.{item.Name}.Name");
                itemDescription.Opacity = 1;
                itemDescription.PivotOrigin = RectanglePoint.LeftTop;
                itemDescription.Position = infoContainer.BoundingBox.GetPoint(RectanglePoint.LeftTop, 5, 3);
                itemDescription.Text = $"@Item.{item.Name}.Description";
                itemIcon.Image = item.MetaItem.Image;

                // HP
                if (item.MetaItem.Effect.HP != null)
                {
                    hpBonus.Position = itemDescription.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, 1);
                    hpBonus.Amount = item.MetaItem.Effect.HP.MaximumValue;
                }

                if (item.MetaItem.Category == ItemCategory.Gadget)
                {
                    if (item.IsEquipped)
                        buttonEquip.Text = Localization.GetValue(InventoryVerb.TakeOff);
                    else
                        buttonEquip.Text = Localization.GetValue(InventoryVerb.Equip);
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
            var totalWidth = (categoryIcons.Count * iconWidth) + (categoryIcons.Count * spacing);
            float x = navigationBar.BoundingBox.GetPoint(RectanglePoint.Top).X - (totalWidth / 2) + (iconWidth / 2);
            float y = navigationBar.BoundingBox.GetPoint(RectanglePoint.Top, 0, -7).Y;

            for (var i = 0; i < categoryIcons.Count; i++)
            {
                categoryIcons[i].X = x + (i * (iconWidth + spacing));
                categoryIcons[i].Y = y;

                if (categories[i] is not ItemCategory itemCategory)
                    continue;

                categoryMarkers[i].Image = inventory.HasItems(itemCategory) ? Atlases.UI.InventoryCategoryNotEmpty : null;
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
            if (item.MetaItem.PreventDiscard)
                return false;

            if (buttonDiscard.TestPressed(PlayerIndex.One))
            {
                if (buttonDiscard.IsBeating)
                {
                    Sound.Play(SoundNames.ItemDiscard);
                    grid.DiscardSelectedItem();
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
            if (selectedItem.MetaItem.Category == ItemCategory.Consumable)
            {
                if (buttonConsume.TestPressed(PlayerIndex.One))
                {
                    grid.SelectedSlot.PerformDefaultAction();
                    InvalidateItemInfo();
                    LayoutCategoryIcons();
                    return true;
                }
            }

            return false;
        }

        // TestEquip
        private bool TestEquip(Item item)
        {
            if (!item.MetaItem.IsEquipment)
                return false;

            if (item.IsEquipped && item.MetaItem.Category != ItemCategory.Gadget)
                return false;

            if (buttonEquip.TestPressed(PlayerIndex.One))
            {
                Sound.Play(SoundNames.ItemEquip);

                if (item.IsEquipped)
                    inventory.Unequip(item);
                else
                    inventory.Equip(item);

                InvalidateItemInfo();

                return true;
            }

            return false;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            inventory.Session.HUD.Draw(gameTime);

            // Containers
            Game.SpriteBatch.Begin(Game.Camera);
            navigationBar.Draw(gameTime);
            gridContainer.Draw(gameTime);
            infoTitleContainer.Draw(gameTime);
            infoContainer.Draw(gameTime);

            itemIcon.Draw(gameTime);

            Game.SpriteBatch.End();

            for (int i = 0; i < categoryIcons.Count; i++)
            {
                Effect? shader = null;
                if (currentCategory != categories[i] && categoryIcons[i].BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition))
                {
                    ScaryCastleGame.Effects.ColorSaturation.SetColor(.7f, .7f, .7f, 1);
                    shader = ScaryCastleGame.Effects.ColorSaturation.Effect;
                }

                Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp, shader);
                categoryIcons[i].Draw(gameTime);
                categoryMarkers[i].Opacity = shader == null ? categoryIcons[i].Opacity : 1;
                categoryMarkers[i].Draw(gameTime);
                Game.SpriteBatch.End();
            }

            DrawButtons(gameTime);

            grid.Draw(gameTime);

            nextCategoryButton.Draw(gameTime);
            previousCategoryButton.Draw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            categoryText.Draw(gameTime);
            itemName.Draw(gameTime);
            itemDescription.Draw(gameTime);
            Game.SpriteBatch.End();

            hpBonus.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (HandleMouseInput())
                return HandleInputResult.Handled;

            // Grid
            if (grid.HandleInput(gameTime) == HandleInputResult.Handled)
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

            if (grid.SelectedItem is Item selectedItem)
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

            currentCategory = null;

            lastKnownInput = InputMethod.None;

            InvalidateCategory();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            inventory.Session.HUD.Update(gameTime);

            buttonClose.Update(gameTime);
            buttonConsume.Update(gameTime);
            buttonDiscard.Update(gameTime);
            buttonEquip.Update(gameTime);
            categoryText.Update(gameTime);
            grid.Update(gameTime);
            gridContainer.Update(gameTime);
            itemName.Update(gameTime);
            itemDescription.Update(gameTime);
            nextCategoryButton.Update(gameTime);
            previousCategoryButton.Update(gameTime);

            if (lastKnownInput != InputManager.DefaultPlayer.LastInputMethod)
                lastKnownInput = InputManager.DefaultPlayer.LastInputMethod;

            for (var i = 0; i < categoryIcons.Count; i++)
            {
                categoryIcons[i].Update(gameTime);
            }
        }

        #endregion
    }
}
