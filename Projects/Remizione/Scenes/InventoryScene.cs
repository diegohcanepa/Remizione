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
        private const int maxInfoTextWidth = 80;

        private readonly UITextButton buttonClose;
        private readonly UITextButton buttonEquip;
        private readonly UITextButton buttonUse;
        private readonly List<MetaItemCategory> categories = [MetaItemCategory.Consumable, MetaItemCategory.Equipment, MetaItemCategory.Misc];
        private readonly TextSprite categoryText;
        private readonly InventoryGrid grid;
        private readonly ImageSprite gridContainerImage;
        private readonly UIHealthMeter healthMeter;
        private readonly ImageSprite infoContainerImage;
        private readonly TextSprite itemDescription;
        private readonly TextSprite itemName;
        private InputMethod lastKnownInput;
        private readonly UITextButton nextTabButton;
        private readonly UITextButton previousTabButton;


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

            // Grid container
            this.gridContainerImage = new(Game, Atlases.UI.InventoryGridContainer)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Position = new(10, 28),
            };

            // grid
            this.grid = new(owner.Inventory, 6, 4)
            {
                Position = gridContainerImage.BoundingBox.GetPoint(RectanglePoint.LeftTop, 3, 3)
            };

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
                Scale = ScaleInfo.Text.Large,
            };

            // Close button
            buttonClose = new UITextButton(owner.Game, InputBindings.Close)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -2),
            };

            // Equip button
            buttonEquip = new UITextButton(owner.Game, InputBindings.Equip)
            {
                PivotOrigin = RectanglePoint.RightBottom,
            };

            // Use button
            buttonUse = new UITextButton(owner.Game, InputBindings.UseItem)
            {
                PivotOrigin = RectanglePoint.RightBottom,
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
            if (grid.GetSlotAt(InputManager.DefaultPlayer.Mouse.VirtualPosition)?.Item is Item item)
            {
                sele

                Select(items.IndexOf(item));
                MouseCursor.Instance.AnimateClick();
                Sound.Play(SoundNames.UIHover);
            }
            */

            return false;
        }

        // InvalidateCategory
        private void InvalidateCategory()
        {
            grid.Fill(Owner.Session.SelectedItemCategory);
            categoryText.Text = Localization.GetValue(Owner.Session.SelectedItemCategory);

            var itemName = string.Empty;
            if (Owner.Session.SelectedItemCategory == MetaItemCategory.Consumable)
                grid.SelectSlot(Owner.Session.DefaultConsumableItem);
            else if (Owner.Session.SelectedItemCategory == MetaItemCategory.Equipment)
                grid.SelectSlot(Owner.Session.DefaultEquipmentItem);
            else if (Owner.Session.SelectedItemCategory == MetaItemCategory.Misc)
                grid.SelectSlot(Owner.Session.DefaultMiscItem);
            else
                grid.SelectSlot(0);

            InvalidateItemInfo();
        }

        // InvalidateItemInfo
        private void InvalidateItemInfo()
        {
            if (grid.SelectedSlot?.Item is Item item)
            {
                itemName.Text = TextRepository.GetValue($"Item.{item.Name}.Name") + (item.Level == 0 ? string.Empty : $" +{item.Level}");
                itemDescription.Text = $"@Item.{item.Name}.Description";
                itemDescription.Position = itemName.BoundingBox.GetPoint(RectanglePoint.LeftBottom);
                //itemActionButton.TextColor = ColorPalette.Text.Light;
                //itemActionButton.Text = item.MetaItem.Action == ItemAction.None ? null : $"@ItemActions.{item.MetaItem.Action}";
            }
            else
            {
                itemName.Text = null;
                itemDescription.Text = null;
                //itemActionButton.Text = null;
            }
        }

        // LayoutButtons
        private void LayoutButtons()
        {
            buttonEquip.Position = buttonClose.BoundingBox.GetPoint(RectanglePoint.LeftBottom, -5, 0);
            buttonUse.Position = buttonEquip.BoundingBox.GetPoint(RectanglePoint.LeftBottom, -5, 0);
        }

        // NextCategory
        private void NextCategory()
        {
            var index = categories.IndexOf(Owner.Session.SelectedItemCategory);
            if (index == categories.Count - 1)
                Owner.Session.SelectedItemCategory = categories[0];
            else
                Owner.Session.SelectedItemCategory = categories[index + 1];

            InvalidateCategory();
        }

        // PreviousCategory
        private void PreviousCategory()
        {
            var index = categories.IndexOf(Owner.Session.SelectedItemCategory);
            if (index == 0)
                Owner.Session.SelectedItemCategory = categories[^1];
            else
                Owner.Session.SelectedItemCategory = categories[index - 1];

            InvalidateCategory();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            healthMeter.Draw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera);
            gridContainerImage.Draw(gameTime);
            infoContainerImage.Draw(gameTime);
            Game.SpriteBatch.End();

            buttonClose.Draw(gameTime);
            buttonEquip.Draw(gameTime);
            buttonUse.Draw(gameTime);

            grid.Draw(gameTime);
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
            if (grid.HandleInput(gameTime) == HandleInputResult.Handled)
            {
                InvalidateItemInfo();

                if (grid.SelectedSlot?.Item is Item item)
                {
                    if (item.MetaItem.Category == MetaItemCategory.Consumable)
                        Owner.Session.DefaultConsumableItem = item.Name;

                    else if (item.MetaItem.Category == MetaItemCategory.Equipment)
                        Owner.Session.DefaultEquipmentItem = item.Name;

                    else if (item.MetaItem.Category == MetaItemCategory.Misc)
                        Owner.Session.DefaultMiscItem = item.Name;
                }

                return HandleInputResult.Handled;
            }

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

            Sound.Play(SoundNames.UIInventoryOpen);
            lastKnownInput = InputMethod.None;
            LayoutButtons();

            InvalidateCategory();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            buttonClose.Update(gameTime);
            buttonEquip.Update(gameTime);
            buttonUse.Update(gameTime);
            categoryText.Update(gameTime);
            grid.Update(gameTime);
            gridContainerImage.Update(gameTime);
            healthMeter.Update(gameTime);
            itemName.Update(gameTime);
            itemDescription.Update(gameTime);
            nextTabButton.Update(gameTime);
            previousTabButton.Update(gameTime);

            if (lastKnownInput != InputManager.DefaultPlayer.LastInputMethod)
            {
                LayoutButtons();
                lastKnownInput = InputManager.DefaultPlayer.LastInputMethod;
            }
        }

        #endregion

        // Owner
        public Actor Owner { get; set; }
    }
}
