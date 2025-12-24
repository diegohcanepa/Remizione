using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ScaryCastle
{
    /// <summary>
    /// InventoryScene
    /// </summary>
    public sealed class InventoryScene : Scene
    {
        #region Private fields

        private readonly TextSprite amountText;
        private const float animationSpeed = 14;
        private readonly UIButton buttonAction;
        private readonly UIButton buttonClose;
        private readonly UIButton buttonInfo;
        private readonly ItemInfoScene infoScene;
        private readonly ImageSprite itemCategoryIcon;
        private readonly TextSprite itemNameText;
        private int selectedIndex;
        private readonly GameSession session;
        private readonly ImageSprite slotImage;
        private static readonly Vector2 slotPosition = new(Screen.Center.X, Screen.HUDArea.Bottom - 16);
        private const int spaceBetweenIcons = 15;
        private readonly StickInputController stick = new(GamePadThumbStick.Left) { AutoRepeatRate = 150 };
        private readonly TextSprite title;
        private float visualIndex;
        private readonly List<VisualItem> visualItems = [];

        #endregion

        #region Constructor

        // Constructor
        public InventoryScene(GameSession session)
            : base(session.Game, SceneSettings.PausePreviousScenes)
        {
            this.session = session;

            // SlotImage
            this.slotImage = new(Game, Atlases.UI.ItemGridSlot)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = slotPosition
            };

            // Item name
            itemNameText = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Top),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Item category icon
            itemCategoryIcon = new(Game)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Top, 0, -7),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Amount text
            amountText = new(Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Top,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -3),
                Scale = ScaleInfo.Text.ExtraLarge
            };

            // Close button
            buttonClose = new(Game, InputBindings.Close)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -2),
            };

            // Info button
            buttonInfo = new(Game, InputBindings.Info)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -13),
                AllowPressEffect = false
            };

            // Title
            title = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Bottom,
                Position = slotImage.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -25),
                Scale = ScaleInfo.Text.Huge
            };

            // Action button
            buttonAction = new(Game, InputBindings.UseFriendlyItem)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, 0, -24),
                Sound = Sound.Find(SoundNames.UISelectB)
            };

            this.infoScene = new(Game);
        }

        #endregion

        #region Private members

        // DrawItems
        private void DrawItems(GameTime gameTime)
        {
            // Limitar el rango a 5
            const int currentVisibleRange = 5;

            for (int i = -currentVisibleRange; i <= currentVisibleRange; i++)
            {
                int index = (int)visualIndex + i;
                if (index < 0 || index >= visualItems.Count)
                    continue;

                float offset = i - (visualIndex - (int)visualIndex);
                visualItems[index].Position = slotPosition + new Vector2(offset * spaceBetweenIcons, 0);

                float distance = MathF.Abs(offset);

                // RESTAURADO: Tu escala original (Base 0.8, Min 0.6)
                float scale = MathF.Max(.6f, .8f - (distance * .2f));

                // AJUSTADO: El alpha ahora llega a 0 exactamente en la distancia 5
                // (1 / 5 = 0.2) para que el desvanecimiento sea proporcional al nuevo rango
                float alpha = MathF.Max(0f, 1f - (distance * 0.2f));

                visualItems[index].Opacity = alpha;
                visualItems[index].Scale = new(scale);

                // Solo dibujamos si es mínimamente visible para ahorrar procesos
                if (alpha > 0.01f)
                    visualItems[index].Draw(gameTime);
            }
        }

        // GetInventoryItemAt
        private VisualItem? GetInventoryItemAt(Vector2 position)
        {
            for (int i = 0; i < visualItems.Count; i++)
            {
                if (visualItems[i].BoundingBox.Contains(position))
                    return visualItems[i];
            }

            return null;
        }

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                SceneController.Pop();
                return true;
            }

            if (!InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                return false;

            if (GetInventoryItemAt(InputManager.DefaultPlayer.Mouse.VirtualPosition) is VisualItem item)
            {
                Select(visualItems.IndexOf(item));
                MouseCursor.Instance.AnimateClick();
                Sound.Play(SoundNames.UIHover);
            }

            return false;
        }

        // InvalidateSlot
        private void InvalidateSlot()
        {
            amountText.Text = selectedIndex < 0 || SelectedItem?.Item.MetaItem.IsUnique == true ? null : SelectedItem?.Item.GetDisplayAmount();
            itemCategoryIcon.Image = null;

            buttonAction.Text = null;

            if (SelectedItem?.Item is Item item)
            {
                if (item.MetaItem.IsEquipment)
                {
                    if (item.MetaItem.Category == ItemCategory.Gadget)
                    {
                        itemCategoryIcon.Image = Atlases.UI.InventoryCategoryGadget;
                        buttonAction.Text = Localization.GetValue(item.IsEquipped ? InventoryVerb.TakeOff : InventoryVerb.Equip);
                    }
                    else
                    {
                        if (item.MetaItem.Category == ItemCategory.LeftHand)
                        {
                            itemCategoryIcon.Image = Atlases.UI.InventoryCategoryLeftHand;
                        }
                        else if (item.MetaItem.Category == ItemCategory.RightHand)
                        {
                            itemCategoryIcon.Image = Atlases.UI.InventoryCategoryRightHand;
                        }

                        buttonAction.Text = Localization.GetValue(InventoryVerb.Equip);
                    }
                }
                else if (SelectedItem.Item.MetaItem.Category == ItemCategory.Consumable)
                {
                    buttonAction.Text = Localization.GetValue(InventoryVerb.Use);
                }
            }

        }

        // PerformAction
        private bool PerformAction()
        {
            if (SelectedItem?.Item is not Item item)
                return false;

            // Action
            if (buttonAction.Text != null && buttonAction.TestPressed(PlayerIndex.One))
            {
                if (item.MetaItem.IsEquipment)
                {
                    if (item.IsEquipped)
                        session.Inventory.Unequip(item);
                    else
                        session.Inventory.Equip(item);
                }
                else if (item.MetaItem.IsConsumable)
                {
                    SceneController.Pop();
                    session.Player?.ConsumeItem(item);
                }

                return true;
            }

            return false;
        }

        // Select
        private void Select(int index)
        {
            if (index >= visualItems.Count)
                return;

            selectedIndex = index;
            var item = visualItems[index].Item;
            itemNameText.Text = index < 0 ? null : item.DisplayText;
            InvalidateSlot();
        }

        // SelectedItem
        private VisualItem? SelectedItem => selectedIndex == -1 ? null : visualItems[selectedIndex];

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsCurrentScene || session.IsOutcomeInProgress)
                return;

            session.Inventory.Session.HUD.Draw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera);
            title.Draw(gameTime);
            itemNameText.Draw(gameTime);
            slotImage.Draw(gameTime);
            amountText.Draw(gameTime);
            DrawItems(gameTime);
            itemCategoryIcon.Draw(gameTime);
            Game.SpriteBatch.End();

            buttonClose.Draw(gameTime);
            buttonInfo.Draw(gameTime);

            if (buttonAction.Text != null)
                buttonAction.Draw(gameTime);
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

            // Action
            if (PerformAction())
                return HandleInputResult.Handled;

            // Close
            if (buttonClose.TestPressed(PlayerIndex.One))
            {
                SceneController.Pop();
                return HandleInputResult.Handled;
            }

            // Info
            if (SelectedItem?.Item != null && buttonInfo.TestPressed(PlayerIndex.One))
            {
                infoScene.Item = SelectedItem.Item;
                infoScene.SceneController.Push();
                return HandleInputResult.Handled;
            }

            if (visualItems.Count > 1)
            {
                // Move left
                if (selectedIndex > 0)
                {
                    if (InputBindings.SelectLeft.IsPressed(PlayerIndex.One) || stick.IsLeft(PlayerIndex.One))
                    {
                        Select(Math.Max(0, selectedIndex - 1));
                        Sound.Play(SoundNames.UIHover);
                        return HandleInputResult.Handled;
                    }
                }

                // Move right
                if (selectedIndex < visualItems.Count - 1)
                {
                    if (InputBindings.SelectRight.IsPressed(PlayerIndex.One) || stick.IsRight(PlayerIndex.One))
                    {
                        Select(Math.Min(visualItems.Count - 1, selectedIndex + 1));
                        Sound.Play(SoundNames.UIHover);
                        return HandleInputResult.Handled;
                    }
                }

                // Move up (first item)
                if (InputBindings.SelectUp.IsPressed(PlayerIndex.One) || stick.IsUp(PlayerIndex.One))
                {
                    Select(0);
                    Sound.Play(SoundNames.UIHover);
                    return HandleInputResult.Handled;
                }

                // Move down (last item)
                if (InputBindings.SelectDown.IsPressed(PlayerIndex.One) || stick.IsDown(PlayerIndex.One))
                {
                    Select(visualItems.Count - 1);
                    Sound.Play(SoundNames.UIHover);
                    return HandleInputResult.Handled;
                }
            }

            return HandleInputResult.Handled;
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            Sound.Play(SoundNames.UIInventoryOpen);

            base.OnLoadContent();

            visualItems.Clear();

            for (var i = 0; i < session.Inventory.Count; i++)
            {
                visualItems.Add(new(session.Game, session.Inventory[i]));
            }

            Select(0);

            visualIndex = selectedIndex;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            stick.Update(gameTime);

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            visualIndex += (selectedIndex - visualIndex) * MathF.Min(1f, animationSpeed * deltaTime);

            base.OnUpdate(gameTime);

            buttonClose.Update(gameTime);
            buttonInfo.Update(gameTime);
            buttonAction.Update(gameTime);
        }

        #endregion

        // Text
        public string? Text
        {
            get => title.Text;
            set => title.Text = value;
        }
    }
}
